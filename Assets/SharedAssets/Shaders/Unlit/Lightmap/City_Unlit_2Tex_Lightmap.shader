Shader "EcoGotchi/Unlit/Lightmap/City_Unlit_2Tex_Lightmap" {
	Properties {
		_BlendProgress("Blend", Range (0, 1) ) = 0.0 
		_MainTex ("Base (RGB)", 2D) = ""
		_ToTex ("Base (RGB)", 2D) = ""
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 150
		
		// Non-lightmapped		
		Pass {
			Tags { "LightMode" = "Vertex" }
			
			Lighting Off		
			
			SetTexture [_MainTex] { combine texture }
			SetTexture[_ToTex] { 
				ConstantColor (0,0,0, [_BlendProgress]) 
				combine texture Lerp(constant) previous
			}
		}
		
		// Lightmapped, encoded as dLDR
		Pass {
			Tags { "LightMode" = "VertexLM" }
			
			Lighting Off
			BindChannels {
				Bind "texcoord", texcoord0
				Bind "texcoord", texcoord1
				Bind "texcoord1", texcoord2
			}

			SetTexture[_MainTex] { combine texture }
			
			SetTexture[_ToTex] {
				ConstantColor (0,0,0, [_BlendProgress])
				combine texture Lerp(constant) previous
			}
			
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				combine texture * previous DOUBLE
			}		
		}

		// Lightmapped, encoded as RGBM
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			
			Lighting Off
			BindChannels {
				Bind "texcoord", texcoord0
				Bind "texcoord", texcoord1
				Bind "texcoord1", texcoord2
			}

			SetTexture[_MainTex] { combine texture }
			
			SetTexture[_ToTex] {
				ConstantColor (0,0,0, [_BlendProgress])
				combine texture Lerp(constant) previous
			}
			
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				combine texture * previous DOUBLE
			}		
		}
	}
}