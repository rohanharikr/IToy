Shader "IToy/Crop"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CropTop ("Remove Top", Int) = 0
		_CropBottom ("Remove Bottom", Int) = 0
		_CropLeft ("Remove Left", Int) = 0
		_CropRight ("Remove Right", Int) = 0
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
			int _CropTop;
			int _CropBottom;
			int _CropLeft;
			int _CropRight;

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
				// Remove pixels from the top, bottom, left, and right sides based on the properties
				if (i.uv.y < (_CropBottom / _ScreenParams.y) ||
				    i.uv.y > (1.0 - (_CropTop / _ScreenParams.y)) ||
				    i.uv.x < (_CropLeft / _ScreenParams.x) ||
				    i.uv.x > (1.0 - (_CropRight / _ScreenParams.x)))
				{
					discard;
				}

				// Sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}

			ENDCG
		}
	}
}
