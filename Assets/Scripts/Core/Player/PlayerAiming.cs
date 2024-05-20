using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    
    [SerializeField] private Transform turretTransform;

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        
        Vector2 aimPointWorld = Camera.main.ScreenToWorldPoint(inputReader.AimPosition);
       turretTransform.up = new Vector2(
         aimPointWorld.x -  turretTransform.position.x,
         aimPointWorld.y -  turretTransform.position.y);
    }
}
