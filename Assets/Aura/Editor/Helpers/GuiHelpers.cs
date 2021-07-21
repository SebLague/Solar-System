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
using UnityEditor;

namespace AuraAPI
{
    /// <summary>
    /// Collection of helper functions for a Custom Inspector GUI
    /// </summary>
    public static class GuiHelpers
    {
        /// <summary>
        /// Draws a header with a logo texture
        /// </summary>
        /// <param name="logoTexture">The logo to draw</param>
        public static void DrawHeader(Texture2D logoTexture)
        {
            EditorGUILayout.Separator();

            EditorGUILayout.Separator();

            GuiHelpers.DrawTexture(logoTexture, 512);

            GuiHelpers.DisplayGeneralHelpBox();
        }

        /// <summary>
        /// Draws a texture
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="maxSize">The maximum pixel size</param>
        public static void DrawTexture(Texture2D texture, float maxSize)
        {
            Rect rect = EditorGUILayout.BeginVertical();
            Rect textureRect = GUILayoutUtility.GetAspectRect(texture.width / texture.height, GUILayout.MaxWidth(maxSize));
            textureRect.x += rect.width / 2 - textureRect.width / 2;
            GUI.Label(textureRect, texture);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw an area for specific types
        /// </summary>
        /// <typeparam name="T">The desired type area</typeparam>
        /// <param name="property">The related serialized property</param>
        /// <param name="label">The label to write</param>
        /// <param name="parameters">Some formatted parameters</param>
        public static void DrawArea<T>(ref SerializedProperty property, string label, object parameters = null)
        {
            GUILayout.Button(label, EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if(typeof(T) == typeof(Texture2D))
            {
                GuiHelpers.DrawTexture2DField(ref property);
            }
            else if(typeof(T) == typeof(Texture3D))
            {
                GuiHelpers.DrawTexture3DField(ref property);
            }
            else if(typeof(T) == typeof(TransformParameters))
            {
                GuiHelpers.DrawTransformField(ref property);
            }
            else if(typeof(T) == typeof(VolumeInjectionCommonParameters))
            {
                GuiHelpers.DrawInjectionField(ref property, (int)parameters);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the field required to draw a VolumeInjectionParameters object
        /// </summary>
        /// <param name="injectionProperty">The related serialized property</param>
        /// <param name="displayMask">Int mask to display texture/noise mask</param>
        public static void DrawInjectionField(ref SerializedProperty injectionProperty, int displayMask)
        {
            SerializedProperty strengthProperty = injectionProperty.FindPropertyRelative("strength");
            GuiHelpers.DrawFloatField(ref strengthProperty, "Strength");

            if(displayMask == 1 || displayMask == 3)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Texture Mask Levels", EditorStyles.boldLabel);
                SerializedProperty textureMaskLevelsProperty = injectionProperty.FindPropertyRelative("textureMaskLevelParameters");
                GuiHelpers.DrawLevelsField(ref textureMaskLevelsProperty);
                EditorGUILayout.EndVertical();
            }

            if(displayMask == 2 || displayMask == 3)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Noise Mask Levels", EditorStyles.boldLabel);
                SerializedProperty noiseMaskLevelsProperty = injectionProperty.FindPropertyRelative("noiseMaskLevelParameters");
                GuiHelpers.DrawLevelsField(ref noiseMaskLevelsProperty);
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draws the fields required for a LevelsParameters object
        /// </summary>
        /// <param name="levelsProperty">The related serialized property</param>
        public static void DrawLevelsField(ref SerializedProperty levelsProperty)
        {
            GuiHelpers.DrawContextualHelpBox("The \"Levels\" parameter will filter the input value.\n\nKeeps the value between the \"Level Thresholds\" and remaps the range from 0 to 1.\"");
            SerializedProperty levelLowThresholdProperty = levelsProperty.FindPropertyRelative("levelLowThreshold");
            SerializedProperty levelHiThresholdProperty = levelsProperty.FindPropertyRelative("levelHiThreshold");
            GuiHelpers.DrawMinMaxSlider(ref levelLowThresholdProperty, ref levelHiThresholdProperty, 0, 1, "Level Thresholds");

            SerializedProperty contrastProperty = levelsProperty.FindPropertyRelative("contrast");
            GuiHelpers.DrawFloatField(ref contrastProperty, "Contrast");

            GuiHelpers.DrawContextualHelpBox("The \"Output\" parameters will rescale this new range\n\n0 will now equal the lower \"Output Value\" and 1 will now equal the higher.");
            SerializedProperty outputLowValueProperty = levelsProperty.FindPropertyRelative("outputLowValue");
            GuiHelpers.DrawFloatField(ref outputLowValueProperty, "Output Low Value");
            SerializedProperty outputHiValueProperty = levelsProperty.FindPropertyRelative("outputHiValue");
            GuiHelpers.DrawFloatField(ref outputHiValueProperty, "Output High Value");
        }

        /// <summary>
        /// Draws the fields required for a TransformParameters object
        /// </summary>
        /// <param name="transformProperty">The related serialized property</param>
        public static void DrawTransformField(ref SerializedProperty transformProperty)
        {
            SerializedProperty spaceProperty = transformProperty.FindPropertyRelative("space");
            GuiHelpers.DrawSpaceField(ref spaceProperty, "Relative Space");

            EditorGUILayout.Separator();
            SerializedProperty positionProperty = transformProperty.FindPropertyRelative("position");
            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            GuiHelpers.DrawVector3Field(ref positionProperty);
            SerializedProperty animatePositionProperty = transformProperty.FindPropertyRelative("animatePosition");
            animatePositionProperty.boolValue = EditorGUILayout.Toggle("Animate", animatePositionProperty.boolValue);
            if(animatePositionProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();
                SerializedProperty animatedPositionSpeedProperty = transformProperty.FindPropertyRelative("positionSpeed");
                EditorGUILayout.LabelField("Speed", EditorStyles.miniBoldLabel);
                GuiHelpers.DrawVector3Field(ref animatedPositionSpeedProperty);
                EditorGUILayout.EndVertical();
                GUILayout.Space(16);
            }

            EditorGUILayout.Separator();
            SerializedProperty rotationProperty = transformProperty.FindPropertyRelative("rotation");
            EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
            GuiHelpers.DrawVector3Field(ref rotationProperty);
            SerializedProperty animateRotationProperty = transformProperty.FindPropertyRelative("animateRotation");
            animateRotationProperty.boolValue = EditorGUILayout.Toggle("Animate", animateRotationProperty.boolValue);
            if(animateRotationProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();
                SerializedProperty animatedRotationSpeedProperty = transformProperty.FindPropertyRelative("rotationSpeed");
                EditorGUILayout.LabelField("Speed", EditorStyles.miniBoldLabel);
                GuiHelpers.DrawVector3Field(ref animatedRotationSpeedProperty);
                EditorGUILayout.EndVertical();
                GUILayout.Space(16);
            }

            EditorGUILayout.Separator();
            SerializedProperty scaleProperty = transformProperty.FindPropertyRelative("scale");
            EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
            GuiHelpers.DrawVector3Field(ref scaleProperty);
        }

        /// <summary>
        /// Draws a Texture2D object field
        /// </summary>
        /// <param name="texture2DProperty">The related serialized property</param>
        public static void DrawTexture2DField(ref SerializedProperty texture2DProperty)
        {
            EditorGUILayout.ObjectField(texture2DProperty.objectReferenceValue, typeof(Texture2D), false);
        }

        /// <summary>
        /// Draws a Texture3D object field
        /// </summary>
        /// <param name="texture3DProperty">The related serialized property</param>
        public static void DrawTexture3DField(ref SerializedProperty texture3DProperty)
        {
            EditorGUILayout.ObjectField(texture3DProperty.objectReferenceValue, typeof(Texture3D), false);
        }

        /// <summary>
        /// Draws a float field
        /// </summary>
        /// <param name="floatProperty">The related serialized property</param>
        public static void DrawFloatField(ref SerializedProperty floatProperty)
        {
            GuiHelpers.DrawFloatField(ref floatProperty, "");
        }

        /// <summary>
        /// Draws a float field
        /// </summary>
        /// <param name="floatProperty">The related serialized property</param>
        /// <param name="label">The label to write</param>
        public static void DrawFloatField(ref SerializedProperty floatProperty, string label)
        {
            if(label != "")
            {
                floatProperty.floatValue = EditorGUILayout.FloatField(label, floatProperty.floatValue, GUILayout.MinWidth(15));
            }
            else
            {
                floatProperty.floatValue = EditorGUILayout.FloatField(floatProperty.floatValue, GUILayout.MinWidth(15));
            }
        }

        /// <summary>
        /// Draws a float field that cannot go under 0
        /// </summary>
        /// <param name="floatProperty">The related serialized property</param>
        /// <param name="label">The label to write</param>
        public static void DrawPositiveOnlyFloatField(ref SerializedProperty floatProperty, string label)
        {
            floatProperty.floatValue = EditorGUILayout.FloatField(label, floatProperty.floatValue, GUILayout.MinWidth(15));
            floatProperty.floatValue = Mathf.Max(floatProperty.floatValue, 0);
        }

        /// <summary>
        /// Draws a Vector3 field
        /// </summary>
        /// <param name="vector3Property">The related serialized property</param>
        public static void DrawVector3Field(ref SerializedProperty vector3Property)
        {
            GuiHelpers.DrawVector3Field(ref vector3Property, "");
        }

        /// <summary>
        /// Draws a Vector3 field
        /// </summary>
        /// <param name="vector3Property">The related serialized property</param>
        /// <param name="label">The label to write</param>
        public static void DrawVector3Field(ref SerializedProperty vector3Property, string label)
        {
            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.height = 16;
            GUIContent[] contents = new GUIContent[3];
            contents[0] = new GUIContent("X");
            contents[1] = new GUIContent("Y");
            contents[2] = new GUIContent("Z");

            SerializedProperty xTmpProperty = vector3Property.FindPropertyRelative("x");
            SerializedProperty yTmpProperty = vector3Property.FindPropertyRelative("y");
            SerializedProperty zTmpProperty = vector3Property.FindPropertyRelative("z");
            float[] floats = new float[3]
                             {
                                 xTmpProperty.floatValue,
                                 yTmpProperty.floatValue,
                                 zTmpProperty.floatValue
                             };

            if(label != "")
            {
                EditorGUI.MultiFloatField(rect, new GUIContent(label), contents, floats);
            }
            else
            {
                EditorGUI.MultiFloatField(rect, contents, floats);
            }

            xTmpProperty.floatValue = floats[0];
            yTmpProperty.floatValue = floats[1];
            zTmpProperty.floatValue = floats[2];

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(rect.height);
        }

        /// <summary>
        /// Names of the possible spaces for a transform
        /// </summary>
        private static readonly string[] _spaceStrings = new string[2]
                                               {
                                                   "World",
                                                   "Local"
                                               };

        /// <summary>
        /// Draws a Space field
        /// </summary>
        /// <param name="spaceProperty">The related serialized property</param>
        /// <param name="label">The label to write</param>
        public static void DrawSpaceField(ref SerializedProperty spaceProperty, string label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            spaceProperty.enumValueIndex = GUILayout.Toolbar(spaceProperty.enumValueIndex, GuiHelpers._spaceStrings);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a float slider with a minimum and a maximum value
        /// </summary>
        /// <param name="min">The related serialized property</param>
        /// <param name="max">The related serialized property</param>
        /// <param name="minimumValue">The minimum possible value</param>
        /// <param name="maximumValue">The maximum possible value</param>
        /// <param name="label">The label to write</param>
        /// <param name="invertMaxValue">One minus value</param>
        public static void DrawMinMaxSlider(ref SerializedProperty min, ref SerializedProperty max, float minimumValue, float maximumValue, string label, bool invertMaxValue = false)
        {
            EditorGUILayout.BeginHorizontal();
            float minimumTmp = min.floatValue;
            float maximumTmp = max.floatValue;
            if(invertMaxValue)
            {
                maximumTmp = 1.0f - maximumTmp;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel(label);
            minimumTmp = Mathf.Clamp01(EditorGUILayout.DelayedFloatField(minimumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20)));
            EditorGUILayout.MinMaxSlider(ref minimumTmp, ref maximumTmp, minimumValue, maximumValue, GUILayout.MinWidth(5));
            maximumTmp = Mathf.Max(minimumTmp, Mathf.Clamp01(EditorGUILayout.DelayedFloatField(maximumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20))));

            if(EditorGUI.EndChangeCheck())
            {
                Event e = Event.current;
                if(e.control)
                {
                    minimumTmp = minimumTmp.Snap(0.125f);
                    maximumTmp = maximumTmp.Snap(0.125f);
                }

                if(invertMaxValue)
                {
                    maximumTmp = 1.0f - maximumTmp;
                }

                min.floatValue = minimumTmp;
                max.floatValue = maximumTmp;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a float slider
        /// </summary>
        /// <param name="value">The related serialized property</param>
        /// <param name="minimumValue">The minimum possible value</param>
        /// <param name="maximumValue">The maximum possible value</param>
        /// <param name="label">The label to write</param>
        /// <param name="invertOrientation">One minus value</param>
        public static void DrawSlider(ref SerializedProperty value, float minimumValue, float maximumValue, string label, bool invertOrientation = false)
        {
            EditorGUILayout.BeginHorizontal();
            float tmp = value.floatValue;
            if(invertOrientation)
            {
                tmp = 1.0f - tmp;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel(label);
            tmp = EditorGUILayout.Slider(tmp, minimumValue, maximumValue, GUILayout.MinWidth(5));
            if(EditorGUI.EndChangeCheck())
            {
                Event e = Event.current;
                if(e.control)
                {
                    tmp = tmp.Snap(0.125f);
                }

                if(invertOrientation)
                {
                    tmp = 1.0f - tmp;
                }

                value.floatValue = tmp;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an icon in an area
        /// </summary>
        /// <param name="rect">The screen rectangle</param>
        /// <param name="texture">The icon to draw</param>
        public static void DrawAreaIcon(Rect rect, Texture2D texture)
        {
            Rect volumeShapeTextureRect = rect;
            volumeShapeTextureRect.x += rect.width / 2 - texture.width / 2;
            volumeShapeTextureRect.y += 4;
            volumeShapeTextureRect.width = texture.width;
            volumeShapeTextureRect.height = texture.height;
            GUI.DrawTexture(volumeShapeTextureRect, texture, ScaleMode.ScaleToFit, true);
            GUILayout.Space(volumeShapeTextureRect.y + texture.height + 4);
        }

        /// <summary>
        /// Draws a helpbox when CTRL+ALT are held over the inspector
        /// </summary>
        /// <param name="message">The message to display in the helpbox</param>
        public static void DrawContextualHelpBox(string message)
        {
            if(Event.current.alt && Event.current.control)
            {
                EditorGUILayout.HelpBox("\n" + message + "\n", MessageType.Info);
            }
        }

        /// <summary>
        /// Displays the tip to show the contextual help box
        /// </summary>
        public static void DisplayHelpToShowHelpBox()
        {
            EditorGUILayout.LabelField("Hold CTRL+ALT to show help boxes.", EditorStyles.centeredGreyMiniLabel);
        }

        /// <summary>
        /// Displays a general helpbox
        /// </summary>
        public static void DisplayGeneralHelpBox()
        {
            //GuiHelpers.DrawContextualHelpBox("General Help :\n\n"
            //                                                    + " - Blahblahblah.\n\n"
            //                                                    + " - Blahblahblah as well.");
        }
    }
}
