using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    public Camera shakeCamera;

    public bool isShake = false;    // 쉐이크 상태 체크

    public static ShakeManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        StartCoroutine(ShakeCamera(0.3f, 0.05f, 0.03f, 0f));    // 카메라 쉐이크 동작
    }

    IEnumerator ShakeCamera(float endTime, float senX, float senY, float senZ)
    {
        if (isShake == true)
        {
            Vector3 pos = Vector3.zero;

            pos.x = Random.Range(-senX, senX);
            pos.x = Random.Range(-senY, senY);
            pos.x = Random.Range(-senZ, senZ);

            shakeCamera.transform.position += pos;

            yield return new WaitForSeconds(endTime);

            isShake = false;

            // 선형 보간 함수를 활용하여 위치 복귀
            //shakeCamera.transform.localPosition = Vector3.Lerp(shakeCamera.transform.localPosition, Vector3.zero, Time.deltaTime * 10f);
        }
    }
}
