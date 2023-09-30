using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject _hitbox;
    public float _speed;
    public float _lifetime = 2f;

    private DestroyHitResponder _hitResponder;
    private Rigidbody _rb;

    private void Start()
    {
        _hitResponder = _hitbox.GetComponent<DestroyHitResponder>();


        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, _lifetime);

        _rb.velocity = gameObject.transform.forward * _speed;
    }

    public void Update()
    {
        _hitResponder._hitBox.CheckHit();
    }
   
}
