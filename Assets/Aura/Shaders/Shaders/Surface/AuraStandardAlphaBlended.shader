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

Shader "Aura/Standard Alpha Blend"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
   
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha finalcolor:Aura_Fog
		#pragma target 5.0

		sampler2D _MainTex;
		sampler2D _CameraDepthTexture;

		// TODO : MAKE THIS PROPER WITH INCLUDES (SURFACE SHADERS ARE A NIGHTMARE)
		float4 Aura_FrustumRange;
		sampler3D Aura_VolumetricLightingTexture;
		float InverseLerp(float lowThreshold, float hiThreshold, float value)
		{
			return (value - lowThreshold) / (hiThreshold - lowThreshold);
		}
		float4 Aura_GetFogValue(float3 screenSpacePosition)
		{
			return tex3Dlod(Aura_VolumetricLightingTexture, float4(screenSpacePosition, 0));
		}
		void Aura_ApplyFog(inout fixed4 colorToApply, float3 screenSpacePosition)
		{    
			float4 fogValue = Aura_GetFogValue(screenSpacePosition);
			// Always apply fog attenuation - also in the forward add pass.
			colorToApply.xyz *= fogValue.w;
			// Alpha premultiply mode (used with alpha and Standard lighting function, or explicitly alpha:premul)
			#if _ALPHAPREMULTIPLY_ON
			fogValue.xyz *= colorToApply.w;
			#endif
			// Add inscattering only once, so in forward base, but not forward add.
			#ifndef UNITY_PASS_FORWARDADD
			colorToApply.xyz += fogValue.xyz;
			#endif
		}
		
		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
		};
		
		// From https://github.com/Unity-Technologies/VolumetricLighting/blob/master/Assets/Scenes/Materials/StandardAlphaBlended-VolumetricFog.shader
		void Aura_Fog(Input IN, SurfaceOutputStandard o, inout fixed4 color)
		{
				half3 screenSpacePosition = IN.screenPos.xyz/IN.screenPos.w;
				screenSpacePosition.z = InverseLerp(Aura_FrustumRange.x, Aura_FrustumRange.y, LinearEyeDepth(screenSpacePosition.z));
				Aura_ApplyFog(color, screenSpacePosition);
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
 
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Standard"
}