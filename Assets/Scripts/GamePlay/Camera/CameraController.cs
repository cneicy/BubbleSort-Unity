using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector2 lookDirection;
        [SerializeField] private float mouseSensitivity = 100f;
        private float _xRotation;
        

        private void RotateView()
        {
            var mouseX = lookDirection.x * mouseSensitivity * Time.deltaTime;
            var mouseY = lookDirection.y * mouseSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            player.Rotate(Vector3.up * mouseX);
        }
        public void ReadLookInput(InputAction.CallbackContext context)
        {
            lookDirection = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            RotateView();
        }
    }
}