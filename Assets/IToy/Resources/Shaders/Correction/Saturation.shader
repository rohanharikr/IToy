Shader "IToy/Correction/Saturation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Correction ("Saturation", Range(-100, 100)) = 0
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Correction;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vertex_shader(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 pixel_shader(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
				col.rgb = lerp(gray, col.rgb, (_Correction + 100.0) / 100.0);
				return col;
			}

			ENDCG
		}
	}
}
