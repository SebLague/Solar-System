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
    /// Static class containing extension for int type
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Snaps the value to the closest multiple of the parameter
        /// </summary>
        /// <param name="snapValue">The reference snap value</param>
        /// <returns>The snapped value</returns>
        public static int Snap(this int value, int snapValue)
        {
            return Mathf.RoundToInt(value / (float)snapValue) * snapValue;
        }

        /// <summary>
        /// Snaps the value to the closest multiple of the parameter but forbids the value to be lower than snapValue
        /// </summary>
        /// <param name="snapValue"></param>
        /// <returns>The snapped value</returns>
        public static int SnapMin(this int value, int snapValue)
        {
            return Mathf.Max(value.Snap(snapValue), snapValue);
        }
    }
}
