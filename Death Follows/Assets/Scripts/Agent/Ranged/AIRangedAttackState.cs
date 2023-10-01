using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRangedAttackState : AIState
{
    private NavMeshAgent _agent;
    private GameObject _target;
    private PlayerController _playerController;
    private AISensor _sensor;
    private Gun _gun;
    private float _updateTimer;
    private float _scanTimer;
    private float _turnSpeed = 4f;
    public Animator animator;
    public float fireCooldown = 2.2f;

    public void Enter(AIAgent agent)
    {
        animator = agent.gameObject.GetComponentInChildren<Animator>();
        _gun = agent.gameObject.GetComponent<Gun>();
        _agent = agent.gameObject.GetComponent<NavMeshAgent>();
        _sensor = agent.gameObject.GetComponent<AISensor>();
        _target = GameObject.FindGameObjectWithTag("Player");
        _playerController = _target.GetComponent<PlayerController>();
        _sensor.distance = _sensor.distance * 2;
        _sensor.angle = 20f;
        //_agent.updateRotation = false;
    }

    public void Exit(AIAgent agent)
    {

    }
    public AIStateID GetID()
    {
        return AIStateID.RangedAttack;
    }

    public void Update(AIAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }

        
        
        _updateTimer -= Time.deltaTime;

        if (_updateTimer < 0)
        {
            GetDestination(agent);
            _updateTimer = 0.25f;
        }
        
        if (_agent.isStopped)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Shoot") && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Shoot"))
            {
                animator.Play("Aim", 0, 0.0f);
            }
            agent.gameObject.transform.forward = Vector3.Slerp(agent.gameObject.transform.forward, (_target.transform.position - agent.gameObject.transform.position).normalized, Time.deltaTime * _turnSpeed);

            _scanTimer += Time.deltaTime;
            if (_scanTimer > fireCooldown)
            {

                _scanTimer = 0f;
                _sensor.Scan();
                GameObject[] player = _sensor.Filter(new GameObject[1], "Player");
                if (player[0] != null)
                {
                    animator.Play("Shoot", 0, 0.0f);
                    _gun.Shoot();
                }

            }
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Run") && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Run"))
            {
                animator.Play("Run", 0, 0.0f);
            }
        }
        
    }

    

    private void GetDestination(AIAgent agent)
    {
        if ((agent.gameObject.transform.position - _target.transform.position).magnitude > agent.config.maxDistance)
        {            
            _agent.isStopped = false;
            _agent.destination = _target.transform.position;
        }
        else if ((agent.gameObject.transform.position - _target.transform.position).magnitude < agent.config.minDistance)
        {
            _agent.isStopped = false;
            Vector3 position = agent.gameObject.transform.position;
            Vector3 direction = (position - _target.transform.position).normalized;

            float angle = Random.Range(-agent.config.avoidAngle, agent.config.avoidAngle);
            float distance = agent.config.minDistance* 1.5f * Random.Range(0, agent.config.avoidAngle);
            direction = Quaternion.Euler(0, angle, 0) * direction * distance;

            _agent.destination = direction + position;
        }
        else
        {
            _agent.isStopped = true;           
        }
    }

    
    

}
