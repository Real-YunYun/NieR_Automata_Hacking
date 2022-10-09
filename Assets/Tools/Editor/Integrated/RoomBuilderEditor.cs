using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

[CustomEditor(typeof(RoomBuilder))]
public class RoomBuilderEditor : Editor
{
    private static class RoomBuilderStatics
    {
        static public class CubeColor
        {
            static public Color Ground { get { return new Color(195, 181, 152, 255); } }
            static public Color Default { get { return Color.white; } }
            static public Color Destrucible { get { return Color.gray; } }
            static public Color Danger { get { return Color.red; } }
        }

        static public class BlockObjects
        {
            static public class Prefab
            {
                static public string Path = "Ground/";
                static public GameObject DefaultCube { get { return Resources.Load<GameObject>(Path + "Default Cube"); } }
            }
        }

        static public string Save(RoomInformation Information, string FileName = "0")
        {
            string JSONFormat = JsonUtility.ToJson(Information);
            if (!File.Exists(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt"))
            {
                File.Create(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt");
                File.WriteAllText(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt", JSONFormat);
            }
            else if (File.Exists(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt")) File.WriteAllText(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt", JSONFormat);

            return JSONFormat;
        }

        static public RoomInformation Import(string FileName = "0")
        {
            string JSONFile = File.ReadAllText(Application.dataPath + "/Tools/Rooms/" + FileName + ".txt");
            return JsonUtility.FromJson<RoomInformation>(JSONFile);
        }
    }

    // Displaying Room Layout
    private RoomBuilder RoomBuilderScript;
    private RoomInformation Data = new();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomBuilderScript = (RoomBuilder)target;

        if (GUILayout.Button("Save Room"))
        {
            // Reference to another script
            if (RoomBuilderScript != null && RoomBuilderScript.SavingFileName != "" && RoomBuilderScript.SavingFileName != null)
            {
                Data = RoomBuilderScript.Analyze();
                RoomBuilderStatics.Save(Data, RoomBuilderScript.SavingFileName);
                Debug.Log("Serializing Done\nRoom Type: " + Data.RoomType + " | Room Elements: " + Data.Blocks);
            }
            else Debug.LogError("Parameter \"Saving File Name\" was null or empty.");
        }

        if (GUILayout.Button("Import Room From File"))
        {
            // Reference to another script
            if (RoomBuilderScript != null && RoomBuilderScript.Import() != null)
            {
                Data = RoomBuilderScript.Import();
                Debug.Log("Importing Done\nRoom Type: " + Data.RoomType + " | Room Elements: " + Data.Blocks);
            }
            else Debug.LogError("Parameter \"Importing File Name\" was null or empty.");
        }


    }
}