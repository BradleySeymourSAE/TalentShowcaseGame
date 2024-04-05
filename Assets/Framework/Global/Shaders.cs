// This file was procedurally generated by Weaver. Any modifications will be overwritten.
// This file was generated using Weaver Lite. You can purchase Weaver Pro via the Welcome Panel in the Weaver Window.

#pragma warning disable // All.

/// <summary>This class was procedurally generated by Weaver.</summary>
public static class Shaders
{
    /// <summary>Shader Graphs</summary>
    public static class ShaderGraphs
    {
        /// <summary>Shader Graphs/Dissolve_MixedNoises</summary>
        public static class Dissolve_MixedNoises
        {
            /// <summary>Shader Name (for use with <see cref="UnityEngine.Shader.Find(string)"/>)</summary>
            public const string Name = "Shader Graphs/Dissolve_MixedNoises";

            /// <summary>Main_Texture [2D Texture]</summary>
            public static readonly int _MainTex = UnityEngine.Shader.PropertyToID("_MainTex");

            /// <summary>Dissolve_Amount [Float]</summary>
            public static readonly int _DissolveAmount = UnityEngine.Shader.PropertyToID("_DissolveAmount");

            /// <summary>Cell_Amount [Float]</summary>
            public static readonly int _CellAmount = UnityEngine.Shader.PropertyToID("_CellAmount");

            /// <summary>Scale [Float]</summary>
            public static readonly int _Scale = UnityEngine.Shader.PropertyToID("_Scale");

            /// <summary>Color [Color]</summary>
            public static readonly int _Color = UnityEngine.Shader.PropertyToID("_Color");

            /// <summary>unity_Lightmaps [2D Texture Array]</summary>
            public static readonly int unity_Lightmaps = UnityEngine.Shader.PropertyToID("unity_Lightmaps");

            /// <summary>unity_LightmapsInd [2D Texture Array]</summary>
            public static readonly int unity_LightmapsInd = UnityEngine.Shader.PropertyToID("unity_LightmapsInd");

            /// <summary>unity_ShadowMasks [2D Texture Array]</summary>
            public static readonly int unity_ShadowMasks = UnityEngine.Shader.PropertyToID("unity_ShadowMasks");
        }

        /// <summary>Shader Graphs/Dissolve_SimpleNoise</summary>
        public static class Dissolve_SimpleNoise
        {
            /// <summary>Shader Name (for use with <see cref="UnityEngine.Shader.Find(string)"/>)</summary>
            public const string Name = "Shader Graphs/Dissolve_SimpleNoise";

            /// <summary>Main_Texture [2D Texture]</summary>
            public static readonly int _MainTex = UnityEngine.Shader.PropertyToID("_MainTex");

            /// <summary>Dissolve_Amount [Float]</summary>
            public static readonly int _DissolveAmount = UnityEngine.Shader.PropertyToID("_DissolveAmount");

            /// <summary>Cell_Amount [Float]</summary>
            public static readonly int Vector1_450D3E32 = UnityEngine.Shader.PropertyToID("Vector1_450D3E32");

            /// <summary>Color [Color]</summary>
            public static readonly int _Color = UnityEngine.Shader.PropertyToID("_Color");

            /// <summary>unity_Lightmaps [2D Texture Array]</summary>
            public static readonly int unity_Lightmaps = UnityEngine.Shader.PropertyToID("unity_Lightmaps");

            /// <summary>unity_LightmapsInd [2D Texture Array]</summary>
            public static readonly int unity_LightmapsInd = UnityEngine.Shader.PropertyToID("unity_LightmapsInd");

            /// <summary>unity_ShadowMasks [2D Texture Array]</summary>
            public static readonly int unity_ShadowMasks = UnityEngine.Shader.PropertyToID("unity_ShadowMasks");
        }

        /// <summary>Shader Graphs/Dissolve_Veronio</summary>
        public static class Dissolve_Veronio
        {
            /// <summary>Shader Name (for use with <see cref="UnityEngine.Shader.Find(string)"/>)</summary>
            public const string Name = "Shader Graphs/Dissolve_Veronio";

            /// <summary>Main_Texture [2D Texture]</summary>
            public static readonly int _MainTex = UnityEngine.Shader.PropertyToID("_MainTex");

            /// <summary>Dissolve_Amount [Float]</summary>
            public static readonly int _DissolveAmount = UnityEngine.Shader.PropertyToID("_DissolveAmount");

            /// <summary>Cell_Density [Float]</summary>
            public static readonly int Vector1_450D3E32 = UnityEngine.Shader.PropertyToID("Vector1_450D3E32");

            /// <summary>Angle_Offset [Float]</summary>
            public static readonly int Vector1_7772CFC0 = UnityEngine.Shader.PropertyToID("Vector1_7772CFC0");

            /// <summary>Color [Color]</summary>
            public static readonly int Color_2ED9CBC6 = UnityEngine.Shader.PropertyToID("Color_2ED9CBC6");

            /// <summary>Tilling_Amount [Vector]</summary>
            public static readonly int Vector2_5A865DE9 = UnityEngine.Shader.PropertyToID("Vector2_5A865DE9");

            /// <summary>Offset [Vector]</summary>
            public static readonly int Vector2_D2FA87F5 = UnityEngine.Shader.PropertyToID("Vector2_D2FA87F5");

            /// <summary>unity_Lightmaps [2D Texture Array]</summary>
            public static readonly int unity_Lightmaps = UnityEngine.Shader.PropertyToID("unity_Lightmaps");

            /// <summary>unity_LightmapsInd [2D Texture Array]</summary>
            public static readonly int unity_LightmapsInd = UnityEngine.Shader.PropertyToID("unity_LightmapsInd");

