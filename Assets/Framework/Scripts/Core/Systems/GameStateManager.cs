using Framework.Scripts.Common;
using Framework.Scripts.Common.Attributes;
using Framework.Scripts.Common.Injection;
using Framework.Scripts.Core.Player;
using UnityEngine;
namespace Framework.Scripts.Core.Systems
{

    /// <summary>
    /// Manages the game state, including the player score, lives, and respawning logic.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public sealed class GameStateManager : Singleton<GameStateManager>
    {
        public static event System.Action<float> OnScoreChanged = delegate {  }; 
        public static event System.Action<int> OnPlayerLivesChanged = delegate {  }; 
        public static event System.Action OnGameOver = delegate {  }; 
        public static event System.Action OnRespawn = delegate {  }; 
        
        [Injection, SerializeField, Readonly] private HeroController m_CurrentPlayer;
        [SerializeField] private int m_StartingLives = 3; 
        
        public float Score { get; private set; } = 0;
        public int CurrentPlayerLives { get; private set; }
        public HeroController CurrentPlayer => m_CurrentPlayer; 

        protected override void OnAfterEnable()
        {
            base.OnAfterEnable();
            CurrentPlayerLives = m_StartingLives; 
        }

        protected override void OnAfterDisable()
        {
            base.OnAfterDisable();
        }
        
        
        public static void IncreaseScore(int Value)
        {
            if (Instance == null)
            {
                console.error(nameof(GameStateManager), "Can't add score, instance is not initialized."); 
                return; 
            } 
            Instance.Score += Value; 
            OnScoreChanged(Instance.Score); 
        }

        public static void Respawn(bool UseLives = false)
        {
            if (Instance == null)
            {
                console.error(nameof(GameStateManager), "Can't respawn, instance is not initialized."); 
                return; 
            } 
            if (UseLives)
            {
                console.log(nameof(GameStateManager), "Player died, lives remaining:", Instance.CurrentPlayerLives);
                Instance.CurrentPlayerLives--; 
                OnPlayerLivesChanged(Instance.CurrentPlayerLives); 
                if (Instance.CurrentPlayerLives <= 0)
                {
                    console.log(nameof(GameStateManager), "GAME OVER"); 
                    OnGameOver();
                    Instance.CurrentPlayer.transform.position = CheckpointSystem.Instance.StartingCheckpoint.GetRespawnLocation();
                    return; 
                }
            }
            Vector3 respawnLocation = CheckpointSystem.Instance != null 
                ? CheckpointSystem.Instance.ActiveCheckpoint.GetRespawnLocation() 
                : Vector3.zero;  
            
            Instance.CurrentPlayer.transform.position = respawnLocation; 
            OnRespawn(); 
        }
        
    }
}