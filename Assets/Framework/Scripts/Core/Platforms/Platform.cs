using System;
using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using Framework.Scripts.Core.Player;
using UnityEngine;

namespace Framework.Scripts.Core.Platforms
{
    /// <summary>
    /// Represents the base class for all platforms. Contains common methods and properties all platforms share. 
    /// </summary>
    public class Platform : MonoBehaviour
    {
        //private static readonly int DissolveShaderProperty = Shader.PropertyToID("_DissolveAmount");

        protected Rigidbody2D m_Rigidbody;
        protected Collider2D m_Collider;
        [field: SerializeField] public PlatformSettings Settings { get; protected set; }

        public float VanishStartOffset = 0.0f;
        public float MovementStartOffset = 0.0f; 
        
        public float RotationAngle
        {
            get { return m_CachedRotationAngle; }
            set { m_CachedRotationAngle = Utility.Clamp0360(value); } 
        }
        
        protected Vector3 RotationVector
        {
            get => m_CachedRotationVector = Settings?.RotationAxis switch
            {
                ERotationAxis.XAxis => new Vector3(RotationAngle, 0, 0),
                ERotationAxis.YAxis => new Vector3(0, RotationAngle, 0),
                ERotationAxis.ZAxis => new Vector3(0, 0, RotationAngle),
                _                   => m_CachedRotationVector
            };
        }
        
        
        private Vector3 m_InitialRotation;
        private Vector3 m_CachedRotationVector; 
        private Vector3 m_CachedStartingPosition;  
        private Vector3 m_CachedTargetPosition;
        private float m_CachedRotationAngle;
        private Material m_Material; 
        private float m_Timer;
        // private float m_DissolveTimer;
        private float m_CurrentAngle = 90.0f;
        private DissolveShaderModifier m_DissolveShaderModifier; 

        protected void Awake()
        {
            AssignComponents();
            SetLayerAndMaterial();
            InitializeSettings(); 
        }

        protected void AssignComponents()
        {
            gameObject.Assign(ref m_Collider); 
            gameObject.Assign(ref m_Rigidbody);
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = true; 
            }
        }

        protected void SetLayerAndMaterial()
        {
            gameObject.layer = LayerMask.NameToLayer(nameof(Layers.Ground)); 
            if (TryGetComponent(out Renderer renderer))
            {
                if (DissolveShaderModifier.IsValidDissolveShader(renderer.sharedMaterial.shader) == false)
                {
                    console.warn(this, "Invalid dissolve shader on", name, "platform.");
                    return; 
                }
                m_Material = Instantiate(renderer.sharedMaterial);
                renderer.material = m_Material;
                m_DissolveShaderModifier = new DissolveShaderModifier(m_Material); 
            }
        }

        protected void InitializeSettings()
        {
            if (Settings == null)
            {
                return; 
            }
            m_InitialRotation = transform.rotation.eulerAngles + Settings.RotationOffset; 
            m_CachedStartingPosition = transform.position;
            m_CachedTargetPosition = m_CachedStartingPosition + Settings.MovementOffset;
        }

        protected async void Start()
        {
            if (Settings.ShouldVanishOnStart)
            {
                await new WaitForSeconds(VanishStartOffset); 
                Vanish(); 
            }
        }

        public virtual void Vanish()
        {
            PerformVanishAsync(); 
        }
        
        public virtual void Reappear()
        {
            PerformReappearAsync();
        }

        protected void LateUpdate()
        {
            if (HeroController.Instance == null)
            { 
                return; 
            }
            // do a height check to see if the hero is above the platform, if it is then the collider should be enabled. 
            // Otherwise, the collider should be disabled if it is not already. 
            float heroHeight = HeroController.Instance.transform.position.y;
            if (m_Material == null || m_DissolveShaderModifier == null)
            {
                return; 
            } 
            if (heroHeight > transform.position.y && m_DissolveShaderModifier.DissolveAmount > 1.0f)
            {
                m_Collider.enabled = true; 
            }
            else
            {
                m_Collider.enabled = false; 
            }
            
        }

