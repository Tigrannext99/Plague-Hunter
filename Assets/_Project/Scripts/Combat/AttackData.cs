using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Attack Data", fileName = "AttackData")]
    public class AttackData : ScriptableObject
    {
        [Header("Animation")]
        public string animationStateName = "Attack1";
        public float crossFadeDuration = 0.1f;

        [Header("Timing, seconds")]
        public float duration = 1f;
        public float hitStart = 0.3f;
        public float hitEnd = 0.45f;

        [Header("Hitbox")]
        public Vector3 hitboxOffset = new Vector3(0f, 1f, 1f);
        public Vector3 hitboxSize = new Vector3(1f, 1f, 1.5f);

        [Header("Damage")]
        public int damage = 20;
    }
}