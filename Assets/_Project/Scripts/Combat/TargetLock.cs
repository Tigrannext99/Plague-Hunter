using PlagueHunter.Combat;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class TargetLock : MonoBehaviour
    {
        private PlayerConfig _config;
        private Transform _camera;

        private LockOnTarget _current;

        public bool IsLocked => _current != null;
        public Transform CurrentPoint => _current != null ? _current.Point : null;

        public void Init(PlayerConfig config, Transform cameraTransform)
        {
            _config = config;
            _camera = cameraTransform;
        }

        public void Toggle()
        {
            if (IsLocked)
            {
                Clear();
                return;
            }

            _current = FindTarget();

            if (_current != null)
                Debug.Log($"Lock on: {_current.name}");
        }

        private void Update()
        {
            if (!IsLocked) return;

            if (!_current.IsValid)
            {
                Clear();
                return;
            }

            float sqrDist = (_current.Point.position - transform.position).sqrMagnitude;
            float breakDist = _config.lockOnBreakDistance;

            if (sqrDist > breakDist * breakDist)
                Clear();
        }

        private void Clear()
        {
            _current = null;
            Debug.Log("Lock off");
        }

        private LockOnTarget FindTarget()
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                _config.lockOnRadius,
                _config.enemyLayers);

            Vector3 camForward = _camera.forward;
            camForward.y = 0f;
            camForward.Normalize();

            LockOnTarget best = null;
            float bestAngle = _config.lockOnMaxAngle;

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].TryGetComponent(out LockOnTarget candidate)) continue;
                if (!candidate.IsValid) continue;

                Vector3 toTarget = candidate.Point.position - transform.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude < 0.0001f) continue;

                float angle = Vector3.Angle(camForward, toTarget);

                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    best = candidate;
                }
            }

            return best;
        }
    }
}