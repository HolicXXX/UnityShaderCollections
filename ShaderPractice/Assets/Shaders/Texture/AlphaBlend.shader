// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Texture/AlphaBlend" {
	Properties {
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Main Tex", 2D) = "white" {}
		_AlphaScale("Alpha Scale", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		pass {
			Tags {"LightMode"="ForwardBase"}
			ZWrite Off
			//正常
			Blend SrcAlpha OneMinusSrcAlpha

			//柔和相加
			// Blend OneMinusDstAlpha One

			//两倍相乘
			// Blend DstColor Zero

			//变暗
			// BlendOp Min

			//变亮
			// BlendOp Max

			//滤色
			// Blend OneMinusDstColor One
			// ==
			// Blend One OneMinusSrcColor

			//线性减淡
			// Blend One One

			//剔除哪个面的渲染
			Cull Off// Off / Front / Back

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed4 texColor = tex2D(_MainTex, i.uv);

				fixed3 albedo = texColor.rgb * _Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

				return fixed4(ambient + diffuse, texColor.a * _AlphaScale);//Blend
			}

			ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}
