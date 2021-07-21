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

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///                                                                                                                                                             ///
///     Inspired from 																																			///
/// 	Bart Wronski's publication : https://bartwronski.files.wordpress.com/2014/08/bwronski_volumetric_fog_siggraph2014.pdf#page=60                           ///
///                                                                                                                                                             ///
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

half temporalReprojectionFactor;
float4x4 previousFrameWorldToClipMatrix;
Texture3D<half4> previousFrameLightingVolumeTexture;

// https://github.com/bartwronski/PoissonSamplingGenerator
static const uint SAMPLE_NUM = 64;
static const half2 POISSON_SAMPLES[SAMPLE_NUM] =
{
    half2(-0.07640588584669661f, 0.33643912829181566f),
	half2(0.4603682891206121f, -0.16214851534407315f),
	half2(0.4084708740004196f, 0.3185191164757333f),
	half2(-0.05362013809472688f, -0.20117450575960172f),
	half2(0.20217535415860277f, 0.061621533383742655f),
	half2(0.21573395821908126f, -0.41121748513438927f),
	half2(-0.2949592265086812f, 0.09226435176380277f),
	half2(-0.3405923479636602f, -0.4438064636733101f),
	half2(-0.30499654442143764f, -0.18980579535343145f),
	half2(0.20696874865831327f, -0.18068220212220576f),
	half2(0.46123313431641866f, 0.06431877430102828f),
	half2(0.1699463437722606f, 0.2979548008843864f),
	half2(-0.04172635476663911f, 0.03951187600664341f),
	half2(-0.09818068884760212f, -0.444598812191877f),
	half2(0.44519739828313265f, -0.4049100510821759f),
	half2(-0.37409253370980877f, 0.3141497367365731f),
	half2(-0.21214437004036268f, 0.2411070317491779f),
	half2(-0.4321306098160903f, -0.2901497097651563f),
	half2(0.02741451074790302f, -0.3440369136248822f),
	half2(0.06691513200482002f, 0.4488679854891062f),
	half2(-0.39897387149358343f, -0.04824340870756816f),
	half2(0.07387793866300807f, -0.07979052394705599f),
	half2(-0.1662065245046429f, -0.07204544490657572f),
	half2(0.2817744572708647f, 0.19729998811457627f),
	half2(0.3272179663753686f, -0.07902303096884378f),
	half2(-0.48532474326077146f, 0.21521443439565902f),
	half2(-0.46605381726256234f, 0.48331393302344017f),
	half2(-0.23787196845708847f, 0.40012691610630635f),
	half2(0.06507951581106097f, 0.1667944790374546f),
	half2(-0.1707094160925735f, -0.31871612566190155f),
	half2(0.3174590800125161f, -0.49359755597515975f),
	half2(0.329748389239532f, -0.28501158356956013f),
	half2(0.20386780332434207f, -0.060941916256976314f),
	half2(0.27695501890798346f, 0.37216648625798243f),
	half2(-0.29717215536046804f, -0.32589953779485337f),
	half2(0.32020176895843244f, 0.05031706099251987f),
	half2(0.15349387447363794f, -0.29306472916370063f),
	half2(-0.32499854285625496f, 0.21527500996968085f),
	half2(-0.11057407325931656f, 0.19355726503478976f),
	half2(-0.17506364844262712f, 0.06528313682759768f),
	half2(-0.11388843738637733f, 0.4450412649726869f),
	half2(0.09366057710684017f, -0.44239743856916725f),
	half2(0.19473437505001023f, 0.4667860862252081f),
	half2(0.4210793453599708f, 0.4568152789733342f),
	half2(-0.344498509150036f, 0.4398705471807214f),
	half2(-0.42985583396575056f, -0.17343049535345967f),
	half2(-0.4230335482412495f, 0.10905196472949885f),
	half2(0.07062712137902583f, -0.1963188196865744f),
	half2(-0.48918202667452204f, 0.33198050292769377f),
	half2(-0.2281681963993003f, -0.42925584483192425f),
	half2(0.04227553568380815f, 0.30678144681443953f),
	half2(0.481592504651012f, -0.03790688242982443f),
	half2(0.41113847881375587f, 0.18325205452623938f),
	half2(-0.27478093770518697f, -0.019087749542861965f),
	half2(0.3112229897257863f, -0.17570404134955708f),
	half2(0.4238511641061522f, -0.26403372084429855f),
	half2(0.10219473172003157f, 0.03838932768055159f),
	half2(-0.04392496026640913f, -0.09039127908490785f),
	half2(-0.45795735494323486f, -0.40662868907871463f),
	half2(0.15779619247568688f, 0.19051503688512006f),
	half2(-0.20140878979764365f, -0.15912301876838675f),
	half2(-0.017503260419413635f, -0.47632855698027277f),
	half2(-0.21972913100327074f, 0.15109861863332819f),
	half2(0.34052992865135934f, -0.40036368727883465f),
};

int GetJitterOffsetArrayIndex(uint3 id)
{
    return (_frameID + id.x + id.y * 2) % SAMPLE_NUM;
}

// "Usual" random function proposed by Gil Damoiseaux (@gaxil) -> frac(sin(x) * 43758.5453123
// But performances were not good with the sin function and the sign was lost.
// So I replace the sin with a polynomial approximation (Taylor/Remez were even slower than sin() so I simplified https://github.com/Genbox/Approximation.Net/blob/master/Approximation.NET/Approximation.cs) and use the modulo to keep the fractional part but also the sign.
half GetNoise(half x)
{
    x %= twoPi;
    x -= pi;
    return (((1.27323954f * x) - (0.405284735f * x * x * sign(x))) * 43758.5453123f) % 1;
}

half3 GetJitterOffset(uint3 id)
{
    half3 jitter;
    jitter.xy = POISSON_SAMPLES[GetJitterOffsetArrayIndex(id)];
    jitter.z = GetNoise(id.x * 1.23f + id.y * 0.97f + (id.z + _frameID) * 1.01f + 236526.0f) * 0.5f; // Used to jitter with a Poisson 1D/3D table but the pattern was very visible. Gil Damoiseaux (@gaxil) had the idea of using the random function instead.

    return jitter * temporalReprojectionFactor;
}

void JitterPosition(inout half3 position, uint3 id)
{
    position += GetJitterOffset(id) / bufferResolution.xyz;
}

void ReprojectPreviousFrame(inout half4 accumulationColor, half3 worldPosition)
{
	half4 previousClipPosition = mul(previousFrameWorldToClipMatrix, half4(worldPosition, 1));
	previousClipPosition.xy /= previousClipPosition.w;
	previousClipPosition.xy = previousClipPosition.xy * 0.5f + 0.5f;
	previousClipPosition.z /= cameraRanges.y;

    [branch]
	if (previousClipPosition.w > 0.0f && dot(previousClipPosition.xyz - saturate(previousClipPosition.xyz), 1) == 0.0f)
	{
        half4 previousData = previousFrameLightingVolumeTexture.SampleLevel(_LinearClamp, previousClipPosition.xyz, 0);
        [branch]
        if(previousData.w > 0)
        {
		    accumulationColor = lerp( accumulationColor, previousData, temporalReprojectionFactor);
        }
	}
}