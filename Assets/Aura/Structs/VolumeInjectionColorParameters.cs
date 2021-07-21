﻿/***************************************************************************
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
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    ///Volume injection parameters for color
    /// </summary>
    [Serializable]
    public struct VolumeInjectionColorParameters
    {
        /// <summary>
        /// Injection parameters
        /// </summary>
        public VolumeInjectionCommonParameters injectionParameters;
        /// <summary>
        /// Color of the injection
        /// </summary>
        [SerializeField]
        [ColorCircularPicker]
        public Color color;
    }
}
