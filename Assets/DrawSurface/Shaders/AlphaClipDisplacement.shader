Shader "DrawSurface/Surface/AlphaClipDispalcement"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MaskTex ("MaskTexture", 2D) = "white" {}
        _DisplaceTex ("_DisplaceTex", 2D) = "black" {}
        _Displacement ("Dispalcement", float) = 0.1
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
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
                float3 normal : NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MaskTex;
            sampler2D _DisplaceTex;
            float4 _Color;
            float4 _MaskTex_ST;
            float4 _DisplaceTex_ST;
            float _Displacement;
            fixed _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MaskTex);
                fixed mask = tex2Dlod(_MaskTex, float4(o.uv, 0.0, 0.0));
                fixed displace =tex2Dlod(_DisplaceTex, float4(o.uv, 0.0, 0.0))* _Displacement;
                v.vertex.xyz += normalize(v.normal)* displace * mask;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed displace = tex2D(_DisplaceTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                fixed alpha = displace;
            	clip(alpha - _AlphaCutoff);
            	
            	fixed4 col = _Color;
            	col.a = alpha;
            	
                return col;
            }
            ENDCG
        }
    }
}
