using System.Collections;
using System.Collections.Generic;
using Entities.Status;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(StatusInfliction))]
public class StatusInflictionEditor : Editor {

    public VisualTreeAsset VisualTree;

    public override VisualElement CreateInspectorGUI() {
        VisualElement root = new VisualElement();

        VisualTree.CloneTree(root);
        
        return root;
    }
}