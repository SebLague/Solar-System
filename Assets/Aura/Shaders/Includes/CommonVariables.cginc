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

#define NUM_THREAD_X 8
#define NUM_THREAD_Y 8
#define NUM_THREAD_Z 16

// Time (to be set from Unity)
float time;

// SamplerStates
SamplerState _LinearClamp;
SamplerState _LinearRepeat;
SamplerState _PointClamp;
SamplerState _PointRepeat;

// Const variables
static const float pi = 3.141592653589793f;
static const float twoPi = pi * 2.0f;
static const float halfPi = pi * 0.5f;
static const float e = 2.71828182845904523536f;
static const float n = 1.0f / e;
static const float normalizedDepthBiasExponent = 1.0f;

// Common variables
float4 bufferResolution;
float3 cameraPosition;
float4 cameraRanges;
float4 zParameters;
float volumeDepth; 
float layerDepth;
float invLayerDepth;