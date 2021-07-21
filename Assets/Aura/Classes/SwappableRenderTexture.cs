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
    /// Double buffering render texture
    /// </summary>
    public class SwappableRenderTexture
    {
        #region Private Members
        /// <summary>
        /// The two swapped render textures
        /// </summary>
        private readonly RenderTexture[] _buffers = new RenderTexture[2];
        /// <summary>
        /// The ID of the read texture
        /// </summary>
        private int _readId = 0;
        /// <summary>
        /// The ID of the write texture
        /// </summary>
        private int _writeId = 1;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a 2D swappable RenderTexture
        /// </summary>
        /// <param name="width">The width of the textures</param>
        /// <param name="height">The height of the textures</param>
        /// <param name="format">The format of the textures</param>
        /// <param name="sRgbSampling">The color space of the textures</param>
        /// <param name="wrapMode">The wrap mode of the textures</param>
        /// <param name="filterMode">The filter mode of the textures</param>
        public SwappableRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite sRgbSampling, TextureWrapMode wrapMode, FilterMode filterMode)
        {
            CreateRenderTexture(0, width, height, -1, format, sRgbSampling, wrapMode, filterMode);
            CreateRenderTexture(1, width, height, -1, format, sRgbSampling, wrapMode, filterMode);
        }

        /// <summary>
        /// Constructor for a 3D swappable RenderTexture
        /// </summary>
        /// <param name="width">The width of the textures</param>
        /// <param name="height">The height of the textures</param>
        /// <param name="depth">The depth of the textures</param>
        /// <param name="format">The format of the textures</param>
        /// <param name="sRgbSampling">The color space of the textures</param>
        /// <param name="wrapMode">The wrap mode of the textures</param>
        /// <param name="filterMode">The filter mode of the textures</param>
        public SwappableRenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite sRgbSampling, TextureWrapMode wrapMode, FilterMode filterMode)
        {
            CreateRenderTexture(0, width, height, depth, format, sRgbSampling, wrapMode, filterMode);
            CreateRenderTexture(1, width, height, depth, format, sRgbSampling, wrapMode, filterMode);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the read texture
        /// </summary>
        public RenderTexture ReadBuffer
        {
            get
            {
                return _buffers[_readId];
            }
        }

        /// <summary>
        /// Accessor to the write texture
        /// </summary>
        public RenderTexture WriteBuffer
        {
            get
            {
                return _buffers[_writeId];
            }
        }

        /// <summary>
        /// Creates a formated RenderTexture and assign it to an ID
        /// </summary>
        /// <param name="index">The target ID of the texture</param>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="depth">The depth of the texture</param>
        /// <param name="format">The format of the texture</param>
        /// <param name="sRgbSampling">The color space of the texture</param>
        /// <param name="wrapMode">The wrap mode of the texture</param>
        /// <param name="filterMode">The filter mode of the texture</param>
        private void CreateRenderTexture(int index, int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite sRgbSampling, TextureWrapMode wrapMode, FilterMode filterMode)
        {
            _buffers[index] = new RenderTexture(width, height, 0, format, sRgbSampling);
            if(depth > 0)
            {
                _buffers[index].dimension = TextureDimension.Tex3D;
                _buffers[index].volumeDepth = depth;
            }
            _buffers[index].wrapMode = wrapMode;
            _buffers[index].filterMode = filterMode;
            _buffers[index].enableRandomWrite = true;
            _buffers[index].Create();
        }

        /// <summary>
        /// Swaps the textures
        /// </summary>
        public void Swap()
        {
            int tmp = _readId;
            _readId = _writeId;
            _writeId = tmp;
        }

        /// <summary>
        /// Releases the textures
        /// </summary>
        public void Release()
        {
            _buffers[0].Release();
            _buffers[1].Release();
        }
        #endregion
    }
}
