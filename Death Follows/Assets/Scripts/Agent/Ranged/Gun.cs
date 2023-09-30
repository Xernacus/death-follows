using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    public GameObject gun;

    public void Shoot()
    {
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, gun.transform.position, gun.transform.rotation);
        }

        Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
    }
}
