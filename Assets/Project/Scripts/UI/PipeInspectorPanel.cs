using SmartFactory.Piping.Data;
using TMPro;
using UnityEngine;

namespace SmartFactory.Piping.UI
{
    public class PipeInspectorPanel : MonoBehaviour
    {
        [SerializeField] private SelectionState selection;
        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private TMP_Text outputText;

        [Header("Messages")]
        [SerializeField] private string emptyMessage = "Select a pipe to inspect";
        [SerializeField] private Color emptyColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        [SerializeField] private Color filledColor = Color.white;

        private void OnEnable()
        {
            if (selection != null)
            {
                selection.OnSelectionChanged += HandleSelection;
                HandleSelection(selection.SelectedId);
            }
            if (network != null)
                network.OnChanged += HandleNetworkChanged;
        }

        private void OnDisable()
        {
            if (selection != null)
                selection.OnSelectionChanged -= HandleSelection;
            if (network != null)
                network.OnChanged -= HandleNetworkChanged;
        }

        private void HandleNetworkChanged()
        {
            if (selection != null) HandleSelection(selection.SelectedId);
        }

        private void HandleSelection(string id)
        {
            if (outputText == null) return;

            if (string.IsNullOrEmpty(id) || network == null)
            {
                outputText.text = emptyMessage;
                outputText.color = emptyColor;
                return;
            }

            if (!network.TryGetPipe(id, out var pipe))
            {
                outputText.text = emptyMessage;
                outputText.color = emptyColor;
                return;
            }

            var idShort = string.IsNullOrEmpty(pipe.id)
                ? "?"
                : pipe.id.Substring(0, System.Math.Min(8, pipe.id.Length));

            var virtualPressure = EstimatePressureDrop(pipe);

            outputText.text =
                $"<b>Pipe</b> ({idShort})\n" +
                $"Diameter : {pipe.diameter:0.000} m\n" +
                $"Material : {pipe.material}\n" +
                $"Length   : {pipe.Length:0.000} m\n" +
                $"Virtual ΔP : {virtualPressure:0.000} (analytic; M3 → CFD surrogate)";
            outputText.color = filledColor;
        }

        private static float EstimatePressureDrop(PipeData pipe)
        {
            var d = Mathf.Max(pipe.diameter, 1e-3f);
            return pipe.Length / Mathf.Pow(d, 5f);
        }
    }
}
