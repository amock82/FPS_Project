using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    int curStage;
    int clearStage;
    int titlePlayStage;         

    public UILabel stageSelect; // 스테이지 선택 확인용 라벨

    public static TitleManager instance;

    private void Awake()
    {
        instance = this;
        CryptoPlayerPrefs_HaskeyIntFind("currentStage", curStage = 1);
        CryptoPlayerPrefs_HaskeyIntFind("clearStage", clearStage);

        // 내부 데이터 생성
        CryptoPlayerPrefs_HaskeyIntFind("titlePlayStage", titlePlayStage = 1);
        CryptoPlayerPrefs_HaskeyStringFind("titleGo", "Off");
        CryptoPlayerPrefs_HaskeyStringFind("titleOnlyStage", "On");

        // 데이터 로드
        titlePlayStage = CryptoPlayerPrefs.GetInt("titlePlayStage");
        curStage = CryptoPlayerPrefs.GetInt("currentStage");
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");
    }

    public void StartGame()
    {
        if (stageSelect.text == "Lock")
            return;

        SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        CryptoPlayerPrefs.SetString("titleGo", "Off"); 
    }

    void Update()
    {
        
    }

    // CryptoPlayerPrefs의 해시키를 찾는 함수
    // 해당 키가 있으면 불러오고 없으면 새로 생성 (정수값)
    public void CryptoPlayerPrefs_HaskeyIntFind(string key, int value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetInt(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetInt(key, 0);
        }
    }

    // CryptoPlayerPrefs의 해시키를 찾는 함수
    // 해당 키가 있으면 불러오고 없으면 새로 생성 (실수값)
    public void CryptoPlayerPrefs_HaskeyFloatFind(string key, float value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetFloat(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetFloat(key, 0);
        }
    }

    // CryptoPlayerPrefs의 해시키를 찾는 함수
    // 해당 키가 있으면 불러오고 없으면 새로 생성 (문자열값)
    public void CryptoPlayerPrefs_HaskeyStringFind(string key, string value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetString(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetString(key, "");
        }
    }
}
