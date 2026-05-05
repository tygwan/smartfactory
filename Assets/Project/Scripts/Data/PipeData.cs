using UnityEngine;

namespace SmartFactory.Piping.Data
{
    [System.Serializable]
    public struct PipeData
    {
        public string id;
        public Vector3 start;
        public Vector3 end;
        public float diameter;
        public string material;

        public float Length => Vector3.Distance(start, end);

        public Vector3 Direction
        {
            get
            {
                var v = end - start;
                var len = v.magnitude;
                return len > 1e-6f ? v / len : Vector3.forward;
            }
        }

        public Vector3 Center => 0.5f * (start + end);

        public float Radius => 0.5f * diameter;
    }
}
