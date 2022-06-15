using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioSource audioSource;
    public AudioClip HoverUI;
    public float volume = 0.5f;

    //On Hover UI Element
    public void OnPointerEnter(PointerEventData ped)
    {
        audioSource.PlayOneShot(HoverUI, volume);
    }

    //On Pressed UI Element
    public void OnPointerDown(PointerEventData ped)
    {
        
    }
}
