using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SessionManager : MonoBehaviour
{
    private static SessionManager instance;
    public static SessionManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("instance is null");
            };
            return instance;
        }
    }

    [SerializeField] GameObject loadSceneCanvas;
    GameObject loadSceneConfirmBtn;
    UnityEngine.UI.Slider loadSceneProgressSlider;
    TextMeshProUGUI loadSceneProgressText;
    AsyncOperation asyncSceneLoad;

    private void Awake()
    {
        instance = this;
        loadSceneConfirmBtn = loadSceneCanvas.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
        loadSceneProgressSlider = loadSceneCanvas.gameObject.GetComponentInChildren<UnityEngine.UI.Slider>();
        loadSceneProgressText = loadSceneProgressSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        loadSceneConfirmBtn.SetActive(false);
        loadSceneCanvas.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine( LoadSceneAsync("Game") );
    }
    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadSceneCanvas.SetActive(true);
        asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Start loading");

        asyncSceneLoad.allowSceneActivation = false;
        while (asyncSceneLoad.isDone == false)
        {
            loadSceneProgressSlider.value = asyncSceneLoad.progress;
            loadSceneProgressText.text = (asyncSceneLoad.progress * 100).ToString() + "%";
            if (asyncSceneLoad.progress >= 0.9f)
            {
                loadSceneProgressSlider.value = 1;
                loadSceneProgressText.text = "100%";
                break;
            }
            yield return null;
        }
        Debug.Log("Finish loading");

        loadSceneConfirmBtn.SetActive(true);
    }
    public void ConfirmReadyToChangeScene()
    {
        loadSceneProgressSlider.value = 0;
        loadSceneProgressText.text = "";
        loadSceneConfirmBtn.SetActive(false);
        loadSceneCanvas.SetActive(false);

        asyncSceneLoad.allowSceneActivation = true;
    }
}
