using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderHelper {
    // Set all values from settings object on the shader. Note, variable names must be an exact match in the shader.
    // Settings object can be any class/struct containing vectors/ints/floats
    public static void SetParams (System.Object settings, Material material, string variableNamePrefix = "") {
        var fields = settings.GetType ().GetFields ();
        foreach (var field in fields) {
            var fieldType = field.FieldType;
            string shaderVariableName = variableNamePrefix + field.Name;

            if (fieldType == typeof (UnityEngine.Vector4) || fieldType == typeof (Vector3) || fieldType == typeof (Vector2)) {
                material.SetVector (shaderVariableName, (Vector4) field.GetValue (settings));
            } else if (fieldType == typeof (int)) {
                material.SetInt (shaderVariableName, (int) field.GetValue (settings));
            } else if (fieldType == typeof (float)) {
                material.SetFloat (shaderVariableName, (float) field.GetValue (settings));
            } else {
                Debug.Log ($"Type {fieldType} not implemented");
            }
        }
    }
}