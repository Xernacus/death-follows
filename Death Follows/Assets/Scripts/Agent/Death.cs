using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Death : MonoBehaviour
{
    private GameObject _target;
    private float _offsetDistance = 3f;
    private float _teleportTimer = 0f;
    private float _updateTimer = 0f;
    private float _teleportCooldown = 15f;
    private float _cooldown = 2f;
    protected bool _attacking = false;
    protected bool _charging = false;
    private float _moveSpeed = 10f;
    protected int _attack;
    private Vector3 _offset;
    private Animator _animator;
    private BasicHitResponder _hitResponder;

    public GameObject art;
    public HitBox _hitbox;
    public float slashDistance = 1f;
    public float dashDistance = 3f;
    void Start()
    {
        Debug.Log("Attack");
        _target = GameObject.FindGameObjectWithTag("Player");
        //agent.config.destination = _target.transform;
        _animator = GetComponentInChildren<Animator>();
        //_hitResponder = _hitbox.GetComponent<BasicHitResponder>();
    }


    public void Update()
    {
        if (!Teleporting())
        {           
            if (!_charging)
            {
                if (!_attacking)
                {
                    if (_attack == 2)
                    {                      
                        StartCoroutine(Slash());
                    }
                    else if (_attack == 1)
                    {
                        StartCoroutine(Dash());
                    }
                    else if (_attack == 0)
                    {
                        StartCoroutine(Slash());
                    }
                }                
            }
            else
            {
                transform.position = _target.transform.position + _offset;
                transform.LookAt(_target.transform.position);
            }
        }
        else
        {
            _teleportTimer += Time.deltaTime;

        }
        

    }

    private void GetOffset()
    {
        Debug.Log("did a thing");
        _offset = Vector3.ClampMagnitude(new Vector3(Random.Range(-1, 2) * _offsetDistance, 0, Random.Range(-1, 2) * _offsetDistance), _offsetDistance);
        if (_offset.magnitude == 0)
        {
            _offset = Vector3.ClampMagnitude(new Vector3(-_offsetDistance, 0, -_offsetDistance), _offsetDistance);
        }
    }

    IEnumerator Windup()
    {
        _charging = true;
        switch (_attack)
        {
            case 1:
            case 2:
            case 3:
            default:
                break;
        }
            

        yield return new WaitForSeconds(_cooldown);

        _charging = false;
    }

    private bool Teleporting()
    {
        if (_teleportTimer >= 0f)
        {
            if (_teleportTimer > 2f)
            {
                _teleportTimer = -1f;
                _attack = Random.Range(0, 3);
                Debug.Log(_attack);
                art.SetActive(true);
                StartCoroutine(Windup());
            }
            else
            {
                return true;
            }           
        }
        return false;
    }
    private void Teleport()
    {
        _teleportTimer = 0f;
        GetOffset();
        art.SetActive(false);
        return;
    }

    IEnumerator Slash()
    {
        _attacking = true;
        while (_updateTimer < 0.5)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);
            _updateTimer += Time.deltaTime;
            yield return null;
        }
        _attacking = false;
        _updateTimer = 0f;
        Teleport();
        
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

    IEnumerator Dash()
    {
        _attacking = true;
        while (_updateTimer < 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);//transform.position = Vector3.MoveTowards(transform.position, _marker, Time.deltaTime * _moveSpeed);
            _updateTimer += Time.deltaTime;
            yield return null;
        }
        _updateTimer = 0f;
        _attacking = false;
        Teleport();
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
