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
    /// Struct containing parameters of a directional AuraLight
    /// </summary>
    public struct DirectionalLightParameters
    {
        #region Public Members
        public Vector3 color;
        public Vector3 lightPosition;
        public Vector3 lightDirection;
        public MatrixFloats worldToLightMatrix;
        public MatrixFloats lightToWorldMatrix;
        public int shadowMapIndex;
        public int cookieMapIndex;
        public Vector2 cookieParameters;
        public int enableOutOfPhaseColor;
        public Vector3 outOfPhaseColor;
        #endregion

        #region Functions
        /// <summary>
        /// Returns the byte size of the struct
        /// </summary>
        public static int Size
        {
            get
            {
                int byteSize = 0;
                byteSize += sizeof(float) * 3; //color
                byteSize += sizeof(float) * 3; //lightPosition
                byteSize += sizeof(float) * 3; //lightDirection
                byteSize += MatrixFloats.Size; //worldToLightMatrix
                byteSize += MatrixFloats.Size; //lightToWorldMatrix
                byteSize += sizeof(int); //shadowMapIndex
                byteSize += sizeof(int); //cookieMapIndex
                byteSize += sizeof(float) * 2; //cookieParameters
                byteSize += sizeof(int); //enableOutOfPhaseColor
                byteSize += sizeof(float) * 3; //outOfPhaseColor

                return byteSize;
            }
        }
        #endregion
    }
}
