// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CubeMaps/Reflection" {
	Properties {
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_ReflectColor("Reflection Color", Color) = (1, 1, 1, 1)
		_ReflectAmount("Reflection Amount", Range(0, 1)) = 1
		_CubeMap("Reflection CubeMap", Cube) = "_SkyBox" {}
	}

	SubShader {
		pass {
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			fixed4 _Color;
			fixed4 _ReflectColor;
			fixed _ReflectAmount;
			samplerCUBE _CubeMap;

			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldViewDir : TEXCOORD2;
				float3 worldRefl : TEXCOORD3;
				SHADOW_COORDS(4)
			};

			v2f vert(a2v v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);

				//compute the reflect in world space
				o.worldRefl = reflect(-o.worldViewDir, o.worldNormal);

				TRANSFER_SHADOW(o);

				return o;
			}

			float4 frag(v2f i) : SV_TARGET {
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldViewDir = normalize(i.worldViewDir);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));

				//use the reflect dir in world space to access the cubemap
				fixed3 reflection = texCUBE(_CubeMap,i.worldRefl).rgb * _ReflectColor.rgb;

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				fixed3 color = ambient + lerp(diffuse, reflection, _ReflectAmount) * atten;

				return float4(color, 1.0);
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
