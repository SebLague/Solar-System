Shader "Hidden/OceanMask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../../Includes/Math.cginc"

			struct appdata
			{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
			};

			struct v2f
			{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float3 viewVector : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					// Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
					// (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
					float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
					o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
					return o;
			}

			sampler2D _MainTex;
			static const int maxNumSpheres = 10;
			float4 spheres[maxNumSpheres];
			int numSpheres;

			fixed4 frag (v2f i) : SV_Target
			{
				float3 rayOrigin = _WorldSpaceCameraPos;
				float3 rayDir = normalize(i.viewVector);

				//return float4(rayDir,1);

				float nearest = 0;
				for (int sphereIndex = 0; sphereIndex < numSpheres; sphereIndex ++) {
					float2 hitInfo = raySphere(spheres[sphereIndex].xyz, spheres[sphereIndex].w, rayOrigin, rayDir);
					if (hitInfo.y > 0) {
						return 1;
					}
				}
				return 0;
			}
			ENDCG
		}
	}
}
