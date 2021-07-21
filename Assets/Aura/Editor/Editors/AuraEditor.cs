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

using UnityEditor;
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Custom Inspector for Aura class
    /// </summary>
    [CustomEditor(typeof(Aura))]
    public class AuraEditor : Editor
    {
        #region Public Members
        /// <summary>
        /// The logo to display
        /// </summary>
        public Texture2D logoTexture;
        /// <summary>
        /// Base injection tab icon
        /// </summary>
        public Texture2D baseInjectionIconTexture;
        /// <summary>
        /// Injection icon
        /// </summary>
        public Texture2D injectionIconTexture;
        /// <summary>
        /// Settings tab icon
        /// </summary>
        public Texture2D settingsIconTexture;
        /// <summary>
        /// Grid icon
        /// </summary>
        public Texture2D gridIconTexture;
        /// <summary>
        /// Contributions icon
        /// </summary>
        public Texture2D injectionContributionIconTexture;
        /// <summary>
        /// Experimental feature icon
        /// </summary>
        public Texture2D experimentalFeaturesIconTexture;
        #endregion

        #region Private Members
        /// <summary>
        /// The current displayed tab index
        /// </summary>
        private int _tabIndex;
        /// <summary>
        /// The content of the tabs
        /// </summary>
        private GUIContent[] _tabsContent;
        /// <summary>
        /// The property for base density
        /// </summary>
        private SerializedProperty _baseDensityProperty;
        /// <summary>
        /// The property for base anisotropy
        /// </summary>
        private SerializedProperty _baseAnisotropyProperty;
        /// <summary>
        /// The property for base color
        /// </summary>
        private SerializedProperty _baseColorProperty;
        /// <summary>
        /// The property for base color strength
        /// </summary>
        private SerializedProperty _baseColorStrengthProperty;
        /// <summary>
        /// The property for grid resolution
        /// </summary>
        private SerializedProperty _gridSettingsResolutionProperty;
        /// <summary>
        /// The property for grid far plane
        /// </summary>
        private SerializedProperty _gridSettingsFarPlaneProperty;
        /// <summary>
        /// The property for applying as a post process
        /// </summary>
        private SerializedProperty _applyAsPostProcessProperty;
        /// <summary>
        /// The property for executing in edit mode (DOESN'T WORK FOR NOW)
        /// </summary>
        //private SerializedProperty executeInEditModeProperty;
        /// <summary>
        /// The property for enabling volumes injection
        /// </summary>
        private SerializedProperty _enableVolumesProperty;
        /// <summary>
        /// The property for enabling texture masks
        /// </summary>
        private SerializedProperty _enableVolumesTextureMaskProperty;
        /// <summary>
        /// The property for noise masks
        /// </summary>
        private SerializedProperty _enableVolumesNoiseMaskProperty;
        /// <summary>
        /// The property for enabling Directional Lights
        /// </summary>
        private SerializedProperty _enableDirectionalLightsProperty;
        /// <summary>
        /// The property for enabling Directional Lights shadows
        /// </summary>
        private SerializedProperty _enableDirectionalLightsShadowsProperty;
        /// <summary>
        /// The property for enabling Spot Lights
        /// </summary>
        private SerializedProperty _enableSpotLightsProperty;
        /// <summary>
        /// The property for enabling Spot Lights shadows
        /// </summary>
        private SerializedProperty _enableSpotLightsShadowsProperty;
        /// <summary>
        /// The property for enabling Point Lights
        /// </summary>
        private SerializedProperty _enablePointLightsProperty;
        /// <summary>
        /// The property for enabling Point Lights shadows
        /// </summary>
        private SerializedProperty _enablePointLightsShadowsProperty;
        /// <summary>
        /// The property for enabling Lights cookies
        /// </summary>
        private SerializedProperty _enableLightsCookiesProperty;
        /// <summary>
        /// The property for enabling per cell Occlusion Culling
        /// </summary>
        private SerializedProperty _enableOcclusionCullingProperty;
        /// <summary>
        /// The property for adjusting per cell Occlusion Culling search accuracy
        /// </summary>
        private SerializedProperty _occlusionCullingAccuracyProperty;
        /// <summary>
        /// The property for enabling Temporal Reprojection
        /// </summary>
        private SerializedProperty _enableTemporalReprojectionProperty;
        /// <summary>
        /// The property for adjusting Temporal Reprojection strength
        /// </summary>
        private SerializedProperty _temporalReprojectionFactorProperty;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/Editor.html)
        private void OnEnable()
        {
            _tabsContent = new GUIContent[2];

            _tabsContent[0] = new GUIContent(" Base Injection", baseInjectionIconTexture);
            _baseDensityProperty = serializedObject.FindProperty("frustum.settings.density");
            _baseAnisotropyProperty = serializedObject.FindProperty("frustum.settings.anisotropy");
            _baseColorProperty = serializedObject.FindProperty("frustum.settings.color");
            _baseColorStrengthProperty = serializedObject.FindProperty("frustum.settings.colorStrength");

            _tabsContent[1] = new GUIContent(" Settings", settingsIconTexture);
            _gridSettingsResolutionProperty = serializedObject.FindProperty("frustum.settings.resolution");
            _gridSettingsFarPlaneProperty = serializedObject.FindProperty("frustum.settings.farClipPlaneDistance");
            _applyAsPostProcessProperty = serializedObject.FindProperty("applyAsPostProcess");

            //executeInEditModeProperty = serializedObject.FindProperty("base.runInEditMode");
            _enableVolumesProperty = serializedObject.FindProperty("frustum.settings.enableVolumes");
            _enableVolumesTextureMaskProperty = serializedObject.FindProperty("frustum.settings.enableVolumesTextureMask");
            _enableVolumesNoiseMaskProperty = serializedObject.FindProperty("frustum.settings.enableVolumesNoiseMask");
            _enableDirectionalLightsProperty = serializedObject.FindProperty("frustum.settings.enableDirectionalLights");
            _enableDirectionalLightsShadowsProperty = serializedObject.FindProperty("frustum.settings.enableDirectionalLightsShadows");
            _enableSpotLightsProperty = serializedObject.FindProperty("frustum.settings.enableSpotLights");
            _enableSpotLightsShadowsProperty = serializedObject.FindProperty("frustum.settings.enableSpotLightsShadows");
            _enablePointLightsProperty = serializedObject.FindProperty("frustum.settings.enablePointLights");
            _enablePointLightsShadowsProperty = serializedObject.FindProperty("frustum.settings.enablePointLightsShadows");
            _enableLightsCookiesProperty = serializedObject.FindProperty("frustum.settings.enableLightsCookies");
            _enableOcclusionCullingProperty = serializedObject.FindProperty("frustum.settings.enableOcclusionCulling");
            _occlusionCullingAccuracyProperty = serializedObject.FindProperty("frustum.settings.occlusionCullingAccuracy");
            _enableTemporalReprojectionProperty = serializedObject.FindProperty("frustum.settings.enableTemporalReprojection");
            _temporalReprojectionFactorProperty = serializedObject.FindProperty("frustum.settings.temporalReprojectionFactor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GuiHelpers.DrawHeader(logoTexture);

            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabsContent);

            EditorGUILayout.Separator();

            switch(_tabIndex)
            {
                case 0 :
                    {
                        DisplayBaseInjectionArea();
                    }
                    break;

                case 1 :
                    {
                        DisplaySettingsTab();
                    }
                    break;
            }

            GuiHelpers.DisplayHelpToShowHelpBox();

            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Displays the content of the base injection tab
        /// </summary>
        private void DisplayBaseInjectionArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);

            GUILayout.Button(new GUIContent(" Ambient Injection", injectionIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Ambient Injection\" parameters set the starting Density, Color and Anisotropy of the environment.");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            GuiHelpers.DrawPositiveOnlyFloatField(ref _baseDensityProperty, "Ambient Density");

            EditorGUILayout.Separator();

            GuiHelpers.DrawSlider(ref _baseAnisotropyProperty, 0, 1, "Ambient Anisotropy");

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Ambient Light");
            EditorGUILayout.PropertyField(_baseColorProperty);

            GuiHelpers.DrawPositiveOnlyFloatField(ref _baseColorStrengthProperty, "Ambient Light Strength");

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays the content of the settings tab
        /// </summary>
        private void DisplaySettingsTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Grid", gridIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Grid\" parameters allow you to determine the density of cells used to compute the volumetric lighting.\n\nThis cubic grid will be remapped on the frustum (the volume visible to the camera) and will range from the camera's near clip distance to the \"Range\" distance parameter (for performance saving and because behind a certain distance, changes are barely noticeable).");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _gridSettingsResolutionProperty.FindPropertyRelative("x").intValue = EditorGUILayout.IntField("Horizontal", _gridSettingsResolutionProperty.FindPropertyRelative("x").intValue);
            _gridSettingsResolutionProperty.FindPropertyRelative("y").intValue = EditorGUILayout.IntField("Vertical", _gridSettingsResolutionProperty.FindPropertyRelative("y").intValue);
            _gridSettingsResolutionProperty.FindPropertyRelative("z").intValue = EditorGUILayout.IntField("Depth", _gridSettingsResolutionProperty.FindPropertyRelative("z").intValue);
            if(GUILayout.Button("Set Resolution"))
            {
                ((Aura)serializedObject.targetObject).frustum.SetResolution(((Aura)serializedObject.targetObject).frustum.settings.resolution);
            }

            EditorGUILayout.Separator();

            GuiHelpers.DrawPositiveOnlyFloatField(ref _gridSettingsFarPlaneProperty, "Range");
            if(_gridSettingsFarPlaneProperty.floatValue < Aura.CameraComponent.nearClipPlane)
            {
                EditorGUILayout.HelpBox("\nATTENTION : \nRange must be bigger than the camera near clip distance.\n", MessageType.Warning);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Contibution", injectionContributionIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Selective Contribution\" parameters allow you to enable/disable what type of contribution will be allowed to be computed.\n\nNote that the existence of the different contributions are handled by the system at runtime.");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _applyAsPostProcessProperty.boolValue = EditorGUILayout.ToggleLeft("Apply as PostProcess", _applyAsPostProcessProperty.boolValue);

            //EditorGUILayout.Separator();
            //
            //executeInEditModeProperty.boolValue = EditorGUILayout.ToggleLeft("Execute in Edit Mode", executeInEditModeProperty.boolValue);

            EditorGUILayout.Separator();

            _enableVolumesProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Volumes", _enableVolumesProperty.boolValue);
            _enableVolumesTextureMaskProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Texture Mask", _enableVolumesTextureMaskProperty.boolValue);
            _enableVolumesNoiseMaskProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Noise Mask", _enableVolumesNoiseMaskProperty.boolValue);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            _enableDirectionalLightsProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Directional Lights", _enableDirectionalLightsProperty.boolValue);
            _enableDirectionalLightsShadowsProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Shadows", _enableDirectionalLightsShadowsProperty.boolValue);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            _enableSpotLightsProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Spot Lights", _enableSpotLightsProperty.boolValue);
            _enableSpotLightsShadowsProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Shadows", _enableSpotLightsShadowsProperty.boolValue);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            _enablePointLightsProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Point Lights", _enablePointLightsProperty.boolValue);
            _enablePointLightsShadowsProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Shadows", _enablePointLightsShadowsProperty.boolValue);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(!_enableSpotLightsProperty.boolValue && !_enableSpotLightsProperty.boolValue && !_enableDirectionalLightsProperty.boolValue);
            _enableLightsCookiesProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Cookies", _enableLightsCookiesProperty.boolValue);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Experimental Features", experimentalFeaturesIconTexture), GuiStyles.areaTitleBarStyle);
            EditorGUILayout.HelpBox("\nATTENTION : \n\nThe following features are still at experimental stage.\n\nThis means that, although stable, they can lead to visual artifacts.\n\nFeel free to contact me if you have any comment about them.\n", MessageType.Warning);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _enableOcclusionCullingProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Occlusion Culling", _enableOcclusionCullingProperty.boolValue);
            GuiHelpers.DrawContextualHelpBox("The \"Occlusion Culling\" allows to compute the maximum visible depth of the frustum grid.\n\nThis leads to avoid computing cells that are invisible to the camera because hidden behind objects.");
            EditorGUILayout.PropertyField(_occlusionCullingAccuracyProperty, new GUIContent("Accuracy"));
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            _enableTemporalReprojectionProperty.boolValue = EditorGUILayout.BeginToggleGroup("Enable Reprojection", _enableTemporalReprojectionProperty.boolValue);
            GuiHelpers.DrawContextualHelpBox("The \"Reprojection\" allows to blend the current (jittered) computed frame with the previous one.\n\nThis leads to a smoother volumetric lighting, especially with a low resolution grid.");
            GuiHelpers.DrawSlider(ref _temporalReprojectionFactorProperty, 0, 1, "Reprojector factor");
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }
        #endregion
    }
}
