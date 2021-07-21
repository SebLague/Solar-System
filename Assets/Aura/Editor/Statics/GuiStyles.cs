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
    /// Collection of custom GuiStyles
    /// </summary>
    public static class GuiStyles
    {
        #region Public Members
        public static GUIStyle areaTitleBarStyle;
        public static GUIStyle topCenteredMiniGreyLabel;
        #endregion

        #region Static Constructor
        /// <summary>
        /// Static constructor
        /// </summary>
        static GuiStyles()
        {
            GuiStyles.areaTitleBarStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                                          {
                                              normal =
                                              {
                                                  textColor = Color.white
                                              },
                                              fontSize = 11
                                          };

            GuiStyles.topCenteredMiniGreyLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                                                 {
                                                     alignment = TextAnchor.UpperCenter
                                                 };
        }
        #endregion
    }
}