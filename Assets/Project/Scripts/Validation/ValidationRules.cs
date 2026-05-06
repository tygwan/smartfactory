using System.Collections.Generic;
using SmartFactory.Piping.Data;
using UnityEngine;

namespace SmartFactory.Piping.Validation
{
    public static class ValidationRules
    {
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
