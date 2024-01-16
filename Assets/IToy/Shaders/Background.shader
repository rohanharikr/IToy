Shader "IToy/Crop"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Top ("Remove Top", Int) = 0
		_Bottom ("Remove Bottom", Int) = 0
		_Left ("Remove Left", Int) = 0
		_Right ("Remove Right", Int) = 0
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
			int _Top;
			int _Bottom;
			int _Left;
			int _Right;

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

			bool isgreen(in vec3 color) {
				return 0.6 * color.g > color.r && 0.6 * color.g > color.b;
			}

			fixed4 pixel_shader(v2f i) : COLOR
			{
				color c = ;
				if (!isgreen(foreground)) {
					color = foreground;
				}
			}

			ENDCG
		}
	}
}
