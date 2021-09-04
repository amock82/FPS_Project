using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIconActive : MonoBehaviour
{
    public GameObject iconActive;
    public GameObject iconLock;

    void Start()
    {
        string tmpStr = gameObject.name;

        if (tmpStr.Length == 7 && CryptoPlayerPrefs.GetInt("clearStage") + 1 >= int.Parse(tmpStr.Substring(6,1)))
        {
            iconActive.SetActive(true);
            iconLock.SetActive(false);
        }
        else
        {
            iconActive.SetActive(false);
            iconLock.SetActive(true);
        }
    }

    void Update()
    {
        
    }

    void OnClick()
    {
        string tmpStr = gameObject.name;

        if ( iconActive.activeSelf == true)     // 스테이지 오픈시
        {
            CryptoPlayerPrefs.SetString("titleOnlyStage", "On");

            CryptoPlayerPrefs.SetInt("titlePlayStage", int.Parse(tmpStr.Substring(6, tmpStr.Length - 6)));

            TitleManager.instance.stageSelect.text = "STAGE " + CryptoPlayerPrefs.GetInt("titlePlayStage");
        }
        else                                    // 스테이지 잠김시
        {
            TitleManager.instance.stageSelect.text = "Lock";                         
        }
    }
}
