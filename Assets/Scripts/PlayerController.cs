using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform    _bullet;
    public Transform    _spPoint;
    public Transform    _mzPoint;

    bool                isFire = false;     // 코루틴 제어플래그

    public bool isAttack = false;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        SearchEnemy();      // 적 감지 호출 (하나의 HP바를 사용할 때 사용)

        if (Input.GetMouseButton(0))
        {
            isAttack = true;
            StartCoroutine(FireBullet());
        }
        else
            isAttack = false;
    }

    IEnumerator FireBullet()
    {
        if (!isFire)
        {
            isFire = true;

            Instantiate(_bullet, _spPoint.position, _spPoint.rotation);
            MuzzleEffect();

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.fireSnd, 0, SoundManager.instance.gunVolum);

            yield return new WaitForSeconds(0.1f);

            isFire = false;
        }
    }

    // 머즐 이펙트
    void MuzzleEffect()
    {
        // 생성 위치값은 이펙트의 코드에서 갱신
        Destroy(Instantiate(ParticleManager.instance.muzzleImpact), 0.2f);
    }

    // 적 감지
    void SearchEnemy()
    {
        Debug.DrawRay(_spPoint.position, _spPoint.forward * 45, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(_spPoint.position, _spPoint.forward, out hit, 45) == true)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyManager.instance.enemyHpBar.SetActive(true);
                EnemyManager.instance.enemyHpBar.GetComponent<EnergyBarToolkit.EnergyBarFollowObject>().followObject = hit.transform.gameObject;
                hit.transform.SendMessage("EnemyInfo", SendMessageOptions.DontRequireReceiver);
            }
            else EnemyManager.instance.enemyHpBar.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gate")
        {
            // 현재층을 클리어층과 비교 후 높을 때만 셋팅
            if(CryptoPlayerPrefs.GetInt("currentStage") > CryptoPlayerPrefs.GetInt("clearStage"))
            {
                CryptoPlayerPrefs.SetInt("clearStage", CryptoPlayerPrefs.GetInt("clearStage") + 1);
            }
           
            Destroy(other.gameObject); // 중복방지를 위한 게이트 제거

            SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        }
    }
}
