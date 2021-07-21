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

uint directionalLightCount;
StructuredBuffer<DirectionalLightParameters> directionalLightDataBuffer;
Texture2DArray<half> directionalShadowMapsArray;
Texture2DArray<half4> directionalShadowDataArray;
Texture2DArray<half> directionalCookieMapsArray;

#if defined(DIRECTIONAL_LIGHTS_SHADOWS_FOUR_CASCADES)
	#define GET_SHADOW_CASCADES(worldPos, shadowData, depth) GetCascadeWeights_FourCascades(worldPos, shadowData)
	#define GET_SHADOW_COORDS(worldPos, shadowData, depth) GetCascadeShadowCoord_FourCascades(worldPos, GET_SHADOW_CASCADES(worldPos, shadowData, depth), shadowData)

	inline half4 GetCascadeWeights_FourCascades(half3 worldPos, DirectionalShadowData shadowData)
	{
		half3 fromCenterA = worldPos - shadowData.shadowSplitSpheres[0].xyz;
		half3 fromCenterB = worldPos - shadowData.shadowSplitSpheres[1].xyz;
		half3 fromCenterC = worldPos - shadowData.shadowSplitSpheres[2].xyz;
		half3 fromCenterD = worldPos - shadowData.shadowSplitSpheres[3].xyz;
		half4 squareDistances = half4(dot(fromCenterA, fromCenterA), dot(fromCenterB, fromCenterB), dot(fromCenterC, fromCenterC), dot(fromCenterD, fromCenterD));
		half4 weights = half4(squareDistances >= shadowData.shadowSplitSqRadii);

		return weights;
	}

	inline half4 GetCascadeShadowCoord_FourCascades(half3 worldPos, half4 cascadeWeights, DirectionalShadowData shadowData)
	{
		return mul(shadowData.world2Shadow[(int)dot(cascadeWeights, half4(1, 1, 1, 1))], half4(worldPos, 1));
	}
#elif defined(DIRECTIONAL_LIGHTS_SHADOWS_TWO_CASCADES)
	#define GET_SHADOW_CASCADES(worldPos, shadowData, depth) GetCascadeWeights_TwoCascades(worldPos, shadowData, depth)
	#define GET_SHADOW_COORDS(worldPos, shadowData, depth) GetCascadeShadowCoord_TwoCascades(worldPos, GET_SHADOW_CASCADES(worldPos, shadowData, depth), shadowData)

	inline half4 GetCascadeWeights_TwoCascades(half3 worldPos, DirectionalShadowData shadowData, half depth)
	{
		half4 zNear = half4(depth >= shadowData.lightSplitsNear);
		half4 zFar = half4(depth < shadowData.lightSplitsFar);
		half4 weights = zNear * zFar;

		return weights;
	}

	inline half4 GetCascadeShadowCoord_TwoCascades(half3 worldPos, half4 cascadeWeights, DirectionalShadowData shadowData)
	{
		half3 sc0 = mul((shadowData.world2Shadow[0]), half4(worldPos,1)).xyz;
		half3 sc1 = mul((shadowData.world2Shadow[1]), half4(worldPos,1)).xyz;
		half3 sc2 = mul((shadowData.world2Shadow[2]), half4(worldPos,1)).xyz;
		half3 sc3 = mul((shadowData.world2Shadow[3]), half4(worldPos,1)).xyz;
		half4 shadowMapCoordinate = half4(sc0 * cascadeWeights.x + sc1 * cascadeWeights.y + sc2 * cascadeWeights.z + sc3 * cascadeWeights.w, 1);

		half  noCascadeWeights = 1 - dot(cascadeWeights, half4(1, 1, 1, 1));
		shadowMapCoordinate.z += noCascadeWeights;

		return shadowMapCoordinate;
	}
#elif defined(DIRECTIONAL_LIGHTS_SHADOWS_ONE_CASCADE)
	#define GET_SHADOW_COORDS(worldPos, shadowData, depth) GetCascadeShadowCoord_OneCascade(worldPos, shadowData)

	inline half4 GetCascadeShadowCoord_OneCascade(half3 worldPos, DirectionalShadowData shadowData)
	{
		return mul((shadowData.world2Shadow[0]), half4(worldPos, 1));
	}
#else
    #define GET_SHADOW_COORDS(worldPos, shadowData, depth) half4(0,0,0,0);
#endif

half SampleShadowMap(half3 worldPosition, DirectionalShadowData shadowData, DirectionalLightParameters lightParameters, half depth)
{
	half4 samplePos = GET_SHADOW_COORDS(worldPosition, shadowData, depth);
	half shadowMapValue = directionalShadowMapsArray.SampleLevel(_LinearRepeat, half3(samplePos.xy, lightParameters.shadowmapIndex), 0).x;

	return step(samplePos.z, shadowMapValue);
}

