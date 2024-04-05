using Animancer;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Damage;
using Framework.Scripts.Core.Player;
using Framework.Scripts.Core.Systems;
using Framework.Scripts.Core.Weapon;
using UnityEngine;


namespace Framework.Scripts.Core.AI
{
    public abstract class AbstractAIEntity : MonoBehaviour, IEntity
    {
        [SerializeField] protected internal AICharacterSettings Settings;
        protected internal StateContext StateContext = new();
        [field: SerializeField] public Transform EyesTransform { get; private set; }
        [field: SerializeField] public Transform AttackOrigin { get; private set; }
        public Transform ActiveTarget { get; private set; } 
        public Vector3 TargetPosition { get; private set; }
        public IWeapon Weapon { get; protected set; }
        public float DamageBuffer { get; set; }

        public Transform GroundDetection; 
        public float GroundDetectionDistance = 0.1f; 
        public LayerMask GroundLayer; 
        private bool m_IsFacingRight;
        private float m_TurnTimer;
        protected bool m_CanMove; 
        
        protected virtual void Awake()
        {
            StateContext.LinkedRigidbody = GetComponent<Rigidbody2D>(); 
            StateContext.LinkedCollider = GetComponent<Collider2D>();
            StateContext.LinkedAnimator = GetComponentInChildren<HybridAnimancerComponent>();
            StateContext.HealthComponent = GetComponent<HealthComponent>(); 
            StateContext.LocalDetectionTree = GetComponentInChildren<LocalDetectionTree>(); 
            StateContext.HealthComponent.Initialize(this, Settings); 
            StateContext.LastAttackTime = Time.time;
            Weapon = GetComponent<IWeapon>(); 
            Weapon.SetWeaponStrategy(Settings.AttackStrategies[0]);
            // Set the default state to ready 
            StateContext.LinkedAnimator.SetInteger(Animations.State, 1); 
        }
        
        protected virtual void OnEnable()
        {
            StateContext.HealthComponent.OnDamageTaken += HandleOnDamageTaken;
            StateContext.HealthComponent.OnDeath += HandleOnDeath; 
        }
        
        protected virtual void OnDisable()
        {
            StateContext.HealthComponent.OnDamageTaken -= HandleOnDamageTaken; 
            StateContext.HealthComponent.OnDeath -= HandleOnDeath; 
        }

        protected virtual void Update()
        {
            StateContext.Update(Time.deltaTime);
            if (DamageBuffer > 0.0f)
            {
                DamageBuffer -= Time.deltaTime;
            }
            if (IsTargetVisible())
            {
                Vector2 toTarget = ActiveTarget.position + Vector3.up * 1.25f - EyesTransform.position;
                if (toTarget.x > 0 != IsFacingRight() && m_TurnTimer <= 0.0f && IsInSameLocationWithTarget() == false)
                {
                    Turn();
                    m_TurnTimer = Settings.TurnCooldownTime; 
                }
            }
            else
            {
                ActiveTarget = null; 
            }
            if (m_TurnTimer > 0.0f)
            {
                m_TurnTimer -= Time.deltaTime; 
            }
        }

        protected virtual void FixedUpdate()
        {
            UpdateGroundDetection();
            if (IsTargetVisible())
            {
                return;
            }
            if (Settings.AllowMovement && m_CanMove)
            {
                UpdateMovement();
            }  
        }
        

        private RaycastHit2D hitResult; 

        protected virtual void UpdateGroundDetection()
        {
            hitResult = Physics2D.Raycast(GroundDetection.position, Vector2.down, GroundDetectionDistance, GroundLayer); 
            if (!hitResult.collider && Settings.AllowMovement)
            {
               // If no ground is detected at the offset ground detection point. Then we should turn around 
                Turn();
                return; 
            }
            hitResult = Physics2D.Raycast(transform.position, Vector2.down, GroundDetectionDistance, GroundLayer); 
            
            // update the characters y position to the point of contact, but only if the position is within a certain range 
            // get the point of contact on the ground and set the y position of the character to that point 
            if (hitResult.point.y < transform.position.y - GroundDetectionDistance)
            {
                return; 
            }
            Vector3 position = transform.position; 
            position.y = hitResult.point.y + 1.0f; 
            transform.position = position;
            
            Debug.DrawRay(GroundDetection.position, Vector2.down * GroundDetectionDistance, Color.red);
        }
        
