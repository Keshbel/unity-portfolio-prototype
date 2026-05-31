using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class ObjectiveView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void Configure(Text text)
        {
            label = text;
        }

        public void Display(string objectiveText)
        {
            label.text = $"Objective: {objectiveText}";
        }
    }
}
