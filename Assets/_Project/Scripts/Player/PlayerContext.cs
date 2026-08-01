using PlagueHunter.Combat;
using PlagueHunter.Core;
using UnityEngine;

namespace PlagueHunter.Player
{
    public class PlayerContext
    {
        public readonly Transform Transform;
        public readonly CharacterController Controller;
        public readonly Animator Animator;
        public readonly PlayerInputReader Input;
        public readonly PlayerConfig Config;
        public readonly Transform CameraTransform;
        public readonly StateMachine StateMachine;
        public readonly PlayerController PlayerController;
        public readonly HitStop HitStop;

        public float CurrentSpeed;
        public float VerticalVelocity;

        public PlayerContext(
            Transform transform,
            CharacterController controller,
            Animator animator,
            PlayerInputReader input,
            PlayerConfig config,
            Transform cameraTransform,
            StateMachine stateMachine,
            PlayerController playerController,
            HitStop hitStop)
        {
            Transform = transform;
            Controller = controller;
            Animator = animator;
            Input = input;
            Config = config;
            CameraTransform = cameraTransform;
            StateMachine = stateMachine;
            PlayerController = playerController;
            HitStop = hitStop;
        }
    }
}