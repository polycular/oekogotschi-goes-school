Shader "EcoGotchi/Diffuse/ColorizeMap" {
	Properties {
		_Tint("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlendMap ("Blend Map (R)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _BlendMap;
		half4 _Tint;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 mainColor = tex2D (_MainTex, IN.uv_MainTex);

			float blendValue = tex2D (_BlendMap, IN.uv_MainTex).r;

			//Tint the mainColor
			mainColor.rgb = lerp(mainColor.rgb, mainColor.rgb*_Tint.rgb, _Tint.a * blendValue);

			o.Albedo = mainColor.rgb;
			o.Alpha = mainColor.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
