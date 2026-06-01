using System;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using LitMotion;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.Presentation
{
    public sealed class GeneratorActivationView : MonoBehaviour
    {
        private MotionHandle pulseHandle;
        private IDisposable activationSubscription;
        private Vector3 baseScale;

        [SerializeField]
        private Renderer statusRenderer;

        [SerializeField]
        private Color activatedColor = new Color(0.2f, 1f, 0.35f);

        public void Configure(Renderer renderer, Color color)
        {
            statusRenderer = renderer;
            activatedColor = color;
        }

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            activationSubscription?.Dispose();
            baseScale = transform.localScale;
            activationSubscription = eventBus.Subscribe<GeneratorActivatedEvent>(_ => Play());
        }

        private void OnDestroy()
        {
            activationSubscription?.Dispose();
            pulseHandle.TryCancel();
        }

        private void Play()
        {
            if (statusRenderer != null)
            {
                statusRenderer.material.color = activatedColor;
            }

            PresentationTweenHelper.PulseScale(this, transform, ref pulseHandle, baseScale, 1.25f, 0.6f);
        }
    }
}
