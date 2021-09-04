using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieProcess : MonoBehaviour
{
    enum ZOMBIESTATE
    {
        IDLE,
        MOVE,
        ATTACK,
        DAMAGE,
        DEAD
    }

    ZOMBIESTATE         zombieState = ZOMBIESTATE.IDLE;

    Vector3             currentVelocity;    // 현재위치
    float               gravity = 10;       // 중력값

    CharacterController cc;

    public int          maxHp = 5;
    public int          hp = 5;
    public float        speed = 5.0f;
    public float        rotSpeed = 10.0f;
    public float        attackbleRange = 1.7f;
    public float        traceRange = 10.0f;

    Transform           target;

    Animator            _ani;

    // 웨이포인트 학습
    Vector3 enemyOrigin;
    SWS.PathManager path;

    bool isAtkSnd = false;  // 좀비 하울링 효과음 제어 플래그

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        _ani = GetComponent<Animator>();

        enemyOrigin = transform.position;   // 현재위치 기억
        path = Instantiate(EnemyPathManager.instance.enemyPath[0]);

        this.GetComponent<SWS.splineMove>().pathContainer = path;
    }

    void Start()
    {
        path.transform.position = new Vector3(transform.position.x, path.transform.position.y, transform.position.z);

        this.GetComponent<SWS.splineMove>().StartMove();

        this.GetComponent<SWS.splineMove>().events[0].AddListener(delegate { EnemyManager.instance.PauseToResume(gameObject); });
        this.GetComponent<SWS.splineMove>().events[1].AddListener(delegate { EnemyManager.instance.PauseToResume(gameObject); });
        this.GetComponent<SWS.splineMove>().events[2].AddListener(delegate { EnemyManager.instance.PauseToResume(gameObject); });
    }

    void Update()
    {
        //EnemyInfo();    // 체력 정보 갱신, 독립적인 hp바를 생성할 때만 주석해제

        if (zombieState == ZOMBIESTATE.DEAD)
            return;

        if(hp <= 0)
            zombieState = ZOMBIESTATE.DEAD;

        //currentVelocity.y -= gravity * Time.deltaTime;
        //cc.Move(currentVelocity * Time.deltaTime);

        float distance = Vector3.Distance(target.position, transform.position);

        //Debug.Log(zombieState);

        switch (zombieState)
        {
            case ZOMBIESTATE.IDLE:
                {
                    //_ani.SetBool("isIdle", true);
                    _ani.SetBool("isAttack", false);
                    _ani.SetBool("isMove", false);

                    if (distance < traceRange)
                    {
                        zombieState = ZOMBIESTATE.MOVE;

                        if (distance <= attackbleRange)
                        {
                            zombieState = ZOMBIESTATE.ATTACK;
                        }
                    }
                    break;
                }

            case ZOMBIESTATE.MOVE:
                {
                    //_ani.SetBool("isIdle", false);
                    _ani.SetBool("isAttack", false);
                    _ani.SetBool("isMove", true);
                    _ani.SetBool("isWalk", false);

                    this.GetComponent<SWS.splineMove>().Stop();     // 웨이포인트 중지

                    Vector3 dir = target.position - transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    cc.SimpleMove(dir * speed);

                    // 선형 보간 함수 : Quaternion.Lerp
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

                    if (distance > traceRange)
                    {
                        zombieState = ZOMBIESTATE.IDLE;

                        MonsterAwake();     // 몬스터 초기화
                    }

                    if (distance <= attackbleRange)
                    {
                        zombieState = ZOMBIESTATE.ATTACK;
                    }

                    break;
                }

            case ZOMBIESTATE.ATTACK:
                {
                    //_ani.SetBool("isIdle", false);
                    _ani.SetBool("isAttack", true);
                    _ani.SetBool("isMove", false);
                    _ani.SetBool("isWalk", false);

                    this.GetComponent<SWS.splineMove>().Stop();     // 웨이포인트 중지

                    Vector3 dir = target.position - transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    // 선형 보간 함수 : Quaternion.Lerp
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

                    StartCoroutine(ZombieAtkSnd());

                    //if(distance > attackbleRange)
                    //{
                    //    zombieState = ZOMBIESTATE.MOVE;
                    //}

                    // 기획적인 요소
                    // 공격 모션중 해당 이벤트가 발생되게 하여 관련 함수를 수행할 때

                    break;
                }

            case ZOMBIESTATE.DAMAGE:
                {
                    break;
                }
            case ZOMBIESTATE.DEAD:
                {
                    Instantiate(ParticleManager.instance.deadEffectImpact, transform.position, transform.rotation);
                    Destroy(gameObject);

                    EnemyManager.instance.killCnt++;
                    EnemyManager.instance.enemyKillBar.GetComponent<EnergyBar>().SetValueCurrent(EnemyManager.instance.maxSpawnCnt - EnemyManager.instance.killCnt);

                    // Hp바 제거
                    //if(name.Length == 7)
                    //{
                    //    Destroy(GameObject.Find("EnemyHp_" + name.Substring(6, 1)));
                    //}
                    //else
                    //{
                    //    Destroy(GameObject.Find("EnemyHp_" + name.Substring(6, 2)));
                    //}
                    EnemyManager.instance.enemyHpBar.SetActive(false);

                    if (EnemyManager.instance.killCnt >= EnemyManager.instance.maxSpawnCnt)
                    {
                        Debug.Log("stage clear");
                        GameManager.instance.GateGen();
                    }

                    // 경험치
                    GameManager.instance.ExpCall(EnemyManager.instance.enemysExp[0]);

                    SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.bloodDeath, 0, SoundManager.instance.sfxVolum);

                    // 코인 획득
                    GameManager.instance.CoinAdd(EnemyManager.instance.enemyCoinAmount[Random.Range(0, EnemyManager.instance.enemyCoinAmount.Length)]);

                    break;
                }
        }
    }

    void MonsterAwake()
    {
        _ani.SetBool("isIdle", true);
        _ani.SetBool("isAttack", false);
        _ani.SetBool("isMove", false);

        transform.position = enemyOrigin;                   // 위치 복귀
        this.GetComponent<SWS.splineMove>().StartMove();    // 웨이포인트 복구

        hp = maxHp;
    }

    // 모델링 이벤트 호출 함수
    void IsAtkToRun()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > attackbleRange)
        {
            zombieState = ZOMBIESTATE.MOVE;
        }
    }

    void EnemyDamage()
    {
        hp -= CryptoPlayerPrefs.GetInt("playerAtk");

        zombieState = ZOMBIESTATE.DAMAGE;
        _ani.SetTrigger("hit");

        this.GetComponent<SWS.splineMove>().Pause();

        Invoke("IdleState", 0.5f);

        SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.bloodStabHit, 0, SoundManager.instance.sfxVolum);
    }

    void IdleState()
    {
        zombieState = ZOMBIESTATE.IDLE;
        _ani.SetBool("isIdle", true);
        _ani.SetBool("isAttack", false);
        _ani.SetBool("isMove", false);
        _ani.SetBool("isWalk", false);

        Invoke("ToResum", 1.0f);   // 1초후 재검사

        //this.GetComponent<SWS.splineMove>().Pause();
    }

    void ToResum()
    {
        if (_ani.GetBool("isIdle"))
        {
            _ani.SetBool("isIdle", false);
            _ani.SetBool("isWalk", true);

            this.GetComponent<SWS.splineMove>().Resume();
        }
    }

    void EnemyInfo()
    {
        EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueMax(maxHp);
        EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueMin(0);
        EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);

        /*
        if (name.Length == 7)
        {
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueMax(maxHp);
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueMin(0);
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueCurrent(hp);
        }
        else
        {
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueMax(maxHp);
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueMin(0);
            GameObject.Find("EnemyHp_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueCurrent(hp);
        }
        */
    }

    // 하울링 효과음
    IEnumerator ZombieAtkSnd()
    {
        if (isAtkSnd == false)
        {
            isAtkSnd = true;

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.zombieAkt, 0, SoundManager.instance.sfxVolum);

            yield return new WaitForSeconds(4.0f);

            isAtkSnd = false;
        }
    }
}
