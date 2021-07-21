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

StructuredBuffer<VolumeData> volumeDataBuffer;
uint volumeCount;
Texture3D<half4> volumeMaskTexture;

half GetShapeGradient(VolumeData volumeData, inout half3 position)
{
    half gradient = 1;

    [branch]
	if (volumeData.shape == 1)
	{
		position = TransformPoint(position, volumeData.transform.ToMatrix());
		gradient = ClampedInverseLerp(volumeData.yPositiveFade, 0, position.y);
	}
	else if (volumeData.shape == 2)
	{
		position = TransformPoint(position, volumeData.transform.ToMatrix());
		half x = ClampedInverseLerp(-0.5f, -0.5f + volumeData.xNegativeFade, position.x) - ClampedInverseLerp(0.5f - volumeData.xPositiveFade, 0.5f, position.x);
		half y = ClampedInverseLerp(-0.5f, -0.5f + volumeData.yNegativeFade, position.y) - ClampedInverseLerp(0.5f - volumeData.yPositiveFade, 0.5f, position.y);
		half z = ClampedInverseLerp(-0.5f, -0.5f + volumeData.zNegativeFade, position.z) - ClampedInverseLerp(0.5f - volumeData.zPositiveFade, 0.5f, position.z);
		gradient = saturate(lerp(0, lerp(0, x, y), z));
	}
	else if (volumeData.shape == 3)
	{
		position = TransformPoint(position, volumeData.transform.ToMatrix());
		gradient = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position));
	}
	else if (volumeData.shape == 4)
	{
		position = TransformPoint(position, volumeData.transform.ToMatrix());
		half y = ClampedInverseLerp(-0.5f, -0.5f + volumeData.yNegativeFade, position.y) - ClampedInverseLerp(0.5f - volumeData.yPositiveFade, 0.5f, position.y);
		half xz = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position.xz));
		gradient = lerp(0, xz, y);
	}
	else if (volumeData.shape == 5)
	{
		position = TransformPoint(position, volumeData.transform.ToMatrix());
		half z = ClampedInverseLerp(1, 1.0f - volumeData.zPositiveFade * 2, position.z);
		half xy = ClampedInverseLerp(0.5f, 0.5f - volumeData.xPositiveFade * 0.5f, length(position.xy / saturate(position.z)));
		gradient = lerp(0, xy, z);
	}

	return gradient;
}

void ComputeVolumeContribution(VolumeData volumeData, half3 worldPosition, inout half density, inout half anisotropy, inout half3 color)
{
	half gradient = GetShapeGradient(volumeData, worldPosition);

    [branch]
	if (gradient > 0)
	{		
        half densityMask = 1.0f;
        half anisotropyMask = 1.0f;
        half3 colorMask = half3(1,1,1);

        #if ENABLE_VOLUMES_TEXTURE_MASK
        [branch]
        if(volumeData.textureData.index > -1)
        {
	        uint width;
	        uint height;
	        uint depth;
	        volumeMaskTexture.GetDimensions(width, height, depth);
	        half3 samplingPosition = GetCombinedTexture3dCoordinates(worldPosition, (half)width, (half)depth, (half)volumeData.textureData.index, volumeData.textureData.transform.ToMatrix(), volumeData.textureData.wrapMode, volumeData.textureData.filterMode);
	        half4 textureMask = volumeMaskTexture.SampleLevel(_LinearClamp, samplingPosition, 0);
        
            [branch]
            if(volumeData.textureData.clipOnAlpha && volumeData.textureData.clippingThreshold > textureMask.w)
            {
                return;
            }
        
            densityMask *= LevelValue(volumeData.densityTextureLevelsParameters, textureMask.w);
            anisotropyMask *= LevelValue(volumeData.anisotropyTextureLevelsParameters, textureMask.w);
            colorMask *= LevelValue(volumeData.colorTextureLevelsParameters, textureMask.xyz);
        }
        #endif
        
        #if ENABLE_VOLUMES_NOISE_MASK
        [branch]
        if(volumeData.noiseData.enable)
        {
	        half3 noisePosition = TransformPoint(worldPosition, volumeData.noiseData.transform.ToMatrix());
            half noiseMask = snoise(half4(noisePosition, (time + volumeData.noiseData.offset) * volumeData.noiseData.speed)) * 0.5f + 0.5f;

			densityMask *= LevelValue(volumeData.densityNoiseLevelsParameters, noiseMask);
			anisotropyMask *= LevelValue(volumeData.anisotropyNoiseLevelsParameters, noiseMask);
			colorMask *= LevelValue(volumeData.colorNoiseLevelsParameters, noiseMask);
        }
        #endif
        
		gradient = pow(abs(gradient), volumeData.falloffExponent);
    
        [branch]
	    if (volumeData.injectDensity)
	    {
		    density += volumeData.densityValue * gradient * densityMask;
	    }
    
        [branch]
        if (volumeData.injectAnisotropy)
        {
		    anisotropy += volumeData.anisotropyValue * gradient * anisotropyMask;
        }
    
        [branch]
	    if (volumeData.injectColor == 1)
	    {
	        color += volumeData.colorValue * gradient * colorMask;
        }
	}
}

void ComputeVolumesInjection(half3 worldPosition, half3 viewVector, inout half4 accumulationColor, inout half density, inout half anisotropy)
{
    [allow_uav_condition]
	for (uint i = 0; i < volumeCount; ++i)
	{
		ComputeVolumeContribution(volumeDataBuffer[i], worldPosition, density, anisotropy, accumulationColor.xyz);
	}

	density = max(0, density);
	anisotropy = saturate(anisotropy);
}