using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Common;
using Framework.Scripts.Common;
using UnityEngine;
namespace Framework.Scripts.Core.Systems
{
    [DefaultExecutionOrder(-100)]
    public sealed class CheckpointSystem : Singleton<CheckpointSystem>
    {
        private List<Checkpoint> m_RegisteredCheckpoints = new(); 
        [SerializeField] private Checkpoint m_StartingCheckpoint; 
        public Checkpoint ActiveCheckpoint { get; private set; }
        public Checkpoint StartingCheckpoint => m_StartingCheckpoint; 

        protected override void OnAfterEnable()
        {
            base.OnAfterEnable();
            if (m_StartingCheckpoint != null)
            {
                SetActiveCheckpoint(m_StartingCheckpoint); 
            } 
        }

        public static void RegisterCheckpoint(Checkpoint Checkpoint)
        {
            if (Instance == null)
            {
                console.error(nameof(CheckpointSystem), "Can't register checkpoint, instance is nullptr"); 
                return; 
            }
            // If the checkpoint is not already registered, add it to the list 
            if (Instance.m_RegisteredCheckpoints.Contains(Checkpoint) == false)
            {
                Instance.m_RegisteredCheckpoints.Add(Checkpoint); 
            } 
            // If there is no active checkpoint, set this one as active 
            if (Instance.ActiveCheckpoint == null)
            {
                SetActiveCheckpoint(Checkpoint); 
            }
        }

        public static void UnregisterCheckpoint(Checkpoint Checkpoint)
        {
            // If the checkpoint is registered, remove it from the list 
            if (Instance != null && Instance.m_RegisteredCheckpoints.Contains(Checkpoint))
            {
                Instance.m_RegisteredCheckpoints.Remove(Checkpoint); 
            }
        }
        
        // Set the active checkpoint to the provided checkpoint 
        public static void SetActiveCheckpoint(Checkpoint Checkpoint)
        {
            if (Instance == null)
            {
                console.error(nameof(CheckpointSystem), "Can't set active checkpoint, instance is nullptr");
                return; 
            }
            if (Instance.ActiveCheckpoint != Checkpoint)
            {
                Instance.ActiveCheckpoint = Checkpoint; 
                console.log(nameof(CheckpointSystem), "Active checkpoint set to", Checkpoint.gameObject.name);
            } 
        } 
    }
}