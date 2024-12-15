using UnityEngine;

namespace KoftaAndKonafa.Helpers
{
    public class WorldCanvasLookAt : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            // Cache the main camera reference
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera != null)
            {
                // Make the canvas face the camera
                transform.LookAt(transform.position + _mainCamera.transform.forward);
            }
        }
    }
}