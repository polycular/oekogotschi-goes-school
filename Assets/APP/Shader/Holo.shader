// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Holo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Holo_Speed ("Holo Speed", Float) = 1
		_Time_Total ("Used to get time since anim start", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
				float4 world_pos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Holo_Col;
			float _Holo_Speed;
			float _Time_Total;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.world_pos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				float part = (_Time_Total % _Holo_Speed) / _Holo_Speed;

				if (i.uv.y < part)
				{
					col = fixed4(0,0,0,0);
				}
								
				return col;
			}
			ENDCG
		}
	}
}
