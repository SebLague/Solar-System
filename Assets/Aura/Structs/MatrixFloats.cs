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
    /// Ordered struct of Vector4 representing a matrix to be sent to the compute shader
    /// </summary>
    public struct MatrixFloats
    {
        #region Public Members
        /// <summary>
        /// Matric column A
        /// </summary>
        public Vector4 a;
        /// <summary>
        /// Matric column B
        /// </summary>
        public Vector4 b;
        /// <summary>
        /// Matric column C
        /// </summary>
        public Vector4 c;
        /// <summary>
        /// Matric column D
        /// </summary>
        public Vector4 d;
        #endregion

        #region Functions
        /// <summary>
        /// Returns the bytes size of the struct
        /// </summary>
        public static int Size
        {
            get
            {
                return sizeof(float) * 16;
            }
        }

        /// <summary>
        /// Converts a Matrix4x4 to MatrixFloats format
        /// </summary>
        /// <param name="matrix">The matrix to be converted</param>
        /// <returns>The matrix converted into the MatrixFloats format</returns>
        public static MatrixFloats ToMatrixFloats(Matrix4x4 matrix)
        {
            MatrixFloats matrixFloats = new MatrixFloats
                                        {
                                            a = matrix.GetColumn(0),
                                            b = matrix.GetColumn(1),
                                            c = matrix.GetColumn(2),
                                            d = matrix.GetColumn(3)
                                        };

            return matrixFloats;
        }
        #endregion
    }
}
