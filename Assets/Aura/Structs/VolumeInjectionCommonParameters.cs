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
    /// Base volume injection parameters for density/anisotropy/color
    /// </summary>
    [Serializable]
    public struct VolumeInjectionCommonParameters
    {
        /// <summary>
        /// Enables the injection
        /// </summary>
        public bool enable;
        /// <summary>
        /// Sets the strength of the injection
        /// </summary>
        public float strength;
        /// <summary>
        /// Levels parameters for the texture mask. Similar to the Levels adjustement tool in Photoshop
        /// </summary>
        public LevelsParameters textureMaskLevelParameters;
        /// <summary>
        /// Levels parameters for the noise mask. Similar to the Levels adjustement tool in Photoshop
        /// </summary>
        public LevelsParameters noiseMaskLevelParameters;
    }
}
