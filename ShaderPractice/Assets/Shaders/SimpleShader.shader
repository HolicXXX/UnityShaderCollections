// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/SimpleShader" {
    Properties {
        _Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
    }

	SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;

            //name means application to vertex shader
            struct a2v {
                //顶点坐标
                float4 vertex : POSITION;
                //法线向量
                float3 normal : NORMAL;
                //模型的第一套纹理坐标
                float4 texcoord : TEXCOORD0;
            };

            //name means vertex to fragment
            struct v2f {
                //裁剪空间的坐标
                float4 pos : SV_POSITION;
                //颜色信息
                fixed3 color : COLOR0;
            };

            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.normal * 0.5 + fixed3(0.5, 0.5, 0.5);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed3 c = i.color;
                c *= _Color.rgb;
                return fixed4(c, 1.0);
            }

            ENDCG
        }
    }
}
