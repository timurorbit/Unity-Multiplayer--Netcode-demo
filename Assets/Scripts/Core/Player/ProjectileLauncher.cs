using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    
    [SerializeField] private InputReader inputReader;

    [SerializeField] private CoinWallet wallet;

    [SerializeField] private Transform projectileSpawnPoint;
    
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    
    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")] [SerializeField] private float projectileSpeed;

    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;

    private bool isPointerOverUI;

    private bool shouldFire;
    
    private float timer;
    
    private float muzzleFlashTimer;
    private void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }
        
        if (!IsOwner)
        {
            return;
        }

        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!shouldFire)
        {
            return;
        }

        if (timer > 0)
        {
            return;
        }

        if (wallet.totalCoins.Value < costToFire)
        {
            return;
        }
        
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        
        timer = 1 / fireRate;
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        
       GameObject projectile = Instantiate(clientProjectilePrefab, spawnPosition, Quaternion.identity);

       projectile.transform.up = direction;
       
       Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());

       if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
       {
           rb.velocity = rb.transform.up * projectileSpeed;
       }
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (wallet.totalCoins.Value < costToFire)
        {
            return;
        }
        
        wallet.SpendCoins(costToFire);
        
        GameObject projectile = Instantiate(serverProjectilePrefab, spawnPosition, Quaternion.identity);
        projectile.transform.up = direction;

        SpawnDummyProjectileClientRpc(spawnPosition, direction);
        
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());

        if (projectile.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamageOnContact))
        {
            dealDamageOnContact.SetOwner(OwnerClientId);
        }
        
        if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
    
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (IsOwner)
        {
            return;
        }
        SpawnDummyProjectile(spawnPosition, direction);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        if (shouldFire)
        {
            if (isPointerOverUI)
            {
                return;
            }
        }
        this.shouldFire = shouldFire;
    }
}
