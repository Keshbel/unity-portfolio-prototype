using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.Presentation
{
    public sealed class DamageFeedbackView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private Color damageColor = new(1f, 0.25f, 0.25f);

        private MotionHandle colorHandle;
        private MotionHandle scaleHandle;
        private Color baseColor;
        private Vector3 baseScale;

        public void Configure(Text text)
        {
            label = text;
            CaptureBaseValues();
        }

        public void Play()
        {
            CaptureBaseValuesIfNeeded();
            PresentationTweenHelper.FlashColor(this, label, ref colorHandle, baseColor, damageColor, 0.45f);
            PresentationTweenHelper.PulseScale(this, transform, ref scaleHandle, baseScale, 1.18f, 0.45f);
        }

        private void Awake()
        {
            CaptureBaseValuesIfNeeded();
        }

        private void OnDestroy()
        {
            colorHandle.TryCancel();
            scaleHandle.TryCancel();
        }

        private void CaptureBaseValuesIfNeeded()
        {
            if (label != null && baseScale == default)
            {
                CaptureBaseValues();
            }
        }

        private void CaptureBaseValues()
        {
            baseColor = label.color;
            baseScale = transform.localScale;
        }
    }
}
