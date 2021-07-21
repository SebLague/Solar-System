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

///
///			MatrixFloats
///
struct MatrixFloats
{
	half4 a;
	half4 b;
	half4 c;
	half4 d;

	half4x4 ToMatrix()
	{
		return half4x4(half4(a.x, b.x, c.x, d.x), half4(a.y, b.y, c.y, d.y), half4(a.z, b.z, c.z, d.z), half4(a.w, b.w, c.w, d.w));
	}
};

///
///			VolumeLevelsData
///
struct VolumeLevelsData
{
	half levelLowThreshold;
	half levelHiThreshold;
	half outputLowValue;
	half outputHiValue;
	half contrast;
};

///
///			VolumetricTextureData
///
struct VolumetricTextureData
{
	MatrixFloats transform;
	int index;
    int wrapMode;
    int filterMode;
    int clipOnAlpha;
    half clippingThreshold;
};

///
///			VolumetricNoiseData
///
struct VolumetricNoiseData
{
	int enable;
	MatrixFloats transform;
	half speed;
	half offset;
};

///
///			VolumeData
///
struct VolumeData
{
	MatrixFloats transform;
	int shape;
	/*
		Global      = 0
		Planar      = 1
		Box         = 2
		Sphere      = 3
		Cylinder    = 4
		Cone        = 5
	*/
	half falloffExponent;
	half xPositiveFade;
	half xNegativeFade;
	half yPositiveFade;
	half yNegativeFade;
	half zPositiveFade;
	half zNegativeFade;
	VolumetricTextureData textureData;
	VolumetricNoiseData noiseData;
	int injectDensity;
	half densityValue;
    VolumeLevelsData densityTextureLevelsParameters;
    VolumeLevelsData densityNoiseLevelsParameters;
    int injectAnisotropy;
    half anisotropyValue;
    VolumeLevelsData anisotropyTextureLevelsParameters;
    VolumeLevelsData anisotropyNoiseLevelsParameters;
    int injectColor;
    half3 colorValue;
    VolumeLevelsData colorTextureLevelsParameters;
    VolumeLevelsData colorNoiseLevelsParameters;
};

///
///			DirectionalShadowData
///
struct DirectionalShadowData
{
	half4 shadowSplitSqRadii;
	half4 lightSplitsNear;
	half4 lightSplitsFar;
	half4 shadowSplitSpheres[4];
	half4x4 world2Shadow[4];
	half4 lightShadowData;
};

///
///			DirectionalLightParameters
///
struct DirectionalLightParameters
{
    half3 color;
	half3 lightPosition;
	half3 lightDirection;
	MatrixFloats worldToLightMatrix;
	MatrixFloats lightToWorldMatrix;
	int shadowmapIndex;
	int cookieMapIndex;
	half2 cookieParameters;
	int enableOutOfPhaseColor;
    half3 outOfPhaseColor;
};

///
///			SpotLightParameters
///
struct SpotLightParameters
{
    half3 color;
	half3 lightPosition;
	half3 lightDirection;
	half lightRange;
	half lightCosHalfAngle;
	half2 angularFalloffParameters;
	half2 distanceFalloffParameters;
	MatrixFloats worldToShadowMatrix;
	int shadowMapIndex;
	half shadowStrength;
	int cookieMapIndex;
	half3 cookieParameters;
};

///
///			PointLightParameters
///
struct PointLightParameters
{
    half3 color;
    half3 lightPosition;
    half lightRange;
    half2 distanceFalloffParameters;
    MatrixFloats worldToShadowMatrix;
	#if UNITY_VERSION >= 201730
    half2 lightProjectionParameters;
	#endif
    int shadowMapIndex;
    half shadowStrength;
	int cookieMapIndex;
	half3 cookieParameters;
};