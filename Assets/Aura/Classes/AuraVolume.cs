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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AuraAPI
{
    /// <summary>
    ///     Component allowing the volume injection of density/anisotropy/color
    /// </summary>
    [AddComponentMenu("Aura/Aura Volume", 2)]
    [ExecuteInEditMode]
    [Serializable]
    public class AuraVolume : CullableObject
    {
        #region Public members
        /// <summary>
        /// Shape of the volume and its fading parameters"  
        /// </summary>
        [SerializeField]
        public VolumeInjectionShape volumeShape;
        /// <summary>
        ///     Volumetric texture mask (RGB for color, A for density and anisotropy)
        /// </summary>
        [SerializeField]
        public VolumetricTexture textureMask;
        /// <summary>
        ///     Volumetric noise mask
        /// </summary>
        public DynamicNoiseParameters noiseMask;
        /// <summary>
        /// Density injection parameters
        /// </summary>
        public VolumeInjectionParameters density;
        /// <summary>
        ///     Scattering injection parameters
        /// </summary>
        public VolumeInjectionParameters anisotropy;
        /// <summary>
        ///     Color injection parameters
        /// </summary>
        public VolumeInjectionColorParameters color;
#if UNITY_EDITOR /// <summary>
/// The color of the gizmo in the editor
/// </summary>
        [SerializeField]
        public Color gizmoColor;
#endif
        #endregion

        #region Private members
        /// <summary>
        ///     Packed data to be sent to the compute shader
        /// </summary>
        private VolumeData _volumeData;
        /// <summary>
        ///     Tells if the component is succesfully initialized
        /// </summary>
        private bool _isInitialized;
        /// <summary>
        ///     Previous texture activation state for texture activation state changed event
        /// </summary>
        private bool _previousTextureMaskState;
        /// <summary>
        ///     Previous texture reference for texture reference changed event
        /// </summary>
        private Texture3D _previousTextureMask;
        #endregion

        #region Events
        /// <summary>
        ///     Texture activation state changed event's delegate
        /// </summary>
        public event Action<AuraVolume> OnTextureMaskStateChanged;
        /// <summary>
        ///     Texture reference changed event's delegate
        /// </summary>
        public event Action<AuraVolume> OnTextureMaskChanged;
        #endregion

        #region Monobehaviour functions
#if UNITY_EDITOR
        private void Reset()
        {
            gizmoColor = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 0.5f, 1, 1);
        }
#endif

        private void OnEnable()
        {
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

        private void Start()
        {
            _volumeData = new VolumeData();

            RaiseTextureMaskStateChangedEvent();
            RaiseTextureMaskChangedEvent();
        }
        #endregion

        #region Functions
        /// <summary>
        ///     Initalize the component
        /// </summary>
        private void Initialize()
        {
            Aura.VolumesManager.Register(this);

            Aura.OnPreCullEvent += Aura_onPreCullEvent;
            Aura.OnPreRenderEvent += Aura_onPreRenderEvent;

            _isInitialized = true;
        }

        /// <summary>
        ///     Uninitialize the component
        /// </summary>
        private void Uninitialize()
        {
            Aura.VolumesManager.Unregister(this);

            RaiseTextureMaskStateChangedEvent();

            Aura.OnPreCullEvent -= Aura_onPreCullEvent;
            Aura.OnPreRenderEvent -= Aura_onPreRenderEvent;

            _isInitialized = false;
        }

        /// <summary>
        ///     Called when Aura raises OnPreCullEvent
        /// </summary>
        private void Aura_onPreCullEvent()
        {
            if(this == null)
            {
                Aura.OnPreCullEvent -= Aura_onPreCullEvent;
                return;
            }

            UpdateBoundingSphere();
        }

        /// <summary>
        ///     Called when Aura raises OnPreRenderEvent
        /// </summary>
        private void Aura_onPreRenderEvent(Camera camera)
        {
            if(this == null)
            {
                Aura.OnPreRenderEvent -= Aura_onPreRenderEvent;
                return;
            }

            PackData();
        }

        /// <summary>
        ///     Packs the data
        /// </summary>
        private void PackData()
        {
            _volumeData.transform = MatrixFloats.ToMatrixFloats(transform.worldToLocalMatrix);
            _volumeData.shape = (int)volumeShape.shape;
            _volumeData.falloffExponent = volumeShape.fading.falloffExponent;

            switch(volumeShape.shape)
            {
                case VolumeTypeEnum.Box :
                    {
                        _volumeData.xPositiveFade = volumeShape.fading.xPositiveCubeFade;
                        _volumeData.xNegativeFade = volumeShape.fading.xNegativeCubeFade;
                        _volumeData.yPositiveFade = volumeShape.fading.yPositiveCubeFade;
                        _volumeData.yNegativeFade = volumeShape.fading.yNegativeCubeFade;
                        _volumeData.zPositiveFade = volumeShape.fading.zPositiveCubeFade;
                        _volumeData.zNegativeFade = volumeShape.fading.zNegativeCubeFade;
                    }
                    break;

                case VolumeTypeEnum.Cone :
                    {
                        _volumeData.xPositiveFade = volumeShape.fading.angularConeFade;
                        _volumeData.zPositiveFade = volumeShape.fading.distanceConeFade;
                    }
                    break;

                case VolumeTypeEnum.Cylinder :
                    {
                        _volumeData.xPositiveFade = volumeShape.fading.widthCylinderFade;
                        _volumeData.yPositiveFade = volumeShape.fading.yPositiveCylinderFade;
                        _volumeData.yNegativeFade = volumeShape.fading.yNegativeCylinderFade;
                    }
                    break;

                case VolumeTypeEnum.Planar :
                    {
                        _volumeData.yPositiveFade = volumeShape.fading.heightPlaneFade;
                    }
                    break;

                case VolumeTypeEnum.Sphere :
                    {
                        _volumeData.xPositiveFade = volumeShape.fading.distanceSphereFade;
                    }
                    break;
            }

            if(textureMask.enable != _previousTextureMaskState)
            {
                RaiseTextureMaskStateChangedEvent();
            }

            if(textureMask.texture != _previousTextureMask)
            {
                RaiseTextureMaskChangedEvent();
            }

            if(textureMask.enable)
            {
                Matrix4x4 localMatrix = textureMask.transform.Matrix;
                _volumeData.textureData.transform = MatrixFloats.ToMatrixFloats(textureMask.transform.space == Space.World ? localMatrix * transform.localToWorldMatrix : localMatrix);
                _volumeData.textureData.wrapMode = textureMask.wrapMode == VolumetricTextureWrapModeEnum.SameAsSource ? (textureMask.texture.wrapMode == TextureWrapMode.Clamp ? 0 : 1) : (int)textureMask.wrapMode;
                _volumeData.textureData.filterMode = textureMask.filterMode == VolumetricTextureFilterModeEnum.SameAsSource ? (textureMask.texture.filterMode == FilterMode.Point ? 0 : 1) : (int)textureMask.filterMode;
                _volumeData.textureData.clipOnAlpha = textureMask.clipComputationBasedOnAlpha ? 1 : 0;
                _volumeData.textureData.clippingThreshold = textureMask.clippingThreshold;
            }
            _volumeData.textureData.index = textureMask.textureIndex;

            _volumeData.noiseData.enable = noiseMask.enable ? 1 : 0;
            if(noiseMask.enable)
            {
                Matrix4x4 localMatrix = noiseMask.transform.Matrix;
                _volumeData.noiseData.transform = MatrixFloats.ToMatrixFloats(noiseMask.transform.space == Space.World ? localMatrix * transform.localToWorldMatrix : localMatrix);
                _volumeData.noiseData.speed = noiseMask.speed;
                _volumeData.noiseData.offset = noiseMask.offset;
            }

            _volumeData.injectDensity = density.injectionParameters.enable ? 1 : 0;
            _volumeData.densityValue = density.injectionParameters.strength;
            _volumeData.densityTextureLevelsParameters = density.injectionParameters.textureMaskLevelParameters.Data;
            _volumeData.densityNoiseLevelsParameters = density.injectionParameters.noiseMaskLevelParameters.Data;

            _volumeData.injectAnisotropy = anisotropy.injectionParameters.enable ? 1 : 0;
            _volumeData.anisotropyValue = anisotropy.injectionParameters.strength;
            _volumeData.anisotropyTextureLevelsParameters = anisotropy.injectionParameters.textureMaskLevelParameters.Data;
            _volumeData.anisotropyNoiseLevelsParameters = anisotropy.injectionParameters.noiseMaskLevelParameters.Data;

            _volumeData.injectColor = color.injectionParameters.enable ? 1 : 0;
            _volumeData.colorValue = new Vector3(color.color.r, color.color.g, color.color.b) * color.injectionParameters.strength;
            _volumeData.colorTextureLevelsParameters = color.injectionParameters.textureMaskLevelParameters.Data;
            _volumeData.colorNoiseLevelsParameters = color.injectionParameters.noiseMaskLevelParameters.Data;
        }

        /// <summary>
        ///     Retrieves the packed data of the volume
        /// </summary>
        /// <returns>The packed data of the volume</returns>
        public VolumeData GetData()
        {
            return _volumeData;
        }

        /// <summary>
        ///     Computes the sphere radius englobing the scaled normalized cube
        /// </summary>
        /// <returns></returns>
        private float GetRadiusFromBoundingBox()
        {
            return Mathf.Abs(Vector3.Distance(Vector3.zero, transform.localScale));
        }

        /// <summary>
        ///     Updates the bounding sphere data
        /// </summary>
        private void UpdateBoundingSphere()
        {
            Vector3 position = transform.position;
            float radius = float.MaxValue;

            switch(volumeShape.shape)
            {
                case VolumeTypeEnum.Box :
                    {
                        radius = GetRadiusFromBoundingBox();
                    }
                    break;

                case VolumeTypeEnum.Sphere :
                    {
                        radius = Mathf.Max(Mathf.Abs(transform.localScale.x), Mathf.Max(Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z))) * 0.5f;
                    }
                    break;

                case VolumeTypeEnum.Cylinder :
                    {
                        radius = GetRadiusFromBoundingBox();
                    }
                    break;

                case VolumeTypeEnum.Cone :
                    {
                        position += transform.forward * transform.localScale.z * 0.5f;
                        radius = GetRadiusFromBoundingBox();
                    }
                    break;
            }

            UpdateBoundingSphere(position, radius);
        }

        /// <summary>
        ///     Raises the texture activation state changed event
        /// </summary>
        private void RaiseTextureMaskStateChangedEvent()
        {
            if(OnTextureMaskStateChanged != null)
            {
                OnTextureMaskStateChanged(this);
            }

            _previousTextureMaskState = textureMask.enable;
        }

        /// <summary>
        ///     Raises the texture reference changed event
        /// </summary>
        private void RaiseTextureMaskChangedEvent()
        {
            if(OnTextureMaskChanged != null)
            {
                OnTextureMaskChanged(this);
            }

            _previousTextureMask = textureMask.texture;
        }
        #endregion

        #region GameObject constructor
#if UNITY_EDITOR
        /// <summary>
        /// Generic method for crating a GameObject with a AuraVolume component assigned
        /// </summary>
        /// <param name="name">Name of the created GameObject</param>
        /// <param name="shape">Desired volume shape</param>
        /// <returns>The created AuraVolume gameObject</returns>
        public static GameObject CreateGameObject(string name, VolumeTypeEnum shape)
        {
            return CreateGameObject(new MenuCommand(null), name, shape);
        }
        /// <summary>
        /// Generic method for crating a GameObject with a AuraVolume component assigned
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        /// <param name="name">Name of the created GameObject</param>
        /// <param name="shape">Desired volume shape</param>
        /// <returns>The created AuraVolume gameObject</returns>
        private static GameObject CreateGameObject(MenuCommand menuCommand, string name, VolumeTypeEnum shape)
        {
            GameObject newGameObject = new GameObject(name);
            GameObjectUtility.SetParentAndAlign(newGameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create " + newGameObject.name);
            Selection.activeObject = newGameObject;

            newGameObject.AddComponent<AuraVolume>();
            newGameObject.GetComponent<AuraVolume>().volumeShape.shape = shape;

            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.falloffExponent = 2.75f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.xPositiveCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.xNegativeCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.yPositiveCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.yNegativeCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.zPositiveCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.zNegativeCubeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.angularConeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.distanceConeFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.widthCylinderFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.yNegativeCylinderFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.yPositiveCylinderFade = 0.25f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.heightPlaneFade = 1.0f;
            newGameObject.GetComponent<AuraVolume>().volumeShape.fading.distanceSphereFade = 0.25f;

            newGameObject.GetComponent<AuraVolume>().textureMask = new VolumetricTexture();
            newGameObject.GetComponent<AuraVolume>().textureMask.transform.scale = Vector3.one;
            newGameObject.GetComponent<AuraVolume>().textureMask.wrapMode = VolumetricTextureWrapModeEnum.SameAsSource;
            newGameObject.GetComponent<AuraVolume>().textureMask.filterMode = VolumetricTextureFilterModeEnum.SameAsSource;
            newGameObject.GetComponent<AuraVolume>().textureMask.clippingThreshold = 0.5f;

            newGameObject.GetComponent<AuraVolume>().noiseMask.speed = 0.125f;
            newGameObject.GetComponent<AuraVolume>().noiseMask.transform.scale = Vector3.one;

            newGameObject.GetComponent<AuraVolume>().density.injectionParameters.textureMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().density.injectionParameters.noiseMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().density.injectionParameters.enable = true;                                         // To have something visible when a volume is added
            newGameObject.GetComponent<AuraVolume>().density.injectionParameters.strength = 1;                                          // To have something visible when a volume is added

            newGameObject.GetComponent<AuraVolume>().anisotropy.injectionParameters.textureMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().anisotropy.injectionParameters.noiseMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().anisotropy.injectionParameters.strength = 0.1f;

            newGameObject.GetComponent<AuraVolume>().color.injectionParameters.textureMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().color.injectionParameters.noiseMaskLevelParameters.SetDefaultValues();
            newGameObject.GetComponent<AuraVolume>().color.injectionParameters.enable = true;                                           // To have something visible when a volume is added
            newGameObject.GetComponent<AuraVolume>().color.injectionParameters.strength = 1;                                            // To have something visible when a volume is added
            newGameObject.GetComponent<AuraVolume>().color.color = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);                            // To have something visible when a volume is added

            return newGameObject;
        }

        /// <summary>
        /// Creates a "global" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Global", false, 0)]
        static void CreateGlobalGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Global Volume", VolumeTypeEnum.Global);
        }

        /// <summary>
        /// Creates a "planar" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Planar", false, 1)]
        static void CreatePlanarGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Planar Volume", VolumeTypeEnum.Planar);
        }

        /// <summary>
        /// Creates a "box" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Box", false, 2)]
        static void CreateBoxGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Box Volume", VolumeTypeEnum.Box);
        }

        /// <summary>
        /// Creates a "sphere" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Sphere", false, 3)]
        static void CreateSphereGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Sphere Volume", VolumeTypeEnum.Sphere);
        }

        /// <summary>
        /// Creates a "cylinder" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Cylinder", false, 4)]
        static void CreateCylinderGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Cylinder Volume", VolumeTypeEnum.Cylinder);
        }

        /// <summary>
        /// Creates a "cone" volume
        /// </summary>
        /// <param name="menuCommand">Data relative to the invoked menu</param>
        [MenuItem("GameObject/Aura/Volume/Cone", false, 5)]
        static void CreateConeGameObject(MenuCommand menuCommand)
        {
            CreateGameObject(menuCommand, "Aura Cone Volume", VolumeTypeEnum.Cone);
        }
