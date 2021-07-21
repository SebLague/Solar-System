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
    /// Manager that handles directional AuraLights
    /// </summary>
    public class DirectionalLightsManager
    {
        #region Public Members
        /// <summary>
        /// The cookie map size
        /// </summary>
        public static readonly Vector2Int cookieMapSize = new Vector2Int(256, 256); // TODO : EXPOSE AS DYNAMIC PARAMETER
        #endregion

        #region Private Members
        /// <summary>
        /// The shadow map size for one cascade (AKA no cascade)
        /// </summary>
        private static readonly Vector2Int _shadowMapSizeOneCascade = new Vector2Int(256, 256); // TODO : EXPOSE AS DYNAMIC PARAMETER
        /// <summary>
        /// The shadow map size for two cascades (based on one cascade size)
        /// </summary>
        private static readonly Vector2Int _shadowMapSizeTwoCascades = new Vector2Int(DirectionalLightsManager._shadowMapSizeOneCascade.x * 2, DirectionalLightsManager._shadowMapSizeOneCascade.y);
        /// <summary>
        /// The shadow map size for four cascades (based on one cascade size)
        /// </summary>
        private static readonly Vector2Int _shadowMapSizeFourCascades = new Vector2Int(DirectionalLightsManager._shadowMapSizeOneCascade.x * 2, DirectionalLightsManager._shadowMapSizeOneCascade.y * 2);
        /// <summary>
        /// The candidate lights
        /// </summary>
        private readonly List<AuraLight> _lights = new List<AuraLight>();
        /// <summary>
        /// The composer that will collect the shadow maps and stack them in a Texture2DArray
        /// </summary>
        private readonly ShadowmapsCollector _shadowmapsCollector;
        /// <summary>
        /// The composer that will collect the shadow data maps and stack them in a Texture2DArray
        /// </summary>
        private readonly DirectionalShadowDataCollector _shadowDataCollector;
        /// <summary>
        /// The composer that will collect the cookie maps and stack them in a Texture2DArray
        /// </summary>
        private readonly Texture2DArrayComposer _cookieMapsCollector;
        /// <summary>
        /// The collected packed data
        /// </summary>
        private DirectionalLightParameters[] _parameters;
        /// <summary>
        /// Used for checking changes in the number of shadow cascades
        /// </summary>
        private int _previousCascadesCount;
        #endregion

        #region Events
        /// <summary>
        /// Event raised when the number of shadow cascades changes
        /// </summary>
        public event Action OnCascadesCountChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DirectionalLightsManager()
        {
            _shadowmapsCollector = new ShadowmapsCollector(DirectionalLightsManager.ShadowMapSize.x, DirectionalLightsManager.ShadowMapSize.y);
            _shadowDataCollector = new DirectionalShadowDataCollector(32, 1);
            _cookieMapsCollector = new Texture2DArrayComposer(DirectionalLightsManager.cookieMapSize.x, DirectionalLightsManager.cookieMapSize.y, TextureFormat.R8, true);
            _previousCascadesCount = QualitySettings.shadowCascades;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the shadow map size (depending on the shadow cascade count)
        /// </summary>
        public static Vector2Int ShadowMapSize
        {
            get
            {
                switch(QualitySettings.shadowCascades)
                {
                    case 4 :
                        return DirectionalLightsManager._shadowMapSizeFourCascades;
                    case 2 :
                        return DirectionalLightsManager._shadowMapSizeTwoCascades;
                    default :
                        return DirectionalLightsManager._shadowMapSizeOneCascade;
                }
            }
        }

        /// <summary>
        /// Tells if there are any shadow caster
        /// </summary>
        public bool HasShadowCasters
        {
            get
            {
                return _shadowmapsCollector.HasTexture && _shadowDataCollector.HasTexture;
            }
        }

        /// <summary>
        /// Accessor to the Texture2DArray containing the stacked shadow maps
        /// </summary>
        public Texture2DArray ShadowMapsArray
        {
            get
            {
                return _shadowmapsCollector.Texture;
            }
        }

        /// <summary>
        /// Accessor to the Texture2DArray containing the stacked shadow data maps
        /// </summary>
        public Texture2DArray ShadowDataArray
        {
            get
            {
                return _shadowDataCollector.Texture;
            }
        }

        /// <summary>
        /// Tells if there are any cookie caster
        /// </summary>
        public bool HasCookieCasters
        {
            get
            {
                return _cookieMapsCollector.HasTexture;
            }
        }

        /// <summary>
        /// Accessor to the Texture2DArray containing the stacked cookie maps
        /// </summary>
        public Texture2DArray CookieMapsArray
        {
            get
            {
                return _cookieMapsCollector.Texture;
            }
        }

        /// <summary>
        /// Tells if has candidate lights
        /// </summary>
        public bool HasCandidateLights
        {
            get
            {
                return _lights.Count > 0;
            }
        }

        /// <summary>
        /// Accessor to the compute buffer containing the packed data of the visible lights
        /// </summary>
        public ComputeBuffer DataBuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Disposes the members
        /// </summary>
        public void Dispose()
        {
            ReleaseBuffer();
        }

        /// <summary>
        /// Setup the data buffers containing the packed data of the visible lights
        /// </summary>
        private void SetupBuffers()
        {
            ReleaseBuffer();
            InitBuffer();
            _parameters = new DirectionalLightParameters[_lights.Count];
        }

        /// <summary>
        /// Releases the compute buffer containing the packed data of the visible lights
        /// </summary>
        private void ReleaseBuffer()
        {
            try
            {
                DataBuffer.Release();
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Initializes the compute buffer containing the packed data of the visible lights
        /// </summary>
        private void InitBuffer()
        {
            if(HasCandidateLights)
            {
                DataBuffer = new ComputeBuffer(_lights.Count, DirectionalLightParameters.Size);
            }
        }

        /// <summary>
        /// Register an AuraLights to be taken into account
        /// </summary>
        /// <param name="light">The candidate light</param>
        /// <returns>True if registration went well, false otherwise</returns>
        public bool Register(AuraLight light, bool castShadows, bool castCookie)
        {
            if(light.Type == LightType.Directional && !_lights.Contains(light))
            {
                _lights.Add(light);
                SetupBuffers();

                if(castShadows)
                {
                    _shadowmapsCollector.AddTexture(light.shadowMapRenderTexture);
                    _shadowDataCollector.AddTexture(light.shadowDataRenderTexture);
                }

                if(castCookie)
                {
                    _cookieMapsCollector.AddTexture(light.cookieMapRenderTexture);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Unregisters an AuraLights
        /// </summary>
        /// <param name="light">The candidate light</param>
        /// <returns>True if unregistration went well, false otherwise</returns>
        public bool Unregister(AuraLight light)
        {
            if(_lights.Contains(light))
            {
                if(light.cookieMapRenderTexture != null)
                {
                    _cookieMapsCollector.RemoveTexture(light.cookieMapRenderTexture);
                }

                if(light.shadowMapRenderTexture != null)
                {
                    _shadowmapsCollector.RemoveTexture(light.shadowMapRenderTexture);
                    _shadowDataCollector.RemoveTexture(light.shadowDataRenderTexture);
                }

                _lights.Remove(light);
                SetupBuffers();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the manager (collects shadows/cookies/data and packs them)
        /// </summary>
        public void Update()
        {
            if(HasCandidateLights)
            {
                _shadowmapsCollector.Generate();
                _shadowDataCollector.Generate();
                _cookieMapsCollector.Generate();

                for(int i = 0; i < _lights.Count; ++i)
                {
                    _lights[i].SetShadowMapIndex(_shadowmapsCollector.GetTextureIndex(_lights[i].shadowMapRenderTexture));
                    _lights[i].SetCookieMapIndex(_cookieMapsCollector.GetTextureIndex(_lights[i].cookieMapRenderTexture));
                    _parameters[i] = _lights[i].GetDirectionnalParameters();
                }

                DataBuffer.SetData(_parameters);
            }

            if(OnCascadesCountChanged != null && _previousCascadesCount != QualitySettings.shadowCascades)
            {
                _shadowmapsCollector.Resize(DirectionalLightsManager.ShadowMapSize.x, DirectionalLightsManager.ShadowMapSize.y);
                _previousCascadesCount = QualitySettings.shadowCascades;
                OnCascadesCountChanged();
            }
        }
        #endregion
    }
}
