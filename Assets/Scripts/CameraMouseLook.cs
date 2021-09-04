using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    public float    sens = 500f;   //회전감도
    float rotationX = 0;
    float rotationY = 0;

    void Start()
    {
        
    }

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotationX += x * sens * Time.deltaTime;
        rotationY += y * sens * Time.deltaTime;

        if(rotationY < -80)
        {
            rotationY = -80;
        }
        else if (rotationY > 80)
        {
            rotationY = 80;
        }

        // 회전값은 일괄 변경, 오일러공식 등을 이용
        transform.eulerAngles = new Vector3(-rotationY, rotationX, 0);
    }
}
