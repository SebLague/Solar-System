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

using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Texture2DArray composer used for collecting directional shadow data maps
    /// </summary>
    public class DirectionalShadowDataCollector : Texture2DArrayComposer
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sizeX">The width of the built Texture2DArray</param>
        /// <param name="sizeY">The height of the built Texture2DArray</param>
        public DirectionalShadowDataCollector(int sizeX, int sizeY) : base(sizeX, sizeY, TextureFormat.RGBAFloat, true)
        {
            alwaysGenerateOnUpdate = true;
        }
        #endregion
    }
}
