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
    /// Static class containing extension for Camera type
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Computes the world position of the frustum corners with custom near/far clip distances
        /// </summary>
        /// <param name="nearClipPlaneDistance">The near clip distance</param>
        /// <param name="farClipPlaneDistance">The far clip distance</param>
        /// <returns>
        /// An array with the world position of the corners in the following order : nearTopLeft, nearTopRight, nearBottomRight, nearBottomLeft, farTopLeft, farTopRight, farBottomRight, farBottomLeft
        /// </returns>
        public static Vector4[] GetFrustumCornersWorldPosition(this Camera camera, float nearClipPlaneDistance, float farClipPlaneDistance)
        {
            return new Vector4[]
                   {
                       camera.ViewportToWorldPoint(new Vector3(0, 1, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, nearClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, farClipPlaneDistance))
                   };
        }
    }
}
