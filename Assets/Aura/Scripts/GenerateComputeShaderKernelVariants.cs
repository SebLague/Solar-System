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
using System.IO;
using UnityEngine;

namespace AuraAPI
{
    /// <summary>
    /// Generates all the possible variants for the ComputeDataComputeShader out of the FrustumParametersEnum enum
    /// </summary>
    [ExecuteInEditMode]
    public class GenerateComputeShaderKernelVariants : MonoBehaviour
    {
#if UNITY_EDITOR
        public string kernelName = "ComputeDataBuffer";
        public string destinationFilename = "ComputeDataComputeShader Kernel Variants";
        [Space]
        public bool generateNow;

        private string _outputString;

        private void Update()
        {
            if(generateNow)
            {
                int enumLength = Enum.GetNames(typeof(FrustumParametersEnum)).Length;
                enumLength -= 1; //Because of the EnableNothing = 0

                int maxIteration = 0;
                for(int i = 0; i < enumLength; ++i)
                {
                    maxIteration += 1 << i;
                }

                _outputString = "";
                for(int i = 0; i <= maxIteration; ++i)
                {
                    string tmpString = ((FrustumParametersEnum)i).ToString();

                    _outputString += "// Kernel " + i + "\n";

                    tmpString = tmpString.Replace(",", "");
                    tmpString = tmpString.InsertStringBeforeUpperCaseLetters("_");
                    tmpString = tmpString.ToUpperInvariant();

                    _outputString += "#pragma kernel " + kernelName + " " + tmpString + "\n";
                }

                string filepath = Application.dataPath + "\\" + destinationFilename + ".txt";

                if(File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                File.WriteAllText(filepath, _outputString);

                generateNow = false;
            }
        }
#endif
    }
}
