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
    /// Collection of settings defining the computation of the volumetric data
    /// </summary>
    public struct FrustumSettingsParameters
    {
        #region Public Members
        /// <summary>
        /// Enables the computation of volumes
        /// </summary>
        public bool enableVolumes;
        /// <summary>
        /// Enables the computation of volumes' texture mask
        /// </summary>
        public bool enableVolumesTextureMask;
        /// <summary>
        /// Enables the computation of volumes' noise mask
        /// </summary>
        public bool enableVolumesNoiseMask;
        /// <summary>
        /// Enables the computation of directional lights
        /// </summary>
        public bool enableDirectionalLights;
        /// <summary>
        /// Enables the computation of directional lights' shadow
        /// </summary>
        public bool enableDirectionalLightsShadows;
        /// <summary>
        /// Enables the computation of spot lights
        /// </summary>
        public bool enableSpotLights;
        /// <summary>
        /// Enables the computation of spot lights' shadow
        /// </summary>
        public bool enableSpotLightsShadows;
        /// <summary>
        /// Enables the computation of point lights
        /// </summary>
        public bool enablePointLights;
        /// <summary>
        /// Enables the computation of point lights' shadow
        /// </summary>
        public bool enablePointLightsShadows;
        /// <summary>
        /// Enables the computation of lights' cookie
        /// </summary>
        public bool enableLightsCookies;
        /// <summary>
        /// Enables depth occlusion culling
        /// </summary>
        public bool enableOcclusionCulling;
        /// <summary>
        /// The accuracy of the occlusion search
        /// </summary>
        public OcclusionCullingAccuracyEnum occlusionCullingAccuracy;
        /// <summary>
        /// Enables temporal reprojection
        /// </summary>
        public bool enableTemporalReprojection;
        /// <summary>
        /// Amount of reprojection with the previous frame
        /// </summary>
        [Range(0, 1)]
        public float temporalReprojectionFactor;
        /// <summary>
        /// The resolution of the frustum grid
        /// </summary>
        public Vector3Int resolution;
        /// <summary>
        /// The maximum distance where the volumetric data will be computed
        /// </summary>
        public float farClipPlaneDistance;
        #endregion

        #region Functions
        /// <summary>
        /// Initializes the struct to default values
        /// </summary>
        public void Init()
        {
            enableVolumes = true;
            enableVolumesTextureMask = true;
            enableVolumesNoiseMask = true;
            enableDirectionalLights = true;
            enableDirectionalLightsShadows = true;
            enableSpotLights = true;
            enableSpotLightsShadows = true;
            enablePointLights = true;
            enablePointLightsShadows = true;
            enableLightsCookies = true;
            enableOcclusionCulling = true;
            enableTemporalReprojection = true;
            temporalReprojectionFactor = 0.9f;
            resolution = new Vector3Int(160, 90, 128);
            farClipPlaneDistance = 25.0f;
        }
        #endregion
    }
}
