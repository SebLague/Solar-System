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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AuraAPI
{
    //[ExecuteInEditMode]
    public class LightFlicker : MonoBehaviour
    {
        private float _currentFactor = 1.0f;
        private Vector3 _currentPos;
        private float _deltaTime;
        private Vector3 _initPos;
        private float _targetFactor;
        private Vector3 _targetPos;

        private float _time;
        private float _timeLeft;
        public Color baseColor;
        public float maxFactor = 1.2f;
        public float minFactor = 1.0f;
        public float moveRange = 0.1f;
        public float speed = 0.1f;

        private void Start()
        {
            Random.InitState((int)transform.position.x + (int)transform.position.y);
        }

        //

        private void OnEnable()
        {
            _initPos = transform.localPosition;
            _currentPos = _initPos;
        }

        //

        private void OnDisable()
        {
            transform.localPosition = _initPos;
        }

        //

#if !UNITY_EDITOR
    private void Update()
    {
        _deltaTime = Time.deltaTime;
#else
        void OnRenderObject()
        {
            float currentTime = (float)EditorApplication.timeSinceStartup;
            _deltaTime = currentTime - _time;
            _time = currentTime;
#endif

            if (_timeLeft <= _deltaTime)
            {
                _targetFactor = Random.Range(minFactor, maxFactor);
                _targetPos = _initPos + Random.insideUnitSphere * moveRange;
                _timeLeft = speed;
            }
            else
            {
                float weight = _deltaTime / _timeLeft;
                _currentFactor = Mathf.Lerp(_currentFactor, _targetFactor, weight);

                //GetComponent<AuraLight>().overridingColor = baseColor * _currentFactor;
                GetComponent<Light>().color = baseColor * _currentFactor;
                _currentPos = Vector3.Lerp(_currentPos, _targetPos, weight);
                transform.localPosition = _currentPos;
                _timeLeft -= _deltaTime;
            }
        }
    }
}
