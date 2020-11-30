// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Lit Outline Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[Toggle] _Emission("Emission", Int) = 0
		[HDR]_EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			Pass
		{
			Name "Outline"
			Tags {"IgnoreProjector"="True" "RenderType"="Opaque" "Queue"="Transparent"}
			Cull Front
			ZWrite On
			ZTest Less
			Lighting Off

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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _OutlineColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = _SinTime;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv + _SinTime);
			
					return _OutlineColor;
			}
			ENDCG
		}

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float4 _EmissionColor;
			int _Emission;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				if (_Emission == 1)
					c.rgb += _EmissionColor;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
    FallBack "Diffuse"
}
