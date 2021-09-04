using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum PLAYERSTATE
    {
        AWAKE = 0,
        IDLE,
        MOVE,
        ATTACK,
        DAMAGE,
        RELOAD,
        DEAD
    }

    public PLAYERSTATE playerState = PLAYERSTATE.AWAKE;

    Transform tr;

    bool isRun = false;     // 효과음 제어 플래그(걷기)

    public static PlayerState instance;

    private void Awake()
    {
        tr = transform;
        instance = this;
    }

    void Update()
    {
        //Debug.Log(playerState);

        bool isMove = Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d");

        switch (playerState)
        {
            case PLAYERSTATE.AWAKE:
                PlayerAwake();
                break;

            case PLAYERSTATE.IDLE:
                if(isMove)
                {
                    playerState = PLAYERSTATE.MOVE;
                }
                else if (PlayerController.instance.isAttack)
                {
                    playerState = PLAYERSTATE.ATTACK;
                }
                else
                {
                    playerState = PLAYERSTATE.IDLE;
                }

                break;

            case PLAYERSTATE.MOVE:
                if( PlayerController.instance.isAttack)
                {
                    playerState = PLAYERSTATE.ATTACK;
                }
                else if (!isMove)
                {
                    playerState = PLAYERSTATE.IDLE;
                }
                StartCoroutine(StepRun());
                break;

            case PLAYERSTATE.ATTACK:
                if (!PlayerController.instance.isAttack && !isMove)
                {
                    playerState = PLAYERSTATE.IDLE;
                }
                else if (!PlayerController.instance.isAttack && isMove)
                {
                    playerState = PLAYERSTATE.MOVE;
                }

                break;

            case PLAYERSTATE.DAMAGE:
                {
                    //EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Bump);
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(2.0f, 2.0f, 0f, 0.5f);
                    
                    if (PlayerController.instance.isAttack)
                    {
                        playerState = PLAYERSTATE.ATTACK;
                    }
                    else if(isMove)
                    {
                        playerState = PLAYERSTATE.MOVE;
                    }
                    else
                    {
                        playerState = PLAYERSTATE.IDLE;
                    }
                }
                break;

            case PLAYERSTATE.RELOAD:

                break;

            case PLAYERSTATE.DEAD:

                break;
        }
    }

    void PlayerAwake()
    {
        playerState = PLAYERSTATE.IDLE;
    }

    // 스탭 효과음
    IEnumerator StepRun()
    {
        if (isRun == false)
        {
            isRun = true;

            yield return new WaitForSeconds(0.3f);

            AudioClip[] rndAudio = { SoundManager.instance.step_A, SoundManager.instance.step_B, SoundManager.instance.step_C };
            SoundManager.instance.PlaySfx(tr.position, rndAudio[Random.Range(0, 3)], 0, SoundManager.instance.sfxVolum);

            isRun = false;
        }
    }
}
