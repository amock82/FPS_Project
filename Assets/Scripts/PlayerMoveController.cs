using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public float        moveSpeed = 10.0f;
    public float        jumpSpeed = 10.0f;

    public float        gravity = 20.0f;

    public Transform    _cameraPos;

    CharacterController _cc;
    float               yVelocity = 0;

    Vector3             moveDir = Vector3.zero;
 
    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        //Move();
    }

    // 카메라 update
    private void LateUpdate()
    {
        Camera.main.transform.position = _cameraPos.position;
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDir.x = x;
        moveDir.y = 0;
        moveDir.z = z;

        // TransformDirection 은 로컬의 이동벡터를 씬의 글로벌 값으로 변환
        moveDir = this.transform.TransformDirection(moveDir);

        moveDir *= moveSpeed;

        yVelocity -= gravity * Time.deltaTime;
        moveDir.y = yVelocity;

        _cc.Move(moveDir * Time.deltaTime);

        // 플레이어의 회전값을 카메라 회전에 맞춤
        this.transform.rotation = Camera.main.transform.rotation;
    }

    void Jump()
    {
        if (_cc.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
            }
        }
    }

    // 캐릭터 컨트롤러 충돌 체크
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log(hit.gameObject.name);
    }
}
