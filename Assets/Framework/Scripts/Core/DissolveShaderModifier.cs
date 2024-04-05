using Framework.Common;
using Framework.Common.Tools.AwaitExtensions.Plugins;
using Framework.Scripts.Common;
using UnityEngine;
namespace Framework.Scripts.Core
{
    public enum EDissolveState
    {
        DissolvingOut,
        DissolvingIn,
        None
    } 
    
    public class DissolveShaderModifier
    {
        private static readonly int s_DissolveShaderProperty = Shader.PropertyToID("_DissolveAmount");
        private const float k_ReappearDissolve = 3.0f; 
        private Material m_Material;
        public event System.Action DissolveOutCompleted = delegate { };
        public event System.Action DissolveInCompleted = delegate { };
        public EDissolveState State { get; private set; } = EDissolveState.None;
        
        public float DissolveAmount
        {
            get => m_Material.GetFloat(s_DissolveShaderProperty);
            set => m_Material.SetFloat(s_DissolveShaderProperty, value);
        }

        public DissolveShaderModifier(Renderer Renderer)
        {
            if (IsValidDissolveShader(Renderer.material.shader))
            {
                m_Material = Object.Instantiate(Renderer.material);
                Renderer.material = m_Material;
            }
            else
            {
                console.error(this, "Renderer material shader is not a valid dissolve shader on", Renderer.gameObject.name); 
            }
        }

        public DissolveShaderModifier(Material Material)
        {
            m_Material = Material; 
        }
        
        public void StartDissolveOut(float startDelay, float duration = 1.0f)
        { 
            State = EDissolveState.DissolvingOut; 
            ProcessDissolveAsync(startDelay, duration, 0.0f);
        }

        public void StartDissolveIn(float startDelay, float duration)
        { 
            State = EDissolveState.DissolvingIn; 
            ProcessDissolveAsync(startDelay, duration, k_ReappearDissolve);
        }
        
        private async void ProcessDissolveAsync(float Delay, float Duration, float FinalValue)
        {
            float timer = 0.0f;
            await new WaitForSeconds(Delay);
            float current = m_Material.GetFloat(s_DissolveShaderProperty);
            while(timer < Duration)
            {
                timer += Time.deltaTime;
                DissolveAmount = Mathf.Lerp(current, FinalValue, timer / Duration);
                await new WaitForUpdate();
            }
            EDissolveState currentState = State; 
            State = EDissolveState.None; 
            switch(currentState)
            {
                case EDissolveState.DissolvingOut:
                    DissolveOutCompleted();
                break;
                case EDissolveState.DissolvingIn:
                    DissolveInCompleted();
                break;
            }
        }


        public static bool IsValidDissolveShader(Shader Shader)
        {
            return Shader != null && 
                Shader.name.Contains(nameof(Shaders.ShaderGraphs.Dissolve_Voronoi_NoiseMixed)) || 
                Shader.name.Contains(nameof(Shaders.ShaderGraphs.Dissolve_Veronio)) || 
                Shader.name.Contains(nameof(Shaders.ShaderGraphs.Dissolve_MixedNoises)) || 
                Shader.name.Contains(nameof(Shaders.ShaderGraphs.Dissolve_SimpleNoise)); 
        }
    }
}