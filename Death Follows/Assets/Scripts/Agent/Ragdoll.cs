using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    public int health;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //ActivateRagdoll();
        }
        GameObject.Destroy(this.gameObject, 2f);
    }
}
