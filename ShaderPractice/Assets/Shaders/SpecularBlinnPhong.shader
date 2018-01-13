// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/SpecularBlinnPhong" {
	Properties {
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_Specular("Specular", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
	}

	SubShader {
		pass {
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				//法线的世界坐标系方向
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				//视角方向，世界坐标系
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {

				//环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				//法线的世界坐标系方向
				fixed3 worldNormal = i.worldNormal;
				//光线方向的世界坐标系方向
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				//计算漫反射
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

				//反射方向，世界坐标系
				fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
				//视角方向，世界坐标系
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				//Blinn-Phong模型的额外中间向量
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				//计算高光反射
				fixed3 specular = _LightColor0.rgb * _Diffuse.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
				
				//环境光色+漫反射颜色+高光反射颜色
				fixed3 color = ambient + diffuse + specular;

				return fixed4(color, 1.0);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
