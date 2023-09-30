using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int health = 3;
    public GameObject soul;
    public GameObject ricochetHitParticle;
    public GameObject art;
    public GameObject death;
    public Vector2 movementDirection { get; private set; }
    private Vector2 _lookDirection;

    private bool _dashing = false;
    public float _dashLength;
    private float _dashSpeed = 2.5f;

    private bool _ricocheting = false;
    public float ricochetSpeed = 8f;

    public Action<Vector2> OnMove;
    public void SubscribeToOnMove(Action<Vector2> callback) => OnMove += callback;
    public void UnsubscribeToOnMove(Action<Vector2> callback) => OnMove -= callback;
       
    PlayerInput _input;
    InputAction _moveAction;
    InputAction _lookAction;
    InputAction _dashAction;


        void OnEnable()
        {
            _input = GetComponent<PlayerInput>();
            _moveAction = _input.actions["Move"];
            _lookAction = _input.actions["Look"];
            _dashAction = _input.actions["Dash"];

            SubInput();
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
            if (_dashing)
            {
                Vector3 _move = transform.forward + new Vector3(movementDirection.x, 0, movementDirection.y) * 0.5f;
                _dashSpeed = _dashSpeed *0.995f;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + _move, Time.deltaTime * moveSpeed * _dashSpeed);
            }
            else
            {
                Vector3 _move = new Vector3(movementDirection.x, 0, movementDirection.y);
                transform.position = Vector3.MoveTowards(transform.position, transform.position + _move, Time.deltaTime * moveSpeed);
            }

        }
        private void HandleRotation()
        {
            Ray ray = Camera.main.ScreenPointToRay(_lookDirection);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
        private void LookAt(Vector3 point)
        {
            Vector3 lookPoint = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(lookPoint);
        }   
        void OnDash(InputAction.CallbackContext context)
        {
            Debug.Log("Dashed");
            StartCoroutine(Dash());
        }
               
        IEnumerator Dash()
        {
            if (!_dashing) {
                _dashSpeed = 2.5f;
                _dashing = true;
                yield return new WaitForSeconds(_dashLength);
                _dashing = false;
            }
        }
        
    public void Damage(int damage)
    {
        if (!_dashing && !_ricocheting)
        {
            health -= damage;
        }        
    }

    public void SoulRicochet()
    {
        Debug.Log("Ricochet Player");
        _ricocheting = true;
        transform.forward = (gameObject.transform.position - death.transform.position).normalized;
        art.SetActive(false);
        Instantiate(ricochetHitParticle, transform.position, transform.rotation);
        Instantiate(soul, gameObject.transform);
    }

    public void RicochetHit(GameObject enemyHit)
    {
        _ricocheting = false;
        art.SetActive(true);
        Instantiate(ricochetHitParticle, enemyHit.transform.position, Quaternion.identity);
        Destroy(enemyHit);
        
    }
}