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
using UnityEngine.Rendering;

namespace AuraAPI
{
    /// <summary>
    /// Collection of texture buffers which contain the computed volumetric data
    /// </summary>
    public class Buffers
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Buffers(Vector3Int resolution)
        {
            Resolution = resolution;
            CreateBuffers();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the resolution of the buffers
        /// </summary>
        public Vector3Int Resolution
        {
            get;
            private set;
        }

        /// <summary>
        /// Accessor to the volumetric data buffer (containing the lighting (RGB) and the density (A))
        /// </summary>
        public SwappableRenderTexture LightingVolumeTextures
        {
            get;
            private set;
        }

        /// <summary>
        /// Accessor to the volumetric accumulated buffer (containing the accumulated lighting)
        /// </summary>
        public RenderTexture FogVolumeTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Accessor to the buffer containing the maximum depth
        /// </summary>
        public SwappableRenderTexture OcclusionTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates the needed texture buffers
        /// </summary>
        private void CreateBuffers()
        {
            LightingVolumeTextures = new SwappableRenderTexture(Resolution.x, Resolution.y, Resolution.z, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear, TextureWrapMode.Clamp, FilterMode.Bilinear);

            FogVolumeTexture = CreateTexture(RenderTextureFormat.ARGBHalf, TextureDimension.Tex3D, TextureWrapMode.Clamp, FilterMode.Bilinear);
            Shader.SetGlobalTexture("Aura_VolumetricLightingTexture", FogVolumeTexture);

            OcclusionTexture = new SwappableRenderTexture(Resolution.x, Resolution.y, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear, TextureWrapMode.Clamp, FilterMode.Point);
        }

        /// <summary>
        /// Generic function to create a texture buffer
        /// </summary>
        /// <param name="format">The desired format</param>
        /// <param name="dimensions">The desired dimensions</param>
        /// <param name="wrapMode">The desired wrap mode</param>
        /// <param name="filterMode">The desired filter mode</param>
        /// <returns>The texture buffer</returns>
        private RenderTexture CreateTexture(RenderTextureFormat format, TextureDimension dimensions, TextureWrapMode wrapMode, FilterMode filterMode)
        {
            RenderTexture texture = new RenderTexture(Resolution.x, Resolution.y, 0, format, RenderTextureReadWrite.Linear);
            texture.dimension = dimensions;
            if(dimensions == TextureDimension.Tex3D)
            {
                texture.volumeDepth = Resolution.z;
            }
            texture.wrapMode = wrapMode;
            texture.filterMode = filterMode;
            texture.enableRandomWrite = true;
            texture.Create();

            return texture;
        }

        /// <summary>
        /// Clears the content of the volumetric data buffer (containing the lighting (RGB) and the density (A)) to 0 (Black)
        /// </summary>
        public void ClearVolumetricFogTexture()
        {
            FogVolumeTexture.Clear(Color.black);
        }

        /// <summary>
        /// Releases the different buffers
        /// </summary>
        public void ReleaseBuffers()
        {
            LightingVolumeTextures.Release();
            FogVolumeTexture.Release();
            OcclusionTexture.Release();
        }
        #endregion
    }
}
