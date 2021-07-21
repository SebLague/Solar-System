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

using System;

namespace AuraAPI
{
    /// <summary>
    /// Bitmask representing the possible parameters for the volumetric data computation
    /// </summary>
    [Flags]
    public enum FrustumParametersEnum
    {
        EnableNothing                           = 0,
        EnableOcclusionCulling                  = 1 << 0,
        EnableTemporalReprojection              = 1 << 1,
        EnableVolumes                           = 1 << 2,
        EnableVolumesNoiseMask                  = 1 << 3,
        EnableVolumesTextureMask                = 1 << 4,
        EnableDirectionalLights                 = 1 << 5,
        EnableDirectionalLightsShadows          = 1 << 6,
        DirectionalLightsShadowsOneCascade      = 1 << 7,
        DirectionalLightsShadowsTwoCascades     = 1 << 8,
        DirectionalLightsShadowsFourCascades    = 1 << 9,
        EnableSpotLights                        = 1 << 10,
        EnableSpotLightsShadows                 = 1 << 11,
        EnablePointLights                       = 1 << 12,
        EnablePointLightsShadows                = 1 << 13,
        EnableLightsCookies                     = 1 << 14
    }
}