        protected void FixedUpdate()
        {
            UpdatePlatformMovement();
            UpdatePlatformRotation(); 
        }

        protected void UpdatePlatformMovement()
        {
            if (Settings.UseMovement == false)
            {
                return; 
            }
            if (Settings.MovementMode is EPlatformMovementMode.Linear)
            {
                float time = -0.5f * (Mathf.Cos(Mathf.PI * Mathf.PingPong(Time.time, Settings.MovementTime + MovementStartOffset) / Settings.MovementTime) - 1); 
                transform.position = Vector3.Lerp(m_CachedStartingPosition, m_CachedTargetPosition, time);
            } 
            if (Settings.MovementMode is EPlatformMovementMode.Circular)
            {
                // Increment the current angle
                // the angle should be the total distance traveled divided by the radius of the circle 
                m_CurrentAngle += Settings.MovementTime * Time.deltaTime; 
                m_CurrentAngle %= 360.0f;
                // Convert the current angle to radians for Sin and Cos functions
                float angleRad = m_CurrentAngle * Mathf.Deg2Rad;
                // Calculate the new position using circular motion equations
                float x = Mathf.Sin(angleRad) * Settings.MovementRadius;
                float y = Mathf.Cos(angleRad) * Settings.MovementRadius;
                // Update the platform's position
                transform.position = new Vector3(x, y, 0.0f) + m_CachedStartingPosition; 
            }
        }
        
        protected void UpdatePlatformRotation()
        {
            if (Settings.UseRotation == false)
            {
                return; 
            } 
            if (Settings.UsePendulumRotation)
            {
                m_Timer += Time.deltaTime;
                RotationAngle = Settings.PendulumRotationRange * Mathf.Sin(m_Timer * Settings.RotationVelocity); 
                float ease = Mathf.SmoothStep(0, 1, Mathf.PingPong(m_Timer, 1.0f)); 
                RotationAngle = Mathf.Lerp(-Settings.PendulumRotationRange, Settings.PendulumRotationRange, ease); 
            }
            else
            {
                RotationAngle += Settings.RotationVelocity * Time.deltaTime; 
            }

            if (Settings.UseLocalRotation)
            {
                m_Rigidbody.transform.localRotation = Quaternion.Euler(m_InitialRotation) * Quaternion.AngleAxis(RotationAngle, RotationVector); 
            }
            else
            {
                m_Rigidbody.transform.rotation = Quaternion.Euler(m_InitialRotation) * Quaternion.AngleAxis(RotationAngle, RotationVector);
            }
        } 

        protected void PerformVanishAsync()
        {
            if (Settings.UseVanish == false && m_DissolveShaderModifier != null && m_DissolveShaderModifier.State is not EDissolveState.None)
            {
                return; 
            }
            m_DissolveShaderModifier.DissolveOutCompleted += OnDissolveOutCompleted;
            m_DissolveShaderModifier.StartDissolveOut(Settings.VanishStartDelay, Settings.VanishDuration);
        }
        
        protected void OnDissolveOutCompleted()
        {
            m_DissolveShaderModifier.DissolveOutCompleted -= OnDissolveOutCompleted; 
            // IsVanishing = false; 
            if (Settings.ShouldReappear)
            {
                PerformReappearAsync(); 
            } 
        } 
        
        protected void PerformReappearAsync()
        {
            if (Settings.UseVanish == false)
            {
                return; 
            } 
            if (m_DissolveShaderModifier.State is EDissolveState.None)
            {
                m_DissolveShaderModifier.DissolveInCompleted += OnReappearCompleted; 
                m_DissolveShaderModifier.StartDissolveIn(Settings.ReappearDelay, Settings.ReappearDuration); 
            } 
        }
        
        protected void OnReappearCompleted()
        {
            m_DissolveShaderModifier.DissolveInCompleted -= OnReappearCompleted; 
            if (Settings.ShouldLoop)
            {
                PerformVanishAsync(); 
            } 
        } 

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (Settings.UseMovement == false)
            {
                return; 
            } 
            
