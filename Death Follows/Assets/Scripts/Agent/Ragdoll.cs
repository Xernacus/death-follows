using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Collider[] colliders;
    public int health;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponentInChildren<Animator>();

        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        foreach (var collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
            }
            
        }        
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        animator.enabled = false;
    }

    public void Damage(int damage)
    {
        Debug.Log("Enemy got damaged");
        health -= damage;
        if (health <= 0)
        {
            ActivateRagdoll();
        }
        GameObject.Destroy(this.gameObject, 2f);
    }
}
