using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ExtractionRoom.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public Vector2 MoveInput
        {
            get
            {
                return new Vector2(
                    ReadAxis(keyboard => keyboard.aKey, keyboard => keyboard.dKey),
                    ReadAxis(keyboard => keyboard.sKey, keyboard => keyboard.wKey));
            }
        }

        public bool InteractPressed => IsPressed(keyboard => keyboard.eKey);

        private static float ReadAxis(
            System.Func<Keyboard, KeyControl> negativeSelector,
            System.Func<Keyboard, KeyControl> positiveSelector)
        {
            return (IsPressed(positiveSelector) ? 1f : 0f) - (IsPressed(negativeSelector) ? 1f : 0f);
        }

        private static bool IsPressed(System.Func<Keyboard, KeyControl> selector)
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is Keyboard keyboard && selector(keyboard).isPressed)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
