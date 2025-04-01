Shader "Custom/BookShader"
{
    Properties
    {
        _LeatherTex ("Leather Texture", 2D) = "white" {}
        _PaperTex ("Paper Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            sampler2D _LeatherTex;
            sampler2D _PaperTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // If the normal is facing the +Z direction, use leather, otherwise use paper
                // Apply leather only to the front-facing face (+X), paper to all others
                // Apply leather texture to the top face, paper to all othersreturn (i.normal.y > 0.5) ? tex2D(_LeatherTex, i.uv) : tex2D(_PaperTex, i.uv);
                return (abs(UnityObjectToWorldNormal(i.normal).y - 1.0) < 0.01) ? tex2D(_LeatherTex, i.uv) : tex2D(_PaperTex, i.uv);
            }
            ENDCG
        }
    }
}