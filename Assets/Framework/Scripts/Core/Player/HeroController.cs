using Animancer;
using Framework.Common;
using Framework.Scripts.Common;
using Framework.Scripts.Common.Injection;
using Framework.Scripts.Core.AI;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Systems;
using Framework.Scripts.Core.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Framework.Scripts.Core.Player
{

    [System.Serializable]
    public class StateContext
    {
        public IDamageProvider SourceProvider; 
        
        public Rigidbody2D LinkedRigidbody;
        public Collider2D LinkedCollider;
        public HybridAnimancerComponent LinkedAnimator;
        public MovementController LinkedMovementController; 
        public HealthComponent HealthComponent;
        public LocalDetectionTree LocalDetectionTree;

        public Vector2 Input_Move;
        public Vector2 Input_Look;
        public bool Input_Jump;
        public bool Input_Run; 
        public bool Input_Crouch;
        public bool Input_Dash;
        public bool Input_PrimaryAction; 
        public bool Input_SecondaryAction;

        public bool IsGrounded;
        public bool IsRunning;
        public bool IsCrouched;
        public bool IsMovementLocked;
        public bool IsLookingLocked;

        public Transform CurrentParent;
        public bool IsFacingRight;
        public bool IsInCrouchTransition;
        public bool TargetCrouchState;
        public float CrouchTransitionProgress = 1.0f;
        public float TimeInAir = 0.0f;

        public float LastAttackTime = 0.0f;  
        
        public Vector3 LastRequestedVelocity = Vector3.zero;
        public Vector3 UpVector => Vector3.up;
        public Vector3 DownVector => Vector3.down;
        public Vector3 RightVector => Vector3.right; 

        public void Update(float dt)
        {
            LocalDetectionTree?.Detections?.UpdatePositions(dt); 
        }
    }
    
    [DefaultExecutionOrder(-1000)]
    public class HeroController : Singleton<HeroController>, IDependencyProvider, IEntity
    {

        [Provider]
        public HeroController ProvideHeroController() {
            return this; 
        } 
        
        [SerializeField,HideInInspector] protected internal StateContext State = new();
        [SerializeField] protected internal HeroSettings Settings; 

        [Header("Debug Controls")]
        [SerializeField] protected bool DEBUG_OverrideMovement = false;
        [SerializeField] protected Vector2 DEBUG_MovementInput;
        [SerializeField] protected bool DEBUG_ToggleLookLock = false;
        [SerializeField] protected bool DEBUG_ToggleMovementLock = false;
        
        public IWeapon Weapon { get; protected set; } 

        protected void Awake()
        {
            State.LinkedRigidbody = GetComponent<Rigidbody2D>(); 
            State.LinkedCollider = GetComponent<Collider2D>();
            State.LinkedMovementController = GetComponent<MovementController>(); 
            State.LinkedAnimator = GetComponentInChildren<HybridAnimancerComponent>();
            State.HealthComponent = GetComponent<HealthComponent>();
            State.HealthComponent.Initialize(this, Settings);
            Weapon = GetComponent<IWeapon>();
            Weapon.Initialize(State);
            console.log(this, Settings.WeaponStrategy.name); 
            Weapon.SetWeaponStrategy(Settings.WeaponStrategy); 
        }

        protected override void OnEnable()
        {
            base.OnEnable(); 
            State.HealthComponent.OnDamageTaken += OnDamageTaken;
            State.HealthComponent.OnDeath += OnDeath;
            GameStateManager.OnRespawn += HandleOnRespawn; 
        }

        private void HandleOnRespawn()
        {
            State.HealthComponent.OnPerformHeal(gameObject, Settings.MaxHealth); 
            State.LinkedAnimator.SetBool(Animations.DieFront, false); 
        }

        protected override void OnDisable()
        {
            base.OnDisable(); 
            State.HealthComponent.OnDamageTaken -= OnDamageTaken; 
            State.HealthComponent.OnDeath -= OnDeath; 
            GameStateManager.OnRespawn -= HandleOnRespawn; 
        }

        protected Vector2 GetMovementInput()
        {
            return Settings?.MovementInputAction?.ReadValue<Vector2>() ?? Vector2.zero; 
        }
        
        protected Vector2 GetLookInput()
        {
            return Settings?.LookInputAction?.ReadValue<Vector2>() ?? Vector2.zero; 
        }
        
        public void EnablePlayerInput()
        {
            console.log(this, "Enabling Input Controls"); 

            if (Settings == null || Settings.InputActions == null)
            {
                console.log(this,"No InputActions assigned to Character '{0}'", name); 
                return;
            }
            Settings.MovementInputAction = Settings.InputActions.FindAction("Movement"); 
            Settings.LookInputAction = Settings.InputActions.FindAction("Look"); 
            Settings.CrouchInputAction = Settings.InputActions.FindAction("Crouch"); 
            Settings.JumpInputAction = Settings.InputActions.FindAction("Jump"); 
            Settings.DashInputAction = Settings.InputActions.FindAction("Dash"); 
            Settings.AttackInputAction = Settings.InputActions.FindAction("Attack"); 
            Settings.InteractInputAction = Settings.InputActions.FindAction("Interact");

            Settings.MovementInputAction?.Enable(); 
            Settings.LookInputAction?.Enable(); 
            
            
            if (Settings.CrouchInputAction != null)
            {
                Settings.CrouchInputAction.started += OnCrouchCallback; 
                Settings.CrouchInputAction.performed += OnCrouchCallback; 
                Settings.CrouchInputAction.canceled += OnCrouchCallback; 
                
                Settings.CrouchInputAction.Enable();
            } 
            if (Settings.JumpInputAction != null)
            {
                Settings.JumpInputAction.started += OnJumpCallback;
                Settings.JumpInputAction.performed += OnJumpCallback; 
                Settings.JumpInputAction.canceled += OnJumpCallback; 
                
                Settings.JumpInputAction.Enable(); 
            } 
            if (Settings.DashInputAction != null)
            {
                Settings.DashInputAction.started += OnDashCallback;
                Settings.DashInputAction.performed += OnDashCallback; 
                Settings.DashInputAction.canceled += OnDashCallback; 
                
                Settings.DashInputAction.Enable(); 
            } 
            
            if (Settings.AttackInputAction != null)
            {
                Settings.AttackInputAction.started += OnAttackCallback;
                Settings.AttackInputAction.performed += OnAttackCallback; 
                Settings.AttackInputAction.canceled += OnAttackCallback; 
                
                Settings.AttackInputAction.Enable(); 
            } 
            
            if (Settings.InteractInputAction != null)
            {
                Settings.InteractInputAction.started += OnInteractCallback;
                Settings.InteractInputAction.performed += OnInteractCallback; 
                Settings.InteractInputAction.canceled += OnInteractCallback; 
                
                Settings.InteractInputAction.Enable(); 
            } 
        }
        
        public void DisablePlayerInput()
        {
            console.log(this, "Disabling input controls..."); 
            if (Settings.MovementInputAction != null)
            {
                Settings.MovementInputAction.Disable();
                Settings.MovementInputAction = null; 
            } 
            if (Settings.LookInputAction != null)
            {
                Settings.LookInputAction.Disable();
                Settings.LookInputAction = null; 
            } 
            if (Settings.CrouchInputAction != null)
            {
                Settings.CrouchInputAction.Disable();
                Settings.CrouchInputAction.started -= OnCrouchCallback;
                Settings.CrouchInputAction.performed -= OnCrouchCallback; 
                Settings.CrouchInputAction.canceled -= OnCrouchCallback; 
                Settings.CrouchInputAction = null; 
            } 
            if (Settings.JumpInputAction != null)
            {
                Settings.JumpInputAction.Disable();
                Settings.JumpInputAction.started -= OnJumpCallback;
                Settings.JumpInputAction.performed -= OnJumpCallback; 
                Settings.JumpInputAction.canceled -= OnJumpCallback; 
                Settings.JumpInputAction = null; 
            } 
            if (Settings.DashInputAction != null)
            {
                Settings.DashInputAction.Disable();
                Settings.DashInputAction.started -= OnDashCallback;
                Settings.DashInputAction.performed -= OnDashCallback; 
                Settings.DashInputAction.canceled -= OnDashCallback; 
                Settings.DashInputAction = null; 
            } 
            if (Settings.AttackInputAction != null)
            {
                Settings.AttackInputAction.Disable();
                Settings.AttackInputAction.started -= OnAttackCallback;
                Settings.AttackInputAction.performed -= OnAttackCallback; 
                Settings.AttackInputAction.canceled -= OnAttackCallback; 
                Settings.AttackInputAction = null; 
            } 
            if (Settings.InteractInputAction != null)
            {
                Settings.InteractInputAction.Disable();
                Settings.InteractInputAction.started -= OnInteractCallback;
                Settings.InteractInputAction.performed -= OnInteractCallback; 
                Settings.InteractInputAction.canceled -= OnInteractCallback; 
                Settings.InteractInputAction = null; 
            } 
        }

        private void OnDashCallback(InputAction.CallbackContext Context)
        {
            console.log(this, "Dash Input: {0}", Context.phase); 
            if (Context.started || Context.performed)
            {
                State.Input_Dash = true; 
                State.LinkedMovementController.OnDashInput();
            }
            else if (Context.canceled)
            {
                State.Input_Dash = false; 
            } 
        }

        private void OnJumpCallback(InputAction.CallbackContext Context)
        {
            console.log(this, "Jump Input: {0}", Context.phase); 
            if (Context.started || Context.performed)
            {
                State.Input_Jump = true; 
                State.LinkedMovementController.OnJumpInput();
            } 
            else if (Context.canceled)
            {
                State.Input_Jump = false; 
                State.LinkedMovementController.OnJumpUpInput();
            }
        }

        private void OnAttackCallback(InputAction.CallbackContext Context) => State.Input_PrimaryAction = Context.started || Context.performed;
        private void OnInteractCallback(InputAction.CallbackContext Context) => State.Input_SecondaryAction = Context.started || Context.performed;
        private void OnCrouchCallback(InputAction.CallbackContext Context) => State.Input_Crouch = Context.started || Context.performed;

        protected override void OnAfterEnable()
        {
            base.OnAfterEnable(); 
            EnablePlayerInput();
        }

        protected override void OnAfterDisable()
        {
            base.OnAfterDisable(); 
            DisablePlayerInput();
        }

        protected void Update()
        {
            if (DEBUG_ToggleLookLock)
            {
                DEBUG_ToggleLookLock = false; 
                State.IsLookingLocked = !State.IsLookingLocked; 
            }
            if (DEBUG_ToggleMovementLock)
            {
                DEBUG_ToggleMovementLock = false; 
                State.IsMovementLocked = !State.IsMovementLocked; 
            }
            State.Input_Move = State.IsMovementLocked ? Vector2.zero : GetMovementInput();
            State.Input_Look = State.IsLookingLocked ? Vector2.zero : GetLookInput();
            State.LinkedMovementController.SetMovementInput(State.Input_Move); 
            
            State.Update(Time.deltaTime);

            UpdateAttackInput();
        }

        private void UpdateAttackInput()
        {
            if (State.LinkedAnimator.GetBool(Animations.Ready) == false || State.LinkedAnimator.Animator.GetInteger(Animations.Dead) > 0)
            {
                console.log(this, "Hero is not ready to attack!"); 
                return;
            }
            if (State.Input_PrimaryAction && Weapon.CanAttack())
            {
                State.LastAttackTime = Time.time;
                State.LinkedAnimator.SetTrigger(Animations.Slash);
            }
        }

        protected void FixedUpdate()
        {
            if (DEBUG_OverrideMovement)
            {
                State.Input_Move = DEBUG_MovementInput; 
            }
            if (State.IsMovementLocked && !DEBUG_OverrideMovement)
            {
                State.Input_Move = Vector2.zero; 
            }
            if (State.IsLookingLocked)
            {
                State.Input_Look = Vector2.zero; 
            } 
            
            State.LastRequestedVelocity = State.LinkedRigidbody.velocity; 
        }

        public void SetMovementLock(bool locked)
        {
            State.IsMovementLocked = locked;
        }

        public void SetLookLock(bool locked)
        {
            State.IsLookingLocked = locked;
        }

        protected void OnDamageTaken()
        {
            State.LinkedAnimator.SetTrigger(Animations.Hit); 
            console.log(this, "Hero took damage!"); 
        }
        
        protected void OnDeath(IEntity Entity)
        {
            State.LinkedAnimator.SetBool(Animations.DieFront, true); 
            console.log(this, "Hero died!");
            GameStateManager.Respawn(true);
        } 
    }
}