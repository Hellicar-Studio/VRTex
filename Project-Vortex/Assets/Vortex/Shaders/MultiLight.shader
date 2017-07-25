Shader "Custom/MultiLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Shininess("Shininess", Range(0.0, 1.0)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define NUM_LIGHTS 20;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float _Shininess;
			uniform float3 _LightPos[20];
			uniform float3 _LightCol[20];
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float3 pos = i.worldPos;

				float3 norm = normalize(i.normal);
				float3 surf2view = normalize(_WorldSpaceCameraPos - pos);
				float3 diffuse = float3(0.0, 0.0, 0.0);
				float3 specular = float3(0.0, 0.0, 0.0);

				for (int i = 0; i < 20; i++) {
					float3 lPos = _LightPos[i].xyz;
					float3 surf2light = normalize(lPos - pos);
					float dcont = max(0.0, dot(norm, surf2light));
					float3 reflection = reflect(-surf2light, norm);
					float scont = pow(max(0.0, dot(surf2view, reflection)), _Shininess);
					float att = 1.0 / (1.0 + 10.0*distance(lPos, pos));
					diffuse += dcont*_LightCol[i] * att;
					//specular += scont*float3(1.0, 1.0, 1.0);
				}

				float3 v = diffuse;

				return float4(v.r, v.g, v.b, 1.0);
			}
			ENDCG
		}
	}
}
