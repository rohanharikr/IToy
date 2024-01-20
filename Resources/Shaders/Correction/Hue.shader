Shader "IToy/Hue"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Correction ("Hue", Range(-180, 180)) = 0
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

			fixed4 RGBToHSV(fixed4 rgb)
			{
				fixed4 hsv;

				float minRGB = min(min(rgb.r, rgb.g), rgb.b);
				float maxRGB = max(max(rgb.r, rgb.g), rgb.b);

				hsv.z = maxRGB; // value

				float delta = maxRGB - minRGB;

				if (maxRGB > 0)
				{
					hsv.y = delta / maxRGB; // saturation
				}
				else
				{
					// r = g = b = 0, so it's grayscale
					hsv.y = 0;
					hsv.x = -1; // undefined hue
					return hsv;
				}

				if (rgb.r == maxRGB)
				{
					hsv.x = (rgb.g - rgb.b) / delta; // between yellow & magenta
				}
				else if (rgb.g == maxRGB)
				{
					hsv.x = 2 + (rgb.b - rgb.r) / delta; // between cyan & yellow
				}
				else
				{
					hsv.x = 4 + (rgb.r - rgb.g) / delta; // between magenta & cyan
				}

				hsv.x *= 60; // degrees

				if (hsv.x < 0)
				{
					hsv.x += 360;
				}

				return hsv;
			}

			fixed4 HSVToRGB(fixed4 hsv)
			{
				fixed4 rgb;

				if (hsv.y == 0)
				{
					// achromatic (gray)
					rgb = fixed4(hsv.z, hsv.z, hsv.z, 1);
				}
				else
				{
					hsv.x /= 60; // sector 0 to 5
					int i = floor(hsv.x);
					float f = hsv.x - i; // factorial part of h
					float p = hsv.z * (1 - hsv.y);
					float q = hsv.z * (1 - hsv.y * f);
					float t = hsv.z * (1 - hsv.y * (1 - f));

					switch (i)
					{
						case 0:
							rgb = fixed4(hsv.z, t, p, 1);
							break;
						case 1:
							rgb = fixed4(q, hsv.z, p, 1);
							break;
						case 2:
							rgb = fixed4(p, hsv.z, t, 1);
							break;
						case 3:
							rgb = fixed4(p, q, hsv.z, 1);
							break;
						case 4:
							rgb = fixed4(t, p, hsv.z, 1);
							break;
						default:
							rgb = fixed4(hsv.z, p, q, 1);
							break;
					}
				}

				return rgb;
			}

			fixed4 pixel_shader(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// Convert RGB to HSV
				fixed4 hsv = RGBToHSV(col);

				// Adjust hue
				hsv.x = (hsv.x + _Correction) % 360;
				if (hsv.x < 0)
					hsv.x += 360;

				// Convert back to RGB
				col = HSVToRGB(hsv);

				return col;
			}

			ENDCG
		}
	}
}
