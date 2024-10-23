using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.Player
{
    public class Motion : MonoBehaviour
    {
        private Rigidbody _rb;
        [SerializeField] private Vector2 direction;
        [SerializeField] private float force;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float idleSpeed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void ReadMoveInput(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var move = transform.right * direction.x + transform.forward * direction.y;
            _rb.AddForce(move * (force * Time.fixedDeltaTime), ForceMode.VelocityChange);
            
            LimitSpeed();
        }

        private void LimitSpeed()
        {
            var velocity = _rb.linearVelocity;
            
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            velocity.z = Mathf.Clamp(velocity.z, -maxSpeed, maxSpeed);
            
            if (direction == Vector2.zero)
            {
                velocity.x = Mathf.Clamp(velocity.x, -idleSpeed, idleSpeed);
                velocity.z = Mathf.Clamp(velocity.z, -idleSpeed, idleSpeed);
            }
            
            _rb.linearVelocity = velocity;
        }
    }
}