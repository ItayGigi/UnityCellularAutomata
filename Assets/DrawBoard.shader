Shader "Unlit/DrawBoard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Texture2D<int> _MainTex;
            float4 _MainTex_ST;
            //SamplerState sampler_MainTex;
            uniform float4 _MainTex_TexelSize;
            Buffer<float4> _stateColors;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the int texture using Load
                int state = _MainTex.Load(int3(i.uv/_MainTex_TexelSize.xy, 0));
                float4 col = _stateColors[state];
                return col;
            }
            ENDCG
        }
    }
}
