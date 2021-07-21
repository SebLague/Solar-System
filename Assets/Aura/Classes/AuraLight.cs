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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AuraAPI
{
    /// <summary>
    /// Component to assign on a GameObject with a Light component if you want this Light to be taken into account in Aura
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Aura/Aura Light")]
    [ExecuteInEditMode]
    public class AuraLight : CullableObject
    {
        #region Public Members
        /// <summary>
        /// Strength of the volumetric light contribution
        /// </summary>
        public float strength = 1.0f;
        /// <summary>
        /// Allows to ignore the light's color and refers to the "overridingColor" property instead
        /// </summary>
        public bool overrideColor;
        /// <summary>
        /// Overriding color if "overrideColor" is checked
        /// </summary>
        [ColorCircularPicker]
        public Color overridingColor = Color.white * 0.9999f;
        /// <summary>
        /// Allows to use light's shadows attenuation
        /// </summary>
        public bool useShadow = true;
        /// <summary>
        /// The light's shadow map
        /// </summary>
        public RenderTexture shadowMapRenderTexture;
        /// <summary>
        /// Allows to use light's cookie attenuation
        /// </summary>
        public bool useCookie = true;
        /// <summary>
        /// The light's cookie map
        /// </summary>
        public RenderTexture cookieMapRenderTexture;
        /// <summary>
        /// Allows to ignore the directional light's color and refers to the \"overridingColor\" property instead
        /// </summary>
        public bool enableOutOfPhaseColor;
        /// <summary>
        /// Strength of out-of-phase color
        /// </summary>
        public float outOfPhaseColorStrength = 0.1f;
        /// <summary>
        /// Out-of-phase color
        /// </summary>
        [ColorCircularPicker]
        public Color outOfPhaseColor = Color.cyan;
        /// <summary>
        /// DO NOT CHANGE : Shader used to copy the directional light's cascaded shadows projection data
        /// </summary>
        public Shader storeShadowDataShader;
        /// <summary>
        /// The directional light's shadow data texture
        /// </summary>
        public RenderTexture shadowDataRenderTexture;
        /// <summary>
        /// Custom distance falloff start (Spot/Point Lights only)
        /// </summary>
        public float customDistanceFalloffThreshold = 0.5f;
        /// <summary>
        /// Custom distance falloff power (Spot/Point Lights only)
        /// </summary>
        public float customDistanceFalloffPower = 2.0f;
        /// <summary>
        /// DO NOT CHANGE : Shader used to store the directional/spot light's cookie map
        /// </summary>
        public Shader storeDirectionalSpotLightCookieMapShader;
        /// <summary>
        /// Custom cookie distance falloff start (Spot/Point Lights only)
        /// </summary>
        public float customCookieDistanceFalloffStartThreshold = 0.1f;
        /// <summary>
        /// Custom cookie distance falloff end (Spot/Point Lights only)
        /// </summary>
        public float customCookieDistanceFalloffEndThreshold = 0.25f;
        /// <summary>
        /// Custom cookie distance falloff power (Spot/Point Lights only)
        /// </summary>
        public float customCookieDistanceFalloffPower = 2.0f;
        /// <summary>
        /// Custom angular falloff start (Spot Lights only)
        /// </summary>
        public float customAngularFalloffThreshold = 0.8f;
        /// <summary>
        /// Custom angular falloff power (Spot Lights only)
        /// </summary>
        public float customAngularFalloffPower = 2.0f;
        /// <summary>
        /// DO NOT CHANGE : Shader used to store the point light's shadow map (renders the TextureCUBE into a Texture2D)
        /// </summary>
        public Shader storePointLightShadowMapShader;
        /// <summary>
        /// DO NOT CHANGE : Mesh used to store the point light's shadow map (renders the TextureCUBE into a Texture2D). TODO : FIND A WAY TO MAKE IT WORK WITHOUT THIS WORKAROUND
        /// </summary>
        public Mesh storePointLightShadowMapMesh;
        /// <summary>
        /// DO NOT CHANGE : Shader used to store the point light's cookie map (renders the TextureCUBE into a Texture2D)
        /// </summary>
        public Shader storePointLightCookieMapShader;
        #endregion

        #region Private Members
        /// <summary>
        /// Tells if the light is registered to the LightsManager
        /// </summary>
        private bool _isRegistered;
        /// <summary>
        /// Tells if the component is initialized
        /// </summary>
        private bool _isInitialized;
        /// <summary>
        /// The attached light component
        /// </summary>
        private Light _lightComponent;
        /// <summary>
        /// Used for checking changes of light's type
        /// </summary>
        private LightType _previousLightType;
        /// <summary>
        /// Used for checking changes in "useShadow"
        /// </summary>
        private bool _previousUseShadow;
        /// <summary>
        /// The index of the light's shadow map in the composed Texture2DArray that is sent to the compute shader
        /// </summary>
        private int _shadowMapIndex;
        /// <summary>
        /// The command buffer used to copy the shadow map at a specific stage of the light's processing
        /// </summary>
        private CommandBuffer _copyShadowmapCommandBuffer;
        /// <summary>
        /// Used for checking changes in "useCookie"
        /// </summary>
        private bool _previousUseCookie;
        /// <summary>
        /// Used for checking changes in the light's cookie texture
        /// </summary>
        private Texture2D _previousCookieTexture;
        /// <summary>
        /// The index of the light's cookie map in the composed Texture2DArray that is sent to the compute shader
        /// </summary>
        private int _cookieMapIndex;
        /// <summary>
        /// Material used to copy the directional light's cascaded shadows projection data
        /// </summary>
        private Material _storeShadowDataMaterial;
        /// <summary>
        /// The command buffer used to copy the directional light's shadow data at a specific stage of the light's processing
        /// </summary>
        private CommandBuffer _storeShadowDataCommandBuffer;
        /// <summary>
        /// Material used to store the point light's shadow map (renders the TextureCUBE into a Texture2D)
        /// </summary>
        private Material _storePointLightShadowMapMaterial;
        /// <summary>
        /// Material used to store the point light's cookie map (renders the TextureCUBE into a Texture2D)
        /// </summary>
        private Material _storeCookieMapMaterial;
        /// <summary>
        /// The collected data of the directional light's the will be packed into a common compute buffer by the lights manager and sent to the compute shader
        /// </summary>
        private DirectionalLightParameters _directionalLightParameters;
        /// <summary>
        /// The collected data of the spot light's the will be packed into a common compute buffer by the lights manager and sent to the compute shader
        /// </summary>
        private SpotLightParameters _spotLightParameters;
        /// <summary>
        /// The collected data of the point light's the will be packed into a common compute buffer by the lights manager and sent to the compute shader
        /// </summary>
        private PointLightParameters _pointLightParameters;
        #endregion

        #region Monobehaviour functions
        private void Awake()
        {
            _lightComponent = GetComponent<Light>();
        }

        private void OnEnable()
        {
            if(!Aura.CheckCompatibility())
            {
                enabled = false;
                return;
            }

            if(Aura.HasInstance)
            {
                Initialize();
            }
            else
            {
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if(_isInitialized)
            {
                Uninitialize();
            }
        }

        private void Update()
        {
            if(_previousUseShadow != CastsShadows || _previousUseCookie != CastsCookie || _previousLightType != Type)
            {
                Reinitialize();
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns the attached light's type
        /// </summary>
        public LightType Type
        {
            get
            {
                return _lightComponent.type;
            }
        }

        /// <summary>
        /// Tells if it should cast shadows
        /// </summary>
        public bool CastsShadows
        {
            get
            {
                return _lightComponent.shadows != LightShadows.None && useShadow;
            }
        }

        /// <summary>
        /// Tells if it should cast cookie
        /// </summary>
        public bool CastsCookie
        {
            get
            {
                return _lightComponent.cookie != null && useCookie;
            }
        }

        /// <summary>
        /// Function run when the OnPreCullEvent is raised on the Aura main component
        /// </summary>
        private void Aura_onPreCullEvent()
        {
            if(this == null)
            {
                Aura.OnPreCullEvent -= Aura_onPreCullEvent;
                return;
            }

            if(CastsShadows)
            {
                switch(Type)
                {
                    case LightType.Point :
                        {
                            _copyShadowmapCommandBuffer.Clear();
                        }
                        break;
                }
            }

            UpdateBoundingSphere();
        }

        /// <summary>
        /// Function run when the OnPreRenderEvent is raised on the Aura main component
        /// </summary>
        private void Aura_onPreRenderEvent(Camera camera)
        {
            if(this == null)
            {
                Aura.OnPreRenderEvent -= Aura_onPreRenderEvent;
                return;
            }

            if(CastsCookie)
            {
                CopyCookieMap();
            }

            PackParameters(camera);
        }

        /// <summary>
        /// Function run when the OnCascadesCountChanged event is raised by the DirectionalLightsManager
        /// </summary>
        private void DirectionalLightsManager_onCascadesCountChanged()
        {
            Reinitialize();
        }

        /// <summary>
        /// Initializes the component (command buffers, registrations, events, managed members ...)
        /// </summary>
        private void Initialize()
        {
            _lightComponent = GetComponent<Light>();
            _previousLightType = Type;

            if (CastsShadows)
            {
                Vector2Int shadowMapSize = new Vector2Int(0, 0);
                switch(Type)
                {
                    case LightType.Directional :
                        {
                            shadowMapSize = DirectionalLightsManager.ShadowMapSize;
                        }
                        break;

                    case LightType.Spot :
                        {
                            shadowMapSize = SpotLightsManager.shadowMapSize;
                        }
                        break;

                    case LightType.Point :
                        {
                            shadowMapSize = PointLightsManager.shadowMapSize;
                        }
                        break;
                }

                shadowMapRenderTexture = new RenderTexture(shadowMapSize.x, shadowMapSize.y, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                shadowMapRenderTexture.Create();
                RenderTargetIdentifier shadowMapRenderTextureIdentifier = BuiltinRenderTextureType.CurrentActive;
                _copyShadowmapCommandBuffer = new CommandBuffer
                                              {
                                                  name = "Aura : Copy light's shadowmap"
                                              };
                _copyShadowmapCommandBuffer.SetShadowSamplingMode(shadowMapRenderTextureIdentifier, ShadowSamplingMode.RawDepth);
                _copyShadowmapCommandBuffer.Blit(shadowMapRenderTextureIdentifier, new RenderTargetIdentifier(shadowMapRenderTexture));
                _lightComponent.AddCommandBuffer(LightEvent.AfterShadowMap, _copyShadowmapCommandBuffer);

                switch(Type)
                {
                    case LightType.Point :
                        {
                            _storePointLightShadowMapMaterial = new Material(storePointLightShadowMapShader);
                        }
                        break;

                    case LightType.Directional :
                        {
                            if(shadowDataRenderTexture == null)
                            {
                                shadowDataRenderTexture = new RenderTexture(32, 1, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                            }

                            _storeShadowDataCommandBuffer = new CommandBuffer
                                                            {
                                                                name = "Aura : Store directional light's shadow data"
                                                            };

                            _storeShadowDataMaterial = new Material(storeShadowDataShader);

                            _lightComponent.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, _storeShadowDataCommandBuffer);

                            _storeShadowDataCommandBuffer.Blit(null, new RenderTargetIdentifier(shadowDataRenderTexture), _storeShadowDataMaterial);

                            Aura.LightsManager.DirectionalLightsManager.OnCascadesCountChanged += DirectionalLightsManager_onCascadesCountChanged;
                        }
                        break;
                }
            }

            _previousUseShadow = CastsShadows;

            if(CastsCookie)
            {
                Vector2Int cookieMapSize = Vector2Int.zero;
                switch(Type)
                {
                    case LightType.Directional :
                        {
                            cookieMapSize = DirectionalLightsManager.cookieMapSize;
                        }
                        break;

                    case LightType.Spot :
                        {
                            cookieMapSize = SpotLightsManager.cookieMapSize;
                        }
                        break;

                    case LightType.Point :
                        {
                            cookieMapSize = PointLightsManager.cookieMapSize;
                        }
                        break;
                }

                cookieMapRenderTexture = new RenderTexture(cookieMapSize.x, cookieMapSize.y, 0, RenderTextureFormat.R8);

                switch(Type)
                {
                    case LightType.Point :
                        {
                            _storeCookieMapMaterial = new Material(storePointLightCookieMapShader);
                        }
                        break;

                    default :
                        {
                            _storeCookieMapMaterial = new Material(storeDirectionalSpotLightCookieMapShader);
                        }
                        break;
                }
            }

            _previousUseCookie = CastsCookie;

            Aura.LightsManager.Register(this, CastsShadows, CastsCookie);
            _isRegistered = true;

            Aura.OnPreCullEvent += Aura_onPreCullEvent;
            Aura.OnPreRenderEvent += Aura_onPreRenderEvent;

            _isInitialized = true;
        }

        /// <summary>
        /// Uninitializes the component (command buffers, registrations, events, managed members ...)
        /// </summary>
        private void Uninitialize()
        {
            Aura.LightsManager.Unregister(this);
            _isRegistered = false;

            if(_previousUseShadow)
            {
                if(_previousLightType == LightType.Directional)
                {
                    _lightComponent.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, _storeShadowDataCommandBuffer);

                    _storeShadowDataCommandBuffer.Clear();
                    _storeShadowDataCommandBuffer.Release();
                    _storeShadowDataCommandBuffer = null;

                    shadowDataRenderTexture.Release();
                    shadowDataRenderTexture = null;

                    Aura.LightsManager.DirectionalLightsManager.OnCascadesCountChanged -= DirectionalLightsManager_onCascadesCountChanged;
                }

                _lightComponent.RemoveCommandBuffer(LightEvent.AfterShadowMap, _copyShadowmapCommandBuffer);

                _copyShadowmapCommandBuffer.Clear();
                _copyShadowmapCommandBuffer.Release();
                _copyShadowmapCommandBuffer = null;

                shadowMapRenderTexture.Release();
                shadowMapRenderTexture = null;
            }

            Aura.OnPreCullEvent -= Aura_onPreCullEvent;
            Aura.OnPreRenderEvent -= Aura_onPreRenderEvent;

            _isInitialized = false;
        }

        /// <summary>
        /// Uninitializes the component then re-initializes it
        /// </summary>
        private void Reinitialize()
        {
            Uninitialize();
            Initialize();
        }

        /// <summary>
        /// Collects the light's data so it will be packed into a common compute buffer by the lights manager and sent to the compute shader
        /// </summary>
        /// <param name="camera"></param>
        private void PackParameters(Camera camera)
        {
            Vector4 color = (overrideColor ? overridingColor : _lightComponent.color) * _lightComponent.intensity * strength;

            switch(Type)
            {
                case LightType.Directional :
                    {
                        _directionalLightParameters.color = color;
                        _directionalLightParameters.lightPosition = _lightComponent.transform.position;
                        _directionalLightParameters.lightDirection = _lightComponent.transform.forward;
                        Matrix4x4 lightToWorldMatrix = Matrix4x4.TRS(_lightComponent.transform.position, _lightComponent.transform.rotation, Vector3.one);
                        _directionalLightParameters.worldToLightMatrix = MatrixFloats.ToMatrixFloats(lightToWorldMatrix.inverse);
                        _directionalLightParameters.lightToWorldMatrix = MatrixFloats.ToMatrixFloats(lightToWorldMatrix);
                        _directionalLightParameters.shadowMapIndex = CastsShadows ? _shadowMapIndex : -1;

                        _directionalLightParameters.cookieMapIndex = -1;
                        if(CastsCookie)
                        {
                            _directionalLightParameters.cookieMapIndex = _cookieMapIndex;
                            _directionalLightParameters.cookieParameters.x = _lightComponent.cookieSize;
                            _directionalLightParameters.cookieParameters.y = _lightComponent.cookie.wrapMode == TextureWrapMode.Repeat ? 0 : 1;
                        }

                        _directionalLightParameters.enableOutOfPhaseColor = enableOutOfPhaseColor ? 1 : 0;
                        _directionalLightParameters.outOfPhaseColor = (Vector4)outOfPhaseColor * outOfPhaseColorStrength;
                    }
                    break;

                case LightType.Spot :
                    {
                        _spotLightParameters.color = color;
                        _spotLightParameters.lightPosition = _lightComponent.transform.position;
                        _spotLightParameters.lightDirection = _lightComponent.transform.forward;
                        _spotLightParameters.lightRange = _lightComponent.range;
                        _spotLightParameters.lightCosHalfAngle = Mathf.Cos(_lightComponent.spotAngle * 0.5f * Mathf.Deg2Rad);
                        _spotLightParameters.angularFalloffParameters = new Vector2(customAngularFalloffThreshold, customAngularFalloffPower);
                        _spotLightParameters.distanceFalloffParameters = new Vector2(customDistanceFalloffThreshold, customDistanceFalloffPower);

                        _spotLightParameters.shadowMapIndex = -1;
                        if(CastsShadows)
                        {
                            Matrix4x4 worldToLight = Matrix4x4.TRS(_lightComponent.transform.position, _lightComponent.transform.rotation, Vector3.one).inverse;
                            Matrix4x4 proj = Matrix4x4.Perspective(_lightComponent.spotAngle, 1, _lightComponent.shadowNearPlane, _lightComponent.range);
                            Matrix4x4 clip = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f);
                            Matrix4x4 m = clip * proj;
                            m[0, 2] *= -1;
                            m[1, 2] *= -1;
                            m[2, 2] *= -1;
                            m[3, 2] *= -1;
                            Matrix4x4 worldToShadow = m * worldToLight;
                            _spotLightParameters.worldToShadowMatrix = MatrixFloats.ToMatrixFloats(worldToShadow);

                            _spotLightParameters.shadowMapIndex = _shadowMapIndex;
                            _spotLightParameters.shadowStrength = 1.0f - _lightComponent.shadowStrength;
                        }

                        _spotLightParameters.cookieMapIndex = -1;
                        if(CastsCookie)
                        {
                            _spotLightParameters.cookieMapIndex = _cookieMapIndex;
                            _spotLightParameters.cookieParameters.x = customCookieDistanceFalloffStartThreshold;
                            _spotLightParameters.cookieParameters.y = customCookieDistanceFalloffEndThreshold;
                            _spotLightParameters.cookieParameters.z = customCookieDistanceFalloffPower;
                        }
                    }
                    break;

                case LightType.Point :
                    {
                        _pointLightParameters.color = color;
                        _pointLightParameters.lightPosition = _lightComponent.transform.position;
                        _pointLightParameters.lightRange = _lightComponent.range;
                        _pointLightParameters.distanceFalloffParameters = new Vector2(customDistanceFalloffThreshold, customDistanceFalloffPower);

                        _pointLightParameters.shadowMapIndex = -1;
                        if(CastsShadows)
                        {
                            Matrix4x4 worldMatrix = Matrix4x4.TRS(camera.transform.position, transform.rotation, Vector3.one * camera.nearClipPlane * 2);
                            _storePointLightShadowMapMaterial.SetMatrix("_WorldViewProj", GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix * worldMatrix);
                            _storePointLightShadowMapMaterial.EnableKeyword("SHADOWS_CUBE");
                            _storePointLightShadowMapMaterial.EnableKeyword("POINT");
                            _copyShadowmapCommandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
                            _copyShadowmapCommandBuffer.SetRenderTarget(shadowMapRenderTexture);
                            _copyShadowmapCommandBuffer.DrawMesh(storePointLightShadowMapMesh, worldMatrix, _storePointLightShadowMapMaterial, 0);

                            Matrix4x4 worldToShadow = Matrix4x4.TRS(_lightComponent.transform.position, _lightComponent.transform.rotation, Vector3.one * _lightComponent.range).inverse;
                            _pointLightParameters.worldToShadowMatrix = MatrixFloats.ToMatrixFloats(worldToShadow);
                            #if UNITY_2017_3_OR_NEWER
                            _pointLightParameters.lightProjectionParameters = new Vector2(_lightComponent.range / (_lightComponent.shadowNearPlane - _lightComponent.range), (_lightComponent.shadowNearPlane * _lightComponent.range) / (_lightComponent.shadowNearPlane - _lightComponent.range)); // From UnityShaderVariables.cginc:114
                            #endif

                            _pointLightParameters.shadowMapIndex = _shadowMapIndex;
                            _pointLightParameters.shadowStrength = 1.0f - _lightComponent.shadowStrength;
                        }

                        _pointLightParameters.cookieMapIndex = -1;
                        if(CastsCookie)
                        {
                            _pointLightParameters.cookieMapIndex = _cookieMapIndex;
                            _pointLightParameters.cookieParameters.x = customCookieDistanceFalloffStartThreshold;
                            _pointLightParameters.cookieParameters.y = customCookieDistanceFalloffEndThreshold;
                            _pointLightParameters.cookieParameters.z = customCookieDistanceFalloffPower;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns the collected data for the directional light
        /// </summary>
        /// <returns>The packed data</returns>
        public DirectionalLightParameters GetDirectionnalParameters()
        {
            return _directionalLightParameters;
        }

        /// <summary>
        /// Returns the collected data for the spot light
        /// </summary>
        /// <returns>The packed data</returns>
        public SpotLightParameters GetSpotParameters()
        {
            return _spotLightParameters;
        }

        /// <summary>
        /// Returns the collected data for the point light
        /// </summary>
        /// <returns>The packed data</returns>
        public PointLightParameters GetPointParameters()
        {
            return _pointLightParameters;
        }

        /// <summary>
        /// Modifies the index of the shadow map in the composed Texture2DArray that is sent to the compute shader 
        /// </summary>
        /// <param name="index">The new index</param>
        public void SetShadowMapIndex(int index)
        {
            _shadowMapIndex = index;
        }

        /// <summary>
        /// Modifies the index of the cookie map in the composed Texture2DArray that is sent to the compute shader 
        /// </summary>
        /// <param name="index">The new index</param>
        public void SetCookieMapIndex(int index)
        {
            _cookieMapIndex = index;
        }

        /// <summary>
        /// Compute the bounding sphere for the ObjectsCuller
        /// </summary>
        private void UpdateBoundingSphere()
        {
            float radius = float.MaxValue;

            switch(Type)
            {
                case LightType.Point :
                    {
                        radius = _lightComponent.range;
                    }
                    break;

                case LightType.Spot :
                    {
                        // TODO : MORE ACCURATE BOUNDING SPHERE
                        radius = _lightComponent.range;
                    }
                    break;
            }

            UpdateBoundingSphere(transform.position, radius);
        }

        /// <summary>
        /// Copies the light's cookie map into the "cookieMapRenderTexture"
        /// </summary>
        private void CopyCookieMap()
        {
            switch(Type)
            {
                case LightType.Point :
                    {
                        _storeCookieMapMaterial.SetMatrix("_InverseWorldMatrix", _lightComponent.transform.worldToLocalMatrix);
                    }
                    break;
            }

            Graphics.Blit(_lightComponent.cookie, cookieMapRenderTexture, _storeCookieMapMaterial);
        }
        #endregion

        #region GameObject constructor
#if UNITY_EDITOR 
        /// <summary>
        /// Method allowing to add a MenuItem to quickly build GameObjects with the component assigned
        /// </summary>
        /// <param name="name">The name of the new GameObject</param>
        /// <param name="type">The desired light's type</param>
        /// <returns>The created AuraLight gameObject</returns>
        public static GameObject CreateGameObject(string name, LightType type)
        {
            return CreateGameObject(new MenuCommand(null), name, type);
        }
        /// <summary>
        /// Method allowing to add a MenuItem to quickly build GameObjects with the component assigned
        /// </summary>
        /// <param name="menuCommand">Stuff that Unity automatically fills with some editor's contextual infos</param>
        /// <param name="name">The name of the new GameObject</param>
        /// <param name="type">The desired light's type</param>
        /// <returns>The created AuraLight gameObject</returns>
        private static GameObject CreateGameObject(MenuCommand menuCommand, string name, LightType type)
        {
            GameObject newGameObject = new GameObject(name);
            GameObjectUtility.SetParentAndAlign(newGameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + newGameObject.name);
            Selection.activeObject = newGameObject;

            newGameObject.AddComponent<Light>();
            newGameObject.GetComponent<Light>().type = type;
            newGameObject.GetComponent<Light>().shadows = LightShadows.Soft;
            newGameObject.AddComponent<AuraLight>();

            return newGameObject;
        }

        /// <summary>
        /// Creates a new Aura Directional Light
        /// </summary>
        /// <param name="menuCommand">Stuff that Unity automatically fills with some editor's contextual infos</param>
        [MenuItem("GameObject/Aura/Light/Directional", false, 1)]
        static void CreateDirectionalGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Directional Light", LightType.Directional);
        }

        /// <summary>
        /// Creates a new Aura Spot Light
        /// </summary>
        /// <param name="menuCommand">Stuff that Unity automatically fills with some editor's contextual infos</param>
        [MenuItem("GameObject/Aura/Light/Spot", false, 2)]
        static void CreateSpotGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Spot Light", LightType.Spot);
        }

        /// <summary>
        /// Creates a new Aura Point Light
        /// </summary>
        /// <param name="menuCommand">Stuff that Unity automatically fills with some editor's contextual infos</param>
        [MenuItem("GameObject/Aura/Light/Point", false, 3)]
        static void CreatePointGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Point Light", LightType.Point);
        }
#endif
        #endregion
    }
}
