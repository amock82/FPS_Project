using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffect : MonoBehaviour
{
    public static Image image;

    public Sprite blood_01;
    public Sprite blood_02;

    public Color startColor = new Color(1,1,1,0);   // 투명값
    public Color endColor = new Color(1,1,1,1);     // 불투명값

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (PlayerState.instance.playerState == PlayerState.PLAYERSTATE.DAMAGE)
        {
            if (GameManager.instance.playerHp >= GameManager.instance.playerHpMax * 0.2f)
            {
                StartCoroutine(FadeEffect());       // 카메라 혈흔 효과
            }          
        }
        
        if (GameManager.instance.playerHp < GameManager.instance.playerHpMax * 0.2f)
        {
            image.sprite = blood_02;
            image.color = Color.Lerp(endColor, startColor, Mathf.PingPong(Time.time, 1.0f));
        }

    }

    IEnumerator FadeEffect()
    {
        image.sprite = blood_01;
        image.color = startColor;

        for (float i = 1f; i>= -0.1f; i-=0.02f)
        {
            Color color = new Vector4(1, 1, 1, i);
            image.color = color;

            yield return new WaitForEndOfFrame();


        }
    }
}
