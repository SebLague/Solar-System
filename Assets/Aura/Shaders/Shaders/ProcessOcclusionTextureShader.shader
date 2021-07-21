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

Shader "Hidden/Aura/ProcessOcclusionTextureShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../Aura.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float frag (v2f psIn) : SV_Target
			{
				// Finds the deepest value of neighbours
				half maxValue = 0.0f;    
				float samplingU;
				float samplingV;
				[unroll]
				for(int i = -1; i < 2; ++i)
				{
					samplingU = psIn.uv.x + i * _MainTex_TexelSize.x;
					[branch]
					if(samplingU > 0 && samplingU < 1)
					{
						[unroll]
						for(int j = -1; j < 2; ++j)
						{
							samplingV = psIn.uv.y + j * _MainTex_TexelSize.y;
							[branch]
							if(samplingV > 0 && samplingV < 1)
							{
								maxValue = max(maxValue, tex2D(_MainTex, float2(samplingU,samplingV)));
							}
						}
					}        
				}

				return maxValue + (1.0f / bufferResolution.z); // Offsets of one cell depth to avoid "z-fighting effect";
			}
			ENDCG
		}
	}
}
