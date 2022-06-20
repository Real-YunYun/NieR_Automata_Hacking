using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager Parameters")]
    private AudioSource MusicSource;

    //This loads all the Music files on Runtime for when the game needs to play
    //This is bad this is just only 100MB that needs to be loaded in....
    private AudioClip[] MusicAlbum;

    void Awake()
    {
        MusicAlbum = Resources.LoadAll<AudioClip>("Music");
        MusicSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentLevel != "HUB" && GameManager.Instance.CurrentLevel != "Title" && GameManager.Instance.CurrentLevel != "Loading")
        {
            if (!MusicSource.isPlaying)
            {
                int randomMusic = Random.Range(0, 22);
                MusicSource.PlayOneShot(MusicAlbum[randomMusic]);
                Resources.UnloadUnusedAssets();
                GameManager.Instance.MainCameraInstance.transform.Find("UI Canvas/Menu Canvas/Music").GetComponent<Text>().text = "CURRENTLY PLAYING: " + MusicAlbum[randomMusic].name;
            }

            if (GameManager.Instance.CurrentGameState != GameState.Playing) MusicSource.volume = 0.25f;
            else MusicSource.volume = 1f;
        }
    }

}
