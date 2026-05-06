using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartFactory.Piping.Data
{
    [CreateAssetMenu(
        menuName = "SmartFactory/Pipe Network Asset",
        fileName = "PipeNetwork")]
    public class PipeNetworkAsset : ScriptableObject
    {
        [SerializeField] private List<PipeData> pipes = new();

        public IReadOnlyList<PipeData> Pipes => pipes;

        public int Count => pipes.Count;

        public event Action OnChanged;

        public PipeData AddPipe(Vector3 start, Vector3 end, float diameter, string material)
        {
            var pipe = new PipeData
            {
                id = Guid.NewGuid().ToString("N"),
                start = start,
                end = end,
                diameter = diameter,
                material = material,
            };
            pipes.Add(pipe);
            RaiseChanged();
            return pipe;
        }

        public void AddPipe(PipeData pipe)
        {
            if (string.IsNullOrEmpty(pipe.id))
                pipe.id = Guid.NewGuid().ToString("N");
            pipes.Add(pipe);
            RaiseChanged();
        }

        public bool UpdatePipe(PipeData updated)
        {
            var idx = pipes.FindIndex(p => p.id == updated.id);
            if (idx < 0) return false;
            pipes[idx] = updated;
            RaiseChanged();
            return true;
        }

        public bool RemovePipe(string id)
        {
            var idx = pipes.FindIndex(p => p.id == id);
            if (idx < 0) return false;
            pipes.RemoveAt(idx);
            RaiseChanged();
            return true;
        }

        public void Clear()
        {
            if (pipes.Count == 0) return;
            pipes.Clear();
            RaiseChanged();
        }

        public bool TryGetPipe(string id, out PipeData pipe)
        {
            var idx = pipes.FindIndex(p => p.id == id);
            if (idx < 0)
            {
                pipe = default;
                return false;
            }
            pipe = pipes[idx];
            return true;
        }

        public string ToJson(bool prettyPrint = true) => JsonUtility.ToJson(this, prettyPrint);

        public void LoadFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            OnChanged?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var changed = false;
            for (int i = 0; i < pipes.Count; i++)
            {
                if (string.IsNullOrEmpty(pipes[i].id))
                {
                    var p = pipes[i];
                    p.id = Guid.NewGuid().ToString("N");
                    pipes[i] = p;
                    changed = true;
                }
            }
            if (changed) RaiseChanged();
        }
#endif
    }
}
