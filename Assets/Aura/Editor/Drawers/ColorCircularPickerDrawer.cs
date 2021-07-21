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
    /// Circular picker drawer attribute for Color properties
    /// </summary>
    [CustomPropertyDrawer(typeof(ColorCircularPickerAttribute))]
    internal sealed class ColorCircularPickerDrawer : PropertyDrawer
    {
        #region Private Members
        /// <summary>
        /// Maximum pixel size for the drawer
        /// </summary>
        private const int _maxSize = 512;
        /// <summary>
        /// The material used for drawing the picker
        /// </summary>
        private static Material _material;
        /// <summary>
        /// Temporary color to stock when color is pasted
        /// </summary>
        private Color _pastedColor;
        /// <summary>
        /// Temporary bool to tell if there was a color pasted
        /// </summary>
        private bool _wasColorPasted;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/PropertyDrawer.html)
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType == SerializedPropertyType.Color)
            {
                if(Event.current.type == EventType.Layout)
                {
                    return;
                }

                if(ColorCircularPickerDrawer._material == null)
                {
                    ColorCircularPickerDrawer._material = new Material(Shader.Find("Hidden/Aura/GUI/DrawCircularPickerDisk"))
                                                         {
                                                             hideFlags = HideFlags.HideAndDontSave
                                                         };
                }

                if(((ColorCircularPickerAttribute)attribute).showLabel)
                {
                    EditorGUI.LabelField(position, label, GuiStyles.topCenteredMiniGreyLabel);
                    position.y += 8;
                }

                int size = GetSize((int)position.x * 2);

                Rect drawArea = position;
                drawArea.width = size;
                drawArea.height = drawArea.width;
                drawArea.x += position.width * 0.5f - drawArea.width * 0.5f;

                const float colorDiskSize = 0.45f;
                ColorCircularPickerDrawer._material.SetFloat("colorDiskSize", colorDiskSize);

                Vector3 hsv;
                Color.RGBToHSV(property.colorValue, out hsv.x, out hsv.y, out hsv.z);
                hsv = GetInput(drawArea, hsv, colorDiskSize);
                if(_wasColorPasted)
                {
                    property.colorValue = _pastedColor;
                    Color.RGBToHSV(property.colorValue, out hsv.x, out hsv.y, out hsv.z);
                    _wasColorPasted = false;
                }
                Rect sliderRect = drawArea;
                sliderRect.y += sliderRect.height;
                sliderRect.height = EditorGUIUtility.singleLineHeight;
                GUI.Label(sliderRect, "Hue", EditorStyles.centeredGreyMiniLabel);
                sliderRect.y += EditorGUIUtility.singleLineHeight;
                hsv.x = GUI.HorizontalSlider(sliderRect, hsv.x, 0, 0.999999f);
                sliderRect.y += EditorGUIUtility.singleLineHeight * 2;
                GUI.Label(sliderRect, "Saturation", EditorStyles.centeredGreyMiniLabel);
                sliderRect.y += EditorGUIUtility.singleLineHeight;
                hsv.y = GUI.HorizontalSlider(sliderRect, hsv.y, 0.000001f, 1);

                property.colorValue = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);

                if(Event.current.type == EventType.Repaint)
                {
                    Vector2 thumbPos = Vector2.zero;
                    float theta = hsv.x * (Mathf.PI * 2f);
                    float len = hsv.y * colorDiskSize;
                    thumbPos.x = Mathf.Cos(theta + Mathf.PI / 2f);
                    thumbPos.y = -Mathf.Sin(theta - Mathf.PI / 2f);
                    thumbPos *= len;
                    thumbPos += Vector2.one * 0.5f;
                    ColorCircularPickerDrawer._material.SetVector("pickerCoordinates", thumbPos);

                    ColorCircularPickerDrawer._material.SetFloat("alpha", GUI.enabled ? 1f : 0.333f);

                    RenderTexture tmpRenderTarget = RenderTexture.active;
                    RenderTexture renderTarget = RenderTexture.GetTemporary((int)(size * EditorGUIUtility.pixelsPerPoint), (int)(size * EditorGUIUtility.pixelsPerPoint), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                    Graphics.Blit(null, renderTarget, ColorCircularPickerDrawer._material, EditorGUIUtility.isProSkin ? 0 : 1);
                    RenderTexture.active = tmpRenderTarget;
                    GUI.DrawTexture(drawArea, renderTarget);
                    RenderTexture.ReleaseTemporary(renderTarget);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (((ColorCircularPickerAttribute)attribute).showLabel ? EditorGUIUtility.singleLineHeight : 0) + GetSize((int)EditorGUIUtility.singleLineHeight * 3) + (((ColorCircularPickerAttribute)attribute).showLabel ? EditorGUIUtility.singleLineHeight : 0) + (int)EditorGUIUtility.singleLineHeight * 7;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Computes the width based on a given margin
        /// </summary>
        /// <param name="rightMargin">The margin on the right</param>
        /// <returns></returns>
        private int GetSize(int rightMargin)
        {
            int size = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth) - rightMargin;
            size = Mathf.Min(size, ColorCircularPickerDrawer._maxSize);

            return size;
        }

        /// <summary>
        /// Computes the Hue and Saturation values based on a position on a disk
        /// </summary>
        /// <param name="x">The horizontal position</param>
        /// <param name="y">The vertical position</param>
        /// <param name="radius">The radius of the disk</param>
        /// <param name="hue">The output hue (based on the angle difference from the point-center vector and the horizontal)</param>
        /// <param name="saturation">The saturation (based on the normalized distance of the point from the center)</param>
        private void GetHueSaturation(float x, float y, float radius, out float hue, out float saturation)
        {
            hue = Mathf.Atan2(x, -y);
            hue = 1f - (hue > 0 ? hue : Mathf.PI * 2f + hue) / (Mathf.PI * 2f);
            Vector2 scaledPos = new Vector2(x, y);
            saturation = Mathf.Clamp01(scaledPos.magnitude / radius);
        }

        /// <summary>
        /// Watches for mouse interaction with the drawer
        /// </summary>
        /// <param name="bounds">The rectangle on screen to watch</param>
        /// <param name="hsv">The current HSV color</param>
        /// <param name="radius">The raduis of the picker's disk</param>
        /// <returns></returns>
        private Vector3 GetInput(Rect bounds, Vector3 hsv, float radius)
        {
            Event e = Event.current;
            int id = GUIUtility.GetControlID("ColorCircularPickerDrawer".GetHashCode(), FocusType.Passive, bounds);

            Vector2 mousePos = e.mousePosition;
            Vector2 relativePos = mousePos - new Vector2(bounds.center.x, bounds.center.y);
            relativePos.x /= bounds.width;
            relativePos.y /= bounds.height;

            if(e.type == EventType.MouseDown && GUIUtility.hotControl == 0 && bounds.Contains(mousePos))
            {
                switch(e.button)
                {
                    case 0 :
                        if(relativePos.magnitude <= radius)
                        {
                            e.Use();
                            GetHueSaturation(relativePos.x, relativePos.y, radius, out hsv.x, out hsv.y);
                            GUIUtility.hotControl = id;
                            GUI.changed = true;
                        }
                        break;
                    case 1 :
                        GenericMenu copyPasteMenu = new GenericMenu();
                        copyPasteMenu.AddItem(new GUIContent("Copy color value"), false, CopyColorValue, hsv);
                        copyPasteMenu.AddSeparator("");
                        if(ColorUtility.TryParseHtmlString(GUIUtility.systemCopyBuffer, out _pastedColor))
                        {
                            copyPasteMenu.AddItem(new GUIContent("Paste color value"), false, PasteColorValue);
                        }
                        else
                        {
                            copyPasteMenu.AddDisabledItem(new GUIContent("No color value to paste"));
                        }
                        copyPasteMenu.ShowAsContext();

                        GUI.changed = true;
                        e.Use();
                        break;
                }
            }
            else if(e.type == EventType.MouseDrag && e.button == 0 && GUIUtility.hotControl == id)
            {
                e.Use();
                GUI.changed = true;
                GetHueSaturation(relativePos.x, relativePos.y, radius, out hsv.x, out hsv.y);
            }
            else if(e.rawType == EventType.MouseUp && e.button == 0 && GUIUtility.hotControl == id)
            {
                e.Use();
                GUIUtility.hotControl = 0;
            }

            return hsv;
        }

        /// <summary>
        /// Stores the HSV color in the clipboard in the #hex format
        /// </summary>
        /// <param name="hsv">The HSV color (Vector3)</param>
        private void CopyColorValue(object hsv)
        {
            Vector3 hsvValue = (Vector3)hsv;
            Color colorFromHsv = Color.HSVToRGB(hsvValue.x, hsvValue.y, hsvValue.z);
            GUIUtility.systemCopyBuffer = "#" + ColorUtility.ToHtmlStringRGB(colorFromHsv);
        }

        /// <summary>
        /// Retrieves the color from the clipboard and sets the toggle so the current HSV will be replaced on next update
        /// </summary>
        private void PasteColorValue()
        {
            ColorUtility.TryParseHtmlString(GUIUtility.systemCopyBuffer, out _pastedColor);
            _wasColorPasted = true;
        }
        #endregion
    }
}
