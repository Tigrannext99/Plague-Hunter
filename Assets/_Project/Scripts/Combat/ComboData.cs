using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Combo Data", fileName = "ComboData")]
    public class ComboData : ScriptableObject
    {
        public AttackData[] attacks;

        public AttackData Get(int index)
        {
            if (attacks == null || index < 0 || index >= attacks.Length)
                return null;

            return attacks[index];
        }

        public bool HasNext(int index) => attacks != null && index + 1 < attacks.Length;
    }
}