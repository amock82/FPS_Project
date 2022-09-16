using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopOpenBtn;
    public GameObject ShopUI;

    public UILabel coinLabel;
    public UILabel atkLabel;
    public UILabel hpMaxLabel;
    public UILabel atkLevel;
    public UILabel hpMaxLevel;

    int atkPreview = 1;
    int hpMaxPreview = 1;

    bool isAtkChange = false;
    bool isHpMaxChange = false;

    public UILabel atkPriceLabel;
    public UILabel hpMaxPriceLabel;

    // 가격 관리
    public int[] attackPrice;
    public int[] hpMaxPrice;

    // 최고 레벨 상수
    const int MAXVALUE = 3;

    private void Awake()
    {
        
    }

    void Start()
    {
        shopOpenBtn.SetActive(true);
        ShopUI.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OpenShop()
    {
        Time.timeScale = 0;

        shopOpenBtn.SetActive(false);
        ShopUI.SetActive(true);

        coinLabel.text = CryptoPlayerPrefs.GetInt("coin").ToString();
        atkLevel.text = CryptoPlayerPrefs.GetInt("playerAtkLevel").ToString();
        hpMaxLevel.text = CryptoPlayerPrefs.GetInt("playerHpMaxLevel").ToString();

        if (CryptoPlayerPrefs.GetInt("playerAtkLevel") <= MAXVALUE)
        {
            // 공격력 정보
            atkLabel.text = "ATK " + GameManager.instance.playerAtkTable[CryptoPlayerPrefs.GetInt("playerAtkLevel") - 1].ToString();

            // 가격정보
            atkPriceLabel.text = attackPrice[CryptoPlayerPrefs.GetInt("playerAtkLevel") - 1].ToString();
        }

        if (CryptoPlayerPrefs.GetInt("playerHpMaxLevel") <= MAXVALUE)
        {
            // 공격력 정보
            hpMaxLabel.text = "HP MAX " + GameManager.instance.playerHpMaxTable[CryptoPlayerPrefs.GetInt("playerHpMaxLevel") - 1].ToString();

            // 가격정보
            hpMaxPriceLabel.text = hpMaxPrice[CryptoPlayerPrefs.GetInt("playerHpMaxLevel") - 1].ToString();
        }
    }

    public void CloseShop()
    {
        Time.timeScale = 1;

        shopOpenBtn.SetActive(true);
        ShopUI.SetActive(false);

        if (isAtkChange) isAtkChange = false;
        if (isHpMaxChange) isHpMaxChange = false;
    }

    public void AttackPricePreview()
    {
        if (int.Parse(atkLevel.text) < MAXVALUE)
        {
            isAtkChange = true;
            atkPreview = int.Parse(atkLevel.text);
            atkPreview += 1;
            atkLevel.text = atkPreview.ToString();

            atkLabel.text = "ATK " + GameManager.instance.playerAtkTable[atkPreview - 1].ToString();
            atkPriceLabel.text = attackPrice[atkPreview - 1].ToString();
        }
        else
        {
            Debug.Log("더 이상 업그레이드 할 수 없습니다.");
        }
    }

    // 공격력 구입 버튼
    public void AttackPurchase()
    {
        if (isAtkChange && int.Parse(atkLevel.text) <= MAXVALUE)
        {
            if (CryptoPlayerPrefs.GetInt("coin") >= int.Parse(atkPriceLabel.text))
            {
                CryptoPlayerPrefs.SetInt("playerAtkLevel", int.Parse(atkLevel.text));

                int curCoin = CryptoPlayerPrefs.GetInt("coin") - int.Parse(atkPriceLabel.text);
                CryptoPlayerPrefs.SetInt("coin", curCoin);

                isAtkChange = false;

                CryptoPlayerPrefs.SetInt("playerAtk", GameManager.instance.playerAtkTable[CryptoPlayerPrefs.GetInt("playerAtkLevel") - 1]);

                GameManager.instance.coinText.text = curCoin.ToString();
                coinLabel.text = curCoin.ToString();
            }
            else
            {
                Debug.Log("소지 금액이 부족합니다.");
            }
        }
        else
        {
            Debug.Log("더 이상 구입 할 수 없습니다.");
        }

        AttackPricePreview();
    }

    public void HpMaxPricePreview()
    {
        if (int.Parse(hpMaxLevel.text) < MAXVALUE)
        {
            isHpMaxChange = true;
            hpMaxPreview = int.Parse(hpMaxLevel.text);
            hpMaxPreview += 1;
            hpMaxLevel.text = hpMaxPreview.ToString();

            hpMaxLabel.text = "HP MAX " + GameManager.instance.playerHpMaxTable[hpMaxPreview - 1].ToString();
            hpMaxPriceLabel.text = hpMaxPrice[hpMaxPreview - 1].ToString();
        }
        else
        {
            Debug.Log("더 이상 업그레이드 할 수 없습니다.");
        }
    }

    // 체력 구입 버튼
    public void HpMaxPurchase()
    {
        if (isHpMaxChange && int.Parse(hpMaxLevel.text) <= MAXVALUE)
        {
            if (CryptoPlayerPrefs.GetInt("coin") >= int.Parse(hpMaxPriceLabel.text))
            {
                CryptoPlayerPrefs.SetInt("playerHpMaxLevel", int.Parse(hpMaxLevel.text));

                int curCoin = CryptoPlayerPrefs.GetInt("coin") - int.Parse(hpMaxPriceLabel.text);
                CryptoPlayerPrefs.SetInt("coin", curCoin);

                isHpMaxChange = false;

                CryptoPlayerPrefs.SetInt("playerHpMax", GameManager.instance.playerHpMaxTable[CryptoPlayerPrefs.GetInt("playerHpMaxLevel") - 1]);

                // 체력 관련 데이터
                GameManager.instance.playerHpMax = CryptoPlayerPrefs.GetInt("playerHpMax");
                GameManager.instance.playerHp = CryptoPlayerPrefs.GetInt("playerHpMax");

                // 인게임 내 체력 뷰
                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(CryptoPlayerPrefs.GetInt("playerHpMax"));
                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(CryptoPlayerPrefs.GetInt("playerHpMax"));

                GameManager.instance.coinText.text = curCoin.ToString();
                coinLabel.text = curCoin.ToString();
            }
            else
            {
                Debug.Log("소지 금액이 부족합니다.");
            }
        }
        else
        {
            Debug.Log("더 이상 구입 할 수 없습니다.");
        }

        HpMaxPricePreview();
    }
}
