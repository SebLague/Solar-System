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

Texture2DArray _blueNoiseTexturesArray;
int _frameID;

#ifndef UNITY_SHADER_VARIABLES_INCLUDED
float4 _ScreenParams;
#endif

float4 GetBlueNoise(float2 screenPos, int idOffset)
{
	// TODO : HARDCODE OR PASS BLUENOISE DATA
	uint blueNoiseWidth;
	uint blueNoiseHeight;
	uint blueNoiseSize;
	_blueNoiseTexturesArray.GetDimensions(blueNoiseWidth, blueNoiseHeight, blueNoiseSize);
	uint4 blueNoiseSamplingPosition = uint4(screenPos * _ScreenParams.xy % float2(blueNoiseWidth, blueNoiseHeight), (_frameID + idOffset) % blueNoiseSize, 0);
	float4 blueNoise = _blueNoiseTexturesArray.Load(blueNoiseSamplingPosition);
	blueNoise = mad(blueNoise, 2.0f, -1.0f);
	blueNoise = sign(blueNoise)*(1.0f - sqrt(1.0f - abs(blueNoise)));
	blueNoise /= 255.0f;

    return blueNoise;
}