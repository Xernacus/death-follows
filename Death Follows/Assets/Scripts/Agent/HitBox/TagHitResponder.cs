using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagHitResponder : MonoBehaviour, IHitResponder
{
    [SerializeField] private int _damage = 10;
    [SerializeField] public HitBox _hitBox;
    public HitTracker hitTracker;
    public string targetTag = null;

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
        if (targetTag != null && !data.hurtBox.Owner.CompareTag(targetTag))
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