void ComputeShadow(inout half attenuation, DirectionalLightParameters lightParameters, half3 worldPosition, half depth)
{
        DirectionalShadowData shadowData;
		shadowData.shadowSplitSqRadii =		directionalShadowDataArray[int3(0, 0, lightParameters.shadowmapIndex)];
		shadowData.lightSplitsNear =		directionalShadowDataArray[int3(1, 0, lightParameters.shadowmapIndex)];
		shadowData.lightSplitsFar =			directionalShadowDataArray[int3(2, 0, lightParameters.shadowmapIndex)];
		shadowData.shadowSplitSpheres[0] =	directionalShadowDataArray[int3(3, 0, lightParameters.shadowmapIndex)];
		shadowData.shadowSplitSpheres[1] =	directionalShadowDataArray[int3(4, 0, lightParameters.shadowmapIndex)];
		shadowData.shadowSplitSpheres[2] =	directionalShadowDataArray[int3(5, 0, lightParameters.shadowmapIndex)];
		shadowData.shadowSplitSpheres[3] =	directionalShadowDataArray[int3(6, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[0][0] =		directionalShadowDataArray[int3(7, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[0][1] =		directionalShadowDataArray[int3(8, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[0][2] =		directionalShadowDataArray[int3(9, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[0][3] =		directionalShadowDataArray[int3(10, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[1][0] =		directionalShadowDataArray[int3(11, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[1][1] =		directionalShadowDataArray[int3(12, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[1][2] =		directionalShadowDataArray[int3(13, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[1][3] =		directionalShadowDataArray[int3(14, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[2][0] =		directionalShadowDataArray[int3(15, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[2][1] =		directionalShadowDataArray[int3(16, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[2][2] =		directionalShadowDataArray[int3(17, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[2][3] =		directionalShadowDataArray[int3(18, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[3][0] =		directionalShadowDataArray[int3(19, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[3][1] =		directionalShadowDataArray[int3(20, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[3][2] =		directionalShadowDataArray[int3(21, 0, lightParameters.shadowmapIndex)];
		shadowData.world2Shadow[3][3] =		directionalShadowDataArray[int3(22, 0, lightParameters.shadowmapIndex)];
		shadowData.lightShadowData =		directionalShadowDataArray[int3(23, 0, lightParameters.shadowmapIndex)];
        
		half shadowAttenuation = SampleShadowMap(worldPosition, shadowData, lightParameters, depth);
		shadowAttenuation = lerp(shadowData.lightShadowData.x, 1.0f, 1.0f - shadowAttenuation);

		attenuation *= shadowAttenuation;
}

half SampleCookieMapArray(half2 textureCoordinates, int index)
{
	return directionalCookieMapsArray.SampleLevel(_LinearRepeat, half3(textureCoordinates, index), 0).x;
}

void ComputeDirectionalLightInjection(DirectionalLightParameters lightParameters, half3 worldPosition, half distanceToCam, half3 viewVector, inout half4 accumulationColor, half anisotropy)
{
	half anisotropyCosAngle = dot(-lightParameters.lightDirection, viewVector);
	half anisotropyFactor = GetAnisotropyFactor(anisotropyCosAngle, anisotropy);
	half attenuation = 1.0f;
	    
    half3 lightPos = mul(lightParameters.worldToLightMatrix.ToMatrix(), half4(worldPosition, 1)).xyz;
	
    #if defined(ENABLE_DIRECTIONAL_LIGHTS_SHADOWS)
    [branch]
	if (lightParameters.shadowmapIndex > -1)
	{
		ComputeShadow(attenuation, lightParameters, worldPosition, distanceToCam);
	}
    #endif

    #if defined(ENABLE_LIGHTS_COOKIES)
    [branch]
	if (lightParameters.cookieMapIndex > -1)
	{
		lightPos.xy /= lightParameters.cookieParameters.x;
		lightPos.xy += 0.5;
        [branch]
		if (lightParameters.cookieParameters.y > 0)
		{
			lightPos.xy = saturate(lightPos.xy);
		}
		lightPos.xy = frac(lightPos.xy);

		half cookieMapValue = SampleCookieMapArray(lightPos.xy, lightParameters.cookieMapIndex).x;

		attenuation *= cookieMapValue; 
	}
    #endif

	half3 color = lightParameters.color * anisotropyFactor;
    [branch]
	if (lightParameters.enableOutOfPhaseColor)
	{
		color = lerp(lightParameters.outOfPhaseColor, color, saturate(anisotropyFactor));
	}

	accumulationColor.xyz += color * attenuation;
}

void ComputeDirectionalLightsInjection(half3 worldPosition, half distanceToCam, half3 viewVector, inout half4 accumulationColor, half anisotropy)
{
    [allow_uav_condition]
	for (uint i = 0; i < directionalLightCount; ++i)
	{
		ComputeDirectionalLightInjection(directionalLightDataBuffer[i], worldPosition, distanceToCam, viewVector, accumulationColor, anisotropy);
	}
}