using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.Presentation
{
    public static class PresentationTweenHelper
    {
        public static void Fade(
            MonoBehaviour owner,
            CanvasGroup canvasGroup,
            ref MotionHandle handle,
            float from,
            float to,
            float duration)
        {
            handle.TryCancel();
            canvasGroup.alpha = from;
            handle = LMotion.Create(from, to, duration)
                .WithEase(Ease.OutQuad)
                .Bind(canvasGroup, (alpha, group) => group.alpha = alpha)
                .AddTo(owner);
        }

        public static void FadeAndDeactivate(
            MonoBehaviour owner,
            CanvasGroup canvasGroup,
            ref MotionHandle handle,
            float duration)
        {
            handle.TryCancel();
            handle = LMotion.Create(canvasGroup.alpha, 0f, duration)
                .WithEase(Ease.OutQuad)
                .WithOnComplete(() => canvasGroup.gameObject.SetActive(false))
                .Bind(canvasGroup, (alpha, group) => group.alpha = alpha)
                .AddTo(owner);
        }

        public static void PulseScale(
            MonoBehaviour owner,
            Transform target,
            ref MotionHandle handle,
            Vector3 baseScale,
            float multiplier,
            float duration)
        {
            handle.TryCancel();
            target.localScale = baseScale;
            handle = LMotion.Create(baseScale, baseScale * multiplier, duration * 0.5f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .Bind(target, (scale, transform) => transform.localScale = scale)
                .AddTo(owner);
        }

        public static void FlashColor(
            MonoBehaviour owner,
            Graphic graphic,
            ref MotionHandle handle,
            Color baseColor,
            Color flashColor,
            float duration)
        {
            handle.TryCancel();
            graphic.color = baseColor;
            handle = LMotion.Create(baseColor, flashColor, duration * 0.5f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .Bind(graphic, (color, target) => target.color = color)
                .AddTo(owner);
        }
    }
}
