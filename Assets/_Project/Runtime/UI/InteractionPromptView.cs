using ExtractionRoom.Interaction;
using ExtractionRoom.Presentation;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private CanvasGroup canvasGroup;

        private MotionHandle fadeHandle;

        public void Configure(Text text, CanvasGroup group)
        {
            label = text;
            canvasGroup = group;
        }

        public void Display(InteractionPromptData? prompt)
        {
            if (prompt.HasValue)
            {
                label.text = $"[E] {prompt.Value.ActionText}";
                canvasGroup.gameObject.SetActive(true);
                PresentationTweenHelper.Fade(this, canvasGroup, ref fadeHandle, 0f, 1f, 0.28f);
                return;
            }

            PresentationTweenHelper.FadeAndDeactivate(this, canvasGroup, ref fadeHandle, 0.2f);
        }

        private void OnDestroy()
        {
            fadeHandle.TryCancel();
        }
    }
}
