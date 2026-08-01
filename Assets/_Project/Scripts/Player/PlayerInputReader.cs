using System;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class PlayerInputReader : IDisposable
    {
        private readonly PlayerControls _controls;

        public Vector2 Move => _controls.Player.Move.ReadValue<Vector2>();
        public bool RunHeld => _controls.Player.Run.IsPressed();

        public PlayerInputReader()
        {
            _controls = new PlayerControls();
            _controls.Enable();
        }

        public void Dispose() => _controls.Dispose();
    }
}