Shader "Custom/PostEffect/MotionBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurAmount ("Blur Amount", Float) = 1.0
	}
	SubShader {
		CGINCLUDE

		sampler2D _MainTex;
		fixed _BlurAmount;

		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert(appdata_img v) {
			v2f o;

			o.pos = UnityObjectToClipPos(v.vertex);

			o.uv = v.texcoord;

			return o;
		}

		fixed4 fragRGB(v2f i) :SV_TARGET {
			return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
		}

		half4 fragAlpha(v2f i) : SV_TARGET {
			return tex2D(_MainTex, i.uv);
		}

		ENDCG

		ZTest Always Cull Off ZWrite Off

		pass {
			Blend SrcAlpha OneMinusSrcAlpha

			ColorMask RGB

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragRGB

			ENDCG
		}

		pass {
			Blend One Zero

			ColorMask A

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragAlpha

			ENDCG
		}
	}
	FallBack Off
}
