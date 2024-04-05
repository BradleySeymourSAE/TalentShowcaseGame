using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Scripts.Core.UI
{
    public class HealthBarUI : MonoBehaviour 
    {
        [SerializeField] protected float m_Duration = 1.0f;
        [SerializeField] protected Image m_BarImage;

        protected virtual void Awake()
        {
            gameObject.AssignIfNull(ref m_BarImage); 
        }
        
        public virtual void SetBarFill(float CurrentValue, float MaxValue)
        {
            float percentage = Mathf.Clamp01(CurrentValue / MaxValue); 
            InterpolateBarFillAsync(percentage, m_Duration); 
        } 
        
        protected virtual async void InterpolateBarFillAsync(float TargetValue, float Duratio)
        {
            float startValue = m_BarImage.fillAmount;
            float time = 0f;
            while (time < Duratio)
            {
                time += Time.deltaTime;
                float lerpValue = Mathf.Lerp(startValue, TargetValue, time / Duratio);
                m_BarImage.fillAmount = Mathf.Clamp01(lerpValue); 
                await new WaitForEndOfFrame();
            }
        } 
    }
}