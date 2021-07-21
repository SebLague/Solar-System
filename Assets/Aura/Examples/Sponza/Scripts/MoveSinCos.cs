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
    public class MoveSinCos : MonoBehaviour
    {
        private Vector3 _initialPosition;
        public float cosAmplitude; // = Random.Range(0.5f, 2.0f);

        public Vector3 cosDirection = Vector3.right;
        public float cosOffset; // = Random.Range(-Mathf.PI, Mathf.PI);
        public float cosSpeed; // = Random.Range(2.0f, 3.5f);
        public float sinAmplitude; // = Random.Range(0.5f, 2.0f);
        public Vector3 sinDirection = Vector3.up;
        public float sinOffset; // = Random.Range(-Mathf.PI, Mathf.PI);
        public float sinSpeed; // = Random.Range(2.0f, 3.5f);

        public Space space = Space.Self;

        private void Start()
        {
            _initialPosition = transform.position;
        }

        private void Update()
        {
            Vector3 sinVector = sinDirection.normalized * Mathf.Sin(Time.time * sinSpeed + sinOffset) * sinAmplitude;
            Vector3 cosVector = cosDirection.normalized * Mathf.Cos(Time.time * cosSpeed + cosOffset) * cosAmplitude;

            sinVector = space == Space.World ? sinVector : transform.localToWorldMatrix.MultiplyVector(sinVector);
            cosVector = space == Space.World ? cosVector : transform.localToWorldMatrix.MultiplyVector(cosVector);

            transform.position = _initialPosition + sinVector + cosVector;
        }
    }
}
