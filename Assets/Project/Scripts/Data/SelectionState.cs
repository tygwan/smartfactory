using System;
using UnityEngine;

namespace SmartFactory.Piping.Data
{
    [CreateAssetMenu(
        menuName = "SmartFactory/Selection State",
        fileName = "SelectionState")]
    public class SelectionState : ScriptableObject
    {
        [SerializeField] private string selectedId = string.Empty;

        public string SelectedId => selectedId;

        public bool HasSelection => !string.IsNullOrEmpty(selectedId);

        public event Action<string> OnSelectionChanged;

        public void Select(string id)
        {
            if (selectedId == id) return;
            selectedId = id ?? string.Empty;
            RaiseChanged();
        }

        public void Clear()
        {
            if (string.IsNullOrEmpty(selectedId)) return;
            selectedId = string.Empty;
            RaiseChanged();
        }

        private void RaiseChanged()
        {
            OnSelectionChanged?.Invoke(selectedId);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
