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
    /// Custom Inspector for AuraVolume class
    /// </summary>
    [CustomEditor(typeof(AuraVolume))]
    public class AuraVolumeEditor : Editor
    {
        #region Public Members
        /// <summary>
        /// The logo to display
        /// </summary>
        public Texture2D logoTexture;
        /// <summary>
        /// Injection icon
        /// </summary>
        public Texture2D injectionIconTexture;
        /// <summary>
        /// Settings tab icon
        /// </summary>
        public Texture2D settingsIconTexture;
        /// <summary>
        /// Volume shape icon
        /// </summary>
        public Texture2D volumeShapeIconTexture;
        /// <summary>
        /// Texture mask icon
        /// </summary>
        public Texture2D textureMaskIconTexture;
        /// <summary>
        /// Experimental feature icon
        /// </summary>
        public Texture2D experimentalFeaturesIconTexture;
        /// <summary>
        /// Noise mask icon
        /// </summary>
        public Texture2D noiseMaskIconTexture;
        /// <summary>
        /// Density icon
        /// </summary>
        public Texture2D densityIconTexture;
        /// <summary>
        /// Color icon
        /// </summary>
        public Texture2D colorIconTexture;
        /// <summary>
        /// Anisotropy icon
        /// </summary>
        public Texture2D anisotropyIconTexture;
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
        /// The property for choosing the shape of the volume
        /// </summary>
        private SerializedProperty _volumeShapeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout
        /// </summary>
        private SerializedProperty _falloffFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in X+ for a Cube volume
        /// </summary>
        private SerializedProperty _xPositiveCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in X- for a Cube volume
        /// </summary>
        private SerializedProperty _xNegativeCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Y+ for a Cube volume
        /// </summary>
        private SerializedProperty _yPositiveCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Y- for a Cube volume
        /// </summary>
        private SerializedProperty _yNegativeCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Z+ for a Cube volume
        /// </summary>
        private SerializedProperty _zPositiveCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Z- for a Cube volume
        /// </summary>
        private SerializedProperty _zNegativeCubeFadeProperty;
        /// <summary>
        /// The property for adjusting the angular borders' fadeout for a Cone volume
        /// </summary>
        private SerializedProperty _angularConeFadeProperty;
        /// <summary>
        /// The property for adjusting the distance borders' fadeout for a Cone volume
        /// </summary>
        private SerializedProperty _distanceConeFadeProperty;
        /// <summary>
        /// The property for adjusting the barycentric distance borders' fadeout for a Cylinder volume
        /// </summary>
        private SerializedProperty _widthCylinderFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Y- for a Cylinder volume
        /// </summary>
        private SerializedProperty _yNegativeCylinderFadeProperty;
        /// <summary>
        /// The property for adjusting the borders' fadeout in Y+ for a Cylinder volume
        /// </summary>
        private SerializedProperty _yPositiveCylinderFadeProperty;
        /// <summary>
        /// The property for adjusting the fadeout in Y+ for a Plane volume
        /// </summary>
        private SerializedProperty _heightPlaneFadeProperty;
        /// <summary>
        /// The property for adjusting the distance borders' fadeout for a Sphere volume
        /// </summary>
        private SerializedProperty _distanceSphereFadeProperty;
        /// <summary>
        /// The property for enabling texture mask
        /// </summary>
        private SerializedProperty _textureMaskBoolProperty;
        /// <summary>
        /// The property for the texture mask texture source
        /// </summary>
        private SerializedProperty _textureMaskTextureProperty;
        /// <summary>
        /// The property for the texture mask transform
        /// </summary>
        private SerializedProperty _textureMaskTransformProperty;
        /// <summary>
        /// The property for the texture mask wrap mode
        /// </summary>
        private SerializedProperty _textureMaskWrapModeProperty;
        /// <summary>
        /// The property for the texture mask filter mode
        /// </summary>
        private SerializedProperty _textureMaskFilterModeProperty;
        /// <summary>
        /// The property for enabling computation clipping based on the texture mask alpha
        /// </summary>
        private SerializedProperty _textureMaskClipOnAlphaProperty;
        /// <summary>
        /// The property for adjusting the threshold of the computation clipping based on alpha of the texture mask
        /// </summary>
        private SerializedProperty _textureMaskClippingThresholdProperty;
        /// <summary>
        /// The property for enabling noise mask
        /// </summary>
        private SerializedProperty _noiseMaskBoolProperty;
        /// <summary>
        /// The property for the noise mask speed
        /// </summary>
        private SerializedProperty _noiseMaskSpeedProperty;
        /// <summary>
        /// The property for the noise mask offset
        /// </summary>
        private SerializedProperty _noiseMaskOffsetProperty;
        /// <summary>
        /// The property for the nois mask transform
        /// </summary>
        private SerializedProperty _noiseMaskTransformProperty;
        /// <summary>
        /// The property for enabling density injection
        /// </summary>
        private SerializedProperty _densityInjectionBoolProperty;
        /// <summary>
        /// The property for the density injection parameters
        /// </summary>
        private SerializedProperty _densityInjectionParametersProperty;
        /// <summary>
        /// The property for enabling color injection
        /// </summary>
        private SerializedProperty _colorInjectionBoolProperty;
        /// <summary>
        /// The property for the color injection color
        /// </summary>
        private SerializedProperty _colorInjectionColorProperty;
        /// <summary>
        /// The property for the color injection parameters
        /// </summary>
        private SerializedProperty _colorInjectionParametersProperty;
        /// <summary>
        /// The property for enabling anisotropy injection
        /// </summary>
        private SerializedProperty _anisotropyInjectionBoolProperty;
        /// <summary>
        /// The property for anisotropy injection parameters
        /// </summary>
        private SerializedProperty _anisotropyInjectionParametersProperty;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/Editor.html)
        private void OnEnable()
        {
            _tabsContent = new GUIContent[4];

            _tabsContent[0] = new GUIContent(" Settings", settingsIconTexture);
            _volumeShapeProperty = serializedObject.FindProperty("volumeShape.shape");
            _falloffFadeProperty = serializedObject.FindProperty("volumeShape.fading.falloffExponent");
            _xPositiveCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.xPositiveCubeFade");
            _xNegativeCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.xNegativeCubeFade");
            _yPositiveCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.yPositiveCubeFade");
            _yNegativeCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.yNegativeCubeFade");
            _zPositiveCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.zPositiveCubeFade");
            _zNegativeCubeFadeProperty = serializedObject.FindProperty("volumeShape.fading.zNegativeCubeFade");
            _angularConeFadeProperty = serializedObject.FindProperty("volumeShape.fading.angularConeFade");
            _distanceConeFadeProperty = serializedObject.FindProperty("volumeShape.fading.distanceConeFade");
            _widthCylinderFadeProperty = serializedObject.FindProperty("volumeShape.fading.widthCylinderFade");
            _yNegativeCylinderFadeProperty = serializedObject.FindProperty("volumeShape.fading.yNegativeCylinderFade");
            _yPositiveCylinderFadeProperty = serializedObject.FindProperty("volumeShape.fading.yPositiveCylinderFade");
            _heightPlaneFadeProperty = serializedObject.FindProperty("volumeShape.fading.heightPlaneFade");
            _distanceSphereFadeProperty = serializedObject.FindProperty("volumeShape.fading.distanceSphereFade");
            _textureMaskBoolProperty = serializedObject.FindProperty("textureMask.enable");
            _textureMaskTextureProperty = serializedObject.FindProperty("textureMask.texture");
            _textureMaskTransformProperty = serializedObject.FindProperty("textureMask.transform");
            _textureMaskWrapModeProperty = serializedObject.FindProperty("textureMask.wrapMode");
            _textureMaskFilterModeProperty = serializedObject.FindProperty("textureMask.filterMode");
            _textureMaskClipOnAlphaProperty = serializedObject.FindProperty("textureMask.clipComputationBasedOnAlpha");
            _textureMaskClippingThresholdProperty = serializedObject.FindProperty("textureMask.clippingThreshold");
            _noiseMaskBoolProperty = serializedObject.FindProperty("noiseMask.enable");
            _noiseMaskSpeedProperty = serializedObject.FindProperty("noiseMask.speed");
            _noiseMaskOffsetProperty = serializedObject.FindProperty("noiseMask.offset");
            _noiseMaskTransformProperty = serializedObject.FindProperty("noiseMask.transform");

            _tabsContent[1] = new GUIContent(" Density", densityIconTexture);
            _densityInjectionBoolProperty = serializedObject.FindProperty("density.injectionParameters.enable");
            _densityInjectionParametersProperty = serializedObject.FindProperty("density.injectionParameters");

            _tabsContent[2] = new GUIContent(" Color", colorIconTexture);
            _colorInjectionBoolProperty = serializedObject.FindProperty("color.injectionParameters.enable");
            _colorInjectionColorProperty = serializedObject.FindProperty("color.color");
            _colorInjectionParametersProperty = serializedObject.FindProperty("color.injectionParameters");

            _tabsContent[3] = new GUIContent(" Anisotropy", anisotropyIconTexture);
            _anisotropyInjectionBoolProperty = serializedObject.FindProperty("anisotropy.injectionParameters.enable");
            _anisotropyInjectionParametersProperty = serializedObject.FindProperty("anisotropy.injectionParameters");
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
                        DisplaySettingsTab();
                    }
                    break;

                case 1 :
                    {
                        DisplayDensityInjectionTab();
                    }
                    break;

                case 2 :
                    {
                        DisplayColorInjectionTab();
                    }
                    break;

                case 3 :
                    {
                        DisplayAnisotropyInjectionTab();
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
        /// Displays Settings Tab
        /// </summary>
        private void DisplaySettingsTab()
        {
            DisplayShapeSettingsArea();
            EditorGUILayout.Separator();
            DisplayNoiseMaskArea();
            EditorGUILayout.Separator();
            DisplayTextureMaskArea();
            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays Shape Settings Area
        /// </summary>
        private void DisplayShapeSettingsArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);

            GUILayout.Button(new GUIContent(" Volume Shape", volumeShapeIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Shape\" parameter allows you to define the volumetric shape of the volume used for injecting Density, Color or Anisotropy.\n\nYou will also be able to parameter the fading on the borders of the shape, allowing a smooth transition between the inside and the outside of the volume.");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(_volumeShapeProperty, new GUIContent("Shape of the volume"));

            if((VolumeTypeEnum)_volumeShapeProperty.enumValueIndex != VolumeTypeEnum.Global)
            {
                GUILayout.Button("Parameters", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                switch((VolumeTypeEnum)_volumeShapeProperty.enumValueIndex)
                {
                    case VolumeTypeEnum.Box :
                        {
                            GuiHelpers.DrawMinMaxSlider(ref _xNegativeCubeFadeProperty, ref _xPositiveCubeFadeProperty, 0, 1, "Width border attenuation", true);
                            GuiHelpers.DrawMinMaxSlider(ref _yNegativeCubeFadeProperty, ref _yPositiveCubeFadeProperty, 0, 1, "Height border attenuation", true);
                            GuiHelpers.DrawMinMaxSlider(ref _zNegativeCubeFadeProperty, ref _zPositiveCubeFadeProperty, 0, 1, "Depth border attenuation", true);
                        }
                        break;

                    case VolumeTypeEnum.Cone :
                        {
                            GuiHelpers.DrawSlider(ref _angularConeFadeProperty, 0, 1, "Angular border attenuation", true);
                            GuiHelpers.DrawSlider(ref _distanceConeFadeProperty, 0, 1, "Distance border attenuation", true);
                        }
                        break;

                    case VolumeTypeEnum.Cylinder :
                        {
                            GuiHelpers.DrawSlider(ref _widthCylinderFadeProperty, 0, 1, "Width border attenuation", true);
                            GuiHelpers.DrawMinMaxSlider(ref _yNegativeCylinderFadeProperty, ref _yPositiveCylinderFadeProperty, 0, 1, "Height border attenuation", true);
                        }
                        break;

                    case VolumeTypeEnum.Planar :
                        {
                            GuiHelpers.DrawSlider(ref _heightPlaneFadeProperty, 0, 1, "Height attenuation");
                        }
                        break;

                    case VolumeTypeEnum.Sphere :
                        {
                            GuiHelpers.DrawSlider(ref _distanceSphereFadeProperty, 0, 1, "Distance attenuation", true);
                        }
                        break;
                }

                EditorGUILayout.Separator();

                GuiHelpers.DrawPositiveOnlyFloatField(ref _falloffFadeProperty, "Falloff Exponent");

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays Texture Mask Area
        /// </summary>
        private void DisplayTextureMaskArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Texture Mask", textureMaskIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Texture Mask\" parameter allows you to assign a volumetric texture mask to the volume.\nThis texture will be used for masking the data injected.\n\nThe channels of the texture are used as followed :\nRGB -> Will multiply the \"Strength\" parameter of the Color Injection.\nA -> Will multiply the \"Strength\" parameter of the Density and Anisotropy Injection.");
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            GUILayout.Button(new GUIContent(" Experimental Feature", experimentalFeaturesIconTexture), GuiStyles.topCenteredMiniGreyLabel);
            EditorGUILayout.HelpBox("\nATTENTION : \n\nThis feature is still at experimental stage.\n\nThis means that it can lead to performance, stability and/or visual issues.\n\nFeel free to contact me if you have any comment about it.\n", MessageType.Warning);

            EditorGUILayout.Separator();

            _textureMaskBoolProperty.boolValue = EditorGUILayout.ToggleLeft("Enabled", _textureMaskBoolProperty.boolValue);
            if(_textureMaskBoolProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(_textureMaskTextureProperty);

                GuiHelpers.DrawArea<TransformParameters>(ref _textureMaskTransformProperty, "Transform");

                EditorGUILayout.Separator();
                GUILayout.Button("Other Parameters", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_textureMaskWrapModeProperty, new GUIContent("Wrap Mode"));
                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(_textureMaskFilterModeProperty, new GUIContent("Filter Mode"));

                EditorGUILayout.Separator();
                _textureMaskClipOnAlphaProperty.boolValue = EditorGUILayout.Toggle("Clip on Alpha", _textureMaskClipOnAlphaProperty.boolValue);
                if(_textureMaskClipOnAlphaProperty.boolValue)
                {
                    EditorGUILayout.BeginVertical();
                    GuiHelpers.DrawSlider(ref _textureMaskClippingThresholdProperty, 0.0f, 1.0f, "Clipping Theshold");
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays Noise Mask Area
        /// </summary>
        private void DisplayNoiseMaskArea()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Noise Mask", noiseMaskIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Noise Mask\" parameter allows you to assign a dynamic morphing noise mask to the volume.\nThis noise will be used for masking the data injected.");
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _noiseMaskBoolProperty.boolValue = EditorGUILayout.ToggleLeft("Enabled", _noiseMaskBoolProperty.boolValue);
            if(_noiseMaskBoolProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Separator();
                GUILayout.Button("Parameters", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GuiHelpers.DrawFloatField(ref _noiseMaskSpeedProperty, "Speed");
                EditorGUILayout.Separator();
                GuiHelpers.DrawFloatField(ref _noiseMaskOffsetProperty, "Offset");
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
                GuiHelpers.DrawArea<TransformParameters>(ref _noiseMaskTransformProperty, "Transform");

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays Density Injection Tab
        /// </summary>
        private void DisplayDensityInjectionTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);

            GUILayout.Button(new GUIContent(" Density Injection", injectionIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Density Injection\" parameters allows you to add/remove density inside the system.\n\nIn other words, you will be able to increase/decrease the amount of micro particles inside a defined area.\n TIP :The \"Strength\" parameter will accept negative values. Meaning that you will be able to remove Density, Color or Anisotropy as well.");
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _densityInjectionBoolProperty.boolValue = EditorGUILayout.ToggleLeft("Enabled", _densityInjectionBoolProperty.boolValue);
            if(_densityInjectionBoolProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Separator();

                GuiHelpers.DrawArea<VolumeInjectionCommonParameters>(ref _densityInjectionParametersProperty, "Parameters", (object)((_textureMaskBoolProperty.boolValue ? 1 : 0) + (_noiseMaskBoolProperty.boolValue ? 2 : 0)));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays Color Injection Tab
        /// </summary>
        private void DisplayColorInjectionTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Color Injection", injectionIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Color Injection\" parameters allows you to add/remove color inside the system.\n\nIn other words, you will be able to add/remove light inside a defined area.\n TIP :The \"Strength\" parameter will accept negative values. Meaning that you will be able to remove Color.");
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _colorInjectionBoolProperty.boolValue = EditorGUILayout.ToggleLeft("Enabled", _colorInjectionBoolProperty.boolValue);
            if(_colorInjectionBoolProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(_colorInjectionColorProperty);

                GuiHelpers.DrawArea<VolumeInjectionCommonParameters>(ref _colorInjectionParametersProperty, "Parameters", (object)((_textureMaskBoolProperty.boolValue ? 1 : 0) + (_noiseMaskBoolProperty.boolValue ? 2 : 0)));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Displays Anisotropy Injection Tab
        /// </summary>
        private void DisplayAnisotropyInjectionTab()
        {
            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Anisotropy Injection", injectionIconTexture), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("The \"Anisotropy Injection\" parameters allows you to add/remove anisotropy inside the system.\n\nIn other words, you will be able to modify how light from light sources will bounce inside the micro particles and will be deviated by them. Typically, how \"wet\" the micro particles are.\n TIP :The \"Strength\" parameter will accept negative values. Meaning that you will be able to remove Anisotropy as well.");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();

            _anisotropyInjectionBoolProperty.boolValue = EditorGUILayout.ToggleLeft("Enabled", _anisotropyInjectionBoolProperty.boolValue);
            if(_anisotropyInjectionBoolProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Separator();

                GuiHelpers.DrawArea<VolumeInjectionCommonParameters>(ref _anisotropyInjectionParametersProperty, "Parameters", (_textureMaskBoolProperty.boolValue ? 1 : 0) + (_noiseMaskBoolProperty.boolValue ? 2 : 0));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }
        #endregion
    }
}
