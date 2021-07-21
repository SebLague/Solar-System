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
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Collection of parameters defining a texture to be used in a volume
    /// </summary>
    [Serializable]
    public class VolumetricTexture
    {
        #region Public Members
        /// <summary>
        /// Allows to disable the computation of the volume's cell if the alpha value of the texture is under a defined threshold
        /// </summary>
        public bool clipComputationBasedOnAlpha;
        /// <summary>
        /// Threshold used for computation clipping
        /// </summary>
        [Range(0, 1)]
        public float clippingThreshold = 0.5f;
        /// <summary>
        /// Enables the volume texture
        /// </summary>
        public bool enable;
        /// <summary>
        /// Defines the texture sampling filter as bilinear or point
        /// </summary>
        public VolumetricTextureFilterModeEnum filterMode = VolumetricTextureFilterModeEnum.SameAsSource;
        /// <summary>
        /// The source Texture3D
        /// </summary>
        [Texture3DPreview]
        public Texture3D texture;
        /// <summary>
        /// Index of the texture inside the composed volumetric texture inside the volumes manager
        /// </summary>
        public int textureIndex = -1;
        /// <summary>
        /// Allows to set base position, rotation and scale and animate them
        /// </summary>
        public TransformParameters transform;
        /// <summary>
        /// Defines if the texture should loop or if the last pixel should be repeated
        /// </summary>
        public VolumetricTextureWrapModeEnum wrapMode = VolumetricTextureWrapModeEnum.SameAsSource;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VolumetricTexture()
        {
            transform.space = Space.Self;
            transform.position = Vector3.zero;
            transform.rotation = Vector3.zero;
            transform.scale = Vector3.one;
            transform.animatePosition = false;
            transform.positionSpeed = Vector3.zero;
            transform.animateRotation = false;
            transform.rotationSpeed = Vector3.zero;

            textureIndex = -1;
        }
        #endregion
    }
}
