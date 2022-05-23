//This was Orgionally created by GoldenJude, with the _chest and _leg fields. and was named BODYMAT
//Works best with AttachmentHelper (or code that will patch the SetChestEquiped and SetLegEquiped methods to make use of the Female fields
Shader "Custom/ArmorMatShader"
{
    Properties
    {
        _ChestTex("_ChestTex", 2D) = "" {}
        _ChestBumpMap("_ChestBumpMap", 2D) = "" {}
        _ChestMetal("_ChestMetal", 2D) = "" {}

        _LegsTex("_LegsTex", 2D) = "" {}
        _LegsBumpMap("_LegsBumpMap", 2D) = "" {}
        _LegsMetal("_LegsMetal", 2D) = "" {}
		
		_ChestTexFemale("_ChestTexFemale", 2D) = "" {}
        _ChestBumpMapFemale("_ChestBumpMapFemale", 2D) = "" {}
        _ChestMetalFemale("_ChestMetalFemale", 2D) = "" {}

        _LegsTexFemale("_LegsTexFemale", 2D) = "" {}
        _LegsBumpMapFemale("_LegsBumpMapFemale", 2D) = "" {}
        _LegsMetalFemale("_LegsMetalFemale", 2D) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _ChestTex;
        sampler2D _ChestBumpMap;
        sampler2D _ChestMetal;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
        }
        ENDCG
    }
    FallBack "Diffuse"
}
