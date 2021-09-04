using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    AsyncOperation async;   // 비동기화
    AsyncOperation async2;   // 비동기화
    AsyncOperation async3;   // 비동기화

    public GameObject loadingBar;   //  로딩 객체

    float progress;

    private void Awake()
    {
        loadingBar.GetComponent<EnergyBar>().SetValueMin(0);
        loadingBar.GetComponent<EnergyBar>().SetValueMax(100);
    }

    void Start()
    {
        if (CryptoPlayerPrefs.GetString("titleGo") == "On")
        {
            async = SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
        }
        else
        {
            async = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
            async2 = SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive);
            async3 = SceneManager.LoadSceneAsync("Stage", LoadSceneMode.Additive);

            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Update()
    {
        if (CryptoPlayerPrefs.GetString("titleGo") == "Off")
        {
            if (!async3.isDone)
            {
                progress = async3.progress * 100;
                loadingBar.GetComponent<EnergyBar>().valueCurrent = (int)progress;
            }
            if(async3.isDone)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (!async.isDone)
            {
                progress = async.progress * 100;
                loadingBar.GetComponent<EnergyBar>().valueCurrent = (int)progress;
            }
        }
    }
}
