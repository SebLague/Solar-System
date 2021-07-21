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
    /// Custom Inspector for Texture3D class
    /// </summary>
    [CustomEditor(typeof(Texture3D))]
    public class Texture3DEditor : Editor
    {
        #region Public Members
        /// <summary>
        /// Aura Logo
        /// </summary>
        public Texture2D logoTexture;
        #endregion

        #region Private Members
        /// <summary>
        /// The angle of the camera preview
        /// </summary>
        private Vector2 _cameraAngle = new Vector2(127.5f, -22.5f); // This default value will be used when rendering the asset thumbnail (see RenderStaticPreview)
        /// <summary>
        /// The raymarch interations
        /// </summary>
        private int _samplingIterations = 64;
        /// <summary>
        /// The factor of the Texture3D
        /// </summary>
        private float _density = 1;
        //// TODO : INVESTIGATE TO ACCESS THOSE VARIABLES AS THE DEFAULT INSPECTOR IS UGLY
        //private SerializedProperty wrapModeProperty;
        //private SerializedProperty filterModeProperty;
        //private SerializedProperty anisotropyLevelProperty;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/Editor.html)
        /// <summary>
        /// Draws the content of the Inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            GuiHelpers.DrawHeader(logoTexture);

            //serializedObject.Update();

            //// HAD TO DISABLE THE DEFAULT INSPECTOR AS IT MADE PREVIEW LAG
            //DrawDefaultInspector();

            //serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Tells if the Object has a custom preview
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// Draws the toolbar area on top of the preview window
        /// </summary>
        public override void OnPreviewSettings()
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Reset Camera", EditorStyles.miniButton))
            {
                ResetPreviewCameraAngle();
            }
            EditorGUILayout.LabelField("Quality", GUILayout.MaxWidth(50));
            _samplingIterations = EditorGUILayout.IntPopup(_samplingIterations, new string[]
                                                                                {
                                                                                    "16",
                                                                                    "32",
                                                                                    "64",
                                                                                    "128",
                                                                                    "256",
                                                                                    "512"
                                                                                }, new int[]
                                                                                   {
                                                                                       16,
                                                                                       32,
                                                                                       64,
                                                                                       128,
                                                                                       256,
                                                                                       512
                                                                                   }, GUILayout.MaxWidth(50));
            EditorGUILayout.LabelField("Density", GUILayout.MaxWidth(50));
            _density = EditorGUILayout.Slider(_density, 0, 5, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the preview area
        /// </summary>
        /// <param name="rect">The area of the preview window</param>
        /// <param name="backgroundStyle">The default GUIStyle used for preview windows</param>
        public override void OnPreviewGUI(Rect rect, GUIStyle backgroundStyle)
        {
            _cameraAngle = PreviewRenderUtilityHelpers.DragToAngles(_cameraAngle, rect);

            if(Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(rect, ((Texture3D)serializedObject.targetObject).RenderTexture3DPreview(rect, _cameraAngle, 6.5f /*TODO : Find distance with fov and boundingsphere, when non uniform size will be supported*/, _samplingIterations, _density), ScaleMode.StretchToFill, true);
            }
        }

        /// <summary>
        /// Draws the custom preview thumbnail for the asset in the Project window
        /// </summary>
        /// <param name="assetPath">Path of the asset</param>
        /// <param name="subAssets">Array of children assets</param>
        /// <param name="width">Width of the rendered thumbnail</param>
        /// <param name="height">Height of the rendered thumbnail</param>
        /// <returns></returns>
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return ((Texture3D)serializedObject.targetObject).RenderTexture3DStaticPreview(new Rect(0, 0, width, height), _cameraAngle, 6.5f /*TODO : Find distance with fov and boundingsphere, when non uniform size will be supported*/, _samplingIterations, _density);
        }

        /// <summary>
        /// Allows to give a custom title to the preview window
        /// </summary>
        /// <returns></returns>
        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent(serializedObject.targetObject.name + " preview");
        }
        #endregion

        #region Functions
        /// <summary>
        /// Sets back the camera angle
        /// </summary>
        public void ResetPreviewCameraAngle()
        {
            _cameraAngle = new Vector2(127.5f, -22.5f);
        }
        #endregion
    }
}
