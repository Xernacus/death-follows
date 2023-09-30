using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;
    private GameObject muzzleFlashClone;
    public GameObject gun;

    public AudioSource audioSource;
    public AudioClip gunSound;

    public void Shoot()
    {
        if (muzzleFlash != null)
        {
            UnityEngine.Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(gunSound, 1f);
            muzzleFlashClone = Instantiate(muzzleFlash, gun.transform.position, gun.transform.rotation);
            Destroy(muzzleFlashClone, 1f);
        }

        Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation * bulletPrefab.transform.rotation);
    }
}
