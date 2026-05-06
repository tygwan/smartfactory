using System.Collections.Generic;
using SmartFactory.Piping.Data;
using SmartFactory.Piping.Viewer;
using TMPro;
using UnityEngine;

namespace SmartFactory.Piping.UI
{
    public class ClashAlertPanel : MonoBehaviour
    {
        [SerializeField] private PipeView pipeView;
        [SerializeField] private PipeNetworkAsset network;
        [SerializeField] private TMP_Text outputText;

        [Header("Messages")]
        [SerializeField] private string emptyMessage = "✓ No clashes detected";
        [SerializeField] private Color emptyColor = new Color(0.4f, 0.85f, 0.45f, 1f);
        [SerializeField] private Color clashColor = new Color(0.92f, 0.27f, 0.27f, 1f);

        private void OnEnable()
        {
            if (pipeView != null)
                pipeView.OnClashesUpdated += HandleClashes;
            RenderEmpty();
        }

        private void OnDisable()
        {
            if (pipeView != null)
                pipeView.OnClashesUpdated -= HandleClashes;
        }

        private void HandleClashes(IReadOnlyList<(int a, int b)> clashes)
        {
            if (outputText == null) return;

            if (clashes == null || clashes.Count == 0)
            {
                RenderEmpty();
                return;
            }

            var lines = new List<string>(clashes.Count + 1)
            {
                $"⚠ {clashes.Count} clash{(clashes.Count > 1 ? "es" : "")} detected"
            };

            foreach (var (i, j) in clashes)
                lines.Add($"  • {LabelFor(i)} ↔ {LabelFor(j)}");

            outputText.text = string.Join("\n", lines);
            outputText.color = clashColor;
        }

        private void RenderEmpty()
        {
            if (outputText == null) return;
            outputText.text = emptyMessage;
            outputText.color = emptyColor;
        }

        private string LabelFor(int index)
        {
            if (network == null || index < 0 || index >= network.Pipes.Count)
                return $"#{index}";
            var pipe = network.Pipes[index];
            var idShort = string.IsNullOrEmpty(pipe.id)
                ? "?"
                : pipe.id.Substring(0, System.Math.Min(6, pipe.id.Length));
            return $"#{index}({idShort})";
        }
    }
}
