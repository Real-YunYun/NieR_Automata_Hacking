using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Text ProgressBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LoadAsycOperation");
    }

    IEnumerator LoadAsycOperation()
    {
        AsyncOperation ASyncLoad =  SceneManager.LoadSceneAsync(GameManager.Instance.CurrentLevel, LoadSceneMode.Single);

        while (ASyncLoad.progress < 1)
        {
            int TextAmount = Mathf.RoundToInt(ASyncLoad.progress * 58);
            ProgressBar.text = "";
            for (int i = 0; i < TextAmount; i++) ProgressBar.text += "/";
            yield return new WaitForEndOfFrame();
        }
    }

}
