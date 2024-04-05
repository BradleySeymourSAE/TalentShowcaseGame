using Animancer;
using Framework.Scripts.Core.AI;
using Framework.Scripts.Core.AI.Strategies;
using Framework.Scripts.Core.ObjectPool;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Framework.Scripts.Core.Player
{
    [CreateAssetMenu(fileName = "Character Settings", menuName = "Player/Character Settings")] 
    public class HeroSettings : CharacterSettings 
    {
        public float FootstepInterval_Walking = 0.4f;
        public float FootstepInterval_Running = 0.2f;
        public float MinAirTimeForLandedSound = 0.2f;
        
        public FlyweightSettings JumpEffect;
        public FlyweightSettings LandEffect;
        public Color ParticleColor;

        public WeaponStrategy WeaponStrategy; 
        
        // Input
        public InputActionAsset InputActions;
        [HideInInspector] public InputAction MovementInputAction;
        [HideInInspector] public InputAction LookInputAction;
        [HideInInspector] public InputAction CrouchInputAction; 
        [HideInInspector] public InputAction JumpInputAction;
        [HideInInspector] public InputAction DashInputAction;
        [HideInInspector] public InputAction AttackInputAction;
        [HideInInspector] public InputAction InteractInputAction;
    }
}