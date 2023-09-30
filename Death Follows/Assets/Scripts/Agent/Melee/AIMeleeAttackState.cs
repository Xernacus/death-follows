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
    private TagHitResponder _hitResponder;
    public Animator animator;
    private bool _attacking = false;
    //private Vector3 _pastLocation2;

    public void Enter(AIAgent agent)
    {
        animator = agent.gameObject.GetComponentInChildren<Animator>();
        _agent = agent.gameObject.GetComponent<NavMeshAgent>();
        _sensor = agent.gameObject.GetComponent<AISensor>();
        _target = GameObject.FindGameObjectWithTag("Player");
        _playerController = _target.GetComponent<PlayerController>();
        _type = Random.Range(0,2);
        Debug.Log(_type);
        _pastLocation1 = _target.transform.position;
        _sensor.distance = 3f;
        _sensor.angle = 45f;
        _hitResponder = agent.gameObject.GetComponentInChildren<TagHitResponder>();
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

            _scanTimer = 0.5f;
            _sensor.Scan();
            GameObject[] player = _sensor.Filter(new GameObject[1], "Player");
            if (player[0] != null && _attacking != true)
            {
                Debug.Log("Attack");
                AttemptAttack();
                animator.Play("Stab", 0, 0.0f);
                _hitResponder.hitTracker._objectsHit = new List<GameObject>();
                _attacking = true;
            }

        }
        if (_attacking == true)
        {
            AttemptAttack();
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Run") && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Run"))
            {
                animator.Play("Run", 0, 0.0f);
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
        int id = Animator.StringToHash("Stab");
        if (animator.HasState(0, id))
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            //if (state.fullPathHash == id || state.shortNameHash == id)
            //{
                _attacking = true;
                int totalFrames = GetTotalFrames(animator, 0);

                int currentFrame = GetCurrentFrame(totalFrames, GetNormalizedTime(state));
                if (currentFrame > 16 && currentFrame < 21)
                {
                    _hitResponder._hitBox.CheckHit();
                }
                else if(currentFrame > 29)
            {
                _attacking = false;
            }
                return;
            //}
            

        }
        
    }

    private int GetTotalFrames(Animator animator, int layerIndex)
    {
        AnimatorClipInfo[] _clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);
        if (_clipInfos.Length == 0)
        {
            _clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
        }

        AnimationClip clip = _clipInfos[0].clip;
        return Mathf.RoundToInt(clip.length * clip.frameRate);
    }

    private float GetNormalizedTime(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime > 1 ? 1 : stateInfo.normalizedTime;
    }

    private int GetCurrentFrame(int totalFrames, float normalizedTime)
    {
        return Mathf.RoundToInt(totalFrames * normalizedTime);
    }

}
