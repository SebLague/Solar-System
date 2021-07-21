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

#include "Common.cginc"

#define USE_AURA

float4 Aura_FrustumRange;
sampler3D Aura_VolumetricDataTexture;
sampler3D Aura_VolumetricLightingTexture;

//////////// Helper functions
float Aura_RescaleDepth(float depth)
{
    half rescaledDepth = InverseLerp(Aura_FrustumRange.x, Aura_FrustumRange.y, depth);
    return GetBiasedNormalizedDepth(rescaledDepth);
}

float3 Aura_GetFrustumSpaceCoordinates(float4 inVertex)
{
    float4 clipPos = UnityObjectToClipPos(inVertex);

    float z = -UnityObjectToViewPos(inVertex).z;

    float4 cameraPos = ComputeScreenPos(clipPos);
    cameraPos.xy /= cameraPos.w;
    cameraPos.z = z;

    return cameraPos.xyz;
}

//////////// Lighting
float4 Aura_SampleLightingTexture(float3 position)
{
	return tex3Dlod(Aura_VolumetricDataTexture, float4(position, 0));
}
float4 Aura_GetLightingValue(float3 screenSpacePosition)
{
    return Aura_SampleLightingTexture(float3(screenSpacePosition.xy, Aura_RescaleDepth(screenSpacePosition.z)));
}

void Aura_ApplyLighting(inout float3 colorToApply, float3 screenSpacePosition, float lightingFactor)
{
    screenSpacePosition.xy += GetBlueNoise(screenSpacePosition.xy, 1).xy;

    float3 lightingValue = Aura_GetLightingValue(screenSpacePosition).xyz * lightingFactor;

	float3 noise = GetBlueNoise(screenSpacePosition.xy, 2).xyz;

	colorToApply = lightingValue + noise;
}

//////////// Fog
float4 Aura_GetFogValue(float3 screenSpacePosition)
{
    return tex3Dlod(Aura_VolumetricLightingTexture, float4(screenSpacePosition.xy, Aura_RescaleDepth(screenSpacePosition.z), 0));
}

void Aura_ApplyFog(inout float3 colorToApply, float3 screenSpacePosition)
{
    screenSpacePosition.xy += GetBlueNoise(screenSpacePosition.xy, 3).xy;

    float4 fogValue = Aura_GetFogValue(screenSpacePosition);
    float4 noise = GetBlueNoise(screenSpacePosition.xy, 4);

	colorToApply = colorToApply * (fogValue.w + noise.w) + (fogValue.xyz + noise.xyz);
}

// From https://github.com/Unity-Technologies/VolumetricLighting/blob/master/Assets/Scenes/Materials/StandardAlphaBlended-VolumetricFog.shader
void Aura_ApplyFog(inout float4 colorToApply, float3 screenSpacePosition)
{
    screenSpacePosition.xy += GetBlueNoise(screenSpacePosition.xy, 5).xy;
    
	float4 fogValue = Aura_GetFogValue(screenSpacePosition);
    
	float4 noise = GetBlueNoise(screenSpacePosition.xy, 6);

	// Always apply fog attenuation - also in the forward add pass.
    colorToApply.xyz *= (fogValue.w + noise.w);

	// Alpha premultiply mode (used with alpha and Standard lighting function, or explicitly alpha:premul)
	#if _ALPHAPREMULTIPLY_ON
	fogValue.xyz *= colorToApply.w;
	#endif

	// Add inscattering only once, so in forward base, but not forward add.
	#ifndef UNITY_PASS_FORWARDADD
    colorToApply.xyz += (fogValue.xyz + noise.xyz);
	#endif
} 