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
    private float _undodgeableTimer = 0f;
    private float _cooldown = 2f;
    protected bool _attacking = false;
    protected bool _charging = false;
    private float _moveSpeed = 10f;
    protected int _attack;
    private Vector3 _offset;
    private Animator _animator;
    public ParticleSystem particles;
    public GameObject art;
    public HitTracker hitTracker;

    #region Hitboxes
    public BasicHitResponder leftSlashHitbox;
    public BasicHitResponder rightSlashHitbox;
    public BasicHitResponder backDashHitbox;
    public BasicHitResponder frontDashHitbox;
    public BasicHitResponder undodgeableHitbox;
    #endregion

    public float slashDistance = 1f;
    public float dashDistance = 3f;
    void Start()
    {
        Debug.Log("Attack");
        _target = GameObject.FindGameObjectWithTag("Player");
        _animator = GetComponentInChildren<Animator>();
        GetOffset();
        art.SetActive(false);
        var emission = particles.emission;
        emission.enabled = false;
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
                    if (_attack == 0)
                    {                      
                        StartCoroutine(Slash());
                    }
                    else if (_attack == 1)
                    {
                        StartCoroutine(Dash());
                    }
                    else if (_attack == 2)
                    {                       
                        StartCoroutine(Undodgeable());
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
        _undodgeableTimer += Time.deltaTime;

    }

    private void GetOffset()
    {
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
            case 0:
                _cooldown = 0f;
                break;
            case 1:
                _cooldown = 1f;
                break;
            case 2:
                _cooldown = 1f;
                break;

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

                if (_undodgeableTimer > 20f)
                {
                    _attack = Random.Range(0, 3);
                    _undodgeableTimer = 0f;
                }
                else
                {
                    _attack = Random.Range(0, 2);
                }
                

                Debug.Log(_attack);
                art.SetActive(true);
                var emission = particles.emission;
                emission.enabled = true;
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
        var emission = particles.emission;
        emission.enabled = false;
        return;
    }

    IEnumerator Slash()
    {
        int id = Animator.StringToHash("Slash");
        _attacking = true;
        _animator.Play("Slash", 0, 0.0f);
        hitTracker._objectsHit = new List<GameObject>();
        if (_animator.HasState(0, id))
        {
            while (true)
            {
                          
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                if (state.fullPathHash == id || state.shortNameHash == id)
                {
                    int totalFrames = GetTotalFrames(_animator, 0);

                    int currentFrame = GetCurrentFrame(totalFrames, GetNormalizedTime(state));

                    if (currentFrame < 94)
                    {
                        transform.position = _target.transform.position + _offset;
                        transform.LookAt(_target.transform.position);
                    }                   
                    else if (currentFrame > 93 && currentFrame < 101)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);
                        rightSlashHitbox._hitBox.CheckHit();
                    }
                    else if (currentFrame > 100 && currentFrame < 105)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);
                        leftSlashHitbox._hitBox.CheckHit();
                    }
                    else if(currentFrame > 176)
                    {
                        break;
                    }
                    
                }                
                yield return null;
            }
            
        }
        _attacking = false;
        Teleport();
    }

    IEnumerator Dash()
    {
        int id = Animator.StringToHash("Dash");
        _attacking = true;
        _animator.Play("Dash", 0, 0.0f);
        hitTracker._objectsHit = new List<GameObject>();
        if (_animator.HasState(0, id))
        {
            while (true)
            {

                var state = _animator.GetCurrentAnimatorStateInfo(0);
                if (state.fullPathHash == id || state.shortNameHash == id)
                {
                    int totalFrames = GetTotalFrames(_animator, 0);

                    int currentFrame = GetCurrentFrame(totalFrames, GetNormalizedTime(state));

                    if (currentFrame < 48)
                    {
                        transform.position = _target.transform.position + _offset;
                        transform.LookAt(_target.transform.position);
                    }                   
                    else if (currentFrame > 47 && currentFrame < 52)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);
                        backDashHitbox._hitBox.CheckHit();
                    }
                    else if (currentFrame > 51 && currentFrame < 58)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * _moveSpeed);
                        frontDashHitbox._hitBox.CheckHit();
                    }
                    else if (currentFrame > 71)
                    {
                        break;
                    }
                }
                yield return null;
            }

        }   
        _attacking = false;
        Teleport();
    }
    /**
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
}**/

    IEnumerator Undodgeable()
    {
        Debug.Log("Undodge Attack");
        int id = Animator.StringToHash("Dodge3");
        _attacking = true;
        _animator.Play("Dodge1", 0, 0.0f);
        hitTracker._objectsHit = new List<GameObject>();
        
        if (_animator.HasState(0, id))
        {
            while (true)
            {
                _updateTimer += Time.deltaTime;
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                if (_updateTimer > 5f && !(state.fullPathHash == id || state.shortNameHash == id))
                {
                    _animator.Play("Dodge3", 0, 0.0f);
                }                
                if (state.fullPathHash == id || state.shortNameHash == id)
                {
                    int totalFrames = GetTotalFrames(_animator, 0);

                    int currentFrame = GetCurrentFrame(totalFrames, GetNormalizedTime(state));

                    if (currentFrame > 3 && currentFrame < 35)
                    {
                        undodgeableHitbox._hitBox.CheckHit();
                    }                   
                    else if (currentFrame > 34)
                    {
                        break;
                    }
                }
                else
                {
                    transform.position = _target.transform.position + _offset;
                    transform.LookAt(_target.transform.position);
                }
                yield return null;
            }

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
