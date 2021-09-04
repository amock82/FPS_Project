using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;    // 적 프리팹
    public float spawnTime = 2.0f;  // 리젠 시간
    int spawnCnt = 0;
    public int maxSpawnCnt = 3;    // 총 생성 갯수

    bool isSpawn = false;   // 코루틴 제어 플래그

    public int killCnt = 0; // 킬 수 체크

    public GameObject enemyKillBar;
    public GameObject enemyHpBar;

    public float[] enemysExp;   // 적 경험치 관리
    public int[] enemyCoinAmount;   // 적 코인 관리

    public static EnemyManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        enemyKillBar.GetComponent<EnergyBar>().SetValueMax(maxSpawnCnt);
        enemyKillBar.GetComponent<EnergyBar>().SetValueMin(0);
        enemyKillBar.GetComponent<EnergyBar>().SetValueCurrent(maxSpawnCnt);
    }

    void Update()
    {
        if (spawnCnt >= maxSpawnCnt)
            return;

        StartCoroutine(EnemySpawn());
    }

    // 적 생성  프로세스
    IEnumerator EnemySpawn()
    {
        if (isSpawn == false)
        {
            isSpawn = true;

            GameObject enemyObj = Instantiate(enemy);

            // 독립적인 에너지바를 가지고 생성
            //GameObject enemyHp = Instantiate(enemyHpBar);
            //enemyHp.name = "EnemyHp_" + spawnCnt;

            //enemyHp.transform.SetParent(GameObject.Find("Canvas").transform);
            //enemyHp.GetComponent<EnergyBarToolkit.EnergyBarFollowObject>().followObject = enemyObj;

            float x = Random.Range(-20.0f, 20.0f);
            float z = Random.Range(-20.0f, 20.0f);

            enemyObj.transform.position = new Vector3(x, 0.1f, z);

            enemyObj.name = "Enemy_" + spawnCnt;

            yield return new WaitForSeconds(spawnTime);

            isSpawn = false;

            spawnCnt++;
        }
    }

    public void PauseToResume(GameObject obj)
    {
        obj.GetComponent<SWS.splineMove>().Pause();     // 웨이포인트 일시정지

        StartCoroutine(ResumeToPause(1, obj));

        obj.GetComponent<Animator>().SetBool("isIdle", true);
        obj.GetComponent<Animator>().SetBool("isWalk", false);
    }

    IEnumerator ResumeToPause(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);

        obj.GetComponent<SWS.splineMove>().Resume();    // 위이포인트 일시정지 해제

        obj.GetComponent<Animator>().SetBool("isIdle", false);
        obj.GetComponent<Animator>().SetBool("isWalk", true);
    }
}
