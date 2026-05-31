using ExtractionRoom.Presentation;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        private MotionHandle pulseHandle;
        private Vector3 baseScale;

        public void Configure(Text text)
        {
            label = text;
            baseScale = transform.localScale;
        }

        public void Display(string itemId, int count, bool pulse)
        {
            label.text = $"{itemId} x{count}";
            if (pulse)
            {
                PresentationTweenHelper.PulseScale(this, transform, ref pulseHandle, baseScale, 1.12f, 0.24f);
            }
        }

        public void DisplayEmpty()
        {
            label.text = "-";
        }

        private void OnDestroy()
        {
            pulseHandle.TryCancel();
        }
    }
}
