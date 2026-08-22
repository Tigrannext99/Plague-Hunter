using UnityEngine;

namespace PlagueHunter.Combat
{
    [CreateAssetMenu(menuName = "PlagueHunter/Dodge Data")]
    public sealed class DodgeData : ScriptableObject
    {
        [SerializeField] private string _stateName = "Dodge";
        [SerializeField] private float _duration = 0.9f;
        [SerializeField] private float _crossFade = 0.05f;

        [Header("I-frames (seconds)")]
        [SerializeField] private float _iFramesStart = 0.05f;
        [SerializeField] private float _iFramesEnd = 0.45f;

        [Header("Recovery (seconds)")]
        [SerializeField] private float _cancelTime = 0.6f;

        private int _stateHash = -1;

        public string StateName => _stateName;

        public int StateHash
        {
            get
            {
                if (_stateHash == -1) _stateHash = Animator.StringToHash(_stateName);
                return _stateHash;
            }
        }

        public float Duration => _duration;
        public float CrossFade => _crossFade;
        public float IFramesStart => _iFramesStart;
        public float IFramesEnd => _iFramesEnd;
        public float CancelTime => _cancelTime;

        private void OnValidate()
        {
            _stateHash = -1;

            _iFramesEnd = Mathf.Max(_iFramesEnd, _iFramesStart);
            _duration = Mathf.Max(_duration, _iFramesEnd);
            _cancelTime = Mathf.Clamp(_cancelTime, 0f, _duration);
        }
    }
}