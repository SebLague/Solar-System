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
    /// Compose a Texture3D out of several Texture3Ds
    /// </summary>
    internal class VolumetricTextureArrayComposer
    {
        #region Private Members
        /// <summary>
        /// The list of candidate Textures
        /// </summary>
        private readonly List<Texture3D> _texturesList;
        /// <summary>
        /// The format of the generated Texture3D
        /// </summary>
        private readonly TextureFormat _requiredTextureFormat;
        /// <summary>
        /// The cubic size of the generated Texture3D
        /// </summary>
        private readonly int _requiredSize;
        #endregion

        #region Events
        /// <summary>
        /// Event raised when the composed texture has been generated
        /// </summary>
        public event Action OnTextureUpdated;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredTextureFormat">The format of the composed Texture3D</param>
        /// <param name="requiredSize">The size in pixels of the width and the height of the composed Texture3D</param>
        public VolumetricTextureArrayComposer(TextureFormat requiredTextureFormat, int requiredSize)
        {
            _texturesList = new List<Texture3D>();
            _requiredTextureFormat = requiredTextureFormat;
            _requiredSize = requiredSize;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the generated Texture3D
        /// </summary>
        public Texture3D VolumeTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Tells if a Texture3D has been generated
        /// </summary>
        public bool HasVolumeTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Tells if changes were made and Generate() should be called
        /// </summary>
        public bool NeedsToUpdateVolumeTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the onTextureUpdated event
        /// </summary>
        private void RaiseTextureUpdatedEvent()
        {
            if(OnTextureUpdated != null)
            {
                OnTextureUpdated();
            }
        }

        /// <summary>
        /// Clears the candidate textures list
        /// </summary>
        public void ClearTextureList()
        {
            _texturesList.Clear();
            NeedsToUpdateVolumeTexture = true;
        }

        /// <summary>
        /// Adds a new candidate texture to the textures list
        /// </summary>
        /// <param name="texture">The Texture3D to be added</param>
        public void AddTexture(Texture3D texture)
        {
            if(texture != null)
            {
                if(texture.height != _requiredSize || texture.width != _requiredSize || texture.depth != _requiredSize)
                {
                    Debug.LogError("Pixel sizes of Texture3D \"" + texture + "\" does not match the required size of " + _requiredSize + "pixels for every dimensions.", texture);
                    return;
                }

                if(texture.format != _requiredTextureFormat)
                {
                    Debug.LogError("Texture format of Texture3D \"" + texture + "\" does not match the required " + _requiredTextureFormat + " format.", texture);
                    return;
                }

                if(!_texturesList.Contains(texture))
                {
                    _texturesList.Add(texture);
                    NeedsToUpdateVolumeTexture = true;
                }
            }
        }

        /// <summary>
        /// Removes a texture from the candidate textures list
        /// </summary>
        /// <param name="texture">The Texture3D to be removed</param>
        public void RemoveTexture(Texture3D texture)
        {
            if(_texturesList.Contains(texture))
            {
                _texturesList.Remove(texture);
                NeedsToUpdateVolumeTexture = true;
            }
        }

        /// <summary>
        /// Generates the Texture3D composed out of the candidate textures (already handles NeedsToUpdateVolumeTexture parameter check)
        /// </summary>
        public void GenerateComposedTexture3D()
        {
            if(NeedsToUpdateVolumeTexture)
            {
                if(_texturesList.Count > 0)
                {
                    Color[] colorArray = new Color[0];
                    VolumeTexture = new Texture3D(_requiredSize, _requiredSize, _requiredSize * _texturesList.Count, _requiredTextureFormat, false);

                    for(int i = 0; i < _texturesList.Count; ++i)
                    {
                        // TODO : DO WITH GRAPHICS.COPYTEXTURES NOW THAT TEXTURE3D COPY ACTUALLY WORKS
                        colorArray = colorArray.Append(_texturesList[i].GetPixels());
                    }

                    VolumeTexture.SetPixels(colorArray);
                    VolumeTexture.Apply();

                    HasVolumeTexture = true;
                }
                else
                {
                    VolumeTexture = null;
                    HasVolumeTexture = false;
                }

                NeedsToUpdateVolumeTexture = false;

                RaiseTextureUpdatedEvent();
            }
        }

        /// <summary>
        /// Returns the index of the queried texture in the candidate textures list. This index is the same as the corresponding Texture3D inside the composed Texture3D.
        /// </summary>
        /// <param name="texture">The queried texture.</param>
        /// <returns>The index of the texture. -1 if not found.</returns>
        public int GetTextureIndex(Texture3D texture)
        {
            return _texturesList.IndexOf(texture);
        }
        #endregion
    }
}