#endif
        #endregion
    }

    #region Custom gizmo drawer
#if UNITY_EDITOR 
    /// <summary>
    /// Allows to draw custom gizmos for AuraVolume objects
    /// </summary>
    public class AuraVolumeGizmoDrawer
    {
        /// <summary>
        /// Minimum opacity when the object is not selected
        /// </summary>
        private const float minFadeFactor = 0.5f;
        /// <summary>
        /// Computes the opacity according to a weight factor for when the object is not selected
        /// </summary>
        /// <param name="weight">The factor[0("minFadeFactor" parameter), 1(Maximum opacity)]</param>
        /// <returns>The weighted opacity</returns>
        private static float GetFadeFactor(float weight)
        {
            return Mathf.Lerp(minFadeFactor, 1.0f, weight);
        }
        /// <summary>
        /// Thickness factor in pixels of the gizmo
        /// </summary>
        private const float thicknessFactor = 3.0f;
        /// <summary>
        /// Gets the size of the gizmo in respect of its 3D position and the camera
        /// </summary>
        /// <param name="worldPosition">The 3D position of the gizmo</param>
        /// <returns>The scaled thickness</returns>
        private static float GetThickness(Vector3 worldPosition)
        {
            return Mathf.InverseLerp( 0f, 0.5f, Mathf.Pow( 1.0f / HandleUtility.GetHandleSize(worldPosition), 0.5f)) * thicknessFactor;
        }
        
        /// <summary>
        /// Draws a custom gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="gizmoType">Gizmo state</param>
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
        static void DrawGizmoForAuraVolume(AuraVolume component, GizmoType gizmoType)
        {
            bool isFaded = (int)gizmoType == (int)GizmoType.NonSelected || (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType == (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;
            float opacity = isFaded ? 0.5f : 1.0f;
            
            // Draws the gizmo only if depth > pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawGizmo(component, opacity * 0.25f);

            // Then draws the gizmo only if depth <= pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawGizmo(component, opacity);
        }

        /// <summary>
        /// Draws the gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="opacity">The gizmo opacity</param>
        private static void DrawGizmo(AuraVolume component, float opacity)
        {
            switch (component.volumeShape.shape)
            {
                case VolumeTypeEnum.Global:
                    {
                        DrawGlobal(component, opacity);
                    }
                    break;

                case VolumeTypeEnum.Planar:
                    {
                        DrawPlanar(component, opacity);
                    }
                    break;

                case VolumeTypeEnum.Box:
                    {
                        DrawBox(component, opacity);
                    }
                    break;

                case VolumeTypeEnum.Sphere:
                    {
                        DrawSphere(component, opacity);
                    }
                    break;

                case VolumeTypeEnum.Cylinder:
                    {
                        DrawCylinder(component, opacity);
                    }
                    break;

                case VolumeTypeEnum.Cone:
                    {
                        DrawCone(component, opacity);
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws a "Global" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawGlobal(AuraVolume component, float alpha)
        {
            Color color = component.gizmoColor;
            color.a = component.gizmoColor.a * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.25f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.9f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.8f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 2.25f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.7f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 4.0f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.6f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 6.25f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.5f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 9.0f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.4f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 12.25f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.3f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 16.0f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.2f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 20.25f), color, GetThickness(component.transform.position));
            color.a = component.gizmoColor.a * 0.1f * alpha;
            CustomGizmo.DrawCircle(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 25.0f), color, GetThickness(component.transform.position));
        }

        /// <summary>
        /// Draws a "Planar" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawPlanar(AuraVolume component, float alpha)
        {
            float y = component.volumeShape.fading.heightPlaneFade;
            
            Color color = component.gizmoColor;
            color.a *= alpha;

            CustomGizmo.DrawSquare(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));

            int count = 10;        
            for(int i = 0; i < count; ++i)
            {
                float scaledFactor = i * 1.0f / (float)count;
                float fadeFactor = GetFadeFactor(scaledFactor);
                color = component.gizmoColor;
                color.r *= fadeFactor;
                color.g *= fadeFactor;
                color.b *= fadeFactor;
                color.a *= alpha;
                CustomGizmo.DrawSquare(component.transform.localToWorldMatrix, Matrix4x4.TRS(Vector3.up * y * (1.0f - scaledFactor), Quaternion.identity, Vector3.one), color, GetThickness(component.transform.position));
            }
        
        }

        /// <summary>
        /// Draws a "Box" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawBox(AuraVolume component, float alpha)
        {
            float fadeFactor = GetFadeFactor(0);
            Color color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            CustomGizmo.DrawCube(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        
            fadeFactor = GetFadeFactor(0.5f);
            color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            float xPos = (1.0f - component.volumeShape.fading.xPositiveCubeFade) * 2 - 1;
            float xNeg = component.volumeShape.fading.xNegativeCubeFade * 2 - 1;
            float yPos = (1.0f - component.volumeShape.fading.yPositiveCubeFade) * 2 - 1;
            float yNeg = component.volumeShape.fading.yNegativeCubeFade * 2 - 1;
            float zPos = (1.0f - component.volumeShape.fading.zPositiveCubeFade) * 2 - 1;
            float zNeg = component.volumeShape.fading.zNegativeCubeFade * 2 - 1;
        
            Vector3 customPointA = new Vector3(xNeg, yPos, zNeg) * 0.5f;
            Vector3 customPointB = new Vector3(xPos, yPos, zNeg) * 0.5f;
            Vector3 customPointC = new Vector3(xPos, yNeg, zNeg) * 0.5f;
            Vector3 customPointD = new Vector3(xNeg, yNeg, zNeg) * 0.5f;
            Vector3 customPointE = new Vector3(xNeg, yPos, zPos) * 0.5f;
            Vector3 customPointF = new Vector3(xPos, yPos, zPos) * 0.5f;
            Vector3 customPointG = new Vector3(xPos, yNeg, zPos) * 0.5f;
            Vector3 customPointH = new Vector3(xNeg, yNeg, zPos) * 0.5f;
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerA, new Vector3(xNeg, yPos, zNeg) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerB, new Vector3(xPos, yPos, zNeg) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerC, new Vector3(xPos, yNeg, zNeg) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerD, new Vector3(xNeg, yNeg, zNeg) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerE, new Vector3(xNeg, yPos, zPos) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerF, new Vector3(xPos, yPos, zPos) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerG, new Vector3(xPos, yNeg, zPos) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(CustomGizmo.cubeCornerH, new Vector3(xNeg, yNeg, zPos) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        
            CustomGizmo.DrawLineSegment(customPointA, customPointB, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointB, customPointC, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointC, customPointD, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointD, customPointA, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointE, customPointF, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        
            CustomGizmo.DrawLineSegment(customPointF, customPointG, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointG, customPointH, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointH, customPointE, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointE, customPointF, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        
            CustomGizmo.DrawLineSegment(customPointA, customPointE, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointB, customPointF, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointC, customPointG, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(customPointD, customPointH, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        }

        /// <summary>
        /// Draws a "Sphere" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawSphere(AuraVolume component, float alpha)
        {
            float fadeFactor = GetFadeFactor(0);
            Color color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            CustomGizmo.DrawSphere(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position) * 0.6666666666666f);
        
            fadeFactor = GetFadeFactor(0.5f);
            color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            float x = 1.0f - component.volumeShape.fading.distanceSphereFade;
            CustomGizmo.DrawLineSegment(Vector3.up * 0.5f, Vector3.up * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(Vector3.down * 0.5f, Vector3.down * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(Vector3.left * 0.5f, Vector3.left * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(Vector3.right * 0.5f, Vector3.right * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(Vector3.back * 0.5f, Vector3.back * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(Vector3.forward * 0.5f, Vector3.forward * x * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            
            color = component.gizmoColor;
            color.a *= alpha;
            CustomGizmo.DrawSphere(component.transform.localToWorldMatrix, Vector3.zero, Quaternion.identity, Vector3.one * x, color, GetThickness(component.transform.position) * 0.6666666666666f);
        }

        /// <summary>
        /// Draws a "Cylinder" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawCylinder(AuraVolume component, float alpha)
        {
            float fadeFactor = GetFadeFactor(0);
            Color color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            CustomGizmo.DrawCylinder(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            
            fadeFactor = GetFadeFactor(0.5f);
            color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            float x = 1.0f - component.volumeShape.fading.widthCylinderFade;
            float yPos = (1.0f - component.volumeShape.fading.yPositiveCylinderFade) * 2 - 1;
            float yNeg = component.volumeShape.fading.yNegativeCylinderFade * 2 - 1;
            CustomGizmo.DrawLineSegment(new Vector3(0.5f, 0.5f, 0), new Vector3(x, yPos, 0) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(-0.5f, 0.5f, 0), new Vector3(-x, yPos, 0) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.5f, -0.5f, 0), new Vector3(x, yNeg, 0) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(-0.5f, -0.5f, 0), new Vector3(-x, yNeg, 0) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, 0.5f, 0.5f), new Vector3(0, yPos, x) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, 0.5f, -0.5f), new Vector3(0, yPos, -x) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, -0.5f, 0.5f), new Vector3(0, yNeg, x) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, -0.5f, -0.5f), new Vector3(0, yNeg, -x) * 0.5f, component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));

            color = component.gizmoColor;
            color.a *= alpha;
            CustomGizmo.DrawCylinder(component.transform.localToWorldMatrix, (Vector3.up * ((1.0f - component.volumeShape.fading.yPositiveCylinderFade) + component.volumeShape.fading.yNegativeCylinderFade) * 0.5f) - Vector3.up * 0.5f, Quaternion.identity, new Vector3(x, 1.0f - component.volumeShape.fading.yPositiveCylinderFade - component.volumeShape.fading.yNegativeCylinderFade, x), color, GetThickness(component.transform.position));
        }

        /// <summary>
        /// Draws a "Cone" gizmo
        /// </summary>
        /// <param name="component">The target component</param>
        /// <param name="alpha">The base opacity</param>
        private static void DrawCone(AuraVolume component, float alpha)
        {
        
            float fadeFactor = GetFadeFactor(0);
            Color color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            CustomGizmo.DrawCone(component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
        
            fadeFactor = GetFadeFactor(0.5f);
            color = component.gizmoColor;
            color.r *= fadeFactor;
            color.g *= fadeFactor;
            color.b *= fadeFactor;
            color.a *= alpha;
            float z = 1.0f - component.volumeShape.fading.distanceConeFade;
            float xy = Mathf.Lerp(0, 1.0f - component.volumeShape.fading.angularConeFade, z);
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, 0.5f, 1), new Vector3(0, xy * 0.5f, z), component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.0f, -0.5f, 1), new Vector3(0, -xy * 0.5f, z), component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(0.5f, 0.0f, 1), new Vector3(xy * 0.5f, 0.0f, z), component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            CustomGizmo.DrawLineSegment(new Vector3(-0.5f, 0.0f, 1), new Vector3(-xy * 0.5f, 0.0f, z), component.transform.localToWorldMatrix, color, GetThickness(component.transform.position));
            
            color = component.gizmoColor;
            color.a *= alpha;
            CustomGizmo.DrawCone(component.transform.localToWorldMatrix, Vector3.zero, Quaternion.identity, new Vector3(xy, xy, z), color, GetThickness(component.transform.position));
        }
    }
#endif
    #endregion
}
