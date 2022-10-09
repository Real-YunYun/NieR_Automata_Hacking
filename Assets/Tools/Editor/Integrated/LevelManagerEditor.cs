using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // Displaying Room Layout
    private LevelManager LevelManagerScript;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManagerScript = (LevelManager)target;
        LevelManagerScript.LoadDictionary();

        if (GUILayout.Button("Import Room From File"))
        {
            LevelManagerScript.ImportInformation();
        }

        if (GUILayout.Button("Remove Room from Dictionary"))
        {
            
        }

        if (GUILayout.Button("Manually Save Data"))
        {
            LevelManagerScript.SaveDictionary();
            Debug.Log("Manually Saved Dictionary");
        } 
        
        if (GUILayout.Button("Manually Load Data"))
        {
            LevelManagerScript.LoadDictionary();
            Debug.Log("Manually Load Dictionary");
        }
    }
}
