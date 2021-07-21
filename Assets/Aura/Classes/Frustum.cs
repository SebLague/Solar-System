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
using UnityEngine.Profiling;

namespace AuraAPI
{
    /// <summary>
    /// Ranged camera frustum inside which the volumetric data will be computed
    /// </summary>
    [Serializable]
    public class Frustum
    {
        #region Public Members
        /// <summary>
        /// The settings for the data computations 
        /// </summary>
        [SerializeField]
        public FrustumSettings settings;
        #endregion

        #region Private Members
        /// <summary>
        /// Compute Shader in charge of computing the maximum depth for occlusion culling
        /// </summary>
        [SerializeField]
        private ComputeShader _computeMaximumDepthComputeShader;
        /// <summary>
        /// Shader in charge of filtering and formatting the maximum depth
        /// </summary>
        [SerializeField]
        private Shader _processOcclusionMapShader;
        /// <summary>
        /// Material used for filtering and formatting the maximum depth
        /// </summary>
        private Material _processOcclusionMapMaterial;
        /// <summary>
        /// Compute Shader in charge of computing the data contribution inside the frustum
        /// </summary>
        [SerializeField]
        private ComputeShader _computeDataComputeShader;
        /// <summary>
        /// The dispatch size in X for the computeDataComputeShader
        /// </summary>
        private int _computeDataComputeShaderDispatchSizeX;
        /// <summary>
        /// The dispatch size in Y for the computeDataComputeShader
        /// </summary>
        private int _computeDataComputeShaderDispatchSizeY;
        /// <summary>
        /// The dispatch size in Z for the computeDataComputeShader
        /// </summary>
        private int _computeDataComputeShaderDispatchSizeZ;
        /// <summary>
        /// Compute Shader in charge of accumulating the data
        /// </summary>
        [SerializeField]
        private ComputeShader _computeAccumulationComputeShader;
        /// <summary>
        /// The dispatch size in X for the computeAccumulationComputeShader
        /// </summary>
        private int _computeAccumulationComputeShaderDispatchSizeX;
        /// <summary>
        /// The dispatch size in Y for the computeAccumulationComputeShader
        /// </summary>
        private int _computeAccumulationComputeShaderDispatchSizeY;
        /// <summary>
        /// The dispatch size in Z for the computeAccumulationComputeShader
        /// </summary>
        private int _computeAccumulationComputeShaderDispatchSizeZ;
        /// <summary>
        /// The far clip of the volume
        /// </summary>
        private float _farClip;
        /// <summary>
        /// Contains near and far clips of the volume
        /// </summary>
        private Vector4 _cameraRanges;
        /// <summary>
        /// Data for depthmap linearization
        /// </summary>
        private Vector4 _zParameters;
        /// <summary>
        /// The depth of the volume
        /// </summary>
        private float _volumeDepth;
        /// <summary>
        /// The depth of the cells
        /// </summary>
        private float _layerDepth;
        /// <summary>
        /// Inverse depth of cells
        /// </summary>
        private float _inverseLayerDepth;
        /// <summary>
        /// A vector containing the resolution of the buffers
        /// </summary>
        private Vector4 _resolutionVector;
        /// <summary>
        /// The texture buffersd
        /// </summary>
        private Buffers _buffers;
        /// <summary>
        /// Tells if the buffers have been initialized
        /// </summary>
        private bool _hasInitializedBuffers;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Frustum(ComputeShader computeMaximumDepthComputeShader, Shader processOcclusionMapShader, ComputeShader computeDataComputeShader, ComputeShader computeAccumulationComputeShader)
        {
            settings = new FrustumSettings();
            _computeMaximumDepthComputeShader = computeMaximumDepthComputeShader;
            _processOcclusionMapShader = processOcclusionMapShader;
            _computeDataComputeShader = computeDataComputeShader;
            _computeAccumulationComputeShader = computeAccumulationComputeShader;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Computes the volumetric data
        /// </summary>
        public void ComputeData()
        {
            if(!_hasInitializedBuffers)
            {
                CreateBuffers(settings.resolution);
            }

            settings.ComputeFlags();

            #region Variables
            _farClip = Mathf.Min(Aura.CameraComponent.farClipPlane, Mathf.Max(Aura.CameraComponent.nearClipPlane, settings.farClipPlaneDistance));
            _cameraRanges = new Vector4(Aura.CameraComponent.nearClipPlane, _farClip);
            Shader.SetGlobalVector("Aura_FrustumRange", _cameraRanges);
            _zParameters = new Vector4(-1.0f + Aura.CameraComponent.farClipPlane / Aura.CameraComponent.nearClipPlane, 1.0f);
            _zParameters.z = _zParameters.x / Aura.CameraComponent.farClipPlane;
            _zParameters.w = _zParameters.y / Aura.CameraComponent.farClipPlane;
            _volumeDepth = _farClip - Aura.CameraComponent.nearClipPlane;
            _layerDepth = _volumeDepth / _resolutionVector.z;
            _inverseLayerDepth = 1.0f / _layerDepth;
            #endregion

            #region Occlusion culling
            if(settings.HasFlags(FrustumParametersEnum.EnableOcclusionCulling))
            {
                Profiler.BeginSample("Aura : Compute occlusion culling data");

                _computeMaximumDepthComputeShader.SetTextureFromGlobal((int)settings.occlusionCullingAccuracy, "depthTexture", "_CameraDepthTexture"); // TODO : USE EVENT TO SET TEXTURES
                _computeMaximumDepthComputeShader.SetVector("cameraRanges", _cameraRanges);
                _computeMaximumDepthComputeShader.SetVector("zParameters", _zParameters);
                _computeMaximumDepthComputeShader.SetTexture((int)settings.occlusionCullingAccuracy, "occlusionTexture", _buffers.OcclusionTexture.WriteBuffer);
                _computeMaximumDepthComputeShader.Dispatch((int)settings.occlusionCullingAccuracy, settings.resolution.x, settings.resolution.y, 1); //Par blocks puis repasser en resolution
                _buffers.OcclusionTexture.Swap();

                if(_processOcclusionMapMaterial == null)
                {
                    _processOcclusionMapMaterial = new Material(_processOcclusionMapShader);
                }
                _processOcclusionMapMaterial.SetVector("bufferResolution", _resolutionVector);
                Graphics.Blit(_buffers.OcclusionTexture.ReadBuffer, _buffers.OcclusionTexture.WriteBuffer, _processOcclusionMapMaterial);
                _buffers.OcclusionTexture.Swap();

                _computeDataComputeShader.SetTexture(settings.GetId(), "occlusionTexture", _buffers.OcclusionTexture.ReadBuffer); // TODO : USE EVENT TO SET TEXTURES
                _computeAccumulationComputeShader.SetTexture(1, "occlusionTexture", _buffers.OcclusionTexture.ReadBuffer); // TODO : USE EVENT TO SET TEXTURES

                _buffers.FogVolumeTexture.Clear(Color.black);

                Profiler.EndSample();
            }
            #endregion

            #region Compute contributions
            Profiler.BeginSample("Aura : Compute volumetric lighting and density");

            _buffers.LightingVolumeTextures.Swap();
            Shader.SetGlobalTexture("Aura_VolumetricDataTexture", _buffers.LightingVolumeTextures.WriteBuffer); // TODO : USE EVENT TO SET TEXTURES
            _buffers.LightingVolumeTextures.WriteBuffer.Clear(new Color(0, 0, 0, -10));

            _computeDataComputeShader.SetTexture(settings.GetId(), "textureBuffer", _buffers.LightingVolumeTextures.WriteBuffer); // TODO : USE EVENT TO SET TEXTURES
            _computeDataComputeShader.SetTexture(settings.GetId(), "previousFrameLightingVolumeTexture", _buffers.LightingVolumeTextures.ReadBuffer); // TODO : USE EVENT TO SET TEXTURES
            _computeDataComputeShader.SetFloat("time", Aura.Time);
            _computeDataComputeShader.SetVector("cameraPosition", Aura.CameraComponent.transform.position);
            _computeDataComputeShader.SetVector("cameraRanges", _cameraRanges);
            _computeDataComputeShader.SetFloat("layerDepth", _layerDepth);
            _computeDataComputeShader.SetFloat("invLayerDepth", _inverseLayerDepth);
            _computeDataComputeShader.SetFloats("frustumCornersWorldPositionArray", Aura.CameraComponent.GetFrustumCornersWorldPosition(Aura.CameraComponent.nearClipPlane, _farClip).AsFloatArray());
            _computeDataComputeShader.SetFloat("baseDensity", settings.density);
            _computeDataComputeShader.SetFloat("baseAnisotropy", settings.anisotropy);
            _computeDataComputeShader.SetVector("baseColor", settings.color * settings.colorStrength);

            #region Temporal Reprojection
            if (settings.HasFlags(FrustumParametersEnum.EnableTemporalReprojection))
            {
                _computeDataComputeShader.SetFloat("temporalReprojectionFactor", settings.temporalReprojectionFactor);
                _computeDataComputeShader.SetVector("cameraRanges", _cameraRanges);
                _computeDataComputeShader.SetInt("_frameID", Aura.FrameId);
            }
            #endregion

            #region Volumes Injection
            if(settings.HasFlags(FrustumParametersEnum.EnableVolumes))
            {
                _computeDataComputeShader.SetInt("volumeCount", Aura.VolumesManager.Buffer.count);
                _computeDataComputeShader.SetBuffer(settings.GetId(), "volumeDataBuffer", Aura.VolumesManager.Buffer);
            
                if(settings.HasFlags(FrustumParametersEnum.EnableVolumesTextureMask))
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "volumeMaskTexture", Aura.VolumesManager.VolumeTexture); // TODO : USE EVENT TO SET TEXTURES
                }
            }
            #endregion

