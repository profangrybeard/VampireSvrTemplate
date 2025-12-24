using UnityEngine;
using VampireSurvivor.Data;

namespace VampireSurvivor.Core
{
    // Smooth camera follow for 2D gameplay.
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameConfig _config;

        private Vector3 _offset;
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            if (_target != null)
            {
                _offset = transform.position - _target.position;
            }

            if (_config != null && _camera != null)
            {
                _camera.orthographicSize = _config.CameraOrthoSize;
            }
        }

        private void LateUpdate()
        {
            if (_target == null || _config == null) return;

            Vector3 targetPosition = _target.position + _offset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                _config.CameraFollowSpeed * Time.deltaTime
            );
        }

        // Set the camera target at runtime.
        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null)
            {
                _offset = transform.position - _target.position;
            }
        }
    }
}
