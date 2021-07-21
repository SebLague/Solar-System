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

using System;
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Used for transforming reference position for textures/noise inside volumes
    /// </summary>
    [Serializable]
    public struct TransformParameters
    {
        #region Public Members
        /// <summary>
        /// Referential to use for transformations and animations
        /// </summary>
        public Space space;
        /// <summary>
        /// Position of the volume in the selected referencial
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Rotation of the volume in the selected referencial
        /// </summary>
        public Vector3 rotation;
        /// <summary>
        /// Scale of the volume in the selected referencial
        /// </summary>
        public Vector3 scale;
        /// <summary>
        /// Animate position of the volume in the selected referencial?
        /// </summary>
        public bool animatePosition;
        /// <summary>
        /// Speed of the position offset (in meters per second)
        /// </summary>
        public Vector3 positionSpeed;
        /// <summary>
        /// Animate rotation of the volume in the selected referencial?
        /// </summary>
        public bool animateRotation;
        /// <summary>
        /// Speed of the rotation offset (in degrees per second)
        /// </summary>
        public Vector3 rotationSpeed;
        #endregion

        #region Private Members
        /// <summary>
        /// Timestamp for matrix update
        /// </summary>
        private float _timeStamp;
        #endregion

        #region Functions
        /// <summary>
        /// The resulting transformation matrix
        /// </summary>
        public Matrix4x4 Matrix
        {
            get
            {
                float deltaTime = Aura.Time - _timeStamp;
                _timeStamp = Aura.Time;

                if(animatePosition)
                {
                    position += positionSpeed * deltaTime;
                }
                if(animateRotation)
                {
                    rotation += rotationSpeed * deltaTime;
                    rotation.x = rotation.x % 360.0f;
                    rotation.y = rotation.y % 360.0f;
                    rotation.z = rotation.z % 360.0f;
                }

                return Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale).inverse;
            }
        }
        #endregion
    }
}
