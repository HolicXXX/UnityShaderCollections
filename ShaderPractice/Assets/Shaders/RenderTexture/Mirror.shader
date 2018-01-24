Shader "Custom/RenderTexture/Mirror" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
	}
	SubShader {
		pass {
			Tags {"LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct a2v {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;

			v2f vert(a2v v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv.x = 1 - o.uv.x;

				return o;
			}

			float4 frag(v2f o) : SV_TARGET {
				return tex2D(_MainTex, o.uv);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
