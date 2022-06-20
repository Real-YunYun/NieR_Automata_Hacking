using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager Parameters")]
    private AudioSource MusicSource;

    void Awake()
    {
        MusicSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentLevel != "HUB" && GameManager.Instance.CurrentLevel != "Title" && GameManager.Instance.CurrentLevel != "Loading")
        {
            if (!MusicSource.isPlaying)
            {
                AudioClip MusicAlbum = Resources.LoadAll<AudioClip>("Music")[Random.Range(0, 22)];
                MusicSource.PlayOneShot(MusicAlbum);
                GameManager.Instance.MainCameraInstance.transform.Find("UI Canvas/Menu Canvas/Music").GetComponent<Text>().text = "CURRENTLY PLAYING: " + MusicAlbum.name;
                Resources.UnloadUnusedAssets();
            }

            if (GameManager.Instance.CurrentGameState != GameState.Playing) MusicSource.volume = 0.25f;
            else MusicSource.volume = 1f;
        }
    }

}
