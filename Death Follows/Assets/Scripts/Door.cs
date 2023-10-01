using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    private bool open;
    public int openCount;
    EnemyManager enemyManager;
    BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (enemyManager.enemies <= openCount)
        {
            Open();
        }
    }
    public void Open()
    {
        if (!open)
        {
            animator.Play("Door", 0, 0.0f);
            open = true;
            collider.enabled = false;
        }       
    }

}
