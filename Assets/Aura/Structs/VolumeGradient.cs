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
    /// 
    /// </summary>
    [Serializable]
    public struct VolumeGradient
    {
        /// <summary>
        /// Exponent of the fading gradient
        /// </summary>
        public float falloffExponent;

        #region Cube fading parameters
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local X axis
        /// </summary>
        public float xPositiveCubeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the negative local X axis
        /// </summary>
        public float xNegativeCubeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local Y axis
        /// </summary>
        public float yPositiveCubeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the negative local Y axis
        /// </summary>
        public float yNegativeCubeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local Z axis
        /// </summary>
        public float zPositiveCubeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the negative local Z axis
        /// </summary>
        public float zNegativeCubeFade;
        #endregion

        #region Cone fading parameters
        /// <summary>
        /// Normalized size of the fading on the borders, depending on the angle
        /// </summary>
        public float angularConeFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local Z axis
        /// </summary>
        public float distanceConeFade;
        #endregion

        #region Cylinder fading parameters
        /// <summary>
        /// Normalized size of the fading on the borders, depending on the distance with the barycenter
        /// </summary>
        public float widthCylinderFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the negative local Y axis
        /// </summary>
        public float yNegativeCylinderFade;
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local Y axis
        /// </summary>
        public float yPositiveCylinderFade;
        #endregion

        #region Plane fading parameters
        /// <summary>
        /// Normalized size of the fading on the borders, on the positive local Y axis
        /// </summary>
        public float heightPlaneFade;
        #endregion

        #region Sphere fading parameters
        /// <summary>
        ///Normalized size of the fading on the borders, depending on the distance with the center
        /// </summary>
        public float distanceSphereFade;
        #endregion
    }
}
