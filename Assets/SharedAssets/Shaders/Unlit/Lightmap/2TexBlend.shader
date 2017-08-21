// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Same target as "Mobile/Unlit (Supports Lightmap)" shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "EcoGotchi/Unlit/Lightmap/2TexBlend" {
	Properties {
		_BlendProgress ("Blend Value", Range (0,1.0)) = 0.0
		_MainTex ("Base From (RGB)", 2D) = "white" {}
		_ToTex ("Base To (RGB)", 2D) = "white" {}
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
					float2 texcoord1 : TEXCOORD1;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 uv_MainTex : TEXCOORD0;
					half2 uv_Lightmap : TEXCOORD1;
				};

				sampler2D _MainTex;
				sampler2D _ToTex;
				float _BlendProgress;
				float4 _MainTex_ST;

                fixed4 unity_LightmapST;
                // sampler2D unity_Lightmap;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					
                    o.uv_Lightmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    
					return o;
				}
			
				fixed4 frag (v2f IN) : SV_Target
				{
					fixed4 colorA = tex2D (_MainTex, IN.uv_MainTex);
					fixed4 colorB = tex2D (_ToTex, IN.uv_MainTex);
					fixed4 blendColor = lerp(colorA, colorB, _BlendProgress);
					
                    blendColor.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv_Lightmap));
                    

					return blendColor;
				}
			ENDCG
		}
	}
	FallBack "Mobile/Unlit (Supports Lightmap)"
}
