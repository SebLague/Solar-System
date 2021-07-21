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

uint spotLightCount;
StructuredBuffer<SpotLightParameters> spotLightDataBuffer;
Texture2DArray<half> spotShadowMapsArray;
Texture2DArray<half> spotCookieMapsArray;

half SampleSpotShadowMap(SpotLightParameters lightParameters, half4 shadowPosition, half2 offset)
{
	// TODO : CHECK FOR SUPPOSED OFFSET
	half shadowMapValue = 1.0f - spotShadowMapsArray.SampleLevel(_LinearClamp, half3((shadowPosition.xy + offset) / shadowPosition.w, lightParameters.shadowMapIndex), 0);
	return step(shadowPosition.z / shadowPosition.w, shadowMapValue);
}

void ComputeSpotLightInjection(SpotLightParameters lightParameters, half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half anisotropy)
{
	half3 lightVector = normalize(worldPosition - lightParameters.lightPosition);
	half cosAngle = dot(lightParameters.lightDirection.xyz, lightVector);
	half dist = distance(lightParameters.lightPosition.xyz, worldPosition);

    [branch]
	if (dist > lightParameters.lightRange || cosAngle < lightParameters.lightCosHalfAngle)
	{
		return;
	}
	else
	{
        half anisotropyCosAngle = dot(-lightVector, viewVector);
		half anisotropyFactor = GetAnisotropyFactor(anisotropyCosAngle, anisotropy);
		half attenuation = anisotropyFactor;
        
		half4 lightPos = mul(lightParameters.worldToShadowMatrix.ToMatrix(), half4(worldPosition, 1));
		half normalizedDistance = saturate(lightPos.z / lightParameters.lightRange);
        
		attenuation *= GetLightDistanceAttenuation(lightParameters.distanceFalloffParameters, normalizedDistance);
        
		half angleAttenuation = 1;
		angleAttenuation = smoothstep(lightParameters.lightCosHalfAngle, lerp(1, lightParameters.lightCosHalfAngle, lightParameters.angularFalloffParameters.x), cosAngle);
		angleAttenuation = pow(angleAttenuation, lightParameters.angularFalloffParameters.y);
		attenuation *= angleAttenuation;
        
        #if defined(ENABLE_SPOT_LIGHTS_SHADOWS)
        [branch]
		if (lightParameters.shadowMapIndex > -1)
		{
			half shadowAttenuation = SampleSpotShadowMap(lightParameters, lightPos, 0);
			shadowAttenuation = lerp(lightParameters.shadowStrength, 1.0f, shadowAttenuation);
			
			attenuation *= shadowAttenuation;
		}
        #endif
        
        #if defined(ENABLE_LIGHTS_COOKIES)
        [branch]
		if (lightParameters.cookieMapIndex > -1)
		{        
			half cookieMapValue = spotCookieMapsArray.SampleLevel(_LinearRepeat, half3(lightPos.xy / lightPos.w, lightParameters.cookieMapIndex), 0).x;        
            cookieMapValue = lerp(1, cookieMapValue, pow(smoothstep(lightParameters.cookieParameters.x, lightParameters.cookieParameters.y, normalizedDistance), lightParameters.cookieParameters.z));
        
			attenuation *= cookieMapValue;
		}
        #endif
        
		accumulationColor.xyz += lightParameters.color * attenuation;
	}
}

void ComputeSpotLightsInjection(half3 worldPosition, half3 viewVector, inout half4 accumulationColor, half anisotropy)
{
    [allow_uav_condition]
	for (uint i = 0; i < spotLightCount; ++i)
	{
		ComputeSpotLightInjection(spotLightDataBuffer[i], worldPosition, viewVector, accumulationColor, anisotropy);
	}
}