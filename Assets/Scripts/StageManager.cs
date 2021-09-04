using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] stage;

    int currentStage;
    int clearStage;

    private void Awake()
    {
        // 데이터 로드
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");
        //currentStage = clearStage + 1;
        currentStage = CryptoPlayerPrefs.GetInt("currentStage");

        // 인게임내 게이트를 통과한 경우 (다음 스테이지)
        if (CryptoPlayerPrefs.GetString("titleOnlyStage") == "Off")
        {
            // 현재 층이 클리어층보다 낮으면 항상 다음층이 로드되도록
            if (currentStage < clearStage)
            {
                currentStage += 1;
            }
            else
            {
                currentStage = clearStage + 1;
            }
            CryptoPlayerPrefs.SetInt("currentStage", currentStage);
        }
        else    // 타이틀에서 선택한 경우
        {
            currentStage = CryptoPlayerPrefs.GetInt("titlePlayStage");
            Debug.Log(currentStage);
        }
    }

    void Start()
    {
        // 배열 전체 길이 검사, 현재층이 전체길이층보다 높을 때만 수행함
        if (currentStage > stage.Length)
        {
            Instantiate(stage[stage.Length - 1], Vector3.zero, Quaternion.identity);
        }
        else
        {
            Instantiate(stage[currentStage - 1], Vector3.zero, Quaternion.identity);
        }

        CryptoPlayerPrefs.SetInt("currentStage", currentStage);

        Debug.Log("클리어 스테이지 : " + clearStage);
        Debug.Log("현재 스테이지 : " + currentStage);
    }

    void Update()
    {
        
    }
}
