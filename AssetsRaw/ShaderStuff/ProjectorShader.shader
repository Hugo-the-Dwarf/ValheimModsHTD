Shader "Unlit/ProjectTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = mul(unity_Projector, v.vertex);
                float uvZ = mul(unity_ProjectorClip, v.vertex).z;
                o.uv.z = uvZ;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, (i.uv.xy*_MainTex_ST.xy+_MainTex_ST.zw)/i.uv.w);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.a*=1-i.uv.z;
                col.a*=step(-i.uv.z, 0);
                return col;
            }
            ENDCG
        }
    }
}