            if (other.gameObject.TryGetComponent(out MovementController controller) && other.transform.position.y > transform.position.y)
            {
                controller.SetCurrentPlatform(this);
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D other)
        {
            if (Settings.UseMovement == false)
            {
                return; 
            } 
            if (other.gameObject.TryGetComponent(out MovementController controller) && controller.CurrentPlatform == this)
            {
                controller.SetCurrentPlatform(null);
            }
        }

        #if UNITY_EDITOR 
        protected void OnValidate()
        {
            if (Settings == null)
            {
                return; 
            }
            m_CachedStartingPosition = transform.position; 
            m_CachedTargetPosition = m_CachedStartingPosition + Settings.MovementOffset; 
        }

        protected void OnDrawGizmosSelected()
        { 
            if (m_Collider == null) 
            {
                return; 
            } 
            // if the collider is disabled, draw red gizmos to indicate that the platform is not active. 
            Gizmos.color = m_Collider.enabled == false ? Color.red : Color.green; 
            Gizmos.DrawWireCube(m_Collider.bounds.center, m_Collider.bounds.size); 
            
            if (Settings == null)
            {
                return; 
            } 
            if (Settings.UseMovement)
            {
                if (Settings.MovementMode == EPlatformMovementMode.Circular)
                {
                    Gizmos.color = Color.red;
                    for (int i = 0; i < 360; i++)
                    {
                        float rad = i * Mathf.Deg2Rad;
                        float x = Mathf.Sin(rad) * Settings.MovementRadius + m_CachedStartingPosition.x;
                        float y = Mathf.Cos(rad) * Settings.MovementRadius + m_CachedStartingPosition.y;
                        Vector3 pos = new Vector3(x, y, 0.0f);
                        if (i == 0)
                        {
                            Gizmos.DrawSphere(pos, 0.5f); // Draw a larger sphere at the start position
                        }
                        else
                        {
                            Gizmos.DrawLine(pos, pos); // Draw a small dot
                        }
                    }
                    UnityEditor.Handles.color = Color.green;
                    UnityEditor.Handles.DrawWireDisc(m_CachedStartingPosition, Vector3.forward, Settings.MovementRadius);
                }
                else
                {
                    // Draw the movement path 
                    if (m_CachedStartingPosition == Vector3.zero || m_CachedTargetPosition == Vector3.zero)
                    {
                        return;
                    }
                    if (Application.isPlaying == false)
                    {
                        m_CachedStartingPosition = transform.position;
                        m_CachedTargetPosition = m_CachedStartingPosition + Settings.MovementOffset;
                    } 
                    
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(m_CachedStartingPosition, m_CachedTargetPosition);
                    float dist = (m_CachedStartingPosition - m_CachedTargetPosition).magnitude;
                    Vector3 midPoint = (m_CachedStartingPosition + m_CachedTargetPosition) / 2.0f;
                    Gizmos.color = Color.black;
                    UnityEditor.Handles.Label(transform.position + midPoint, $"Distance: {dist}");
                } 
            } 

            // Draw the arc of the pendulum rotation 
            if (Settings.UseRotation && Settings.UsePendulumRotation)
            {
                Vector3 axis = Vector3.forward;
                switch (Settings.RotationAxis)
                {
                    case ERotationAxis.XAxis: axis = Vector3.right; break;
                    case ERotationAxis.YAxis: axis = Vector3.up; break;
                    case ERotationAxis.ZAxis: axis = Vector3.forward; break;
                }

                Vector3 from = Quaternion.AngleAxis(-Settings.PendulumRotationRange, axis) * transform.up;
                UnityEditor.Handles.color = new Color(0, 1, 0, 0.25f);
                UnityEditor.Handles.DrawSolidArc(transform.position, axis, from, Settings.PendulumRotationRange * 2, 3.0f);
            } 
        }
        
        #endif 
    }
}