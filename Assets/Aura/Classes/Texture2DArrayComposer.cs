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
using System.Collections.Generic;
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Builds a Texture2DArray out of a collection of Textures
    /// </summary>
    public class Texture2DArrayComposer
    {
        #region Public Members
        /// <summary>
        /// Forces the Texture2DArray to be re-generated everytime Generate() is called.
        /// </summary>
        public bool alwaysGenerateOnUpdate;
        #endregion

        #region Private Members
        /// <summary>
        /// The color space of the Texture2DArray
        /// </summary>
        private readonly bool _linear;
        /// <summary>
        /// The format of the Texture2DArray
        /// </summary>
        private readonly TextureFormat _requiredTextureFormat;
        /// <summary>
        /// The list of candidate Textures
        /// </summary>
        private readonly List<Texture> _texturesList;
        /// <summary>
        /// The horizontal size of the Texture2DArray
        /// </summary>
        private int _requiredSizeX;
        /// <summary>
        /// The vertical size of the Texture2DArray
        /// </summary>
        private int _requiredSizeY;
        #endregion

        #region Events
        public event EventHandler OnTextureUpdated;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sizeX">The horizontal size of the Texture2DArray</param>
        /// <param name="sizeY">The vertical size of the Texture2DArray</param>
        /// <param name="format">The format of the Texture2DArray</param>
        /// <param name="bypassSrgb">The color space of the Texture2DArray</param>
        public Texture2DArrayComposer(int sizeX, int sizeY, TextureFormat format, bool bypassSrgb)
        {
            _texturesList = new List<Texture>();
            _requiredSizeX = sizeX;
            _requiredSizeY = sizeY;
            _requiredTextureFormat = format;
            _linear = bypassSrgb;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns the amount of candidate Textures
        /// </summary>
        public int TextureCount
        {
            get
            {
                return _texturesList.Count;
            }
        }

        /// <summary>
        /// Accessor to the generated Texture2DArray
        /// </summary>
        public Texture2DArray Texture
        {
            get;
            private set;
        }

        /// <summary>
        /// Tells if a Texture2DArray has been generated
        /// </summary>
        public bool HasTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Tells if changes were made and Generate() should be called
        /// </summary>
        public bool NeedsToUpdateTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the OnTextureUpdated event
        /// </summary>
        public void RaiseTextureUpdatedEvent()
        {
            if(OnTextureUpdated != null)
            {
                OnTextureUpdated(this, new EventArgs());
            }
        }

        /// <summary>
        /// Adds a new candidate Texture
        /// </summary>
        /// <param name="texture">The Texture to be added</param>
        /// <returns>True if successfully added, false otherwise</returns>
        public bool AddTexture(Texture texture)
        {
            if(texture != null)
            {
                if(texture.height != _requiredSizeY || texture.width != _requiredSizeX)
                {
                    Debug.LogError("Pixel sizes of texture \"" + texture + "\" does not match the required size of " + _requiredSizeX + "pixels for width and " + _requiredSizeY + "pixels for height.", texture);
                    return false;
                }

                if(!_texturesList.Contains(texture))
                {
                    _texturesList.Add(texture);
                    NeedsToUpdateTexture = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a Texture from the candidate Textures list
        /// </summary>
        /// <param name="texture">The Texture to remove</param>
        /// <returns>True if successfully added, false otherwise</returns>
        public bool RemoveTexture(Texture texture)
        {
            if(_texturesList.Contains(texture))
            {
                _texturesList.Remove(texture);
                NeedsToUpdateTexture = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Launches the Texture2DArray generation (already handles NeedsToUpdateVolumeTexture and alwaysGenerateOnUpdate parameters check)
        /// </summary>
        public void Generate()
        {
            if(NeedsToUpdateTexture || alwaysGenerateOnUpdate)
            {
                if(_texturesList.Count > 0)
                {
                    if(NeedsToUpdateTexture)
                    {
                        Texture = new Texture2DArray(_requiredSizeX, _requiredSizeY, _texturesList.Count, _requiredTextureFormat, false, _linear);
                    }

                    for(int i = 0; i < _texturesList.Count; ++i)
                    {
                        Graphics.CopyTexture(_texturesList[i], 0, 0, 0, 0, _requiredSizeX, _requiredSizeY, Texture, i, 0, 0, 0);
                    }

                    HasTexture = true;
                }
                else
                {
                    Texture = null;
                    HasTexture = false;
                }

                NeedsToUpdateTexture = false;

                RaiseTextureUpdatedEvent();
            }
        }

        /// <summary>
        /// Returns the ID of the Texture inside the Texture2DArray
        /// </summary>
        /// <param name="texture">The queried Texture</param>
        /// <returns>The index of the Texture in the Texture2DArray, -1 if not found</returns>
        public int GetTextureIndex(Texture texture)
        {
            return _texturesList.IndexOf(texture);
        }

        /// <summary>
        /// Clears the candidate Textures list
        /// </summary>
        public void ClearTexturesList()
        {
            _texturesList.Clear();
        }

        /// <summary>
        /// Changes the size of the Texture2DArray to be generated (candidate Texture list will be cleared)
        /// </summary>
        /// <param name="sizeX">The new horizontal size</param>
        /// <param name="sizeY">The new vertical size</param>
        public void Resize(int sizeX, int sizeY)
        {
            _requiredSizeX = sizeX;
            _requiredSizeY = sizeY;
            ClearTexturesList();
            NeedsToUpdateTexture = true;
        }
        #endregion
    }
}
