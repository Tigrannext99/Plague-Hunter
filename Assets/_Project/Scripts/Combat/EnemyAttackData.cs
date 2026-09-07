using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Enemy Attack Data")]
    public sealed class EnemyAttackData : AttackData
    {
        [Header("Approach")]
        [Tooltip("Дистанция до цели, с которой враг решает начать эту атаку")]
        [SerializeField] private float _range = 1.8f;

        [Header("Hit shape")]
        [Tooltip("Насколько далеко перед врагом ставится точка удара")]
        [SerializeField] private float _reach = 1.6f;
        [SerializeField] private float _hitRadius = 1.8f;

        [Header("Recovery (seconds)")]
        [Tooltip("Пауза после атаки, прежде чем враг снова начнёт выбирать действие")]
        [SerializeField] private float _cooldown = 1.5f;

        [Header("Telegraph")]
        [Tooltip("Показывать наземный маркер во время замаха (от 0 до Hit Start)")]
        [SerializeField] private bool _showTelegraph = true;

        public float Range => _range;
        public float Reach => _reach;
        public float HitRadius => _hitRadius;
        public float Cooldown => _cooldown;
        public bool ShowTelegraph => _showTelegraph;

        /// <summary>Замах — это всё, что до окна урона: по нему заполняется телеграф.</summary>
        public float Windup => HitStart;

        protected override void OnValidate()
        {
            base.OnValidate();

            _reach = Mathf.Max(_reach, 0f);
            _hitRadius = Mathf.Max(_hitRadius, 0.01f);
            _cooldown = Mathf.Max(_cooldown, 0f);

            // Начинать атаку дальше, чем она достаёт, значит гарантированно мазать.
            _range = Mathf.Clamp(_range, 0f, _reach + _hitRadius);
        }
    }
}
