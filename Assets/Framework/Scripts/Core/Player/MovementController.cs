using System.Collections.Generic;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Cameras;
using Framework.Scripts.Core.Platforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Scripts.Core.Player
{
    public enum EClimbingState
    {
        None,
        Grabbing,
        Grabbed,
        Releasing
    }
    public class MovementController : MonoBehaviour
    {
        [field: SerializeField] public MovementSettings Settings { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public CapsuleCollider2D Collider { get; private set; } 
        [field: SerializeField] public PlayerEffects EffectsController { get; private set; }
        [SerializeField] private Transform m_GroundCheckTransform;
        [SerializeField] private Transform m_FrontWallCheckTransform;
        [SerializeField] private Transform m_BackWallCheckTransform;

        
        private bool m_IsGrounded; 
        private bool m_IsJumpCancelled;
        private bool m_IsJumpFalling;
        private float m_WallJumpStartTime;
        private int m_PreviousWallJumpDirection;
        private int m_RemainingDashes;
        private bool m_IsDashRefilling;
        private Vector2 m_LastDashDirection;
        private bool m_IsDashEngaged;
        private Vector2 m_MovementInput;
        private Vector2 m_LookDirection; 
        
        private bool m_IsDashing; 
        private bool m_IsWallSliding;
        private bool m_IsWallHolding;
        private int m_WallDirection = 0; 
        private bool m_IsCrouching; 
        private bool m_IsFacingRight; 
        private bool m_IsJumping; 
        private bool m_IsWallJumping;
        private float m_LastPressedJumpTime; 
        private float m_LastPressedDashTime; 
        private float m_LastOnGroundTime; 
        private float m_LastOnWallTime; 
        private float m_LastOnWallRightTime; 
        private float m_LastOnWallLeftTime; 
        
        private bool m_IsClimbing;
        private Climbable m_ActiveClimbable; 
        private float m_LadderPathPosition; 
        private Vector3 m_LadderStartPosition; 
        private Vector3 m_LadderTargetPosition; 
        private Quaternion m_LadderStartRotation; 
        private Quaternion m_LadderTargetRotation;
        private float m_LadderTime; 
        private EClimbingState m_ClimbingState = EClimbingState.None;

        public Platform CurrentPlatform { get; set; }   
        
        public event System.Action Jumped = delegate { };
        public event System.Action<Vector3, float> Landed = delegate { };

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            EffectsController = GetComponent<PlayerEffects>();
        }

        private void Start()
        {
            SetGravityScale(Settings.GravityScale);
            m_IsFacingRight = true;
        }

        public void SetCurrentPlatform(Platform Platform)
        {
            CurrentPlatform = Platform;
            transform.SetParent(CurrentPlatform?.transform, true); 
        }
        
        public Vector3 GetPosition()
        {
            return Rigidbody?.transform.position ?? transform.position; 
        }
        
        public Quaternion GetRotation()
        {
            return Rigidbody?.transform.rotation ?? transform.rotation; 
        } 

        public bool IsDashing()
        {
            return m_IsDashing; 
        }

        public bool IsDashEngaged()
        {
            return m_IsDashEngaged; 
        }

        public bool IsJumpCancelled()
        {
            return m_IsJumpCancelled; 
        }

        public bool IsWallSliding()
        {
            return m_IsWallSliding; 
        }

        public bool IsWallHolding()
        {
            return m_IsWallHolding; 
        }
        
        public int GetWallDirection()
        {
            return m_WallDirection; 
        } 

        public bool IsGrounded()
        {
            return m_IsGrounded; 
        } 

        public bool IsClimbing()
        {
            return m_IsClimbing; 
        }
        
        public bool IsCrouching()
        {
            return m_IsCrouching; 
        } 
        
        public bool IsFacingRight()
        {
            return m_IsFacingRight; 
        } 
        
        public bool IsJumping()
        {
            return m_IsJumping; 
        } 
        
        public bool IsWallJumping()
        {
            return m_IsWallJumping; 
        } 
        
        public float GetLastOnGroundTime()
        {
            return m_LastOnGroundTime; 
        } 
        
        public float GetLastOnWallTime()
        {
            return m_LastOnWallTime; 
        } 
        
        public float GetLastOnWallRightTime()
        {
            return m_LastOnWallRightTime; 
        } 
        
        public float GetLastOnWallLeftTime()
        {
            return m_LastOnWallLeftTime; 
        } 
        
        public float GetLastPressedJumpTime()
        {
            return m_LastPressedJumpTime; 
        } 
        
        public float GetLastPressedDashTime()
        {
            return m_LastPressedDashTime; 
        }

        public Vector3 GetVelocity()
        {
            return Rigidbody?.velocity ?? Vector3.zero; 
        }

        public void SetMovementInput(Vector2 Input)
        {
            m_MovementInput.x = Input.x;
            m_MovementInput.y = Input.y; 
        }
        
        public void SetLookDirection(Vector2 LookDirection)
        {
            m_LookDirection = LookDirection; 
        } 

        private void Update()
        {
            DecrementTimers();
            UpdateMovementInput();
            UpdateContactStateTimes();
            UpdateJumpingInput();
            UpdateDashState();
            CheckWallSlidingState();
            AdjustGravityScale();
        }

        private void AdjustGravityScale()
        {
            if (IsDashEngaged() || IsWallSliding())
            {
                SetGravityScale(0);
                return;
            }
            
            // Adjust the gravity scale for quick falling 
            if (Rigidbody.velocity.y < 0 && m_MovementInput.y < 0)
            {
                SetGravityScale(Settings.GravityScale * Settings.QuickFallGravityMultiplier);
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Settings.MaximumQuickFallSpeed));
                return;
            }

            // Adjust the gravity scale for when the jump is cancelled 
            if (IsJumpCancelled())
            {
                SetGravityScale(Settings.GravityScale * Settings.JumpCancelGravityMultiplier);
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Settings.MaximumNormalFallSpeed));
                return;
            }
            
            // If the character is either jumping, wall jumping or fall jumping;
            // and character's vertical velocity is below the hang time limit
            if ((IsJumping() || IsWallJumping() || m_IsJumpFalling) && Mathf.Abs(Rigidbody.velocity.y) < Settings.JumpAirHangTimeLimit)
            {
                SetGravityScale(Settings.GravityScale * Settings.MidAirHangGravityMultiplier);
                return;
            }

            // Adjust the gravity scale for normal falling 
            if (Rigidbody.velocity.y < 0)
            {
                SetGravityScale(Settings.GravityScale * Settings.NormalFallGravityMultiplier);
                // Restrict the downward velocity to not exceed the maximum fall speed
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Max(Rigidbody.velocity.y, -Settings.MaximumNormalFallSpeed));
                return;
            }

            SetGravityScale(Settings.GravityScale);
        }

        private void UpdateDashState()
        {
            if (CanDash() && m_LastPressedDashTime > 0)
            {
                console.log(this, "dash pressed"); 
                Sleep(Settings.DashSleepTime);
                m_LastDashDirection = m_LookDirection; 
                m_IsDashing = true;
                m_IsJumping = false;
                m_IsWallJumping = false;
                m_IsJumpCancelled = false;
                m_IsGrounded = false; 
                StartDash(m_LastDashDirection);
            }
        }

        private void CheckWallSlidingState()
        {
            if (CanWallSlide() && ((m_LastOnWallLeftTime > 0 && m_MovementInput.x < 0) || (m_LastOnWallRightTime > 0 && m_MovementInput.x > 0)))
            {
                m_IsWallSliding = true;
            }
            else
            {
                m_IsWallSliding = false;
            }
            m_IsWallHolding = IsWallSliding() && Mathf.Abs(Rigidbody.velocity.y) < 0.1f;

            if (IsWallHolding() && m_RemainingDashes < Settings.MaximumNumberOfDashes)
            {
                RechargeDashAsync(); 
            }
        }

        private void UpdateJumpingInput()
        {
            if (IsJumping() && Rigidbody.velocity.y < 0)
            {
                m_IsJumping = false;
                m_IsJumpFalling = true;
                CameraController.Instance.InterpolateYDampening(true);  
            }
            if (IsWallJumping() && (Time.time - m_WallJumpStartTime > Settings.WallJumpTime))
            {
                m_IsWallJumping = false;
            }
            if (m_LastOnGroundTime > 0 && IsJumping() == false && IsWallJumping() == false)
            {
                m_IsJumpCancelled = false;
                m_IsJumpFalling = false;
            }
            if (IsDashing() == false)
            {
                if (CanJump() && m_LastPressedJumpTime > 0)
                {
                    m_IsJumping = true;
                    m_IsWallJumping = false;
                    m_IsJumpCancelled = false;
                    m_IsJumpFalling = false;
                    m_IsGrounded = false; 
                    Jump();
                    Jumped();
                }
                else if (CanWallJump() && m_LastPressedJumpTime > 0)
                {
                    m_IsWallJumping = true;
                    m_IsJumping = false;
                    m_IsJumpCancelled = false;
                    m_IsJumpFalling = false;
                    m_WallJumpStartTime = Time.time;
                    m_PreviousWallJumpDirection = (m_LastOnWallRightTime > 0) ? -1 : 1;
                    WallJump(m_PreviousWallJumpDirection);
                }
            }
        }

        private void UpdateContactStateTimes()
        {
            if (!IsDashing() && !IsJumping())
            {
                bool isFacingFrontWall = Physics2D.OverlapBox(m_FrontWallCheckTransform.position, Settings.WallCheckScale, 0, Settings.GroundLayerMask);
                bool isFacingBackWall = Physics2D.OverlapBox(m_BackWallCheckTransform.position, Settings.WallCheckScale, 0, Settings.GroundLayerMask);
                
                // Avoid re-performing the physics check, use the results directly
                UpdateGroundedContactTime();

                if (!IsWallJumping())
                {
                    // Use the results we already obtained from the same physics check
                    if ((isFacingFrontWall && IsFacingRight()) || (isFacingBackWall && !IsFacingRight()))
                    {
                        m_WallDirection = 1; 
                        m_LastOnWallRightTime = Settings.CoyoteTime;
                    }
                    if ((isFacingFrontWall && !IsFacingRight()) || (isFacingBackWall && IsFacingRight()))
                    {
                        m_WallDirection = -1; 
                        m_LastOnWallLeftTime = Settings.CoyoteTime;
                    }
                }
                else
                {
                    m_WallDirection = 0; 
                }
                m_LastOnWallTime = Mathf.Max(m_LastOnWallLeftTime, m_LastOnWallRightTime);
            }
        }

        private void UpdateGroundedContactTime()
        {
            if (Physics2D.OverlapBox(m_GroundCheckTransform.position, Settings.GroundCheckScale, 0, Settings.GroundLayerMask))
            {
                if (m_LastOnGroundTime < -0.1f)
                {
                    Landed(GetPosition(), Rigidbody.velocity.magnitude);
                    CameraController.Instance.InterpolateYDampening(false);
                }
                m_IsGrounded = true; 
                m_LastOnGroundTime = Settings.CoyoteTime;
            }
        }

        private void UpdateMovementInput()
        {
            // Look in the direction of movement if there is any, else look towards the mouse position.
            if(m_MovementInput != Vector2.zero)
            {
                m_LookDirection = m_MovementInput.normalized;
            }
            else
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                m_LookDirection = (mousePosition - GetPosition()).normalized;
            }

            bool isMovingOrLookingRight = m_LookDirection.x > 0;
            if (isMovingOrLookingRight != IsFacingRight())
            {
                Turn();
            } 
        }
        
        private void UpdateTilting()
        {
            float progress = 0.0f;
            int directionMultiplier = -1;
            if (IsWallSliding())
            {
                progress = 0.25f; 
            }
            else
            {
                progress = Mathf.InverseLerp(-Settings.MaximumRunningSpeed, Settings.MaximumRunningSpeed, Rigidbody.velocity.x);
                directionMultiplier = (IsFacingRight() ? 1 : -1); 
            }
            float newRotation = ((progress *  Settings.MaxTilt * 2) - Settings.MaxTilt); 
            float rotation = Mathf.LerpAngle(transform.localRotation.eulerAngles.z * directionMultiplier, newRotation, Settings.TiltSpeed);
            transform.localRotation = Quaternion.Euler(0, 0, rotation * directionMultiplier); 
        }

        private void DecrementTimers()
        {
            m_LastOnGroundTime = Mathf.Max(m_LastOnGroundTime - Time.deltaTime, 0.0f); 
            m_LastOnWallTime = Mathf.Max(m_LastOnWallTime - Time.deltaTime, 0.0f);
            m_LastOnWallRightTime = Mathf.Max(m_LastOnWallRightTime - Time.deltaTime, 0.0f);
            m_LastOnWallLeftTime = Mathf.Max(m_LastOnWallLeftTime - Time.deltaTime, 0.0f);
            m_LastPressedJumpTime = Mathf.Max(m_LastPressedJumpTime - Time.deltaTime, 0.0f); 
            m_LastPressedDashTime = Mathf.Max(m_LastPressedDashTime - Time.deltaTime, 0.0f); 
        }

        private void LateUpdate()
        {
            UpdateTilting(); 
        }

        private void FixedUpdate()
        {
            if (!IsDashing() && IsWallJumping())
            {
                Move(Settings.WallJumpMovementSmoothingFactor);
            }
            else if (!IsDashing() && !IsWallJumping())
            {
                Move(1.0f);
            }
            
            if (IsDashing() && IsDashEngaged())
            {
                Move(Settings.DashEndedMovementSmoothingFactor);
            }
  
            if (IsWallSliding())
            {
                UpdateWallSlide();
            }
            //
            // if (IsClimbing())
            // {
            //     UpdateClimbing();
            // }
        }

        public void OnCrouchInput()
        {
            
        }

        public void OnJumpInput()
        {
            m_LastPressedJumpTime = Settings.JumpInputBufferTime;
        }

        public void OnJumpUpInput()
        {
            if (CanJumpCut() || CanWallJumpCut())
            {
                m_IsJumpCancelled = true;
            }
        }

        public void OnDashInput()
        {
            m_LastPressedDashTime = Settings.DashInputBufferDuration;
        }

        public void SetGravityScale(float scale)
        {
            Rigidbody.gravityScale = scale;
        }

        private void Sleep(float duration)
        {
            ProcessSleepAsync(duration); 
        }

        private async void ProcessSleepAsync(float duration)
        {
            Time.timeScale = 0;
            console.log(this, "sleeping for {0} seconds", duration); 
            await new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
        }

        private void Move(float SpeedInterpolation)
        {
            // if speed is less than max walk speed, lerp to walk speed, else lerp to run speed 
            float targetSpeed = m_MovementInput.x * Settings.MaximumRunningSpeed; 
            targetSpeed = Mathf.Lerp(Rigidbody.velocity.x, m_MovementInput.x * Settings.MaximumRunningSpeed, SpeedInterpolation);
            
            float acceleration;
            if (GetLastOnGroundTime() > 0)
            {
                acceleration = (Mathf.Abs(targetSpeed) > 0.05f) ? Settings.RunningAccelerationAmount : Settings.RunningDecelerationAmount;
            }
            else
            {
                acceleration = (Mathf.Abs(targetSpeed) > 0.05f) ? Settings.RunningAccelerationAmount * Settings.AirborneAccelerationFactor : Settings.RunningDecelerationAmount * Settings.AirborneDecelerationFactor;
            }
            if ((IsJumping() || IsWallJumping() || m_IsJumpFalling) && Mathf.Abs(Rigidbody.velocity.y) < Settings.JumpAirHangTimeLimit)
            {
                acceleration *= Settings.MidAirHangAccelerationMultiplier;
                targetSpeed *= Settings.MidAirHangMaxSpeedMultiplier;
            }
            if (Settings.UseConserveMomentum && Mathf.Abs(Rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(Rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && GetLastOnGroundTime() < 0)
            {
                acceleration = 0;
            }
            float difference = targetSpeed - Rigidbody.velocity.x; 
            float movement = difference * acceleration;
            Rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        private void Turn()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            m_IsFacingRight = !IsFacingRight();
        }

        private void Jump()
        {
            m_LastPressedJumpTime = 0;
            m_LastOnGroundTime = 0;
            float force = Settings.InitialJumpForce;
            if (Rigidbody.velocity.y < 0)
            {
                force -= Rigidbody.velocity.y;
            }
            Rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        private void WallJump(int dir)
        {
            m_LastPressedJumpTime = 0;
            m_LastOnGroundTime = 0;
            m_LastOnWallRightTime = 0;
            m_LastOnWallLeftTime = 0;
            m_WallDirection = 0; 
            Vector2 force = new Vector2(Settings.WallJumpForce.x, Settings.WallJumpForce.y);
            force.x *= dir;
            if (Mathf.Sign(Rigidbody.velocity.x) != Mathf.Sign(force.x))
            {
                force.x -= Rigidbody.velocity.x;
            }
            if (Rigidbody.velocity.y < 0)
            {
                force.y -= Rigidbody.velocity.y;
            }
            Rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        private async void StartDash(Vector2 dir)
        {
            m_LastOnGroundTime = 0;
            m_LastPressedDashTime = 0;
            float startTime = Time.time;
            m_RemainingDashes--;
            m_IsDashEngaged = true;
            SetGravityScale(0);
            while(Time.time - startTime <= Settings.DashEngageDuration)
            {
                Rigidbody.velocity = dir.normalized * Settings.DashMaxSpeed;
                await new WaitForFixedUpdate();
            }
            startTime = Time.time;
            m_IsDashEngaged = false;
            SetGravityScale(Settings.GravityScale);
            Rigidbody.velocity = Settings.DashEndSpeed * dir.normalized;
            while(Time.time - startTime <= Settings.DashEndTime)
            {
                await new WaitForFixedUpdate();
            }
            m_IsDashing = false;
        }

        private async void RechargeDashAsync()
        {
            m_IsDashRefilling = true;
            await new WaitForSeconds(Settings.DashRechargeDuration);
            m_IsDashRefilling = false;
            m_RemainingDashes = Mathf.Min(Settings.MaximumNumberOfDashes, m_RemainingDashes + 1);
        }

        private void UpdateWallSlide()
        {
            if (Rigidbody.velocity.y > 0)
            {
                Rigidbody.AddForce(-Rigidbody.velocity.y * Vector2.up, ForceMode2D.Impulse);
            }
            float speedDif = Settings.MaxWallSlideSpeed - Rigidbody.velocity.y;
            float movement = speedDif * Settings.WallSlideAcceleration;
            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
            Rigidbody.AddForce(movement * Vector2.up);
        }

        public void CheckDirectionToFace(bool IsMovingRight)
        {
            if (IsMovingRight != IsFacingRight())
            {
                Turn();
            }
        }

        private bool CanJump()
        {
            return GetLastOnGroundTime() > 0 && !IsJumping();
        }

        private bool CanWallJump()
        {
            return GetLastPressedJumpTime() > 0 && 
                   GetLastOnWallTime() > 0 && 
                   GetLastOnGroundTime() <= 0 && 
                   (!IsWallJumping() || (GetLastOnWallRightTime() > 0 && m_PreviousWallJumpDirection == 1) || (GetLastOnWallLeftTime() > 0 && m_PreviousWallJumpDirection == -1));
        }

        private bool CanJumpCut()
        {
            return IsJumping() && Rigidbody.velocity.y > 0;
        }

        private bool CanWallJumpCut()
        {
            return IsWallJumping() && Rigidbody.velocity.y > 0;
        }

        private bool CanDash()
        {
            if (!IsDashing() && m_RemainingDashes < Settings.MaximumNumberOfDashes && GetLastOnGroundTime() > 0 && !m_IsDashRefilling)
            {
                RechargeDashAsync();
            }
            return m_RemainingDashes > 0;
        }

        public bool CanWallSlide()
        {
            if (GetLastOnWallTime() > 0 && !IsJumping() && !IsWallJumping() && !IsDashing() && GetLastOnGroundTime() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanClimb()
        {
            // Do not allow to climb if crouching
            if (IsCrouching())
                return false;
            // Find a ladder to climb...
            List<Collider2D> overlappedColliders = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            contactFilter.SetLayerMask(Settings.ClimbingLayerMask);
            int overlapCount = Physics2D.OverlapBox(m_GroundCheckTransform.position, Settings.GroundCheckScale, 0, contactFilter, overlappedColliders);
            if (overlapCount == 0)
            {
                console.log(this, "No ladder found"); 
                return false;
            }
            // Is it a ladder?
            if (!overlappedColliders[0].TryGetComponent(out Climbable ladder))
            {
                console.log(this, "Not a ladder component"); 
                return false;
            }

            // Found a ladder, make it active ladder, and return
            m_ActiveClimbable = ladder;
            return true;
        }

        public void StartClimbing()
        {
            // if the player is already climbing, or is not able to climb, return 
            if (IsClimbing() || CanClimb() == false)
            {
                return; 
            }
            // Set the player to climbing state 
            m_ClimbingState = EClimbingState.Grabbing;
            m_LadderStartPosition = GetPosition();
            m_LadderTargetPosition = m_ActiveClimbable.ClosestPointAlongPath(m_LadderStartPosition, out m_LadderPathPosition);
            m_LadderStartRotation = GetRotation();
            m_LadderTargetRotation = m_ActiveClimbable.transform.rotation; 
        }

        public void StopClimbing()
        {
            // if the player is not climbing, or the climbing state is not grabbed then return 
            if (IsClimbing() == false || m_ClimbingState is not EClimbingState.Grabbed)
            {
                return; 
            }
            m_ClimbingState = EClimbingState.Releasing; 
            m_LadderStartPosition = GetPosition();
            m_LadderStartRotation = GetRotation();
            m_LadderTargetPosition = m_LadderStartPosition;
            m_LadderTargetRotation = m_ActiveClimbable.GetBottomPoint().rotation; 
        }

        private void UpdateClimbing()
        {
            Vector3 velocity = Vector3.zero;
            switch(m_ClimbingState)
            {
                case EClimbingState.Grabbing:
                case EClimbingState.Releasing:
                {
                    m_LadderTime += Time.deltaTime;

                    // If the player is grabbing the ladder, interpolate the position and rotation of the player towards the ladder. 
                    if (m_LadderTime <= Settings.ClimbGrabbingTime)
                    {
                        Vector3 interpolatedPosition = Vector3.Lerp(m_LadderStartPosition, m_LadderTargetPosition, (m_LadderTime / Settings.ClimbGrabbingTime));
                        velocity = (interpolatedPosition - GetPosition()) / Time.deltaTime;
                    }

                    // Otherwise, if the target has been reached then change the ladder climbing state. 
                    else
                    {
                        m_LadderTime = 0.0f;
                        if (m_ClimbingState is EClimbingState.Grabbing)
                        {
                            // Switch to the climbing phase 
                            m_ClimbingState = EClimbingState.Grabbed;
                        }
                        else if (m_ClimbingState is EClimbingState.Releasing)
                        {
                            // Exit the climbing state. (Allow the player to fall?) 
                            // TODO: 
                            m_ClimbingState = EClimbingState.None;
                        }
                    }
                    break;
                } 
                case EClimbingState.Grabbed:
                {
                    m_ActiveClimbable.ClosestPointAlongPath(GetPosition(), out m_LadderPathPosition);
                    if (Mathf.Abs(m_LadderPathPosition) < 0.05f)
                    {
                        // Move the player along the ladders path 
                        velocity = m_ActiveClimbable.transform.up * m_MovementInput.y * Settings.ClimbingSpeed; 
                    }
                    else
                    {
                        // If we have reached either ends of the ladder, then release the player from the ladder 
                        m_ClimbingState = EClimbingState.Releasing; 
                        m_LadderStartPosition = GetPosition(); 
                        m_LadderStartRotation = GetRotation();
                        if (m_LadderPathPosition > 0.0f)
                        {
                            // Above the ladder path's top point 
                            m_LadderTargetPosition = m_ActiveClimbable.GetTopPoint().position; 
                            m_LadderTargetRotation = m_ActiveClimbable.GetTopPoint().rotation; 
                        }
                        else if (m_LadderPathPosition < 0.0f)
                        {
                            // Below the ladder path's bottom point 
                            m_LadderTargetPosition = m_ActiveClimbable.GetBottomPoint().position; 
                            m_LadderTargetRotation = m_ActiveClimbable.GetBottomPoint().rotation;  
                        }
                    }
                    break; 
                }
            }
            Rigidbody.velocity = velocity; 
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(m_GroundCheckTransform.position, Settings.GroundCheckScale);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(m_FrontWallCheckTransform.position, Settings.WallCheckScale);
            Gizmos.DrawWireCube(m_BackWallCheckTransform.position, Settings.WallCheckScale);
        }
        #endif
    }
}