/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Although Aura (or Aura 1) is still a free project, it is not    *
*          open-source nor in the public domain anymore.                   *
*          Aura is now governed by the End Used License Agreement of       *
*          the Asset Store of Unity Technologies.                          *
*                                                                          * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

/// Shader used for drawing the Texture3D previews
Shader "Hidden/Aura/DrawTexture3DPreview"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		CGINCLUDE
		#include "UnityCG.cginc"

		int _SamplingQuality;
		sampler3D _MainTex;
		float _Density;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float3 localPos : TEXCOORD0;
			float4 screenPos : TEXCOORD1;
			float3 worldPos : TEXCOORD2;
		};

		v2f vert(appdata_base v)
		{
			v2f OUT;
			OUT.pos = UnityObjectToClipPos(v.vertex);
			OUT.localPos = v.vertex.xyz;
			OUT.screenPos = ComputeScreenPos(OUT.pos);
			COMPUTE_EYEDEPTH(OUT.screenPos.z);
			OUT.worldPos = mul(unity_ObjectToWorld, v.vertex);
			return OUT;
		}

		// usual ray/cube intersection algorithm
		struct Ray
		{
			float3 origin;
			float3 direction;
		};
		bool IntersectBox(Ray ray, out float entryPoint, out float exitPoint)
		{
			float3 invR = 1.0 / ray.direction;
			float3 tbot = invR * (float3(-0.5, -0.5, -0.5) - ray.origin);
			float3 ttop = invR * (float3(0.5, 0.5, 0.5) - ray.origin);
			float3 tmin = min(ttop, tbot);
			float3 tmax = max(ttop, tbot);
			float2 t = max(tmin.xx, tmin.yz);
			entryPoint = max(t.x, t.y);
			t = min(tmax.xx, tmax.yz);
			exitPoint = min(t.x, t.y);
			return entryPoint <= exitPoint;
		}

		float4 frag(v2f IN) : COLOR
		{
			float3 localCameraPosition = UNITY_MATRIX_IT_MV[3].xyz;
	
			Ray ray;
			ray.origin = localCameraPosition;
			ray.direction = normalize(IN.localPos - localCameraPosition);

			float entryPoint, exitPoint;
			IntersectBox(ray, entryPoint, exitPoint);
		
			if (entryPoint < 0.0) entryPoint = 0.0;

			float3 rayStart = ray.origin + ray.direction * entryPoint;
			float3 rayStop = ray.origin + ray.direction * exitPoint;

			float3 start = rayStop;
			float dist = distance(rayStop, rayStart);
			float stepSize = dist / float(_SamplingQuality);
			float3 ds = normalize(rayStop - rayStart) * stepSize;
				
			float4 color = float4(0,0,0,0);
			for (int i = _SamplingQuality; i >= 0; --i)
			{
				float3 pos = start.xyz + 0.5f;
				pos.x = 1.0f - pos.x;
				float4 mask = tex3D(_MainTex, pos);
					
				color.xyz += mask.xyz * mask.w;
				
				start -= ds;
			}
			color *= _Density / (uint)_SamplingQuality;

			// Weird enough, bruteforce seems quicker (to be investigated)
			//const float maxDist = 0.86602540378443864676372317075294f;
			//float stepSize = maxDist / (float)_SamplingQuality;
			//float dist = distance(rayStop, rayStart);
			//int steps = ceil(dist / stepSize);
			//float3 ds = normalize(rayStop - rayStart) * stepSize;
			//float3 color = float3(0,0,0);
			//[unroll(512)]
			//for (int i = steps; i >= 0; --i)
			//{
			//	float3 pos = start.xyz;
			//	pos.xyz = pos.xyz + 0.5f;
			//	float4 mask = tex3D(_MainTex, pos);
			//	
			//	color += mask.xyz * mask.w;
			//
			//	start += ds;
			//}				
			//color *= _Density / steps;

			return color;
		}
		ENDCG

		Pass
		{
			Cull front
			Blend One One
			ZWrite Off

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG

		}
	}
}