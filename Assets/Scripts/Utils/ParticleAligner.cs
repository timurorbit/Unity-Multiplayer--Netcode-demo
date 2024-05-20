using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule particleMain;
    
    private void Start()
    {
        particleMain = GetComponent<ParticleSystem>().main;
    }

    private void Update()
    {
        particleMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
