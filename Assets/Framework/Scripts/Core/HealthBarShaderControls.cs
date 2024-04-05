using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Scripts.Core
{
    
    public class HealthBarShaderControls : HealthBarUI
    {
        [SerializeField] protected Material m_BarMaterial;
        [SerializeField] protected float m_FillDuration = 1.0f;
        
        protected override void Awake()
        {
            base.Awake(); 
            m_BarMaterial = Object.Instantiate(m_BarImage.material); 
            m_BarImage.material = m_BarMaterial; 
        }

        protected void Start()
        {
            SetBarFill(0.25f, 5.0f); 
        }
        
        protected override async void InterpolateBarFillAsync(float TargetValue, float Duration)
        {
            float startValue = m_BarMaterial.GetFloat(Shaders.Custom.BarFluidShader._FillLevel);
            float time = 0f;
            while (time < Duration)
            {
                time += Time.deltaTime;
                float lerpValue = Mathf.Lerp(startValue, TargetValue, time / Duration);
                m_BarMaterial.SetFloat(Shaders.Custom.BarFluidShader._FillLevel, Mathf.Clamp01(lerpValue));
                await new WaitForEndOfFrame();
            }
        } 
    }
}