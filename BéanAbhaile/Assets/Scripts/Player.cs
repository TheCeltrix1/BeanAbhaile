using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private Animator _playerAnimator; //Reserved for Animation purposes UwU Gotta think ahead
    private CharacterController _characterController; //Needed lmao

    [HideInInspector] public Vector2 movementVector; //You don't need to see this

    [Header("Physics")]
    public float gravity = 20f;
    public float speed = 1.5f;
    public float maxFallVelocity = -40f;
    public float jumpHeight = 20f;
    private VelocityGroup _velocityGroup;
    public float groundedVelocity = 0.01f; //Needs to be small. This is required because CharacterController isGrounded dies when on the ground.

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckRadius;
    public Vector3 groundCheckCenter;
    public Color groundCheckGizmoColor;
    public bool useBuiltInGroundCheck = false;

    [Header("Additional Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.F;
    public float interactableDistance;
    public float interactableRadius;
    public LayerMask interactableLayer;

    private float _gravitationalForce;
    public bool Grounded { get; private set; }
    private bool _wasGrounded;
    private Transform _camera; //Camera for rotation

    //States
    public float noise; //Your variable <3
    public bool IsMoving { get; private set; }
    public bool Falling { get { return _gravitationalForce < 0f; } }
    public bool Sprinting { get; private set; }

    private void OnEnable()
    {
        _velocityGroup = new VelocityGroup();
        _camera = Camera.main.transform;
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponentInChildren<Animator>(); //Might be null
        _wasGrounded = true;
    }

    private void Update() {
        noise = 0;
        UgokuUgoku();
        HandleInput();
    }

    private void LateUpdate() => transform.eulerAngles = new Vector3(transform.eulerAngles.x, _camera.eulerAngles.y, transform.eulerAngles.z);
    private void FixedUpdate() => ApplyGravity();

    private void HandleInput() {
        movementVector = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));

        if (Mathf.Abs(movementVector.x) > 0 || Mathf.Abs(movementVector.y) > 0) noise += 40;

        IsMoving = movementVector.magnitude != 0f;
    }

    private void UgokuUgoku()
    {
        //Built in CharacterController ground check is dog shit.
        Grounded = useBuiltInGroundCheck ? _characterController.isGrounded : 
            Physics.CheckSphere(transform.position + groundCheckCenter, groundCheckRadius, groundMask); 
        _velocityGroup.horizontal = transform.right * movementVector.x * speed;
        _velocityGroup.vertical = transform.forward * movementVector.y * speed;

        //Add horizontal and vertical velocity separately to avoid weird behaviour
        _characterController.Move(_velocityGroup.horizontal * Time.deltaTime);
        _characterController.Move(_velocityGroup.vertical * Time.deltaTime);

        if (!Grounded && !_wasGrounded)
            _wasGrounded = true;

        if (Grounded && _wasGrounded)
        {
            _wasGrounded = false;
            _gravitationalForce = groundedVelocity;
        }

        if (Grounded && Input.GetKeyDown(jumpKey))
        {
            noise += 30;
            _gravitationalForce += Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    private void ApplyGravity() {

        if (!Grounded)
        {
            if (_gravitationalForce > maxFallVelocity)
                _gravitationalForce += gravity * Time.deltaTime;
            else
                _gravitationalForce = maxFallVelocity;
        }
        _characterController.Move(Vector3.up * _gravitationalForce);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Grounded ? groundCheckGizmoColor : new Color(0.25f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position + groundCheckCenter, groundCheckRadius);
    }*/

    private List<Transform> _hitInteractables;
    private Transform _currentInteractable;
    private float _distance;

    private void CastInteractableRay()
    {
        if (_hitInteractables == null)
            _hitInteractables = new List<Transform>();

        if (Input.GetKeyDown(interactKey))
        {
            if (_currentInteractable)
                _currentInteractable.SendMessage("PickupItem");
        }

        if (Physics.SphereCast(_camera.transform.position, interactableRadius, _camera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, interactableDistance, interactableLayer))
        {
            if (!_hitInteractables.Contains(hit.transform))
            {
                _hitInteractables.Add(hit.transform);
                _currentInteractable = hit.transform;
                hit.transform.SendMessage("EnableOutline");
            }
            _distance = hit.distance;
        }
        else
        {
            if (_currentInteractable)
            {
                _currentInteractable.SendMessage("DisableOutline");
                _currentInteractable = null;
            }
            _distance = interactableDistance;
        }
        for (int i = 0; i < _hitInteractables.Count; i++)
        {
            Transform t = _hitInteractables[i];
            if (!t || t == _currentInteractable) continue;
            t.SendMessage("DisableOutline");
            _hitInteractables.RemoveAt(i);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Grounded ? groundCheckGizmoColor : new Color(0.25f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position + groundCheckCenter, groundCheckRadius);
        if (!_camera) return;
        Vector3 direction = _camera.transform.TransformDirection(Vector3.forward) * _distance;
        Gizmos.DrawRay(_camera.transform.position, direction);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_camera.transform.position + direction, interactableRadius);
    }
}

public class VelocityGroup {
    public Vector3 horizontal;
    public Vector3 vertical;
}