using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidFieldGenerator))]
public class AsteroidFieldGenerator_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AsteroidFieldGenerator exam = (AsteroidFieldGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            exam.GenerateAsteroidField();
        }
    }
}