            #region Directional lights
            if(settings.HasFlags(FrustumParametersEnum.EnableDirectionalLights))
            {
                _computeDataComputeShader.SetInt("directionalLightCount", Aura.LightsManager.DirectionalLightsManager.DataBuffer.count);
                _computeDataComputeShader.SetBuffer(settings.GetId(), "directionalLightDataBuffer", Aura.LightsManager.DirectionalLightsManager.DataBuffer);

                if(settings.HasFlags(FrustumParametersEnum.EnableDirectionalLightsShadows))
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "directionalShadowMapsArray", Aura.LightsManager.DirectionalLightsManager.ShadowMapsArray); // TODO : USE EVENT TO SET TEXTURES
                    _computeDataComputeShader.SetTexture(settings.GetId(), "directionalShadowDataArray", Aura.LightsManager.DirectionalLightsManager.ShadowDataArray); // TODO : USE EVENT TO SET TEXTURES
                }

                if(settings.HasFlags(FrustumParametersEnum.EnableLightsCookies) && Aura.LightsManager.DirectionalLightsManager.HasCookieCasters)
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "directionalCookieMapsArray", Aura.LightsManager.DirectionalLightsManager.CookieMapsArray); // TODO : USE EVENT TO SET TEXTURES
                }
            }
            #endregion

            #region Spot lights
            if(settings.HasFlags(FrustumParametersEnum.EnableSpotLights))
            {
                _computeDataComputeShader.SetInt("spotLightCount", Aura.LightsManager.SpotLightsManager.DataBuffer.count);
                _computeDataComputeShader.SetBuffer(settings.GetId(), "spotLightDataBuffer", Aura.LightsManager.SpotLightsManager.DataBuffer);
            
                if(settings.HasFlags(FrustumParametersEnum.EnableSpotLightsShadows))
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "spotShadowMapsArray", Aura.LightsManager.SpotLightsManager.ShadowMapsArray); // TODO : USE EVENT TO SET TEXTURES
                }
            
                if(settings.HasFlags(FrustumParametersEnum.EnableLightsCookies) && Aura.LightsManager.SpotLightsManager.HasCookieCasters)
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "spotCookieMapsArray", Aura.LightsManager.SpotLightsManager.CookieMapsArray); // TODO : USE EVENT TO SET TEXTURES
                }
            }
            #endregion

            #region Point lights
            if(settings.HasFlags(FrustumParametersEnum.EnablePointLights))
            {
                _computeDataComputeShader.SetInt("pointLightCount", Aura.LightsManager.PointLightsManager.DataBuffer.count);
                _computeDataComputeShader.SetBuffer(settings.GetId(), "pointLightDataBuffer", Aura.LightsManager.PointLightsManager.DataBuffer);
            
                if(settings.HasFlags(FrustumParametersEnum.EnablePointLightsShadows))
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "pointShadowMapsArray", Aura.LightsManager.PointLightsManager.ShadowMapsArray); // TODO : USE EVENT TO SET TEXTURES
                }
            
                if(settings.HasFlags(FrustumParametersEnum.EnableLightsCookies) && Aura.LightsManager.PointLightsManager.HasCookieCasters)
                {
                    _computeDataComputeShader.SetTexture(settings.GetId(), "pointCookieMapsArray", Aura.LightsManager.PointLightsManager.CookieMapsArray); // TODO : USE EVENT TO SET TEXTURES
                }
            }
            #endregion

            #region Compute
            _computeDataComputeShader.Dispatch(settings.GetId(), _computeDataComputeShaderDispatchSizeX, _computeDataComputeShaderDispatchSizeY, _computeDataComputeShaderDispatchSizeZ);
            Profiler.EndSample();
            #endregion
            #endregion

            #region Accumulate fog texture
            Profiler.BeginSample("Aura : Compute accumulated contributions");
            _computeAccumulationComputeShader.SetFloat("layerDepth", _layerDepth);
            _computeAccumulationComputeShader.SetFloat("invLayerDepth", _inverseLayerDepth);
            _computeAccumulationComputeShader.SetTexture(0, "textureBuffer", _buffers.LightingVolumeTextures.WriteBuffer); // TODO : USE EVENT TO SET TEXTURES
            _computeAccumulationComputeShader.SetTexture(1, "textureBuffer", _buffers.LightingVolumeTextures.WriteBuffer); // TODO : USE EVENT TO SET TEXTURES
            _computeAccumulationComputeShader.SetFloat("normalizationCoefficient", -(_farClip - Aura.CameraComponent.nearClipPlane) / 256.0f); // simplified from : (farClip - Aura.cameraComponent.nearClipPlane) / resolution.z) [->layerDepth] * (bufferResolution.z / 256.0f) [->buffer resolution normalization (256.0f is an abritrary scale factor)] * -1 [->needed for exponential function]
            _computeAccumulationComputeShader.Dispatch(settings.HasFlags(FrustumParametersEnum.EnableOcclusionCulling) ? 1 : 0, _computeAccumulationComputeShaderDispatchSizeX, _computeAccumulationComputeShaderDispatchSizeY, _computeAccumulationComputeShaderDispatchSizeZ);
            Profiler.EndSample();
            #endregion

            _computeDataComputeShader.SetFloats("previousFrameWorldToClipMatrix", FrustumCorners.GetWorldToClipMatrix(Aura.CameraComponent, _farClip).ToFloatArray());
        }

        /// <summary>
        /// Creates texture buffers
        /// </summary>
        /// <param name="resolution">The desired resolution</param>
        private void CreateBuffers(Vector3Int resolution)
        {
            _buffers = new Buffers(resolution);
            _hasInitializedBuffers = true;
            _computeAccumulationComputeShader.SetTexture(0, "fogVolumeTexture", _buffers.FogVolumeTexture);
            _computeAccumulationComputeShader.SetTexture(1, "fogVolumeTexture", _buffers.FogVolumeTexture);
        }

        /// <summary>
        /// Disposes the managed members
        /// </summary>
        public void Dispose()
        {
            _buffers.ReleaseBuffers();
        }

        /// <summary>
        /// Sets a new grid resolution
        /// </summary>
        /// <param name="resolution">The desired resolution</param>
        public void SetResolution(Vector3Int resolution)
        {
            if(Aura.HasInstance)
            {
                uint threadSizeX;
                uint threadSizeY;
                uint threadSizeZ;
                _computeDataComputeShader.GetKernelThreadGroupSizes(0, out threadSizeX, out threadSizeY, out threadSizeZ);

                resolution.x = resolution.x.SnapMin((int)threadSizeX);
                resolution.y = resolution.y.SnapMin((int)threadSizeY);
                resolution.z = resolution.z.SnapMin((int)threadSizeZ);

                settings.resolution = resolution;

                if(_buffers == null || resolution != _buffers.Resolution)
                {
                    if(_buffers != null)
                    {
                        _buffers.ReleaseBuffers();
                    }

                    _resolutionVector = new Vector4(resolution.x, resolution.y, resolution.z, 1);

                    _computeDataComputeShaderDispatchSizeX = resolution.x / (int)threadSizeX;
                    _computeDataComputeShaderDispatchSizeY = resolution.y / (int)threadSizeY;
                    _computeDataComputeShaderDispatchSizeZ = resolution.z / (int)threadSizeZ;
                    _computeAccumulationComputeShaderDispatchSizeX = resolution.x / (int)threadSizeX;
                    _computeAccumulationComputeShaderDispatchSizeY = resolution.y / (int)threadSizeY;
                    _computeAccumulationComputeShaderDispatchSizeZ = resolution.z / (int)threadSizeZ;

                    _computeDataComputeShader.SetVector("bufferResolution", _resolutionVector);
                    _computeAccumulationComputeShader.SetVector("bufferResolution", _resolutionVector);
                    _computeMaximumDepthComputeShader.SetVector("bufferResolution", _resolutionVector);

                    CreateBuffers(resolution);
                }
            }
        }
        #endregion
    }
}
