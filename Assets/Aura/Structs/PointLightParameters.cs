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
    /// Struct containing parameters of a point AuraLight
    /// </summary>
    public struct PointLightParameters
    {
        #region Public Members
        public Vector3 color;
        public Vector3 lightPosition;
        public float lightRange;
        public Vector2 distanceFalloffParameters;
        public MatrixFloats worldToShadowMatrix;
        #if UNITY_2017_3_OR_NEWER
        public Vector2 lightProjectionParameters;
        #endif
        public int shadowMapIndex;
        public float shadowStrength;
        public int cookieMapIndex;
        public Vector3 cookieParameters;
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
                byteSize += sizeof(float); //lightRange
                byteSize += sizeof(float) * 2; //distanceFalloffParameters
                byteSize += MatrixFloats.Size; //worldToLightMatrix
                #if UNITY_2017_3_OR_NEWER
                byteSize += sizeof(float) * 2; //lightProjectionParameters
                #endif
                byteSize += sizeof(int); //shadowMapIndex
                byteSize += sizeof(float); //shadowStrength
                byteSize += sizeof(int); //cookieMapIndex
                byteSize += sizeof(float) * 3; //cookieParameters

                return byteSize;
            }
        }
        #endregion
    }
}
