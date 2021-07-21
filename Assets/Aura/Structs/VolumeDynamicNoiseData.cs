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
    /// Ordered struct of noise parameters to be sent to the compute shader
    /// </summary>
    public struct VolumeDynamicNoiseData
    {
        #region Public Members
        /// <summary>
        /// Enables the dynamic noise computation
        /// </summary>
        public int enable;
        /// <summary>
        /// The tranform of the noise
        /// </summary>
        public MatrixFloats transform;
        /// <summary>
        /// The speed of the noise mutation
        /// </summary>
        public float speed;
        /// <summary>
        /// Offset of the mutation timing to allow de-synchronization
        /// </summary>
        public float offset;
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
                byteSize += sizeof(int); // enable
                byteSize += MatrixFloats.Size; // transform
                byteSize += sizeof(float); // speed
                byteSize += sizeof(float); // offset

                return byteSize;
            }
        }
        #endregion
    }
}
