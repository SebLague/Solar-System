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
    /// Ordered struct of Levels operation parameters to be sent to the compute shader
    /// </summary>
    public struct VolumeLevelsData
    {
        #region Public Members
        /// <summary>
        /// Offsets the bottom values (similar to Levels in Photoshop)
        /// </summary>
        public float levelLowThreshold;
        /// <summary>
        /// Offsets the top values (similar to Levels in Photoshop)
        /// </summary>
        public float levelHiThreshold;
        /// <summary>
        /// Output value of the bottom threshold (similar to Levels in Photoshop, except that it is unclamped here)
        /// </summary>
        public float outputLowValue;
        /// <summary>
        /// Output value of the top threshold (similar to Levels in Photoshop, except that it is unclamped here)
        /// </summary>
        public float outputHiValue;
        /// <summary>
        /// Contrast intensity
        /// </summary>
        public float contrast;
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
                byteSize += sizeof(float); // levelLowThreshold
                byteSize += sizeof(float); // levelHiThreshold
                byteSize += sizeof(float); // outputLowValue
                byteSize += sizeof(float); // outputHiValue
                byteSize += sizeof(float); // contrast

                return byteSize;
            }
        }
        #endregion
    }
}
