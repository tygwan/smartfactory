using SmartFactory.Piping.Viewer;
using UnityEngine;
using UnityEngine.UI;

namespace SmartFactory.Piping.UI
{
    public class LayerToggleController : MonoBehaviour
    {
        [Header("Layer roots")]
        [SerializeField] private GameObject[] fabBackgroundRoots;
        [SerializeField] private GameObject pipingRoot;
        [SerializeField] private GameObject clashAlertRoot;
        [SerializeField] private PipeView pipeView;

        [Header("Toggles (UGUI)")]
        [SerializeField] private Toggle fabToggle;
        [SerializeField] private Toggle pipingToggle;
        [SerializeField] private Toggle clashToggle;

        private void OnEnable()
        {
            if (fabToggle != null)
            {
                fabToggle.onValueChanged.AddListener(SetFabActive);
                SetFabActive(fabToggle.isOn);
            }
            if (pipingToggle != null)
            {
                pipingToggle.onValueChanged.AddListener(SetPipingActive);
                SetPipingActive(pipingToggle.isOn);
            }
            if (clashToggle != null)
            {
                clashToggle.onValueChanged.AddListener(SetClashActive);
                SetClashActive(clashToggle.isOn);
            }
        }

        private void OnDisable()
        {
            if (fabToggle != null) fabToggle.onValueChanged.RemoveListener(SetFabActive);
            if (pipingToggle != null) pipingToggle.onValueChanged.RemoveListener(SetPipingActive);
            if (clashToggle != null) clashToggle.onValueChanged.RemoveListener(SetClashActive);
        }

        private void SetFabActive(bool active)
        {
            if (fabBackgroundRoots == null) return;
            foreach (var go in fabBackgroundRoots)
                if (go != null) go.SetActive(active);
        }

        private void SetPipingActive(bool active)
        {
            if (pipingRoot != null) pipingRoot.SetActive(active);
        }

        private void SetClashActive(bool active)
        {
            if (pipeView != null) pipeView.ShowClashHighlights = active;
            if (clashAlertRoot != null) clashAlertRoot.SetActive(active);
        }
    }
}
