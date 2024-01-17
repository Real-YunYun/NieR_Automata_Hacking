using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData {
    public string CurrentDate = "\0";
    public int MaxHealth = 50;
    public int Bits = 0;

    public int KillCount = 0;
    public int RunCount = 0;

    public List<string> Executables = new List<string>();
    public List<string> Threads = new List<string>();
}