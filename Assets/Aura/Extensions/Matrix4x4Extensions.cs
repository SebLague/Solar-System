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
    /// Collection of extension functions for Matrix4x4 objects
    /// </summary>
    public static class Matrix4X4Extensions
    {
        /// <summary>
        /// Converts the matrix to the MatrixFloats format
        /// </summary>
        /// <returns>The converted MatrixFloats</returns>
        public static MatrixFloats ToAuraMatrixFloats(this Matrix4x4 matrix)
        {
            return MatrixFloats.ToMatrixFloats(matrix);
        }

        /// <summary>
        /// Converts the matrix to an array of floats
        /// </summary>
        /// <returns>The array of floats</returns>
        public static float[] ToFloatArray(this Matrix4x4 matrix)
        {
            float[] matrixFloats =
            {
                matrix[0, 0],
                matrix[1, 0],
                matrix[2, 0],
                matrix[3, 0],
                matrix[0, 1],
                matrix[1, 1],
                matrix[2, 1],
                matrix[3, 1],
                matrix[0, 2],
                matrix[1, 2],
                matrix[2, 2],
                matrix[3, 2],
                matrix[0, 3],
                matrix[1, 3],
                matrix[2, 3],
                matrix[3, 3]
            };

            return matrixFloats;
        }
    }
}
