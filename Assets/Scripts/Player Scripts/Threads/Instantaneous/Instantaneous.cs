using System.Collections;
using System.Collections.Generic;
using Items.Threads;
using UnityEngine;

namespace Items.Threads.Instantaneous {

    public class Instantaneous : Thread {
        [SerializeField] protected bool HasActivated = false;
    }
    
}
