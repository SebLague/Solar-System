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

namespace AuraAPI
{
    /// <summary>
    /// Attribute enabling Texture3D preview in a Monobehaviour component in the Inspector
    /// </summary>
    public class Texture3DPreviewAttribute : PropertyAttribute
    {
        #region Members
        /// <summary>
        /// Allows to show the field (when user drags/drops another Texture3D for example) or not (when Texture3D is changed in code only for example)
        /// </summary>
        public readonly bool showField;
        #endregion

        #region Constructor
        /// <summary>
        /// Declare this attribute in front of the Texture3D field to enable previewing in a Monobehaviour component in the Inspector
        /// </summary>
        /// <param name="showField">Shows or not the Texture3D field on top of the preview (default = true)</param>
        public Texture3DPreviewAttribute(bool showField = true)
        {
            this.showField = showField;
        }
        #endregion
    }
}
