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

using System.Text;

namespace AuraAPI
{
    /// <summary>
    /// Static class containing extension for string type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Insert a string before all upper case letters
        /// </summary>
        /// <param name="insertedString">String that will be inserted before the upper case letter</param>
        /// <param name="ignoreFirstLetter">Should the first letter of the string be ignored? Default = true</param>
        /// <param name="ignoreSpaces">Should insertion be ignored if a space is in front of the upper case letter? Default = true</param>
        /// <returns>The modified string</returns>
        public static string InsertStringBeforeUpperCaseLetters(this string sourceString, string insertedString, bool ignoreFirstLetter = true, bool ignoreSpaces = true)
        {
            if(string.IsNullOrEmpty(sourceString))
            {
                return "";
            }

            StringBuilder newText = new StringBuilder(sourceString.Length * 2);
            if(ignoreFirstLetter)
            {
                newText.Append(sourceString[0]);
            }

            for(int i = 1; i < sourceString.Length; i++)
            {
                if(char.IsUpper(sourceString[i]) && (sourceString[i - 1] != ' ' || !ignoreSpaces))
                {
                    newText.Append(insertedString);
                }
                newText.Append(sourceString[i]);
            }

            return newText.ToString();
        }

        /// <summary>
        /// Insert a char before all upper case letters
        /// </summary>
        /// <param name="insertedCharacter">Char that will be inserted before the upper case letter</param>
        /// <param name="ignoreFirstLetter">Should the first letter of the string be ignored? Default = true</param>
        /// <param name="ignoreSpaces">Should insertion be ignored if a space is in front of the upper case letter? Default = true</param>
        /// <returns>The modified string</returns>
        public static string InsertCharacterBeforeUpperCaseLetters(this string sourceString, char insertedCharacter, bool ignoreFirstLetter = true, bool ignoreSpaces = true)
        {
            return sourceString.InsertStringBeforeUpperCaseLetters(insertedCharacter.ToString(), ignoreFirstLetter, ignoreSpaces);
        }
    }
}
