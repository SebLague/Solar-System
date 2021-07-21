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

using System;

namespace AuraAPI
{
    /// <summary>
    /// Collection of parameters for Levels adjustement. Similar to the same tool in Photoshop.
    /// </summary>
    [Serializable]
    public struct LevelsParameters
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

        #region Private Members
        /// <summary>
        /// Packed data to be sent to the compute shader
        /// </summary>
        private VolumeLevelsData _packedData;
        #endregion

        #region Functions
        /// <summary>
        /// Set default values
        /// </summary>
        public void SetDefaultValues()
        {
            levelLowThreshold = 0;
            levelHiThreshold = 1;
            outputLowValue = 0;
            outputHiValue = 1;
            contrast = 1;
        }

        /// <summary>
        /// Packs the data and returns them
        /// </summary>
        public VolumeLevelsData Data
        {
            get
            {
                _packedData.levelLowThreshold = levelLowThreshold;
                _packedData.levelHiThreshold = levelHiThreshold;
                _packedData.outputLowValue = outputLowValue;
                _packedData.outputHiValue = outputHiValue;
                _packedData.contrast = contrast;

                return _packedData;
            }
        }
        #endregion
    }
}
