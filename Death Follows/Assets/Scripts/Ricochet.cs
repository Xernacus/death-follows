using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricochet : MonoBehaviour
{
    [SerializeField] GameObject _hitbox;
    private RicochetHitResponder _hitResponder;
    private float _gracePeriod;

    public void Start()
    {
        _hitResponder = _hitbox.GetComponent<RicochetHitResponder>();
    }

    public void Update()
    {
        _gracePeriod += Time.deltaTime;
        if (_gracePeriod >= 0.25)
        {
            _hitResponder._hitBox.CheckHit();
        }      
    }
}
