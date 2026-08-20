using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Combo Data")]
    public sealed class ComboData : ScriptableObject
    {
        [SerializeField] private AttackData[] _attacks;

        public int Length => _attacks.Length;
        public AttackData this[int index] => _attacks[index];
    }
}