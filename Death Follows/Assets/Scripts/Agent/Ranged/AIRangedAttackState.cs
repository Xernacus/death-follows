using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class AIRangedAttackState : AIState
{
    private NavMeshAgent _agent;
    private GameObject _target;
    private PlayerController _playerController;
    private AISensor _sensor;
    private float _updateTimer;
    private float _scanTimer;
    private float _turnSpeed = 1f;
    

    public void Enter(AIAgent agent)
    {
        _agent = agent.gameObject.GetComponent<NavMeshAgent>();
        _sensor = agent.gameObject.GetComponent<AISensor>();
        _target = GameObject.FindGameObjectWithTag("Player");
        _playerController = _target.GetComponent<PlayerController>();
        _sensor.distance = _sensor.distance * 2;
        _sensor.angle = 20f;
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
            agent.gameObject.transform.forward = Vector3.Slerp(agent.gameObject.transform.forward, (_target.transform.position - agent.gameObject.transform.forward).normalized, Time.deltaTime * _turnSpeed);

            _scanTimer -= Time.deltaTime;

            if (_scanTimer < 0)
            {

                _scanTimer = 0.33f;
                _sensor.Scan();
                GameObject[] player = _sensor.Filter(new GameObject[1], "Player");
                if (player[0] != null)
                {
                    Debug.Log("Attack");
                    AttemptAttack();
                }

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
            float distance = agent.config.minDistance * Random.Range(0, agent.config.avoidAngle);
            direction = Quaternion.Euler(0, angle, 0) * direction * distance;

            _agent.destination = direction + position;
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    private void AttemptAttack()
    {
        /**
        int id = Animator.StringToHash("Slash");
        if (_animator.HasState(0, id))
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            while (state.fullPathHash == id || state.shortNameHash == id)
            {
                _attacking = true;
                int totalFrames = GetTotalFrames(_animator, 0);

                int currentFrame = GetCurrentFrame(totalFrames, GetNormalizedTime(state));
                if (currentFrame > 64 && currentFrame < 90)
                {
                    _hitResponder._hitBox.CheckHit();
                }
                return;
            }
            if (_attacking == true)
            {
                Teleport();
                _attacking = false;
            }
            _animator.Play("Slash", 0, 0.0f);
            _hitResponder._objectsHit = new List<GameObject>();
        }
        **/
    }

}