        public bool IsFacingRight()
        {
            return m_IsFacingRight;
        }

        public bool IsInSameLocationWithTarget()
        {
            return (Mathf.Abs(transform.position.x - ActiveTarget.position.x) < Settings.LookAtTargetThreshold &&
                    Mathf.Abs(transform.position.y - ActiveTarget.position.y) < Settings.LookAtTargetThreshold);
        }
        
        public bool IsTargetVisible()
        {
            if (StateContext.LocalDetectionTree == null || StateContext.LocalDetectionTree.Detections == null || StateContext.LocalDetectionTree.Detections.Count == 0)  
            {
                return false; 
            }
            if (Weapon.WeaponStrategy == null)
            {
                return false; 
            }
            DetectableTarget target = StateContext.LocalDetectionTree.Detections.FindClosest(transform.position, x => x != null && x.gameObject != this.gameObject); 
            if (target == null)
            {
                return false; 
            } 
            Vector2 directionToTarget = target.transform.position + Vector3.up * 1.25f - EyesTransform.position; 
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= Settings.VisionRange)
            {
                float angleToTarget = Vector2.Angle(EyesTransform.right, directionToTarget); 
                if (angleToTarget <= Settings.VisionAngle)
                {
                    RaycastHit2D hit = Physics2D.Raycast(EyesTransform.position, directionToTarget, distanceToTarget, Settings.VisionLayerMask); 
                    // Color color = hit.collider != null ? Color.green : Color.red; 
                    if (hit.collider != null)
                    {
                        if (hit.collider.transform == target.transform) 
                        {
                            ActiveTarget = target.transform;
                            return true;
                        }
                        else 
                        { 
                            ActiveTarget = null;
                            return false; 
                        }
                    }
                }
            }
            return false; 
        } 
        
        protected virtual void Turn()
        {
            Vector3 rotation = transform.rotation.eulerAngles; 
            rotation.y += 180; 
            transform.rotation = Quaternion.Euler(rotation); 
            m_IsFacingRight = !IsFacingRight();
        }

        protected virtual void UpdateMovement(float SpeedInterpolationRate = 1.0f)
        {
            // If the character is dead 
            if (StateContext.LinkedAnimator.GetInteger(Animations.State) == 9)
            {
                return; 
            } 
            // if there is an active target 
            if (IsTargetVisible())
            {
                StateContext.LinkedAnimator.SetInteger(Animations.State,1);
                return; 
            }
            
            float targetSpeed = m_IsFacingRight 
                ? Settings.RunningSpeed 
                : -Settings.RunningSpeed;
            
            // Set running state 
            StateContext.LinkedAnimator.SetInteger(Animations.State, Mathf.Abs(targetSpeed) > 1.0f ? 3 : 1);  
            
            targetSpeed = Mathf.Lerp(
                StateContext.LinkedRigidbody.velocity.x, 
                targetSpeed,
                SpeedInterpolationRate);            
            
            float acceleration = Mathf.Abs(targetSpeed) > 0.05f 
                ? Settings.RunningAccelerationAmount 
                : Settings.RunningDecelerationAmount; 
            
            float difference = targetSpeed - StateContext.LinkedRigidbody.velocity.x;
            float movement = difference * acceleration; 
            Vector3 updatedVelocity = movement * Vector3.right;
            transform.position += updatedVelocity * Time.fixedDeltaTime; 
        }
        
        protected virtual void HandleOnDeath(IEntity Entity)
        {
            GameStateManager.IncreaseScore(Settings.ScoreValue); 
        }
        
        protected virtual void OnDeathAnimationCompleted_Callback()
        {
            
        } 

        protected virtual void HandleOnDamageTaken()
        {
            DamageBuffer = Settings.DamageBufferTime; 
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (hitResult.collider != null)
            {
                Gizmos.color = Color.red; 
                Gizmos.DrawSphere(hitResult.point, 0.1f); 
            } 
        }
    }
}