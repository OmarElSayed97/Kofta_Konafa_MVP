using UnityEngine;

namespace KoftaAndKonafa
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        // Movement speed of the character
        public float moveSpeed = 5f;

        // Animator parameter name for running animation
        private const string IsRunningBool = "Move";

        // References
        private Rigidbody _rigidbody;
        private Animator _animator;
        
        // Movement input
        private Vector3 _movementInput;

        private void Awake()
        {
            // Get components
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Get movement input
            _movementInput.x = Input.GetAxisRaw("Horizontal");
            _movementInput.z = Input.GetAxisRaw("Vertical");

            // Normalize the movement input to prevent faster diagonal movement
            _movementInput = _movementInput.normalized;

            // Set animator bool based on movement
            bool isRunning = _movementInput != Vector3.zero;
            _animator.SetBool(IsRunningBool, isRunning);

            // Rotate the character to face movement direction
            if (isRunning)
                transform.rotation = Quaternion.LookRotation(_movementInput);


        }

        private void FixedUpdate()
        {
            // Apply movement
            _rigidbody.velocity = _movementInput * moveSpeed;
        }

    }
}
