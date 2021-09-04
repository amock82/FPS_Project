using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform _tr;
    Rigidbody _rig;

    float bulletSpeed = 1700;

    void Start()
    {
        _tr = transform;     // 자기자신 참조
        _rig = GetComponent<Rigidbody>();

        _rig.AddForce(_tr.forward * bulletSpeed, ForceMode.Force);
        Destroy(_tr.gameObject, 1.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 rndRot = Vector3.right * Random.Range(200, 360);

        Destroy(this);

        int colLayer = collision.gameObject.layer;

        //Debug.Log(colLayer);

        if(colLayer == LayerMask.NameToLayer("Stage"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.concImpact, _tr.position, _tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);
        }
        else if (colLayer == LayerMask.NameToLayer("Ground"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.sandImpact, _tr.position, _tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);
        }
        else if (colLayer == LayerMask.NameToLayer("Enemy"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.bloodImpact, _tr.position, _tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);

            collision.gameObject.SendMessage("EnemyDamage", SendMessageOptions.DontRequireReceiver);
        }
    }


    void Quaterniion()  // 학습용, 사용하지 않음
    {
        // Quaternion
        // 유니티의 각도는 Quaternion과 EulerAngles의 두가지 체계가 있다.
        // Quaternion은(사원수) 복소수를 이용한 4개의 실수(x, y, z, w)로 유니티는
        // 각도계산에 쿼터니언을 사용한다.
        // 쿼터니언의 값은 읽기 전용으로 개발자가 이 값을 직접 변경할 수가 없다.

        Quaternion aAng = _tr.rotation;
        Vector3 qV = Vector3.right * 40;
        _tr.transform.localRotation = Quaternion.Euler(qV);

        // EulerAngles
        // 오일러 각도는 60분법으로 (x, y, z)의 3차원 벡터이다. 우리가 사용했던
        // 인스펙터에서의 Rotation 속성은 오일러 각도로 표시된다.
        // 오브젝트의 회전을 60분법 각도로 지시할 경우 EulerAngles를 이용한다.
        // 시계방향이 양(+)의 회전이다.

        Vector3 ang_a = transform.eulerAngles;  // 오일러 각도 읽기 예시

        ang_a.x = 20;
        ang_a.x = 30;
        ang_a.x = 50;

        _tr.eulerAngles = new Vector3(_tr.rotation.x, 60, _tr.rotation.z);

        Vector3 _ang = _tr.eulerAngles;
        _ang.x = 20;
        _tr.eulerAngles = _ang;
    }

}
