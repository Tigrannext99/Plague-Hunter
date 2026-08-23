using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Combo Data")]
    public sealed class ComboData : ScriptableObject
    {
        [SerializeField] private AttackData[] _attacks;

        public int Length => _attacks?.Length ?? 0;
        public AttackData this[int index] => _attacks[index];

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_attacks == null || _attacks.Length == 0)
            {
                Debug.LogError($"[ComboData] {name}: список атак пуст", this);
                return;
            }

            for (int i = 0; i < _attacks.Length; i++)
                if (_attacks[i] == null)
                    Debug.LogError($"[ComboData] {name}: пустой слот {i}", this);
        }
#endif
    }
}
