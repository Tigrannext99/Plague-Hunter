using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Player Attack Data")]
    public sealed class PlayerAttackData : AttackData
    {
        [Header("Combo window (seconds)")]
        [SerializeField] private float _comboStart = 0.4f;
        [SerializeField] private float _comboEnd = 0.8f;

        [Header("Recovery (seconds)")]
        [SerializeField] private float _cancelTime = 0.35f;

        [Header("Hit shape")]
        [SerializeField] private Vector3 _hitBoxHalfExtents = new Vector3(0.15f, 0.15f, 0.5f);

        [Header("Feedback")]
        [SerializeField] private float _impulseScale = 1f;

        public float ComboStart => _comboStart;
        public float ComboEnd => _comboEnd;
        public float CancelTime => _cancelTime;
        public Vector3 HitBoxHalfExtents => _hitBoxHalfExtents;
        public float ImpulseScale => _impulseScale;

        protected override void OnValidate()
        {
            _comboEnd = Mathf.Max(_comboEnd, _comboStart);
            RequireDuration(_comboEnd);

            base.OnValidate();

            // Отмена доступна на всей длине атаки — раннее прерывание
            // это осознанный размен урона на выживание.
            _cancelTime = Mathf.Clamp(_cancelTime, 0f, Duration);

            _impulseScale = Mathf.Max(_impulseScale, 0f);
        }
    }
}
