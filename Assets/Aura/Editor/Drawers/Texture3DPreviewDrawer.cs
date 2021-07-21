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
    /// Custom drawer for Texture3DPreview attribute
    /// </summary>
    [CustomPropertyDrawer(typeof(Texture3DPreviewAttribute))]
    internal sealed class Texture3DPreviewDrawer : PropertyDrawer
    {
        #region Private Members
        /// <summary>
        /// Maximum preview size in the Inspector
        /// </summary>
        private const int _maxSize = 512;
        /// <summary>
        /// The material which the preview will be rendered with
        /// </summary>
        private static Material _material;
        /// <summary>
        /// String to show if the field is not a Texture3D
        /// </summary>
        private const string _notObjectOrTexture3DString = "Texture3DPreview attribute can only be used with Texture3D fields.";
        /// <summary>
        /// The angle of the camera preview
        /// </summary>
        private Vector2 _cameraAngle = new Vector2(127.5f, -22.5f);
        /// <summary>
        /// The raymarch interations
        /// </summary>
        private int _samplingIterations = 64;
        /// <summary>
        /// The factor of the Texture3D
        /// </summary>
        private float _density = 1;
        #endregion

        #region Functions
        /// <summary>
        /// Gets if the Texture3D should be displayed
        /// </summary>
        private bool ShowField
        {
            get
            {
                return ((Texture3DPreviewAttribute)attribute).showField;
            }
        }

        /// <summary>
        /// Checks if the drawn property is a UnityEngine.Object derived type
        /// </summary>
        /// <param name="property">The drawn object</param>
        /// <returns>True if the drawn object is a UnityEngine.Object derived type, false otherwise</returns>
        private bool CheckIfObjectType(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        /// <summary>
        /// Checks if the drawn property is a Texture3D
        /// </summary>
        /// <param name="property">The drawn object</param>
        /// <returns>True if the drawn object is a Texture3D, false otherwise</returns>
        private bool CheckIfTexture3D(SerializedProperty property)
        {
            return property.objectReferenceValue is Texture3D;
        }

        /// <summary>
        /// Checks if the drawn property is null
        /// </summary>
        /// <param name="property">The drawn object</param>
        /// <returns>True if the drawn object is null, false otherwise</returns>
        private bool CheckIfNull(SerializedProperty property)
        {
            return property.objectReferenceValue == null;
        }

        /// <summary>
        /// Computes the size of the preview accordingly to the Inspector width
        /// </summary>
        /// <param name="rightMargin">The margin reserved on the right of the preview</param>
        /// <returns>The size of the preview</returns>
        private int ComputePreviewSize(int rightMargin)
        {
            int size = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth) - rightMargin;
            size = Mathf.Min(size, Texture3DPreviewDrawer._maxSize);

            return size;
        }

        /// <summary>
        /// Sets back the camera angle
        /// </summary>
        public void ResetPreviewCameraAngle()
        {
            _cameraAngle = new Vector2(127.5f, -22.5f);
        }
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/PropertyDrawer.html)
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float size = 0;

            if(CheckIfObjectType(property))
            {
                if(ShowField)
                {
                    size += EditorGUIUtility.singleLineHeight;
                }

                if(!CheckIfNull(property))
                {
                    if(CheckIfTexture3D(property))
                    {
                        size += EditorGUIUtility.singleLineHeight;
                        size += ComputePreviewSize((int)EditorGUIUtility.singleLineHeight);
                        size += EditorGUIUtility.singleLineHeight;
                    }
                    else
                    {
                        size += EditorGUIUtility.singleLineHeight;
                    }
                }
                else
                {
                    size += EditorGUIUtility.singleLineHeight * 3;
                }
            }
            else
            {
                size += EditorGUIUtility.singleLineHeight;
            }

            return size;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(CheckIfObjectType(property))
            {
                if(ShowField)
                {
                    Rect rect = position;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.ObjectField(rect, property, typeof(Texture3D));
                    position.y += EditorGUIUtility.singleLineHeight;
                }

                if(!CheckIfNull(property))
                {
                    if(CheckIfTexture3D(property))
                    {
                        if(Event.current.type == EventType.Layout)
                        {
                            return;
                        }

                        position.y += EditorGUIUtility.singleLineHeight;

                        int size = ComputePreviewSize((int)position.x * 2);
                        Rect drawArea = position;
                        drawArea.width = size;
                        drawArea.height = drawArea.width;
                        drawArea.x += position.width * 0.5f - drawArea.width * 0.5f;

                        _cameraAngle = PreviewRenderUtilityHelpers.DragToAngles(_cameraAngle, drawArea);
                        if(Event.current.type == EventType.Repaint)
                        {
                            GUI.Box(drawArea, ((Texture3D)property.objectReferenceValue).RenderTexture3DPreview(drawArea, _cameraAngle, 6.5f /*TODO : Find distance with fov and boundingsphere, when non uniform size will be supported*/, _samplingIterations, _density));
                        }

                        Rect rect = drawArea;
                        rect.y += drawArea.height;
                        rect.height = EditorGUIUtility.singleLineHeight;
                        rect.width = 100; // TODO : not use magic numbers
                        if(GUI.Button(rect, "Reset Camera", EditorStyles.miniButton))
                        {
                            ResetPreviewCameraAngle();
                        }
                        rect.x += rect.width;
                        rect.width = 60; // TODO : not use magic numbers
                        EditorGUI.LabelField(rect, "Quality :");
                        rect.x += rect.width;
                        _samplingIterations = EditorGUI.IntPopup(rect, _samplingIterations, new[]
                                                                                            {
                                                                                                "16",
                                                                                                "32",
                                                                                                "64",
                                                                                                "128",
                                                                                                "256",
                                                                                                "512"
                                                                                            }, new[]
                                                                                               {
                                                                                                   16,
                                                                                                   32,
                                                                                                   64,
                                                                                                   128,
                                                                                                   256,
                                                                                                   512
                                                                                               });
                        rect.x += rect.width;
                        rect.width = 60; // TODO : not use magic numbers
                        EditorGUI.LabelField(rect, "Density :");
                        rect.x += rect.width;
                        rect.width = drawArea.width - 280; // TODO : not use magic numbers
                        _density = EditorGUI.Slider(rect, _density, 0, 5);
                    }
                    else
                    {
                        EditorGUI.LabelField(position, Texture3DPreviewDrawer._notObjectOrTexture3DString, EditorStyles.boldLabel);
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, "Texture3D field is empty.", EditorStyles.centeredGreyMiniLabel);
                }
            }
            else
            {
                EditorGUI.LabelField(position, Texture3DPreviewDrawer._notObjectOrTexture3DString, EditorStyles.boldLabel);
            }
        }
        #endregion
    }
}
