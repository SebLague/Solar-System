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

namespace AuraAPI
{
    /// <summary>
    /// Static class containing extension for generic type
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Appends an array to the current one
        /// </summary>
        /// <param name="appendedArray">The array to be appended</param>
        /// <returns>The combined array</returns>
        public static T[] Append<T>(this T[] array, T[] appendedArray)
        {
            T[] newArray = new T[array.Length + appendedArray.Length];

            for(int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            for(int j = 0; j < appendedArray.Length; j++)
            {
                newArray[array.Length + j] = appendedArray[j];
            }

            return newArray;
        }
    }
}
