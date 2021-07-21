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

/// Shader used for drawing the color disk of the CircularPicker attribute
Shader "Hidden/Aura/GUI/DrawCircularPickerDisk"
{
    CGINCLUDE

        #include "UnityCG.cginc"

        #define PI 3.14159265359
        #define PI2 6.28318530718

		float colorDiskSize;
		float2 pickerCoordinates;
		float alpha;

        float3 HsvToRgb(float3 rgb)
        {
			rgb.x = 1.0 - ((rgb.x > 0.0) ? rgb.x : PI2 + rgb.x) / PI2;

            float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 p = abs(frac(rgb.xxx + K.xyz) * 6.0 - K.www);
            return rgb.z * lerp(K.xxx, saturate(p - K.xxx), rgb.y);
        }

        float4 DrawDisk(v2f_img i, float offsetColor)
        {
            float4 color = (0.0).xxxx;
            float2 uvc = i.uv - (0.5).xx;
			float dist = sqrt(dot(uvc, uvc));
            float delta = fwidth(dist);
            float angle = atan2(uvc.x, uvc.y);

			color.w = smoothstep(0.5f, colorDiskSize + delta, dist) * offsetColor;

            float circleMask = smoothstep(colorDiskSize + delta, colorDiskSize - delta, dist);
			float saturationMask = smoothstep(0, colorDiskSize - delta, dist);
            float hue = angle;
            float4 c = float4(HsvToRgb(float3(angle, saturationMask, 1)), 1.0) * circleMask;
            color += c;

			float2 refPos = pickerCoordinates;
			const float sizeFactor = 0.02f;
			const float widthFactor = 0.005f;
			float uvDist = distance(refPos, i.uv);
			float ring = smoothstep(sizeFactor - widthFactor, sizeFactor, uvDist) - smoothstep(sizeFactor, sizeFactor + widthFactor, uvDist);
			color *= lerp(0.65f, 1.0f, pow(smoothstep(sizeFactor + delta * widthFactor, sizeFactor * 5, uvDist) + smoothstep(sizeFactor - delta * widthFactor, 0, uvDist), 0.25));
			color += ring;

			color.w *= alpha;

            return color;
        }

        float4 FragDiskDark(v2f_img i) : SV_Target
        {
            return DrawDisk(i, 0.3);
        }

        float4 FragDiskLight(v2f_img i) : SV_Target
        {
            return DrawDisk(i, 0.6);
        }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        // (0) Dark skin
        Pass
        {
            CGPROGRAM

                #pragma vertex vert_img
                #pragma fragment FragDiskDark

            ENDCG
        }

        // (1) Light skin
        Pass
        {
            CGPROGRAM

                #pragma vertex vert_img
                #pragma fragment FragDiskLight

            ENDCG
        }
    }
}
