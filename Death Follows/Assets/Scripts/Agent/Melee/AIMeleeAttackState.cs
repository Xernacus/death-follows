using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMeleeAttackState : AIState
{
    private NavMeshAgent _agent;
    private GameObject _target;
    private PlayerController _playerController;
    private AISensor _sensor;
    private float _updateTimer;
    private float _scanTimer;
    private int _type;
    private Vector3 _pastLocation1;
    //private Vector3 _pastLocation2;

    public void Enter(AIAgent agent)
    {       
        _agent = agent.gameObject.GetComponent<NavMeshAgent>();
        _sensor = agent.gameObject.GetComponent<AISensor>();
        _target = GameObject.FindGameObjectWithTag("Player");
        _playerController = _target.GetComponent<PlayerController>();
        _type = Random.Range(0,2);
        Debug.Log(_type);
        _pastLocation1 = _target.transform.position;
        _sensor.distance = _sensor.distance / 2;
        _sensor.angle = 45f;
        //_pastLocation2 = _target.transform.position;
    }

    public void Exit(AIAgent agent)
    {

    }
    public AIStateID GetID()
    {
        return AIStateID.MeleeAttack;
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
            if (_type == 0)
            {
                _agent.destination = FindBehindPath(agent);
            }
            if (_type == 1)
            {
                _agent.destination = FindAheadPath(agent);
            }
            _updateTimer = 0.25f;
        }

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

    private Vector3 FindBehindPath(AIAgent agent)
    {
        Vector3 destination = _pastLocation1;
        //_pastLocation1 = _pastLocation2;
        _pastLocation1 = _target.transform.position;
        return destination;
    }

    private Vector3 FindAheadPath(AIAgent agent)
    {
        Vector3 ahead = new Vector3(_playerController.movementDirection.x * Time.deltaTime * _playerController.moveSpeed * 120, 0, _playerController.movementDirection.y * Time.deltaTime * _playerController.moveSpeed * 120);
        Vector3 destination = _target.transform.position + ahead;
       return destination;
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
