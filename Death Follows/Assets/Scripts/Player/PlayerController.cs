using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController _characterController;

    public Vector2 movementDirection { get; private set; }
    private Vector2 _lookDirection;
    private Vector3 _playerVelocity;

    private PlayerControls _playerControls;
    private PlayerInput _playerInput;

    private InputAction _dashAction;
    private bool _dashing = false;
    public float _dashLength;
    public float _dashSpeed;
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerControls = new PlayerControls();
        _playerInput = GetComponent<PlayerInput>();

    }

    private void OnEnable()
    {
        _dashAction = _playerInput.actions["Dash"];
        _playerControls.Enable();
        Debug.Log(_dashAction);
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    private void HandleInput()
    {
        if (_dashing)
        {
            return;
        }
        
        movementDirection = _playerControls.Controls.Movement.ReadValue<Vector2>();
        _lookDirection = _playerControls.Controls.Aim.ReadValue<Vector2>();
        if (_dashAction.triggered)
        {
            Debug.Log("Dashed");
            StartCoroutine(Dash());
        }
    }

    private void HandleMovement()
    {
        if (_dashing)
        {
            //_characterController.Move(_lookDirection * Time.deltaTime * moveSpeed);
        }
        else
        {
            Vector3 _move = new Vector3(movementDirection.x, 0, movementDirection.y);
            _characterController.Move(_move * Time.deltaTime * moveSpeed);
            //transform.position = Vector3.MoveTowards(transform.position, _move, Time.deltaTime * moveSpeed);

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

    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("Dashed");
        StartCoroutine(Dash());
    }

    private void LookAt(Vector3 point)
    {
        Vector3 lookPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(lookPoint);
    }
    IEnumerator Dash()
    {
        moveSpeed *= _dashSpeed;
        _dashing = true;
        yield return new WaitForSeconds(_dashLength);
        _dashing = false;
        moveSpeed /= _dashSpeed;
    }
}
