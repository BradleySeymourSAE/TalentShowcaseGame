using Framework.Scripts.Core.Systems;
using TMPro;
using UnityEngine;
namespace Framework.Scripts.Core.UI
{
    public class ScorePanelUI : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI ScoreText;

        protected virtual void OnEnable() => GameStateManager.OnScoreChanged += OnScoreChanged;
        protected virtual void OnDisable() => GameStateManager.OnScoreChanged -= OnScoreChanged;

        private void OnScoreChanged(float NewScore)
        {
            ScoreText.text = $"Score: {NewScore}"; 
        }
    }
}