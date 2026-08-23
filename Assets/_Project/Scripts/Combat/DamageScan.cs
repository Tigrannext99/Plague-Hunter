using System.Collections.Generic;
using UnityEngine;

namespace PlagueHunter.Combat
{
    /// <summary>
    /// Общий поиск целей по перекрытию и выдача урона.
    /// Раньше этот цикл был продублирован в атаке игрока и в атаке болванки.
    /// </summary>
    public static class DamageScan
    {
        public static int Box(
            Vector3 center,
            Vector3 halfExtents,
            Quaternion rotation,
            LayerMask mask,
            Collider[] buffer)
        {
            return Physics.OverlapBoxNonAlloc(
                center, halfExtents, buffer, rotation, mask, QueryTriggerInteraction.Ignore);
        }

        public static int Sphere(Vector3 center, float radius, LayerMask mask, Collider[] buffer)
        {
            return Physics.OverlapSphereNonAlloc(
                center, radius, buffer, mask, QueryTriggerInteraction.Ignore);
        }

        /// <param name="alreadyHit">
        /// Необязательный фильтр повторных попаданий в пределах одной атаки.
        /// </param>
        public static void Apply(
            Collider[] buffer,
            int count,
            float damage,
            HashSet<IDamageable> alreadyHit = null)
        {
            for (int i = 0; i < count; i++)
            {
                if (!buffer[i].TryGetComponent(out IDamageable damageable)) continue;
                if (alreadyHit != null && !alreadyHit.Add(damageable)) continue;

                damageable.TakeDamage(damage);
            }
        }
    }
}
