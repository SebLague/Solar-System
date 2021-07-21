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
    /// Collection of static functions for computing a camera's frustum corners
    /// </summary>
    internal static class FrustumCorners
    {
        #region Private Members
        /// <summary>
        /// Clip space corners' position
        /// </summary>
        private static readonly Vector3[] _frustumClipPos =
        {
            new Vector3(-1, 1, -1), // A
            new Vector3(1, 1, -1), // B
            new Vector3(1, -1, -1), // C
            new Vector3(-1, -1, -1), // D
            new Vector3(-1, 1, 1), // E
            new Vector3(1, 1, 1), // F
            new Vector3(1, -1, 1), // G
            new Vector3(-1, -1, 1) // H
        };
        /*

                            E_______________F
                           /|		    _/	|
                          /	|		  _/	|
                         /  |	    _/		|	FAR
                        /	|	  _/		|
                       /	H____/__________G
                      A____/__B        __/
                      |	 _/	  |     __/
            NEAR	  |_/	  |  __/
                      D_______C_/

        */
        #endregion

        #region Functions
        /// <summary>
        /// Computes the world position of the corners of the frustum
        /// </summary>
        /// <param name="frustumClipToCameraInverseProjMatrix">The clip to camera inverse projection matrix</param>
        /// <returns>An array containing the world position of the corners of the frustum</returns>
        public static Vector3[] GetFrustumCornersWorldPosition(Matrix4x4 frustumClipToCameraInverseProjMatrix)
        {
            Vector3[] frustumWorldPos = new Vector3[8];

            for(int i = 0; i < 8; ++i)
            {
                frustumWorldPos[i] = frustumClipToCameraInverseProjMatrix.MultiplyPoint(FrustumCorners._frustumClipPos[i]);
            }

            return frustumWorldPos;
        }

        /// <summary>
        /// Computes the size of the camera plane at the specified distance
        /// </summary>
        /// <param name="fov">The field of view of the camera</param>
        /// <param name="distance">The distance</param>
        /// <returns>The size of the camera plane at the specified distance</returns>
        public static float GetCameraSizeAtDistance(float fov, float distance)
        {
            return distance * Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f);
        }

        /// <summary>
        /// Computes the world position of the corners of the frustum
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="nearClipPlaneDistance">The near plane</param>
        /// <param name="farClipPlaneDistance">The far plane</param>
        /// <returns>An array containing the world position of the corners of the frustum</returns>
        public static Vector3[] GetFrustumCornersWorldPosition(Camera camera, float nearClipPlaneDistance, float farClipPlaneDistance)
        {
            Vector3[] frustumWorldPos = new Vector3[8];

            float nearY = FrustumCorners.GetCameraSizeAtDistance(camera.fieldOfView, nearClipPlaneDistance);
            float nearX = nearY * camera.aspect;
            float farY = FrustumCorners.GetCameraSizeAtDistance(camera.fieldOfView, farClipPlaneDistance);
            float farX = farY * camera.aspect;

            Matrix4x4 matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, Vector3.one);

            for(int i = 0; i < 8; ++i)
            {
                Vector3 pos = FrustumCorners._frustumClipPos[i];
                if(pos.z < 0)
                {
                    pos.x *= nearX;
                    pos.y *= nearY;
                    pos.z = nearClipPlaneDistance;
                }
                else
                {
                    pos.x *= farX;
                    pos.y *= farY;
                    pos.z = farClipPlaneDistance;
                }

                frustumWorldPos[i] = matrix.MultiplyPoint3x4(pos);
            }

            return frustumWorldPos;
        }

        /// <summary>
        /// Computes the world position of the corners of the frustum
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="farClipPlaneDistance">The far plane</param>
        /// <returns>An array containing the world position of the corners of the frustum</returns>
        public static Vector3[] GetFrustumCornersWorldPosition(Camera camera, float farClipPlaneDistance)
        {
            return new Vector3[8]
                   {
                       camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane)),
                       camera.ViewportToWorldPoint(new Vector3(0, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 1, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(1, 0, farClipPlaneDistance)),
                       camera.ViewportToWorldPoint(new Vector3(0, 0, farClipPlaneDistance))
                   };
        }

        /// <summary>
        /// Returns the camera's Clip space to World space matrix
        /// </summary>
        /// <param name="cameraComponent">The camera</param>
        /// <param name="farClipPlane">The far clip plane</param>
        /// <returns>The camera's Clip space to World space matrix</returns>
        public static Matrix4x4 GetClipToWorldMatrix(Camera cameraComponent, float farClipPlane)
        {
            float tmp = cameraComponent.farClipPlane;
            cameraComponent.farClipPlane = farClipPlane;

            Matrix4x4 invProjMatrix = cameraComponent.projectionMatrix.inverse;
            Matrix4x4 cameraToWorldMatrix = cameraComponent.cameraToWorldMatrix;
            Matrix4x4 clipToWorldMatrix = cameraToWorldMatrix * invProjMatrix;

            cameraComponent.farClipPlane = tmp;

            return clipToWorldMatrix;
        }

        /// <summary>
        /// Returns the camera's World space to Clip space matrix
        /// </summary>
        /// <param name="cameraComponent">The camera</param>
        /// <param name="farClipPlane">The far clip plane</param>
        /// <returns>The camera's World space to Clip space matrix</returns>
        public static Matrix4x4 GetWorldToClipMatrix(Camera cameraComponent, float farClipPlane)
        {
            float tmp = cameraComponent.farClipPlane;
            cameraComponent.farClipPlane = farClipPlane;

            Matrix4x4 worldToCameraMatrix = cameraComponent.worldToCameraMatrix;
            Matrix4x4 projectionMatrix = cameraComponent.projectionMatrix;
            Matrix4x4 worldToClipMatrix = projectionMatrix * worldToCameraMatrix;

            cameraComponent.farClipPlane = tmp;

            return worldToClipMatrix;
        }
        #endregion
    }
}
