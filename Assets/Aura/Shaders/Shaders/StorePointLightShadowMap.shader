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

Shader "Hidden/Aura/StorePointLightShadowMap"
{
	SubShader
	{
		Pass
		{
			ZTest Off
			Cull Front
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma shader_feature SHADOWS_CUBE
			#pragma shader_feature POINT

			#ifdef SHADOWS_DEPTH
				#define SHADOWS_NATIVE
			#endif

			#if UNITY_VERSION >= 201730
				#define SHADOWS_CUBE_IN_DEPTH_TEX
			#endif
		
			#include "UnityCG.cginc"
			#include "UnityShadowLibrary.cginc"
			#include "../Aura.cginc"
		
			float4x4 _WorldViewProj;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(_WorldViewProj, v.vertex);
				o.uv = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
			#undef DIRECTIONAL
			#undef DIRECTIONAL_COOKIE
			#undef SHADOWS_SCREEN
			#undef SPOT
			#undef SHADOWS_DEPTH
			#undef POINT_COOKIE

	#if defined(POINT) && defined(SHADOWS_CUBE)
				float2 uv = i.uv.xy / i.uv.w;
				float3 ray = GetNormalizedVectorFromNormalizedYawPitch(uv);
				
				#if UNITY_VERSION >= 201730
					float depth = _ShadowMapTexture.SampleLevel(_PointClamp, ray, 0).x;
					return float4(depth, _LightProjectionParams.z, _LightProjectionParams.w, 0);
				#else
					float depth = SampleCubeDistance(ray);
					return float4(depth, _LightPositionRange.w, 0, 0);
				#endif

	#else
				return float4(1, 0.5, 1, 1);
	#endif
			}
			ENDCG
		}
	}
}
