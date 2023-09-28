using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricochet : MonoBehaviour
{
    [SerializeField] GameObject _hitbox;
    private RicochetHitResponder _hitResponder;

    public void Start()
    {
        _hitResponder = _hitbox.GetComponent<RicochetHitResponder>();
    }

    public void Update()
    {
        _hitResponder._hitBox.CheckHit();
    }
}
