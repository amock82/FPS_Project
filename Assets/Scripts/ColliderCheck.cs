using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            case 11:        // Player 레이어
                {
                    GameManager.instance.playerHp--;
                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(GameManager.instance.playerHpMax);
                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMin(0);
                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(GameManager.instance.playerHp);

                    //Debug.Log(GameManager.instance.playerHp);

                    PlayerState.instance.playerState = PlayerState.PLAYERSTATE.DAMAGE;

                    SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.handAtk, 0, SoundManager.instance.sfxVolum);
                    //ShakeManager.instance.isShake = true;

                    break;
                }
        }
    }
}
