using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.playerHp = GameManager.instance.playerHpMax;

            GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(GameManager.instance.playerHp);

            BloodEffect.image.color = new Color(1, 1, 1, 0);
            // 사운드 효과음 처리

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.itemUse, 0, SoundManager.instance.sfxVolum);
        }
    }
}
