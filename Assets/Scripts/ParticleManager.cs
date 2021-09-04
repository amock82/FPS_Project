using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject concImpact;
    public GameObject sandImpact;
    public GameObject muzzleImpact;

    public GameObject bloodImpact;

    public GameObject deadEffectImpact;

    public static ParticleManager instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
