using System;
using UnityEngine;
using UnityEngine.InputSystem;
 public enum InteractType
{
    Level = 0b_0000_0000,
    Item = 0b_0000_0001
}

public class Interact : MonoBehaviour
{
    [Header("Interact Parameters")]
    [Tooltip("What shows to the player when they step into the interact zone of this GameObject")]
    public string ToolTip = "\0";
    public InteractType Type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) GameManager.Instance.PromptInteract(ToolTip, true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Keyboard.current.eKey.isPressed) Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) GameManager.Instance.PromptInteract();
    }

    public virtual void Activate() { return; }
}
