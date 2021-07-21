#if GAIA_PRESENT && UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using AuraAPI;
using System.Linq;

namespace Gaia.GX.RaphaelErnaelsten
{
    /// <summary>
    /// Extension to add Aura to Gaia terrains
    /// </summary>
    public class GaiaExtension : MonoBehaviour
    {
        private static readonly Vector3Int _veryLowQualityResolution = new Vector3Int(40, 23, 16);
        private static readonly Vector3Int _lowQualityResolution = new Vector3Int(80, 45, 32);
        private static readonly Vector3Int _mediumQualityResolution = _lowQualityResolution * 2;
        private static readonly Vector3Int _highQualityResolution = _mediumQualityResolution * 2;
        private static readonly Vector3Int _ultraHighQualityResolution = _highQualityResolution * 2;
        private static readonly float _spawnDistanceFromCamera = 50.0f;
        private static readonly float _spawnHeightTolerance = 10.0f;
        private static readonly int _normalAveragingRings = 3;
        private static readonly int _normalAveragingSamplesFactor = 5;

        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Raphael Ernaelsten (@RaphErnaelsten)";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Aura - Volumetric Lighting";
        }

        #endregion

        #region Methods exposed by Gaia as buttons must be prefixed with GX_  
        #region Presets
        public static void GX_Presets_Dawn()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.1f;
            mainComponent.frustum.settings.anisotropy = 0.75f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.0777f, 0.50f, 1);
            mainComponent.frustum.settings.colorStrength = 0.25f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 10.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.0777f, 0.92f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 1.0f;

                directionalLights[i].strength = 1.0f;
                directionalLights[i].enableOutOfPhaseColor = true;
                directionalLights[i].outOfPhaseColor = Color.HSVToRGB(0.025f, 0.6f, 1);
                directionalLights[i].outOfPhaseColorStrength = 0.5f;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
            AuraVolume globalVolume = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global).GetComponent<AuraVolume>();
            globalVolume.noiseMask.enable = true;
            globalVolume.noiseMask.transform.scale = Vector3.one * 5.0f;
            globalVolume.density.injectionParameters.enable = true;
            globalVolume.density.injectionParameters.strength = 0.1f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.contrast = 5.0f;
            globalVolume.color.injectionParameters.enable = false;
            globalVolume.anisotropy.injectionParameters.enable = true;
            globalVolume.anisotropy.injectionParameters.strength = 0.1f;
            globalVolume.anisotropy.injectionParameters.noiseMaskLevelParameters.contrast = 3.0f;
            globalVolume.anisotropy.injectionParameters.noiseMaskLevelParameters.outputLowValue = -1.0f;
        }

        public static void GX_Presets_SunnyDay()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.02f;
            mainComponent.frustum.settings.anisotropy = 0.7f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.55f, 0.30f, 1);
            mainComponent.frustum.settings.colorStrength = 0.15f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 50.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.12f, 0.35f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 1.4f;

                directionalLights[i].strength = 2.5f;
                directionalLights[i].enableOutOfPhaseColor = true;
                directionalLights[i].outOfPhaseColor = Color.HSVToRGB(0.55f, 0.6f, 1);
                directionalLights[i].outOfPhaseColorStrength = 1.5f;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
        }

        public static void GX_Presets_RainyDay()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.5f;
            mainComponent.frustum.settings.anisotropy = 0.75f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.533f, 0.1f, 1);
            mainComponent.frustum.settings.colorStrength = 0.1f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 50.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.27f, 0.15f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 0.8f;

                directionalLights[i].strength = 0.5f;
                directionalLights[i].enableOutOfPhaseColor = false;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
            AuraVolume globalVolume = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global).GetComponent<AuraVolume>();
            globalVolume.noiseMask.enable = true;
            globalVolume.noiseMask.speed = 0.15f;
            globalVolume.noiseMask.transform.scale = Vector3.one * 3.0f;
            globalVolume.density.injectionParameters.enable = true;
            globalVolume.density.injectionParameters.strength = 0.2f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.contrast = 15.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputLowValue = 0.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputHiValue = -1.0f;
            globalVolume.color.injectionParameters.enable = false;
            globalVolume.anisotropy.injectionParameters.enable = false;
        }

        public static void GX_Presets_Sunset()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.05f;
            mainComponent.frustum.settings.anisotropy = 0.6f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.05f, 0.50f, 1);
            mainComponent.frustum.settings.colorStrength = 0.15f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 10.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.036f, 0.92f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 1.4f;

                directionalLights[i].strength = 1.0f;
                directionalLights[i].enableOutOfPhaseColor = true;
                directionalLights[i].outOfPhaseColor = Color.HSVToRGB(0.025f, 0.77f, 1);
                directionalLights[i].outOfPhaseColorStrength = 0.5f;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
        }

        public static void GX_Presets_Forest()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.25f;
            mainComponent.frustum.settings.anisotropy = 0.6f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.2f, 0.4f, 1);
            mainComponent.frustum.settings.colorStrength = 0.1f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 50.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.12f, 0.35f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 1.0f;

                directionalLights[i].strength = 0.7f;
                directionalLights[i].enableOutOfPhaseColor = true;
                directionalLights[i].outOfPhaseColor = Color.HSVToRGB(0.32f, 0.56f, 1);
                directionalLights[i].outOfPhaseColorStrength = 0.1f;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
            AuraVolume globalVolume = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global).GetComponent<AuraVolume>();
            globalVolume.noiseMask.enable = true;
            globalVolume.noiseMask.speed = 0.15f;
            globalVolume.noiseMask.transform.scale = Vector3.one * 3.0f;
            globalVolume.density.injectionParameters.enable = true;
            globalVolume.density.injectionParameters.strength = 0.1f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.contrast = 15.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputLowValue = 0.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputHiValue = -1.0f;
            globalVolume.color.injectionParameters.enable = false;
            globalVolume.anisotropy.injectionParameters.enable = false;
        }

        public static void GX_Presets_Desert()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.settings.density = 0.35f;
            mainComponent.frustum.settings.anisotropy = 0.15f;
            mainComponent.frustum.settings.color = Color.HSVToRGB(0.122f, 0.50f, 1);
            mainComponent.frustum.settings.colorStrength = 0.2f;

            AuraLight[] lights = SetupLights();
            AuraLight[] directionalLights = SortOutLightsByType(lights, LightType.Directional);
            if (directionalLights.Length == 0)
            {
                directionalLights = new AuraLight[1];
                directionalLights[0] = AuraLight.CreateGameObject("Aura Directional Light", LightType.Directional).GetComponent<AuraLight>();
            }
            for (int i = 0; i < directionalLights.Length; ++i)
            {
                Vector3 tmpRotation = directionalLights[i].transform.rotation.eulerAngles;
                tmpRotation.x = 50.0f;
                directionalLights[i].transform.rotation = Quaternion.Euler(tmpRotation);

                directionalLights[i].GetComponent<Light>().color = Color.HSVToRGB(0.09f, 0.5f, 1.0f);
                directionalLights[i].GetComponent<Light>().intensity = 1.4f;

                directionalLights[i].strength = 0.7f;
                directionalLights[i].enableOutOfPhaseColor = false;
            }

            AuraVolume[] auraVolumes = GetVolumes();
            DisableActiveAuraVolumes(auraVolumes);
            AuraVolume globalVolume = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global).GetComponent<AuraVolume>();
            globalVolume.noiseMask.enable = true;
            globalVolume.noiseMask.speed = 0.15f;
            globalVolume.noiseMask.transform.scale = Vector3.one * 3.0f;
            globalVolume.density.injectionParameters.enable = true;
            globalVolume.density.injectionParameters.strength = 0.2f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.contrast = 15.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputLowValue = 0.0f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.outputHiValue = -1.0f;
            globalVolume.color.injectionParameters.enable = false;
            globalVolume.anisotropy.injectionParameters.enable = false;
        }

        public static void GX_Presets_RemoveAuraComponents()
        {
            AuraVolume[] auraVolumes = FindObjectsOfType<AuraVolume>();
            for (int i = 0; i < auraVolumes.Length; ++i)
            {
                DestroyImmediate(auraVolumes[i]);
            }

            AuraLight[] auraLights = FindObjectsOfType<AuraLight>();
            for (int i = 0; i < auraLights.Length; ++i)
            {
                DestroyImmediate(auraLights[i]);
            }

            Aura[] auraComponents = FindObjectsOfType<Aura>();
            for (int i = 0; i < auraComponents.Length; ++i)
            {
                DestroyImmediate(auraComponents[i]);
            }
        }
        #endregion
        #region Presets
        public static void GX_Volumes_GlobalDensityNoise()
        {
            GaiaSceneInfo gaiaSceneInfo = GaiaSceneInfo.GetSceneInfo();

            GameObject globalVolumeObject = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global);
            globalVolumeObject.transform.position = gaiaSceneInfo.m_centrePointOnTerrain;
            AuraVolume globalVolume = globalVolumeObject.GetComponent<AuraVolume>();
            SetupVolumeDefaultNoise(globalVolume);
            SetupVolumeDefaultDensity(globalVolume);
            globalVolume.color.injectionParameters.enable = false;
            globalVolume.anisotropy.injectionParameters.enable = false;
        }

        public static void GX_Volumes_GlobalAnisotropyNoise()
        {
            GaiaSceneInfo gaiaSceneInfo = GaiaSceneInfo.GetSceneInfo();

            GameObject globalVolumeObject = AuraVolume.CreateGameObject("Aura Global Volume", VolumeTypeEnum.Global);
            globalVolumeObject.transform.position = gaiaSceneInfo.m_centrePointOnTerrain;
            AuraVolume globalVolume = globalVolumeObject.GetComponent<AuraVolume>();
            SetupVolumeDefaultNoise(globalVolume);
            globalVolume.density.injectionParameters.enable = false;
            globalVolume.color.injectionParameters.enable = false;
            SetupVolumeDefaultAnisotropy(globalVolume);
        }

        public static void GX_Volumes_FogBox()
        {
            GameObject volumeObject = AuraVolume.CreateGameObject("Aura Box Volume", VolumeTypeEnum.Box);
            SetSpawnTransform(volumeObject);

            AuraVolume volume = volumeObject.GetComponent<AuraVolume>();
            SetupDefaultNoiseVolume(volume);
            volume.density.injectionParameters.strength = 1.0f;
        }

        public static void GX_Volumes_FogPlanar()
        {
            GameObject volumeObject = AuraVolume.CreateGameObject("Aura Planar Volume", VolumeTypeEnum.Planar);
            SetSpawnTransform(volumeObject, false);

            AuraVolume volume = volumeObject.GetComponent<AuraVolume>();
            SetupDefaultNoiseVolume(volume);
            volume.density.injectionParameters.strength = 1.0f;
        }

        public static void GX_Volumes_MagicalArea()
        {
            GameObject volumeObject = AuraVolume.CreateGameObject("Aura Magical Area Volume", VolumeTypeEnum.Cylinder);
            SetSpawnTransform(volumeObject);

            AuraVolume volume = volumeObject.GetComponent<AuraVolume>();
            volume.volumeShape.fading.widthCylinderFade = 0.5f;
            volume.volumeShape.fading.yNegativeCylinderFade = 0.0f;
            volume.volumeShape.fading.yPositiveCylinderFade = 0.85f;
            SetupDefaultNoiseVolume(volume);
            volume.noiseMask.speed = 0.25f;
            volume.noiseMask.transform.scale = new Vector3(2.0f, 1.0f, 2.0f);
            volume.density.injectionParameters.enable = true;
            volume.density.injectionParameters.strength = 0.5f;
            volume.density.injectionParameters.noiseMaskLevelParameters.outputLowValue = 1.0f;
            volume.density.injectionParameters.noiseMaskLevelParameters.outputHiValue = 1.0f;
            volume.color.injectionParameters.enable = true;
            volume.color.color = Color.HSVToRGB(0.72f, 0.8f, 1.0f);
            volume.color.injectionParameters.strength = 1.0f;
            volume.color.injectionParameters.noiseMaskLevelParameters.contrast = 100.0f;
            volume.color.injectionParameters.noiseMaskLevelParameters.outputLowValue = -5.0f;
            volume.color.injectionParameters.noiseMaskLevelParameters.outputHiValue = 1.0f;
            volume.anisotropy.injectionParameters.enable = true;
            volume.anisotropy.injectionParameters.strength = 1.0f;
            volume.anisotropy.injectionParameters.noiseMaskLevelParameters.outputLowValue = 1.0f;
            volume.anisotropy.injectionParameters.noiseMaskLevelParameters.outputHiValue = 1.0f;
        }

        public static void GX_Volumes_DarkArea()
        {
            GameObject volumeObject = AuraVolume.CreateGameObject("Aura Dark Area Volume", VolumeTypeEnum.Cylinder);
            SetSpawnTransform(volumeObject);

            AuraVolume volume = volumeObject.GetComponent<AuraVolume>();
            volume.volumeShape.fading.widthCylinderFade = 0.5f;
            volume.volumeShape.fading.yNegativeCylinderFade = 0.0f;
            volume.volumeShape.fading.yPositiveCylinderFade = 0.75f;
            SetupVolumeDefaultDensity(volume);
            volume.density.injectionParameters.strength = 0.75f;
            volume.color.injectionParameters.enable = true;
            volume.color.color = Color.HSVToRGB(0.49f, 0.0f, 1.0f);
            volume.color.injectionParameters.strength = -10;
            SetupVolumeDefaultAnisotropy(volume);
            volume.anisotropy.injectionParameters.strength = 1.0f;
        }
        #endregion

        #region Quality
        public static void GX_Quality_SetVeryLowQuality()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.SetResolution(_veryLowQualityResolution);
        }

        public static void GX_Quality_SetLowQuality()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.SetResolution(_lowQualityResolution);
        }

        public static void GX_Quality_SetMediumQuality()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.SetResolution(_mediumQualityResolution);
        }

        public static void GX_Quality_SetHighQuality()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.SetResolution(_highQualityResolution);
        }

        public static void GX_Quality_SetUltraHighQuality()
        {
            Aura mainComponent = SetupAura();
            mainComponent.frustum.SetResolution(_ultraHighQualityResolution);
        }
        #endregion

        #region More Information
        public static void GX_MoreInformation_AboutAura()
        {
            EditorUtility.DisplayDialog("About Aura", "Aura is an open source volumetric lighting solution for Unity. Aura simulates the scattering of the light in the environmental medium and the illumination of micro-particles that are present in this environment but invisible to the eye/camera. This phoenomenon is commonly known as \"volumetric fog\".", "OK");
        }

        public static void GX_MoreInformation_Twitter()
        {
            Application.OpenURL("https://twitter.com/RaphErnaelsten");
        }

        public static void GX_MoreInformation_VisitAuraOnGithub()
        {
            Application.OpenURL("https://github.com/raphael-ernaelsten/Aura");
        }

        public static void GX_MoreInformation_VisitAuraOnTheAssetStore()
        {
            Application.OpenURL("http://u3d.as/16gj");
        }
        #endregion

        #region Functions
        private static AuraLight[] SetupLights()
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            AuraLight[] auraLights = new AuraLight[lights.Length];
            for (int i = 0; i < lights.Length; ++i)
            {
                AuraLight auraLight = lights[i].gameObject.GetComponent<AuraLight>();
                if (auraLight == null)
                {
                    auraLight = lights[i].gameObject.AddComponent<AuraLight>();
                }

                auraLights[i] = auraLight;
            }

            return auraLights;
        }

        public static AuraLight[] SortOutLightsByType(AuraLight[] auraLights, LightType type)
        {
            return auraLights.Where(x => x.Type == type).ToArray();
        }

        private static AuraVolume[] GetVolumes()
        {
            AuraVolume[] auraVolumes = GameObject.FindObjectsOfType<AuraVolume>();

            return auraVolumes;
        }

        private static void DisableActiveAuraVolumes(AuraVolume[] auraVolumes)
        {
            for (int i = 0; i < auraVolumes.Length; ++i)
            {
                if (auraVolumes[i].gameObject.activeInHierarchy)
                {
                    auraVolumes[i].gameObject.SetActive(false);
                    Debug.LogWarning("The AuraVolume's gameObject named \"" + auraVolumes[i].gameObject.name + "\" has been disabled to achieve the Preset's goal.", auraVolumes[i]);
                }
            }
        }

        private static void SetupVolumeDefaultNoise(AuraVolume volume)
        {
            volume.noiseMask.enable = true;
            volume.noiseMask.speed = UnityEngine.Random.Range(0.075f, 0.25f);
            volume.noiseMask.offset = UnityEngine.Random.Range(0.0f, 100.0f);
            volume.noiseMask.transform.scale = Vector3.one * 3.0f;
        }

        private static void SetupVolumeDefaultDensity(AuraVolume globalVolume)
        {
            globalVolume.density.injectionParameters.enable = true;
            globalVolume.density.injectionParameters.strength = 0.1f;
            globalVolume.density.injectionParameters.noiseMaskLevelParameters.contrast = 10.0f;
        }

        private static void SetupVolumeDefaultAnisotropy(AuraVolume volume)
        {
            volume.anisotropy.injectionParameters.enable = true;
            volume.anisotropy.injectionParameters.strength = 0.25f;
            volume.anisotropy.injectionParameters.noiseMaskLevelParameters.contrast = 5.0f;
            volume.anisotropy.injectionParameters.noiseMaskLevelParameters.outputLowValue = 1.0f;
            volume.anisotropy.injectionParameters.noiseMaskLevelParameters.outputHiValue = -1.0f;
        }

        private static void SetupDefaultNoiseVolume(AuraVolume volume)
        {
            SetupVolumeDefaultNoise(volume);
            SetupVolumeDefaultDensity(volume);
            volume.color.injectionParameters.enable = false;
            SetupVolumeDefaultAnisotropy(volume);
        }

        private static Aura SetupAura()
        {
            Aura mainComponent = FindObjectOfType<Aura>();
            if (mainComponent == null)
            {
                Camera camera = Camera.main;
                if (camera == null)
                {
                    camera = FindObjectOfType<Camera>();
                }
                if (camera == null)
                {
                    camera = new GameObject("Main Camera", new Type[] { typeof(Camera) }).GetComponent<Camera>();
                }

                mainComponent = camera.gameObject.AddComponent<Aura>();
                mainComponent.frustum.SetResolution(_highQualityResolution);
            }

            mainComponent.frustum.settings.occlusionCullingAccuracy = OcclusionCullingAccuracyEnum.Highest; // Because of the trees' leaves

            return mainComponent;
        }

        private static bool GetSpawnPosition(Camera camera, out Vector3 spawnPosition)
        {
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo, _spawnDistanceFromCamera, (int)Gaia.TerrainHelper.GetActiveTerrainLayer()))
            {
                spawnPosition = hitInfo.point;
                return true;
            }
            else
            {
                spawnPosition = camera.transform.position + camera.transform.forward * _spawnDistanceFromCamera;

                Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
                float terrainHeightUnderSpawnPosition = terrain.SampleHeight(spawnPosition);
                Vector3 positionOnTerrain = terrain.transform.localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, terrainHeightUnderSpawnPosition, 0.0f));
                positionOnTerrain.x = spawnPosition.x;
                positionOnTerrain.z = spawnPosition.z;
            
                if(Mathf.Abs(spawnPosition.y - positionOnTerrain.y) < _spawnHeightTolerance || spawnPosition.y < positionOnTerrain.y)
                {
                    spawnPosition = positionOnTerrain;
                    return true;
                }

                return false;
            }
        }

        private static Vector2 GetTerrainNormalizedPosition(Terrain terrain, Vector3 worldPosition)
        {
            Vector3 localPosition = terrain.transform.worldToLocalMatrix.MultiplyPoint(worldPosition);
            Vector2 normalizedPosition;
            normalizedPosition.x = localPosition.x / terrain.terrainData.bounds.size.x;
            normalizedPosition.y = localPosition.z / terrain.terrainData.bounds.size.z;

            return normalizedPosition;
        }

        private static Vector3 GetTerrainNormal(Vector3 worldPosition, Terrain terrain)
        {
            Vector2 normalizedPosition = GetTerrainNormalizedPosition(terrain, worldPosition);

            return terrain.terrainData.GetInterpolatedNormal(normalizedPosition.x, normalizedPosition.y);
        }

        private static void GetSpawnData(out Vector3 position, out Quaternion rotation, out Vector3 normal, bool offsetFromTerrain)
        {
            Camera camera = SceneView.lastActiveSceneView.camera;
            bool isSpawnOnTerrain = GetSpawnPosition(camera, out position);
            normal = Vector3.up;
            rotation = isSpawnOnTerrain ? GetSpawnRotation(camera, position, _spawnDistanceFromCamera * 0.25f, out normal) : Quaternion.identity;
            position += offsetFromTerrain && isSpawnOnTerrain ? normal * _spawnHeightTolerance * 0.5f : Vector3.zero;
        }

        private static Quaternion GetSpawnRotation(Camera camera, Vector3 spawnPosition, float averagingRange, out Vector3 normal)
        {
            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
            normal = GetTerrainNormal(spawnPosition, terrain);
            for(int i = 1; i <= _normalAveragingRings; ++i)
            {
                float distanceFromPosition = averagingRange * (float)i / (float)_normalAveragingRings;
                int samplesAroundRing = _normalAveragingSamplesFactor * i;
                for (int j = 0; j < samplesAroundRing; ++j)
                {
                    float angle = (float)j / (float)samplesAroundRing * 2.0f * Mathf.PI;
                    Vector2 offsetPosition = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector3 samplingPosition = spawnPosition + ((new Vector3(offsetPosition.x, 0.0f, offsetPosition.y)) * distanceFromPosition);

                    normal += GetTerrainNormal(samplingPosition, terrain);
                }
            }
            normal.Normalize();

            Vector3 forwardDirection = -Vector3.Cross(Vector3.Cross(camera.transform.forward, normal), normal).normalized;
            return Quaternion.LookRotation(forwardDirection, normal);
        }

        private static void SetSpawnTransform(GameObject gameObject, bool offsetFromTerrain = true)
        {
            Vector3 spawnPosition;
            Quaternion spawnRotation;
            Vector3 spawnNormal;
            GetSpawnData(out spawnPosition, out spawnRotation, out spawnNormal, offsetFromTerrain);

            gameObject.transform.rotation = spawnRotation;
            gameObject.transform.position = spawnPosition;
            gameObject.transform.localScale = new Vector3(_spawnDistanceFromCamera * 0.5f, _spawnHeightTolerance, _spawnDistanceFromCamera * 0.5f);
        }
        #endregion
        #endregion

        #region Helper methods

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        private static string GetAssetPath(string name)
        {
            #if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            #endif
            return null;
        }

        /// <summary>
        /// Get the asset prefab if we can find it in the project
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetAssetPrefab(string name)
        {
            #if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".prefab"))
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }
            #endif
            return null;
        }

        /// <summary>
        /// Get the range from the terrain or return a default range if no terrain
        /// </summary>
        /// <returns></returns>
        public static float GetRangeFromTerrain()
        {
            Terrain terrain = GetActiveTerrain();
            if (terrain != null)
            {
                return Mathf.Max(terrain.terrainData.size.x, terrain.terrainData.size.z) / 2f;
            }
            return 1024f;
        }

        /// <summary>
        /// Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrain if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.isActiveAndEnabled)
            {
                return terrain;
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    return terrain;
                }
            }
            return null;
        }

        #endregion
    }
}
#endif