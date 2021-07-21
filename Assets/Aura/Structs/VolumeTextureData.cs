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

namespace AuraAPI
{
    /// <summary>
    /// Ordered struct of texture parameters to be sent to the compute shader
    /// </summary>
    public struct VolumeTextureData
    {
        #region Public Members
        /// <summary>
        /// The tranform of the texture
        /// </summary>
        public MatrixFloats transform;
        /// <summary>
        /// The index of the texture in the composed volumetric texture. The "enable" parameter is included in this as it is set to -1 if enable == false
        /// </summary>
        public int index;
        /// <summary>
        /// Defines if the texture should loop or if the last pixel should be repeated
        /// </summary>
        public int wrapMode;
        /// <summary>
        /// Defines the texture sampling filter as bilinear or point
        /// </summary>
        public int filterMode;
        /// <summary>
        /// Allows to disable the computation of the volume's cell if the alpha value of the texture is under a defined threshold
        /// </summary>
        public int clipOnAlpha;
        /// <summary>
        /// Threshold used for computation clipping
        /// </summary>
        public float clippingThreshold;
        #endregion

        #region Functions
        /// <summary>
        /// Returns the bytes size of the struct
        /// </summary>
        public static int Size
        {
            get
            {
                int byteSize = 0;
                byteSize += MatrixFloats.Size; // transform
                byteSize += sizeof(int); // index
                byteSize += sizeof(int); // wrapMode
                byteSize += sizeof(int); // filterMode
                byteSize += sizeof(int); // clipOnAlpha
                byteSize += sizeof(float); // clippingThreshold

                return byteSize;
            }
        }
        #endregion
    }
}
