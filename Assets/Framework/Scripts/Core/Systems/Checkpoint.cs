using Framework.Scripts.Common;
using Framework.Scripts.Common.TriggerToolkit;
using UnityEngine;
namespace Framework.Scripts.Core.Systems
{
    /// <summary>
    ///     An empty class that represents a checkpoint in the game world
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] protected TriggerToolkit m_TriggerDetectionComponent;
        [SerializeField] protected Transform m_RespawnPoint; 

        protected void Awake()
        {
            gameObject.Assign(ref m_TriggerDetectionComponent); 
        }
        
        protected void OnEnable()
        {
            CheckpointSystem.RegisterCheckpoint(this); 
            m_TriggerDetectionComponent.OnTriggerEvent += HandleDetection;
        }
        
        protected void OnDisable()
        {
            CheckpointSystem.UnregisterCheckpoint(this); 
            m_TriggerDetectionComponent.OnTriggerEvent -= HandleDetection;
        }

        public Vector3 GetRespawnLocation()
        {
            return m_RespawnPoint?.position ?? transform.position; 
        }

        protected virtual void HandleDetection(Collider2D Collider, TriggerCondition.ETriggerEventType EventType)
        {
            if (EventType is TriggerCondition.ETriggerEventType.ENTER)
            {
                CheckpointSystem.SetActiveCheckpoint(this);
                console.log(this, "Player reached checkpoint", this.gameObject.name); 
            }
        }
        
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_RespawnPoint)
            {
                // Draw a sphere at the respawn point with handles 
                Gizmos.color = Color.green; 
                Gizmos.DrawSphere(m_RespawnPoint.position, 0.5f);
                UnityEditor.Handles.color = Color.black; 
                UnityEditor.Handles.Label(m_RespawnPoint.position + Vector3.up * 1.2f, "Respawn Point"); 
            }    
        }
        #endif 
    }
}