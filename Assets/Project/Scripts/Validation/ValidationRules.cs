using System.Collections.Generic;
using SmartFactory.Piping.Data;
using UnityEngine;

namespace SmartFactory.Piping.Validation
{
    public readonly struct SharpBend
    {
        public readonly int PipeA;
        public readonly int PipeB;
        public readonly float AngleDeg;
        public readonly Vector3 JointPosition;

        public SharpBend(int a, int b, float angleDeg, Vector3 jointPosition)
        {
            PipeA = a;
            PipeB = b;
            AngleDeg = angleDeg;
            JointPosition = jointPosition;
        }
    }

    public static class ValidationRules
    {
        public static List<SharpBend> DetectSharpBends(
            IReadOnlyList<PipeData> pipes,
            float minBendAngleDeg = 30f,
            float bucketSize = 0.05f)
        {
            var result = new List<SharpBend>();
            if (pipes == null || pipes.Count < 2) return result;

            var endpoints = new Dictionary<Vector3Int, List<(int idx, Vector3 dir, Vector3 pos)>>();

            for (int i = 0; i < pipes.Count; i++)
            {
                var pipe = pipes[i];
                if (pipe.Length < 1e-4f) continue;
                AddEndpoint(endpoints, BucketKey(pipe.start, bucketSize), i,
                    (pipe.end - pipe.start).normalized, pipe.start);
                AddEndpoint(endpoints, BucketKey(pipe.end, bucketSize), i,
                    (pipe.start - pipe.end).normalized, pipe.end);
            }

            foreach (var kv in endpoints)
            {
                var list = kv.Value;
                if (list.Count < 2) continue;
                for (int a = 0; a < list.Count; a++)
                for (int b = a + 1; b < list.Count; b++)
                {
                    if (list[a].idx == list[b].idx) continue;
                    var dot = Vector3.Dot(list[a].dir, list[b].dir);
                    var bendAngle = Mathf.Acos(Mathf.Clamp(-dot, -1f, 1f)) * Mathf.Rad2Deg;
                    if (bendAngle < minBendAngleDeg)
                        result.Add(new SharpBend(list[a].idx, list[b].idx, bendAngle, list[a].pos));
                }
            }
            return result;
        }

        private static Vector3Int BucketKey(Vector3 p, float size)
        {
            return new Vector3Int(
                Mathf.RoundToInt(p.x / size),
                Mathf.RoundToInt(p.y / size),
                Mathf.RoundToInt(p.z / size));
        }

        private static void AddEndpoint(
            Dictionary<Vector3Int, List<(int idx, Vector3 dir, Vector3 pos)>> endpoints,
            Vector3Int key, int idx, Vector3 dir, Vector3 pos)
        {
            if (!endpoints.TryGetValue(key, out var list))
            {
                list = new List<(int, Vector3, Vector3)>();
                endpoints[key] = list;
            }
            list.Add((idx, dir, pos));
        }

        public static List<(int a, int b)> DetectClashes(
            IReadOnlyList<PipeData> pipes, float clearance = 0f)
        {
            var result = new List<(int, int)>();
            if (pipes == null || pipes.Count < 2) return result;

            for (int i = 0; i < pipes.Count; i++)
            {
                for (int j = i + 1; j < pipes.Count; j++)
                {
                    var a = pipes[i];
                    var b = pipes[j];
                    var minDist = ClosestDistanceBetweenSegments(
                        a.start, a.end, b.start, b.end);
                    var threshold = a.Radius + b.Radius + clearance;
                    if (minDist < threshold)
                        result.Add((i, j));
                }
            }
            return result;
        }

        public static float ClosestDistanceBetweenSegments(
            Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
        {
            var d1 = a1 - a0;
            var d2 = b1 - b0;
            var r = a0 - b0;
            var aDot = Vector3.Dot(d1, d1);
            var eDot = Vector3.Dot(d2, d2);
            var fDot = Vector3.Dot(d2, r);
            const float EPS = 1e-7f;

            float s, t;

            if (aDot <= EPS && eDot <= EPS)
                return r.magnitude;

            if (aDot <= EPS)
            {
                s = 0f;
                t = Mathf.Clamp01(fDot / eDot);
            }
            else
            {
                var cDot = Vector3.Dot(d1, r);
                if (eDot <= EPS)
                {
                    t = 0f;
                    s = Mathf.Clamp01(-cDot / aDot);
                }
                else
                {
                    var bDot = Vector3.Dot(d1, d2);
                    var denom = aDot * eDot - bDot * bDot;
                    s = denom > EPS
                        ? Mathf.Clamp01((bDot * fDot - cDot * eDot) / denom)
                        : 0f;
                    t = (bDot * s + fDot) / eDot;
                    if (t < 0f)
                    {
                        t = 0f;
                        s = Mathf.Clamp01(-cDot / aDot);
                    }
                    else if (t > 1f)
                    {
                        t = 1f;
                        s = Mathf.Clamp01((bDot - cDot) / aDot);
                    }
                }
            }

            var cp1 = a0 + d1 * s;
            var cp2 = b0 + d2 * t;
            return (cp1 - cp2).magnitude;
        }
    }
}
