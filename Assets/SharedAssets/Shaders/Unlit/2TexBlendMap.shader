// Derived from "Unlit/Texture" shader.
// - no lighting
// - no lightmap support
// - no per-material color
Shader "EcoGotchi/Unlit/2TexBlendMap" {
	Properties {
		_BlendProgress ("Blend Value", Range (0,1.0)) = 0.0
		_MainTex ("Base From (RGB)", 2D) = "white" {}
		_ToTex ("Base To (RGB)", 2D) = "white" {}
		_BlendMap ("Blend Map (R)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
	
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 uv_MainTex : TEXCOORD0;
				};

				sampler2D _MainTex;
				sampler2D _ToTex;
				sampler2D _BlendMap;
				float _BlendProgress;
				float4 _MainTex_ST;
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				fixed4 frag (v2f IN) : SV_Target
				{
					fixed4 colorA = tex2D (_MainTex, IN.uv_MainTex);
					fixed4 colorB = tex2D (_ToTex, IN.uv_MainTex);
					float blendValue = _BlendProgress * tex2D (_BlendMap, IN.uv_MainTex).r;
					return lerp(colorA, colorB, blendValue);
				}
			ENDCG
		}
	}
	FallBack "Unlit/Texture"
}
