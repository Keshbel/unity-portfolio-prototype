using ExtractionRoom.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void Configure(Text text)
        {
            label = text;
        }

        public void Display(InteractionPromptData? prompt)
        {
            var hasPrompt = prompt.HasValue;
            label.gameObject.SetActive(hasPrompt);
            label.text = hasPrompt ? $"[E] {prompt.Value.ActionText}" : string.Empty;
        }
    }
}
