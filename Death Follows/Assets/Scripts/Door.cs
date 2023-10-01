using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    private bool open;
    public int openCount;
    EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        animator = GetComponent<Animator>();
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
        }       
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Open();
        }
        
    }
}
