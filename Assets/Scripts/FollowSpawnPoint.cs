using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSpawnPoint : MonoBehaviour
{
    Transform tr;

    private void Awake()
    {
        tr = transform;
    }

    void Start()
    {
        
    }

    void Update()
    {
        tr.position = PlayerController.instance._mzPoint.position;
        tr.rotation = PlayerController.instance._mzPoint.rotation;
    }
}
