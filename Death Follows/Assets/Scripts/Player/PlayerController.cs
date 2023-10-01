using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int health = 3;
    public GameObject soul;
    public GameObject ricochetHitParticle;
    private GameObject ricochetHitParticleClone;
    public GameObject hitParticle;
    private GameObject hitParticleClone;
    public ParticleSystem dodgeParticles;
    public GameObject art;
    public GameObject death;
    public Image damageOverlay;
    public Vector2 movementDirection { get; private set; }
    private Vector2 _lookDirection;

    public AudioSource audioSource;
    public AudioClip dodgeSound;
    public AudioClip hurtSound;

    private bool _dashing = false;
    public float dashLength = 0.75f;
    public float dashTimer = 1f;
    private float _dashSpeed = 2f;

    private bool _ricocheting = false;
    public float ricochetSpeed = 8f;

    public Action<Vector2> OnMove;
    public void SubscribeToOnMove(Action<Vector2> callback) => OnMove += callback;
    public void UnsubscribeToOnMove(Action<Vector2> callback) => OnMove -= callback;
       
    PlayerInput _input;
    InputAction _moveAction;
    InputAction _lookAction;
    InputAction _dashAction;

    Rigidbody[] rigidbodies;
    public Animator animator;

    public Image fadeOverlay;

    private CharacterController _characterController;

    void OnEnable()
    {
        _input = GetComponent<PlayerInput>();
        _moveAction = _input.actions["Move"];
        _lookAction = _input.actions["Look"];
        _dashAction = _input.actions["Dash"];
        SubInput();

        var dodgeEmission = dodgeParticles.emission;
        dodgeEmission.enabled = false;
        rigidbodies = art.GetComponentsInChildren<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
        DeactivateRagdoll();
    }

        void OnDisable() => UnsubInput();

        void Update()
        {
            if (_ricocheting)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * ricochetSpeed);
                return;
            }
            HandleInput();
            HandleMovement();
            if (_dashing)
            {
                return;
            }
            HandleRotation();
    }
             
        void SubInput() => _dashAction.performed += OnDash;
        void UnsubInput() => _dashAction.performed -= OnDash;

        private void HandleInput()
        {
            if (_dashing)
            {
                return;
            }

            movementDirection = _moveAction.ReadValue<Vector2>();
            _lookDirection = _lookAction.ReadValue<Vector2>();
        }

        private void HandleMovement()
        {
        dashTimer -= Time.deltaTime;
        if (_dashing)
        {
            Vector3 _move = transform.forward + new Vector3(movementDirection.x, 0, movementDirection.y) * 0.5f;
            _dashSpeed -= Time.deltaTime*3f*_dashSpeed;
            //transform.position = Vector3.MoveTowards(transform.position, transform.position + _move, Time.deltaTime * moveSpeed * _dashSpeed);
            _characterController.Move(_move * Time.deltaTime * moveSpeed * _dashSpeed);
        }
        else
        {
            if (movementDirection.magnitude > 0 && animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Run") && animator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Run"))
            {
                animator.Play("Run", 0, 0.05f);
            }
            if (movementDirection.magnitude == 0f && animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Idle"))
            {
                animator.Play("Idle", 0, 0.05f);
            }
            Vector3 _move = new Vector3(movementDirection.x, 0, movementDirection.y);
            // transform.position = Vector3.MoveTowards(transform.position, transform.position + _move, Time.deltaTime * moveSpeed);
            _characterController.Move(_move * Time.deltaTime * moveSpeed);
        }

        }
        private void HandleRotation()
        {
        LookAt(new Vector3(gameObject.transform.position.x + movementDirection.x, gameObject.transform.position.y, gameObject.transform.position.z + movementDirection.y));
        /**
            Ray ray = Camera.main.ScreenPointToRay(_lookDirection);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        **/
    }
        private void LookAt(Vector3 point)
        {
            Vector3 lookPoint = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(lookPoint);
        }   
        void OnDash(InputAction.CallbackContext context)
        {
            if (!_dashing && dashTimer < 0f && !_ricocheting)
            {
                dashTimer = 1f;
                StartCoroutine(Dash());
            }           
        }
               
        IEnumerator Dash()
        {
            animator.Play("Roll", 0, 0);
            _dashSpeed = 2f;
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(dodgeSound, 1f);
            var dodgeEmission = dodgeParticles.emission;
            dodgeEmission.enabled = true;
            _dashing = true;
            yield return new WaitForSeconds(dashLength);
            dodgeEmission.enabled = false;
            _dashing = false;
        }
        
    public void Damage(int damage)
    {
        if (!_dashing && !_ricocheting)
        {
            health -= damage;
            hitParticleClone = Instantiate(hitParticle, transform.position + Vector3.up, transform.rotation);
            Destroy(hitParticleClone, 1f);
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(hurtSound, 1f);
            if (health <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(DamageOverlay());
            }
            
        }        
    }
    IEnumerator DamageOverlay()
    {
        while (damageOverlay.color.a < 0.35f)
        {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, damageOverlay.color.a + .04f);
            yield return null;
        }      
        while (damageOverlay.color.a > 0f)
        {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, damageOverlay.color.a - .0025f);
            yield return null;
        }
    }
    public void SoulRicochet()
    {
        Debug.Log("Ricochet Player");
        _ricocheting = true;
        transform.forward = (gameObject.transform.position - death.transform.position).normalized;
        art.SetActive(false);
        // particle and soul
        ricochetHitParticleClone = Instantiate(ricochetHitParticle, transform.position + Vector3.up, transform.rotation);
        Destroy(ricochetHitParticleClone, 1f);
        Instantiate(soul, gameObject.transform);
    }

    public void RicochetHit(GameObject enemyHit)
    {
        _ricocheting = false;
        art.SetActive(true);
        ricochetHitParticleClone = Instantiate(ricochetHitParticle, transform.position + Vector3.up, transform.rotation);
        Destroy(ricochetHitParticleClone, 1f);
        var boss = enemyHit.GetComponent<BossRagdoll>();
        if (boss != null)
        {
            boss.Damage(10);
        }
        else
        {
            Destroy(enemyHit);
        }
        
    }

    public void Die()
    {
        
        StartCoroutine(ScreenFade());

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

    IEnumerator ScreenFade()
    {
        while (fadeOverlay.color.a < 1f)
        {
            gameObject.transform.position = gameObject.transform.position;
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, fadeOverlay.color.a + .001f);
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");

    }
}
  