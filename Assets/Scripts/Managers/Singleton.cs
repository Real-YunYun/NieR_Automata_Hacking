using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T _instance;
    public static T Instance {
        get {
            if (_instance == null) {
                var Objects = FindObjectsOfType(typeof(T)) as T[];
                if (Objects.Length > 0) _instance = Objects[0];
                if (Objects.Length > 1) Debug.LogError("There are more than one " + typeof(T).Name + " in the scene");
                if (_instance == null)
                {
                    GameObject Object = new GameObject();
                    Object.hideFlags = HideFlags.HideAndDontSave;
                    _instance = Object.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}


public class SingletonPersistent<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
}