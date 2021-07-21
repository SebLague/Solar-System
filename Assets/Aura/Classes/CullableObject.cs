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
    /// Class to inherit from when using an ObjectCuller
    /// </summary>
    public abstract class CullableObject : MonoBehaviour
    {
        #region Private Members
        /// <summary>
        /// The bounding sphere used to cull with the camera
        /// </summary>
        private BoundingSphere _boundingSphereMember;
        #endregion

        #region Functions
        /// <summary>
        /// Accessor to the bounding sphere used to cull with the camera
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get
            {
                return _boundingSphereMember;
            }
        }

        /// <summary>
        /// Updates the bounding sphere used to cull with the camera
        /// </summary>
        /// <param name="position">The new postition</param>
        /// <param name="radius">The new radius</param>
        public void UpdateBoundingSphere(Vector3 position, float radius)
        {
            _boundingSphereMember.position = position;
            _boundingSphereMember.radius = radius;
        }

        /// <summary>
        /// Updates the bounding sphere used to cull with the camera
        /// </summary>
        /// <param name="boundingSphere">The reference bounding sphere</param>
        public void UpdateBoundingSphere(BoundingSphere boundingSphere)
        {
            _boundingSphereMember = boundingSphere;
        }
        #endregion
    }
}
