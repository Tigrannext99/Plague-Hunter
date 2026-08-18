using PlagueHunter.Player;
using UnityEngine;

namespace PlagueHunter.Core
{
    public sealed class GameplayRoot : MonoBehaviour
    {
        [SerializeField] private PlayerRoot _player;

        private GameplayInputReader _input;

        public void Compose(GameplayInputReader input)
        {
            _input = input;

            _input.AttackPressed += OnAttackPressed;
            _input.DodgePressed += OnDodgePressed;
            _input.LockOnPressed += OnLockOnPressed;

            _player.Compose(input, Camera.main.transform);

            Debug.Log("[GameplayRoot] Composed");
        }

        private void OnDestroy()
        {
            if (_input == null) return;

            _input.AttackPressed -= OnAttackPressed;
            _input.DodgePressed -= OnDodgePressed;
            _input.LockOnPressed -= OnLockOnPressed;
        }

        private void OnAttackPressed() => Debug.Log("Attack");
        private void OnDodgePressed() => Debug.Log("Dodge");
        private void OnLockOnPressed() => Debug.Log("LockOn");
    }
}