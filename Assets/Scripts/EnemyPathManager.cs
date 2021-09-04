using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    // 패스 정보가 담겨 있는 객체, SWS 네임스페이스로부터 설정
    public SWS.PathManager[] enemyPath;

    public static EnemyPathManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
