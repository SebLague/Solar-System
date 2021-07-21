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
    /// Collection of function/variables related to the PreviewRenderUtility class
    /// </summary>
    public static class PreviewRenderUtilityHelpers
    {
        #region Private Members
        /// <summary>
        /// A static copy of the PreviewRenderUtility class
        /// </summary>
        private static PreviewRenderUtility _instance;
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the PreviewRenderUtility instance
        /// </summary>
        public static PreviewRenderUtility Instance
        {
            get
            {
                if(PreviewRenderUtilityHelpers._instance == null)
                {
                    PreviewRenderUtilityHelpers._instance = new PreviewRenderUtility();
                }

                return PreviewRenderUtilityHelpers._instance;
            }
        }

        /// <summary>
        /// Transforms the drag delta of the mouse on the UI into Euler angles
        /// </summary>
        /// <param name="angles">Input angles</param>
        /// <param name="position">The area where the mouse will be watched</param>
        /// <returns>The modified angles</returns>
        public static Vector2 DragToAngles(Vector2 angles, Rect position)
        {
            int controlId = GUIUtility.GetControlID("DragToAngles".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch(current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown :
                    if(position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = controlId;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp :
                    if(GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag :
                    if(GUIUtility.hotControl == controlId)
                    {
                        angles -= current.delta / Mathf.Min(position.width, position.height) * 180;
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }

            return angles;
        }
        #endregion
    }
}
