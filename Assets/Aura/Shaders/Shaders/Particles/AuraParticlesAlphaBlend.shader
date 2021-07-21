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

Shader "Aura/Particles/Alpha Blend"
{
	Properties
	{
		[Header(Aura usage properties)][Space][KeywordEnum(Pixel, Vertex)] _UsageStage("Stage", Float) = 0
		[KeywordEnum(Light, Fog, Both)] _UsageType("Type", Float) = 0
		_LightingFactor("Lighting Factor", Float) = 1

		[Header(Properties)][Space]_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_SoftParticleDistanceFade("Soft Particles Distance Fade", Float) = 0.1
	}

	Category
	{
		SubShader
		{
			Pass
			{
			
				Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
				ColorMask RGB
				Cull Off Lighting Off ZWrite Off

				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
					#pragma shader_feature _USAGESTAGE_VERTEX _USAGESTAGE_PIXEL
					#pragma shader_feature _USAGETYPE_LIGHT _USAGETYPE_FOG _USAGETYPE_BOTH

					#define PREMULTIPLY_ALPHA(color)

					float _LightingFactor;
					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed4 _TintColor;
					sampler2D_float _CameraDepthTexture;
					float _SoftParticleDistanceFade;
						
					#include "UnityCG.cginc"
					#include "../../Includes/AuraParticlesUsage.cginc"
				ENDCG 
			}
		}	
	}
}
