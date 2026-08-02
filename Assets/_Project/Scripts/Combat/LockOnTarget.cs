using UnityEngine;

namespace PlagueHunter.Combat
{
    public class LockOnTarget : MonoBehaviour
    {
        [SerializeField] private Transform point;

        private Health _health;

        public Transform Point => point != null ? point : transform;
        public bool IsValid => _health == null || !_health.IsDead;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }
    }
}