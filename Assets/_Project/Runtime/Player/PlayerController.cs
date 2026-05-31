using UnityEngine;

namespace ExtractionRoom.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputReader inputReader;

        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        [SerializeField, Min(0f)]
        private float gravity = 20f;

        private CharacterController characterController;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputReader ??= GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            var moveInput = inputReader.MoveInput;
            var movement = new Vector3(moveInput.x, 0f, moveInput.y);

            if (movement.sqrMagnitude > 1f)
            {
                movement.Normalize();
            }

            if (movement.sqrMagnitude > 0f)
            {
                transform.forward = movement;
            }

            verticalVelocity = characterController.isGrounded ? -1f : verticalVelocity - gravity * Time.deltaTime;
            var velocity = movement * moveSpeed;
            velocity.y = verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
