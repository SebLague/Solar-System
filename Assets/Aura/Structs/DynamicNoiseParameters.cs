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
    /// Collection of parameters for 4D dynamic noise mask to be used in a volume
    /// </summary>
    [Serializable]
    public struct DynamicNoiseParameters
    {
        /// <summary>
        /// Enables the dynamic noise computation
        /// </summary>
        public bool enable;
        /// <summary>
        /// The speed of the mutation
        /// </summary>
        public float speed;
        /// <summary>
        /// Offset of the mutation timing to allow de-synchronization
        /// </summary>
        public float offset;
        /// <summary>
        /// Allows to set base position, rotation and scale and animate them
        /// </summary>
        public TransformParameters transform;
    }
}
