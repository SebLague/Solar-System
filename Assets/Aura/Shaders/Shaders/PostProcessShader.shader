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

Shader "Hidden/Aura/PostProcessShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			
			#include "UnityCG.cginc"
			#include "../Aura.cginc"

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

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			fixed4 frag (v2f psIn) : SV_Target
			{
				float depth = tex2D(_CameraDepthTexture, psIn.uv);
				depth = LinearEyeDepth(depth);

				float4 backColor = tex2D(_MainTex, psIn.uv);
				Aura_ApplyFog(backColor.xyz, float3(psIn.uv, depth));

				//// Debug fogless backbuffer
				//const float thumbnailFactor = 0.25f;
				//float4 thumbnail = tex2D(_MainTex, psIn.uv / thumbnailFactor);
				//float thumbnailMask = step(psIn.uv.x, thumbnailFactor) * step(psIn.uv.y, thumbnailFactor);
				//backColor = lerp(backColor, thumbnail, thumbnailMask);

				return backColor;
			}
			ENDCG
		}
	}
}
