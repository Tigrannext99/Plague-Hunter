using System.Collections;
using UnityEngine;

namespace PlagueHunter.Combat
{
    public class HitStop : MonoBehaviour
    {
        [SerializeField] private float scale = 0.08f;
        [SerializeField] private float duration = 0.07f;

        private Coroutine _routine;

        public void Play()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            Time.timeScale = scale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
            _routine = null;
        }

        private void OnDisable() => Time.timeScale = 1f;
    }
}