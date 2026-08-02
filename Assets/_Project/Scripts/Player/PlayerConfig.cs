using PlagueHunter.Combat;
using UnityEngine;

namespace PlagueHunter.Player
{
    [CreateAssetMenu(menuName = "PlagueHunter/Player Config", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        public float walkSpeed = 2f;
        public float runSpeed = 5.5f;
        public float speedSharpness = 14f;
        public float rotationSharpness = 16f;

        [Header("Gravity")]
        public float gravity = -25f;
        public float groundedStick = -2f;

        [Header("Animation")]
        public float animDampTime = 0.1f;

        [Header("Combat")]
        public LayerMask enemyLayers;
        public ComboData[] combos;

        [Header("Attack")]
        public float attackTurnDuration = 0.15f;
        public float attackTurnSpeed = 720f;

        [Header("Lock-On")]
        public float lockOnRadius = 12f;
        public float lockOnMaxAngle = 60f;
        public float lockOnBreakDistance = 16f;

        public ComboData GetRandomCombo()
        {
            if (combos == null || combos.Length == 0) return null;
            return combos[Random.Range(0, combos.Length)];
        }
    }
}