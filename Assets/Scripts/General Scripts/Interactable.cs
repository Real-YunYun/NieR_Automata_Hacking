using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public abstract class Interactable : MonoBehaviour {
        
    // What makes an interactable
    // Needs someway to interact with maybe an interface? or just native events?

    protected abstract void OnInteracted(Entity InteractedEntity, int slot = 0);

    
}
