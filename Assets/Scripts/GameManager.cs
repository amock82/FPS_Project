using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int playerHp;

    public int playerHpMax;
    public GameObject playerEnergyBar;

    // 레벨 관련
    public int playerLevel;
    public float[] playerLimitExp;  // 목표경험치
    public float playerCurrentExp;  // 현재 경험치
    public float playerMaxExp;      // 현재 최대 경험치 값

    // 뷰
    public GameObject playerLevelView;  // 레벨 뷰
    public GameObject playerExpBar;     // 경험치 바

    // 스테이지 관련
    int currentStage = 1;
    int clearStage = 0;

    public GameObject gate;

    // 골드 정보
    public int coin = 0;
    public Text coinText;

    // 공격력, 체력 정보
    public int playerAtk = 1;
    public int[] playerAtkTable;
    public int[] playerHpMaxTable;

    int playerAtkLevel = 1;
    int playerHpMaxLevel = 1;

    public Canvas hud;

    bool isHud = false;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        StartCoroutine(HudStart());

        //--------------------------------------------
        // 보안성 암호화 파라미터 설정
        // 내부에 저장할 데이터 생성
        CryptoPlayerPrefs_HaskeyIntFind("playerLevel", playerLevel = 1);
        CryptoPlayerPrefs_HaskeyFloatFind("playerCurrentExp", playerCurrentExp = 0);            // 현재 경험치
        CryptoPlayerPrefs_HaskeyFloatFind("playerMaxExp", playerMaxExp = playerLimitExp[0]);    // 목표 경험치
        CryptoPlayerPrefs_HaskeyIntFind("currentStage", currentStage = 1);      // 현재층
        CryptoPlayerPrefs_HaskeyIntFind("clearStage", clearStage = 0);          // 클리어 층
        CryptoPlayerPrefs_HaskeyIntFind("coin", coin = 1000);          // 클리어 층

        CryptoPlayerPrefs_HaskeyIntFind("playerAtk", playerAtk = 1);         
        CryptoPlayerPrefs_HaskeyIntFind("playerAtkLevel", playerAtkLevel = 1);         
        CryptoPlayerPrefs_HaskeyIntFind("playerHpMax", playerHp = 100);         
        CryptoPlayerPrefs_HaskeyIntFind("playerHpMaxLevel", playerHpMaxLevel = 1);         

        //--------------------------------------------
        playerLevel = CryptoPlayerPrefs.GetInt("playerLevel");    
        playerCurrentExp = CryptoPlayerPrefs.GetFloat("playerCurrentExp");
        playerMaxExp = CryptoPlayerPrefs.GetFloat("playerMaxExp");
        currentStage = CryptoPlayerPrefs.GetInt("currentStage");
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");
        coin = CryptoPlayerPrefs.GetInt("coin");

        playerAtk = CryptoPlayerPrefs.GetInt("playerAtk");
        playerAtkLevel = CryptoPlayerPrefs.GetInt("playerAtkLevel");
        playerHpMax = CryptoPlayerPrefs.GetInt("playerHpMax");
        playerHpMaxLevel = CryptoPlayerPrefs.GetInt("playerHpMaxLevel");

        //playerLevel = 1;    // 플레이어 레벨 초기화
        playerLevelView.GetComponentInChildren<Text>().text = playerLevel.ToString();

        //playerExpBar.GetComponent<EnergyBar>().SetValueCurrent(Mathf.CeilToInt(playerCurrentExp))
        playerExpBar.GetComponent<EnergyBar>().SetValueCurrent((int)playerCurrentExp);

        //playerMaxExp = playerLimitExp[0];
        playerExpBar.GetComponent<EnergyBar>().SetValueMax((int)playerMaxExp);

        playerHp = playerHpMax;     // 체력 초기화
        coinText.text = coin.ToString();
    }

    IEnumerator HudStart()
    {
        hud.enabled = false;

        while (!isHud)
        {
            isHud = true;

            if (SceneManager.GetSceneByName("Stage").isLoaded)
            {
                hud.enabled = true;
                break;
            }
            yield return new WaitForSeconds(0.2f);

            isHud = false;
        } 
    }

    public void BuildDeActive()
    {
        if (GameObject.Find("Stage(Clone)"))
        {
            GameObject.Find("Stage(Clone)").SetActive(false);
        }
    }

    public void ClearPrefsData()
    {
        CryptoPlayerPrefs.DeleteAll();
    }

    void Start()
    {
        // 커서 고정(마우스 거리에 상관없이 항상 게임뷰에 고정)
        Cursor.lockState = CursorLockMode.Locked;

        // 커서 숨김
        //Cursor.visible = false;

        playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(playerHpMax);
        playerEnergyBar.GetComponent<EnergyBar>().SetValueMin(0);
        playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(playerHp);

        SoundManager.instance.PlayBgm(SoundManager.instance.mainBgm, 0, true);
    }

    void Update()
    {
        
    }

    // 타이틀로 돌아가기 버튼
    public void TitleBtn()
    {
        CryptoPlayerPrefs.SetString("titleGo", "On");

        SceneManager.LoadScene("Loading");
    }

    // 코인 획득
    public int CoinAdd(int amount)
    {
        int tCoin = CryptoPlayerPrefs.GetInt("coin");
        tCoin += amount;    // 획득한 코인 누적

        coinText.text = tCoin.ToString();

        CryptoPlayerPrefs.SetInt("coin", tCoin);

        return tCoin;
    }

    public void GateGen()
    {
        CryptoPlayerPrefs.SetString("titleOnlyStage", "Off");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Instantiate(gate, player.transform.position, Quaternion.identity);

    }

    // 경험치 계산
    public float ExpCall(float exp)
    {
        playerCurrentExp += exp;

        if (playerLevel < playerLimitExp.Length)
        {
            if (playerCurrentExp >= playerLimitExp[playerLevel - 1])
            {
                playerCurrentExp -= playerLimitExp[playerLevel - 1];

                playerLevel++;
                LevelUp();

                playerMaxExp = playerLimitExp[playerLevel - 1];
            } 
        }
        else if (playerCurrentExp >= playerLimitExp[playerLimitExp.Length-1])
        {
            playerCurrentExp -= playerLimitExp[playerLimitExp.Length - 1];

            playerLevel++;
            LevelUp();

            playerMaxExp = playerLimitExp[playerLimitExp.Length - 1];
        }

        // 데이터 저장
        CryptoPlayerPrefs.SetInt("playerLevel", playerLevel);
        CryptoPlayerPrefs.SetFloat("playerCurrentExp", playerCurrentExp);
        CryptoPlayerPrefs.SetFloat("playerMaxExp", playerMaxExp);

        // 레벨, 경험치 뷰
        playerLevelView.GetComponentInChildren<Text>().text = playerLevel.ToString();
        playerExpBar.GetComponent<EnergyBar>().SetValueMax((int)playerMaxExp);
        playerExpBar.GetComponent<EnergyBar>().SetValueMin(0);
        playerExpBar.GetComponent<EnergyBar>().SetValueCurrent((int)playerCurrentExp);

        return exp;
    }

    void LevelUp()
    {
        SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.levelUp, 0, SoundManager.instance.sfxVolum);
    }

    // CryptoPlayerPrefs의 해시키를 찾는 함수
    // 해당 키가 있으면 불러오고 없으면 새로 생성 (정수값)
    public void CryptoPlayerPrefs_HaskeyIntFind(string key, int value)
    {
        if(!CryptoPlayerPrefs.HasKey(key))
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
