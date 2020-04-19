Shader "DrawSurface/Brushes/MaxBrush" {

	SubShader {

		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		Lighting Off Cull Off ZTest Always ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "DrawSurfaceBrushInclude.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed stamp = tex2D(_MainTex, i.texcoord);
				fixed surface = tex2D(_SurfaceTex, i.texcoord1);
				fixed result = max(stamp, surface);
				return saturate(fixed4(result,result,result, 1));
			}
			ENDCG
		}
	}
}
