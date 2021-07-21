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
    /// Tool for generating Texture3D out of a Texture2D containing slices
    /// </summary>
    public class CreateTexture3DAssetTool : EditorWindow
    {
        #region Public Members
        /// <summary>
        /// The logo to display
        /// </summary>
        public Texture2D logoTexture;
        /// <summary>
        /// Texture3D icon
        /// </summary>
        public Texture2D texture3DIcon;
        #endregion

        #region Private Members
        /// <summary>
        /// The Texture2D used as source
        /// </summary>
        private Texture2D _sourceTexture;
        /// <summary>
        /// The reference size used to cut the source texture and build the Texture3D
        /// </summary>
        private int _referenceSize;
        /// <summary>
        /// The order of reading the slices in the source texture
        /// </summary>
        private Texture3DReadingOrderEnum _readingOrder;
        /// <summary>
        /// The path of the source texture asset
        /// </summary>
        private string _sourceTexturePath;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/EditorWindow.html)
        [MenuItem("Tools/Aura/Create Texture3D Asset")]
        private static void Init()
        {
            CreateTexture3DAssetTool window = (CreateTexture3DAssetTool)EditorWindow.GetWindow(typeof(CreateTexture3DAssetTool));
            window.titleContent.text = "Create Texture3D Asset";
            window.Show();
        }

        private void OnGUI()
        {
            GuiHelpers.DrawHeader(logoTexture);

            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.miniButton);
            GUILayout.Button(new GUIContent(" Create Texture3D Asset", texture3DIcon), GuiStyles.areaTitleBarStyle);
            GuiHelpers.DrawContextualHelpBox("Create a composite Texture3D from a Texture2D.");

            EditorGUILayout.Separator();

            GuiHelpers.DrawContextualHelpBox("Source texture used to build the Texture3D.");
            EditorGUILayout.LabelField("Source Texture");
            _sourceTexture = (Texture2D)EditorGUILayout.ObjectField(_sourceTexture, typeof(Texture2D), false);

            EditorGUILayout.Separator();

            if(_sourceTexture != null)
            {
                _sourceTexturePath = AssetDatabase.GetAssetPath(_sourceTexture);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(_sourceTexturePath);

                if(textureImporter.isReadable)
                {
                    EditorGUILayout.Separator();

                    GuiHelpers.DrawContextualHelpBox("Target size of the 3D texture.");
                    _referenceSize = EditorGUILayout.DelayedIntField("Reference Size", _referenceSize);
                    _referenceSize = Mathf.Max(_referenceSize, 16);
                    _referenceSize = Mathf.ClosestPowerOfTwo(_referenceSize);

                    EditorGUILayout.Separator();

                    EditorGUILayout.HelpBox("\nHorizontal tiles count : " + HorizontalTilesCount + "\nVertical tiles count : " + VerticalTilesCount + "\n\nTotal amount of tiles : " + TotalTilesCount + "\n", MessageType.Info);

                    EditorGUILayout.Separator();

                    if(AreParametersValid)
                    {
                        GuiHelpers.DrawContextualHelpBox("The order of which the tiles should be assembled to create the 3D texture (should we assemble line by line or column by column?).");
                        EditorGUILayout.LabelField("Reading Order");
                        _readingOrder = (Texture3DReadingOrderEnum)EditorGUILayout.EnumPopup(_readingOrder);

                        EditorGUILayout.Separator();

                        GuiHelpers.DrawContextualHelpBox("Generates a new Texture3D from the source texture.");
                        if(GUILayout.Button(new GUIContent("Generate")))
                        {
                            GenerateVolumetricTexture(_sourceTexture);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("ERROR : the total amount of tiles should be equal to the reference tile size. In this case : " + _referenceSize, MessageType.Error);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("ERROR : the source texture asset should be marked as \"READABLE\".", MessageType.Error);
                    if(GUILayout.Button(new GUIContent("Mark as \"READABLE\"")))
                    {
                        textureImporter.isReadable = true;
                        textureImporter.SaveAndReimport();
                    }
                }

                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();

            GuiHelpers.DisplayHelpToShowHelpBox();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Tells if a source texture was provided
        /// </summary>
        private bool HasSourceTexture
        {
            get
            {
                return _sourceTexture != null;
            }
        }

        /// <summary>
        /// Tells the amount of horizontal tiles based on the reference size
        /// </summary>
        public int HorizontalTilesCount
        {
            get
            {
                return HasSourceTexture ? _sourceTexture.width / _referenceSize : 0;
            }
        }

        /// <summary>
        /// Tells the amount of vertical tiles based on the reference size
        /// </summary>
        public int VerticalTilesCount
        {
            get
            {
                return HasSourceTexture ? _sourceTexture.height / _referenceSize : 0;
            }
        }

        /// <summary>
        /// Tells the total amount of tiles based on the reference size
        /// </summary>
        public int TotalTilesCount
        {
            get
            {
                return HasSourceTexture ? HorizontalTilesCount * VerticalTilesCount : 0;
            }
        }

        /// <summary>
        /// tells if the parameters are valid for generating the Texture3D
        /// </summary>
        public bool AreParametersValid
        {
            get
            {
                return HasSourceTexture && /*referenceSize >= 16 &&*/ TotalTilesCount == _referenceSize;
            }
        }

        /// <summary>
        /// Generates the Texture3D
        /// </summary>
        /// <param name="sourceTexture">The source texture asset</param>
        /// <returns>True if parameters were valid, false otherwise</returns>
        public bool GenerateVolumetricTexture(Texture2D sourceTexture)
        {
            if(AreParametersValid)
            {
                Texture3D volumetricTexture = new Texture3D(_referenceSize, _referenceSize, _referenceSize, sourceTexture.format, false);
                volumetricTexture.wrapMode = sourceTexture.wrapMode;
                volumetricTexture.wrapModeU = sourceTexture.wrapModeU;
                volumetricTexture.wrapModeV = sourceTexture.wrapModeV;
                volumetricTexture.wrapModeW = sourceTexture.wrapModeW;
                volumetricTexture.filterMode = sourceTexture.filterMode;
                volumetricTexture.mipMapBias = 0;
                volumetricTexture.anisoLevel = 0;

                Color[] colorArray = new Color[0];
                switch(_readingOrder)
                {
                    case Texture3DReadingOrderEnum.RowMajor :
                        {
                            for(int i = 0; i < HorizontalTilesCount; ++i)
                            {
                                for(int j = 0; j < VerticalTilesCount; ++j)
                                {
                                    colorArray = colorArray.Append(sourceTexture.GetPixels(i * _referenceSize, j * _referenceSize, _referenceSize, _referenceSize));
                                }
                            }
                        }

                        break;

                    case Texture3DReadingOrderEnum.ColumnMajor :
                        {
                            for(int i = VerticalTilesCount - 1; i >= 0; --i)
                            {
                                for(int j = 0; j < HorizontalTilesCount; ++j)
                                {
                                    colorArray = colorArray.Append(sourceTexture.GetPixels(j * _referenceSize, i * _referenceSize, _referenceSize, _referenceSize));
                                }
                            }
                        }

                        break;
                }

                volumetricTexture.SetPixels(colorArray);
                volumetricTexture.Apply();

                AssetDatabase.CreateAsset(volumetricTexture, System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(sourceTexture)) + "\\" + sourceTexture.name + "_Texture3D.asset");

                return true;
            }

            return false;
        }
        #endregion
    }
}
