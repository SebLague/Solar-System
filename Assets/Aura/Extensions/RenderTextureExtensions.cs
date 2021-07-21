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

namespace UnityEngine
{
    /// <summary>
    /// Extensions methods for RenderTexture objects
    /// </summary>
    internal static class RenderTextureExtensions
    {
        /// <summary>
        /// Clears the render texture by the specified color.
        /// </summary>
        /// <param name="clearColor">Color used to clear the render texture</param>
        internal static void Clear(this RenderTexture texture, Color clearColor)
        {
            RenderTexture tmp = RenderTexture.active;
            Graphics.SetRenderTarget(texture, 0, CubemapFace.Unknown, -1);
            GL.Clear(true, true, clearColor);
            Graphics.SetRenderTarget(tmp);
        }
    }
}
