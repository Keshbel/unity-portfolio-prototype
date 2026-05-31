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

        [Inject]
        public void Construct(IEventBus eventBus)
        {
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
            PresentationTweenHelper.PulseScale(this, transform, ref pulseHandle, baseScale, 1.25f, 0.6f);
        }
    }
}
