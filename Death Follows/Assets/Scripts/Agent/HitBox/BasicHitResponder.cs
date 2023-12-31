using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHitResponder : MonoBehaviour, IHitResponder
{
    [SerializeField] private int _damage = 10;
    [SerializeField] public HitBox _hitBox;
    public HitTracker hitTracker;
    

    int IHitResponder.Damage { get => _damage; }
    // Start is called before the first frame update
    void Start()
    {
        _hitBox.HitResponder = this;
    }

    //Should be called whenever attack
    bool IHitResponder.CheckHit(HitData data)
    {
        if (hitTracker._objectsHit.Contains(data.hurtBox.Owner))
        {
            return false;
        }       
        return true; 
    }
    void IHitResponder.Response(HitData data)
    {
        hitTracker._objectsHit.Add(data.hurtBox.Owner);
    }
}
