using UnityEngine;

namespace SmartFactory.Piping.Viewer
{
    public class PipeViewItem : MonoBehaviour
    {
        [SerializeField] private string pipeId;

        public string PipeId
        {
            get => pipeId;
            set => pipeId = value;
        }
    }
}
