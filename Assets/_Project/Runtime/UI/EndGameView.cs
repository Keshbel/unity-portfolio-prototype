using ExtractionRoom.Presentation;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class EndGameView : MonoBehaviour
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

        public void Hide()
        {
            fadeHandle.TryCancel();
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
        }

        public void ShowWin()
        {
            Show("Extraction Complete");
        }

        public void ShowLose()
        {
            Show("Mission Failed");
        }

        private void Show(string message)
        {
            label.text = message;
            canvasGroup.gameObject.SetActive(true);
            PresentationTweenHelper.Fade(this, canvasGroup, ref fadeHandle, 0f, 1f, 0.35f);
        }

        private void OnDestroy()
        {
            fadeHandle.TryCancel();
        }
    }
}