            /// <summary>unity_ShadowMasks [2D Texture Array]</summary>
            public static readonly int unity_ShadowMasks = UnityEngine.Shader.PropertyToID("unity_ShadowMasks");
        }

        /// <summary>Shader Graphs/Dissolve_Voronoi_NoiseMixed</summary>
        public static class Dissolve_Voronoi_NoiseMixed
        {
            /// <summary>Shader Name (for use with <see cref="UnityEngine.Shader.Find(string)"/>)</summary>
            public const string Name = "Shader Graphs/Dissolve_Voronoi_NoiseMixed";

            /// <summary>Main_Texture [2D Texture]</summary>
            public static readonly int _MainTex = UnityEngine.Shader.PropertyToID("_MainTex");

            /// <summary>Dissolve_Amount [Float]</summary>
            public static readonly int Vector1_E974001A = UnityEngine.Shader.PropertyToID("Vector1_E974001A");

            /// <summary>Cell_Amount [Float]</summary>
            public static readonly int Vector1_450D3E32 = UnityEngine.Shader.PropertyToID("Vector1_450D3E32");

            /// <summary>CellDensity [Float]</summary>
            public static readonly int Vector1_BF7B720C = UnityEngine.Shader.PropertyToID("Vector1_BF7B720C");

            /// <summary>Twil_Strength [Float]</summary>
            public static readonly int Vector1_D0EB4D5 = UnityEngine.Shader.PropertyToID("Vector1_D0EB4D5");

            /// <summary>Center [Vector]</summary>
            public static readonly int Vector2_F70A8497 = UnityEngine.Shader.PropertyToID("Vector2_F70A8497");

            /// <summary>Color [Color]</summary>
            public static readonly int Color_2ED9CBC6 = UnityEngine.Shader.PropertyToID("Color_2ED9CBC6");

            /// <summary>unity_Lightmaps [2D Texture Array]</summary>
            public static readonly int unity_Lightmaps = UnityEngine.Shader.PropertyToID("unity_Lightmaps");

            /// <summary>unity_LightmapsInd [2D Texture Array]</summary>
            public static readonly int unity_LightmapsInd = UnityEngine.Shader.PropertyToID("unity_LightmapsInd");

            /// <summary>unity_ShadowMasks [2D Texture Array]</summary>
            public static readonly int unity_ShadowMasks = UnityEngine.Shader.PropertyToID("unity_ShadowMasks");
        }
    }

    /// <summary>Custom</summary>
    public static class Custom
    {
        /// <summary>Custom/BarFluidShader</summary>
        public static class BarFluidShader
        {
            /// <summary>Shader Name (for use with <see cref="UnityEngine.Shader.Find(string)"/>)</summary>
            public const string Name = "Custom/BarFluidShader";

            /// <summary>Noise Size [Vector]</summary>
            public static readonly int _NoiseSize = UnityEngine.Shader.PropertyToID("_NoiseSize");

            /// <summary>Noise Speed [Vector]</summary>
            public static readonly int _NoiseSpeed = UnityEngine.Shader.PropertyToID("_NoiseSpeed");

            /// <summary>Mask texture [2D Texture]</summary>
            public static readonly int _MaskTexture = UnityEngine.Shader.PropertyToID("_MaskTexture");

            /// <summary>Flashes Multiplier [Float]</summary>
            public static readonly int _FlashesMultiplier = UnityEngine.Shader.PropertyToID("_FlashesMultiplier");

            /// <summary>Waves Multiplier [Float]</summary>
            public static readonly int _WavesMultiplier = UnityEngine.Shader.PropertyToID("_WavesMultiplier");

            /// <summary>Hot Line Color [Color]</summary>
            public static readonly int _HotLineColor = UnityEngine.Shader.PropertyToID("_HotLineColor");

            /// <summary>Hot Line Height [Range(Default=0.01, Min=0, Max=0.1)]</summary>
            public static readonly int _HotLineHeight = UnityEngine.Shader.PropertyToID("_HotLineHeight");

            /// <summary>Hot Line Brightness [Range(Default=1, Min=0, Max=10)]</summary>
            public static readonly int _HotLineBrightness = UnityEngine.Shader.PropertyToID("_HotLineBrightness");

            /// <summary>Fill Level [Range(Default=1, Min=0, Max=1)]</summary>
            public static readonly int _FillLevel = UnityEngine.Shader.PropertyToID("_FillLevel");

            /// <summary>Sprite Texture [2D Texture]</summary>
            public static readonly int _MainTex = UnityEngine.Shader.PropertyToID("_MainTex");

            /// <summary>Stencil ID [Float]</summary>
            public static readonly int _Stencil = UnityEngine.Shader.PropertyToID("_Stencil");

            /// <summary>Stencil Operation [Float]</summary>
            public static readonly int _StencilOp = UnityEngine.Shader.PropertyToID("_StencilOp");

            /// <summary>Stencil Comparison [Float]</summary>
            public static readonly int _StencilComp = UnityEngine.Shader.PropertyToID("_StencilComp");

            /// <summary>Stencil Write Mask [Float]</summary>
            public static readonly int _StencilWriteMask = UnityEngine.Shader.PropertyToID("_StencilWriteMask");

            /// <summary>Stencil Read Mask [Float]</summary>
            public static readonly int _StencilReadMask = UnityEngine.Shader.PropertyToID("_StencilReadMask");

            /// <summary>Color Mask [Float]</summary>
            public static readonly int _ColorMask = UnityEngine.Shader.PropertyToID("_ColorMask");
        }
    }
}