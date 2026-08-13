using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlagueHunter.Core
{
    public sealed class GameplayInputReader : PlayerControls.IGameplayActions, IDisposable
    {
        private readonly PlayerControls _controls;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action AttackPressed;
        public event Action DodgePressed;
        public event Action LockOnPressed;

        public GameplayInputReader()
        {
            _controls = new PlayerControls();
            _controls.Gameplay.SetCallbacks(this);
        }

        public void Enable() => _controls.Gameplay.Enable();
        public void Disable() => _controls.Gameplay.Disable();

        public void Dispose()
        {
            _controls.Gameplay.RemoveCallbacks(this);
            _controls.Disable();
            _controls.Dispose();
        }

        public void OnMove(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext ctx) => Look = ctx.ReadValue<Vector2>();

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if(ctx.performed) AttackPressed?.Invoke();
        } 

        public void OnDodge(InputAction.CallbackContext ctx)
        {
            if(ctx.performed) DodgePressed?.Invoke();
        } 

        public void OnLockOn(InputAction.CallbackContext ctx)
        {
            if(ctx.performed) LockOnPressed?.Invoke();
        } 
    }    
}