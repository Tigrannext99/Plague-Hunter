using UnityEngine;

namespace PlagueHunter.Core
{
    public sealed class GameplayRoot : MonoBehaviour
    {
        private GameplayInputReader _input;

        public void Compose(GameplayInputReader input)
        {
            _input = input;

            _input.AttackPressed += OnAttackPressed;
            _input.DodgePressed += OnDodgePressed;
            _input.LockOnPressed += OnLockOnPressed;

            Debug.Log("[GameplayRoot] Composed");
        }

        private void OnDestroy()
        {
            if (_input == null) return;

            _input.AttackPressed -= OnAttackPressed;
            _input.DodgePressed -= OnDodgePressed;
            _input.LockOnPressed -= OnLockOnPressed;
        }

        private void Update()
        {
            if (_input == null) return;
            if (_input.Move.sqrMagnitude > 0.01f) Debug.Log($"Move {_input.Move}");
            if (_input.Look.sqrMagnitude > 0.01f) Debug.Log($"Look {_input.Look}");
        }

        private void OnAttackPressed() => Debug.Log("Attack");
        private void OnDodgePressed() => Debug.Log("Dodge");
        private void OnLockOnPressed() => Debug.Log("LockOn");
    }
}