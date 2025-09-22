using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;


[CustomEditor(typeof(RoomBuilder))]
public class RoomBuilderEditor : Editor {
    // Displaying Room Layout
    private RoomBuilder RoomBuilderScript;
    public RoomInformation Data = new();

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        RoomBuilderScript = (RoomBuilder)target;

        if (GUILayout.Button("Save Room")) {
            if (RoomBuilderScript) {
                Data = RoomBuilderScript.Save();
                RoomDictionary Dictionary = JsonUtility.FromJson<RoomDictionary>(File.ReadAllText(Application.dataPath + "/Dictionary/Rooms.dictionary"));

                // Pre room addition!
                if (RoomBuilderScript.BackupDictionaries) {
                    if (!File.Exists(Application.dataPath + "/Dictionary/Rooms.dictionary.prebackup"))
                        File.Create(Application.dataPath + "/Dictionary/Rooms.dictionary.prebackup");
                    string PreJSONFormatData = JsonUtility.ToJson(Dictionary, true);
                    File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary.prebackup",
                        PreJSONFormatData);
                }

                // Dictionary serialization
               Dictionary.AddToDictionary(Data);
               if (!File.Exists(Application.dataPath + "/Dictionary/Rooms.dictionary"))
                    File.Create(Application.dataPath + "/Dictionary/Rooms.dictionary");
                string JSONFormatData = JsonUtility.ToJson(Dictionary, true);
                File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary", JSONFormatData);
                Debug.Log("Saved!!");

                //Post Addition Serialization
                if (RoomBuilderScript.BackupDictionaries) {
                    if (!File.Exists(Application.dataPath + "/Dictionary/Rooms.dictionary.backup"))
                        File.Create(Application.dataPath + "/Dictionary/Rooms.dictionary.backup");
                    File.WriteAllText(Application.dataPath + "/Dictionary/Rooms.dictionary.backup", JSONFormatData);
                }
            }
        }
        
    
        if (GUILayout.Button("Import Room")) {
            if (RoomBuilderScript != null) RoomBuilderScript.Import();
        }

        if (GUILayout.Button("Clear Room")) {
            if (RoomBuilderScript) RoomBuilderScript.ClearLayout();
        }
        
        if (GUILayout.Button("Filter Dictionary (Debug)")) {
            if (RoomBuilderScript) RoomBuilderScript.Filter();
        }
    } 
}