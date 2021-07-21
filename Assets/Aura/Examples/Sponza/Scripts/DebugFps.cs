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
    /// Computes the average FPS over a specified duration
    /// </summary>
    public class DebugFps : MonoBehaviour
    {
        #region Public Members
        /// <summary>
        /// The duration while the average FPS will be computed
        /// </summary>
        public float interval = 1;
        #endregion

        #region Private Members
        /// <summary>
        /// The accumulated FPS since the last timeout
        /// </summary>
        private float _accumulationValue;
        /// <summary>
        /// The amount of frames since the last timeout
        /// </summary>
        private int _framesCount;
        /// <summary>
        /// The time of the the last timeout
        /// </summary>
        private float _timestamp;
        /// <summary>
        /// The raw FPS (1/deltaTime)
        /// </summary>
        private float _rawFps;
        /// <summary>
        /// The last computed mean FPS
        /// </summary>
        private float _meanFps;
        #endregion

        #region Overriden base class functions (https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)
        private void Update()
        {
            if(Time.time - _timestamp > interval)
            {
                _meanFps = _accumulationValue / _framesCount;
                _timestamp = Time.time;
                _framesCount = 0;
                _accumulationValue = 0;
            }

            ++_framesCount;
            _rawFps = 1.0f / Time.deltaTime;
            _accumulationValue += _rawFps;
        }

        private void OnGUI()
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Mean FPS over " + interval + " second(s) = " + _meanFps + "\nRaw FPS = " + _rawFps);
        }
        #endregion
    }
}
