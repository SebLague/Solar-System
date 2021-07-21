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
    /// Static class containing extension for Vector3 type
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Formats a Vector3 into a array of floats
        /// </summary>
        /// <returns>The array of floats</returns>
        public static float[] AsFloatArray(this Vector3 vector)
        {
            float[] floatArray = new float[3];
            floatArray[0] = vector.x;
            floatArray[1] = vector.y;
            floatArray[2] = vector.z;

            return floatArray;
        }

        /// <summary>
        /// Formats an array of Vector3 into a array of floats
        /// </summary>
        /// <returns>The array of floats</returns>
        public static float[] AsFloatArray(this Vector3[] vector)
        {
            float[] floatArray = new float[vector.Length * 3];

            for(int i = 0; i < vector.Length; ++i)
            {
                floatArray[i * 3] = vector[i].x;
                floatArray[i * 3 + 1] = vector[i].y;
                floatArray[i * 3 + 2] = vector[i].z;
            }

            return floatArray;
        }
    }
}
