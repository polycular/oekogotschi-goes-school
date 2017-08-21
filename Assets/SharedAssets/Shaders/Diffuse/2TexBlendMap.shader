Shader "EcoGotchi/Diffuse/2TexBlendMap" {
	Properties {
		_BlendProgress ("Blend Value", Range (0,1.0)) = 0.0
		_MainTex ("Base From (RGB)", 2D) = "white" {}
		_ToTex ("Base To (RGB)", 2D) = "white" {}
		_BlendMap ("Blend Map (R)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surfaceMain Lambert
		
		sampler2D _MainTex;
		sampler2D _ToTex;
		sampler2D _BlendMap;
		float _BlendProgress;

		struct Input {
			float2 uv_MainTex;
		};

		void surfaceMain (Input IN, inout SurfaceOutput o) {
			half4 colorA = tex2D (_MainTex, IN.uv_MainTex);
			half4 colorB = tex2D (_ToTex, IN.uv_MainTex);
			float blendValue = _BlendProgress * tex2D (_BlendMap, IN.uv_MainTex).r;
			half4 colorBlended = lerp(colorA, colorB, blendValue);
			o.Albedo = colorBlended.rgb;
			o.Alpha = colorBlended.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
