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
    /// Custom Inspector for AuraLight class
    /// </summary>
    [CustomEditor(typeof(AuraLight))]
    [CanEditMultipleObjects]
    public class AuraLightEditor : Editor
    {
        #region Public Members
        /// <summary>
        /// The logo to display
        /// </summary>
        public Texture2D logoTexture;
        /// <summary>
        /// Settings tab icon
        /// </summary>
        public Texture2D settingsIconTexture;
        /// <summary>
        /// Additional settings tab icon
        /// </summary>
        public Texture2D additionalSettingsIconTexture;
        /// <summary>
        /// Light icon
        /// </summary>
        public Texture2D lightIconTexture;
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
        /// The property for strength
        /// </summary>
        private SerializedProperty _strengthProperty;
        /// <summary>
        /// The property for enabling light color overriding
        /// </summary>
        private SerializedProperty _overrideColorProperty;
        /// <summary>
        /// The property for color to override with
        /// </summary>
        private SerializedProperty _overridingColorProperty;
        /// <summary>
        /// The property for enabling shadows
        /// </summary>
        private SerializedProperty _useShadowsProperty;
        /// <summary>
        /// The property for enabling cookies
        /// </summary>
        private SerializedProperty _useCookieProperty;
        /// <summary>
        /// The property for enabling out-of-phase color for directional lights
        /// </summary>
        private SerializedProperty _enableOutOfPhaseColorProperty;
        /// <summary>
        /// The property for color to use when out-of-phase
        /// </summary>
        private SerializedProperty _outOfPhaseColorProperty;
        /// <summary>
        /// The property for strength of the out-of-phase color
        /// </summary>
        private SerializedProperty _outOfPhaseColorStrengthProperty;
        /// <summary>
        /// The property for custom angular falloff start threshold for spot lights
        /// </summary>
        private SerializedProperty _customAngularFalloffThresholdProperty;
        /// <summary>
        /// The property for custom angular falloff exponent for spot lights
        /// </summary>
        private SerializedProperty _customAngularFalloffPowerProperty;
        /// <summary>
        /// The property for custom distance falloff start threshold for spot/point lights
        /// </summary>
        private SerializedProperty _customDistanceFalloffThresholdProperty;
        /// <summary>
        /// The property for custom distance falloff exponent for spot/point lights
        /// </summary>
        private SerializedProperty _customDistanceFalloffPowerProperty;
        /// <summary>
        /// The property for the start of the custom distance fade-in for spot/point lights' cookies
        /// </summary>
        private SerializedProperty _customCookieDistanceFalloffLowThresholdProperty;
        /// <summary>
        /// The property for the end of the custom distance fade-in for spot/point lights' cookies
        /// </summary>
        private SerializedProperty _customCookieDistanceFalloffHiThresholdProperty;
        /// <summary>
        /// The property for the exponent of the custom distance fade-in for spot/point lights' cookies
        /// </summary>
        private SerializedProperty _customCookieDistanceFalloffPowerProperty;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/Editor.html)
        private void OnEnable()
        {
            _tabsContent = new GUIContent[2];

            _tabsContent[0] = new GUIContent(" Base Parameters", settingsIconTexture);
            _strengthProperty = serializedObject.FindProperty("strength");
            _overrideColorProperty = serializedObject.FindProperty("overrideColor");
            _overridingColorProperty = serializedObject.FindProperty("overridingColor");
            _useShadowsProperty = serializedObject.FindProperty("useShadow");
            _useCookieProperty = serializedObject.FindProperty("useCookie");

            _tabsContent[1] = new GUIContent(" Additional Parameters", additionalSettingsIconTexture);
            _enableOutOfPhaseColorProperty = serializedObject.FindProperty("enableOutOfPhaseColor");
            _outOfPhaseColorProperty = serializedObject.FindProperty("outOfPhaseColor");
            _outOfPhaseColorStrengthProperty = serializedObject.FindProperty("outOfPhaseColorStrength");

            _customAngularFalloffThresholdProperty = serializedObject.FindProperty("customAngularFalloffThreshold");
            _customAngularFalloffPowerProperty = serializedObject.FindProperty("customAngularFalloffPower");
            _customDistanceFalloffThresholdProperty = serializedObject.FindProperty("customDistanceFalloffThreshold");
            _customDistanceFalloffPowerProperty = serializedObject.FindProperty("customDistanceFalloffPower");
            _customCookieDistanceFalloffLowThresholdProperty = serializedObject.FindProperty("customCookieDistanceFalloffStartThreshold");
            _customCookieDistanceFalloffHiThresholdProperty = serializedObject.FindProperty("customCookieDistanceFalloffEndThreshold");
            _customCookieDistanceFalloffPowerProperty = serializedObject.FindProperty("customCookieDistanceFalloffPower");
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
                        DisplayCommonSettingsArea();
                    }
                    break;

                case 1 :
                    {
                        switch(((AuraLight)serializedObject.targetObject).Type)
                        {
                            case LightType.Directional :
                                {
                                    DisplayDirectionalLightAdditionalSettingsArea();
                                }
                                break;

                            case LightType.Spot :
                                {
                                    DisplaySpotLightAdditionalSettingsArea();
                                }
                                break;

                            case LightType.Point :
                                {
                                    DisplayPointLightAdditionalSettingsArea();
                                }
                                break;
                        }
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
        /// Displays the common parameters tab
        /// </summary>
        private void DisplayCommonSettingsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Base Parameters", lightIconTexture), GuiStyles.areaTitleBarStyle);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("The \"Strength\" parameter allows you to multiply the intensity of the light source in the system.");
            GuiHelpers.DrawFloatField(ref _strengthProperty, "Strength");

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("The \"Override Color\" parameter allows you to replace the light's color in the system.");
            _overrideColorProperty.boolValue = EditorGUILayout.ToggleLeft("Override Color", _overrideColorProperty.boolValue);
            if(_overrideColorProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                GuiHelpers.DrawContextualHelpBox("The \"Overriding Color\" is the color that will replace the light's color in the system.");
                EditorGUILayout.PropertyField(_overridingColorProperty);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("The \"Enable Shadows\" parameter allows you to compute the light's shadows (if enabled) in the system.");
            _useShadowsProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Shadows", _useShadowsProperty.boolValue);

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("The \"Enable Cookie\" parameter allows you to compute the light's cookie (if enabled) in the system.");
            _useCookieProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Cookie", _useCookieProperty.boolValue);

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays the additional parameters tab for directional lights
        /// </summary>
        private void DisplayDirectionalLightAdditionalSettingsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Additional Parameters", lightIconTexture), GuiStyles.areaTitleBarStyle);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("The \"Enable Out-Of-Phase Color\" parameter allows you to use a color when the view angle is not towards the directional light (the decay is controlled by the anisotropy factor.");
            _enableOutOfPhaseColorProperty.boolValue = EditorGUILayout.ToggleLeft("Enable Out-Of-Phase Color", _enableOutOfPhaseColorProperty.boolValue);
            EditorGUILayout.Separator();
            if(_enableOutOfPhaseColorProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                GuiHelpers.DrawContextualHelpBox("The strength of the color.");
                GuiHelpers.DrawFloatField(ref _outOfPhaseColorStrengthProperty, "Strength");

                GuiHelpers.DrawContextualHelpBox("The color when the view direction is not towards the directional light.");
                EditorGUILayout.PropertyField(_outOfPhaseColorProperty);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays the additional parameters tab for spot lights
        /// </summary>
        private void DisplaySpotLightAdditionalSettingsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Additional Parameters", lightIconTexture), GuiStyles.areaTitleBarStyle);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            DisplayLightAngularAttenuationParameters();

            EditorGUILayout.Separator();

            DisplayLightDistanceAttenuationParameters();

            if(((AuraLight)serializedObject.targetObject).CastsCookie)
            {
                EditorGUILayout.Separator();

                DisplayCookieDistanceAttenuationParameters();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays the additional parameters tab for point lights
        /// </summary>
        private void DisplayPointLightAdditionalSettingsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Additional Parameters", lightIconTexture), GuiStyles.areaTitleBarStyle);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            DisplayLightDistanceAttenuationParameters();

            if(((AuraLight)serializedObject.targetObject).CastsCookie)
            {
                EditorGUILayout.Separator();

                DisplayCookieDistanceAttenuationParameters();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays angular fadeout parameters for spot lights
        /// </summary>
        private void DisplayLightAngularAttenuationParameters()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Angular Attenuation", EditorStyles.boldLabel);

            GuiHelpers.DrawContextualHelpBox("The \"Threshold\" parameter is the normalized angle when the fade will start, until 1.");
            GuiHelpers.DrawSlider(ref _customAngularFalloffThresholdProperty, 0.0f, 1.0f, "Threshold");

            GuiHelpers.DrawContextualHelpBox("The \"Exponent\" parameter is the curve of the fading.");
            GuiHelpers.DrawPositiveOnlyFloatField(ref _customAngularFalloffPowerProperty, "Exponent");

            GuiHelpers.DrawContextualHelpBox("Allows to reset to Unity's default values.");
            if(GUILayout.Button("Reset"))
            {
                _customAngularFalloffThresholdProperty.floatValue = 0.8f;
                _customAngularFalloffPowerProperty.floatValue = 2.0f;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays distance fadeout parameters for spot/point lights
        /// </summary>
        private void DisplayLightDistanceAttenuationParameters()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Distance Attenuation", EditorStyles.boldLabel);

            GuiHelpers.DrawContextualHelpBox("The \"Threshold\" parameter is the normalized distance when the fade will start, until 1.");
            GuiHelpers.DrawSlider(ref _customDistanceFalloffThresholdProperty, 0.0f, 1.0f, "Threshold");

            GuiHelpers.DrawContextualHelpBox("The \"Exponent\" parameter is the curve of the fading.");
            GuiHelpers.DrawPositiveOnlyFloatField(ref _customDistanceFalloffPowerProperty, "Exponent");

            GuiHelpers.DrawContextualHelpBox("Allows to reset to Unity's default values.");
            if(GUILayout.Button("Reset"))
            {
                _customDistanceFalloffThresholdProperty.floatValue = 0.5f;
                _customDistanceFalloffPowerProperty.floatValue = 2.0f;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays distance fadein parameters for spot/point lights' cookies
        /// </summary>
        private void DisplayCookieDistanceAttenuationParameters()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Cookie Fade-In Attenuation", EditorStyles.boldLabel);

            GuiHelpers.DrawContextualHelpBox("The \"Thresholds\" parameters are the normalized range where the cookie will fade in.");
            GuiHelpers.DrawMinMaxSlider(ref _customCookieDistanceFalloffLowThresholdProperty, ref _customCookieDistanceFalloffHiThresholdProperty, 0.0f, 1.0f, "Fade-In Thresholds");

            GuiHelpers.DrawContextualHelpBox("The \"Exponent\" parameter is the curve of the fading.");
            GuiHelpers.DrawPositiveOnlyFloatField(ref _customCookieDistanceFalloffPowerProperty, "Exponent");

            GuiHelpers.DrawContextualHelpBox("Allows to reset to Unity's default values.");
            if(GUILayout.Button("Reset"))
            {
                _customCookieDistanceFalloffLowThresholdProperty.floatValue = 0.1f;
                _customCookieDistanceFalloffHiThresholdProperty.floatValue = 0.25f;
                _customAngularFalloffPowerProperty.floatValue = 2.0f;
            }

            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}
