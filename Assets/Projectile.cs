using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject _swordSpawn;
    public GameObject _projectile;
    public float _projectileForce;

    public void Shoot()
    {
        GameObject SpawnedProjectile;
        SpawnedProjectile = Instantiate(_projectile, _swordSpawn.transform.position, _swordSpawn.transform.rotation) as GameObject;

        Rigidbody RB;
        RB = SpawnedProjectile.GetComponent<Rigidbody>();
        RB.AddForce(transform.up * _projectileForce);
        
        Destroy(SpawnedProjectile, 2.0f);
    }
}