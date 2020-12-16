using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public const float kTwoPI = Mathf.PI * 2f;
    public static Vector3 PlayerPosition { get; set; }
    private Animator _playerAnimator; //Reserved for Animation purposes UwU Gotta think ahead
    private CharacterController _characterController; //Needed lmao
    public static Player instance;

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

    [Header("Atmosphere")]
    public Range flashlightBlinkRateRange;
    private Light _flashlight;
    private bool _blinking;
    private AudioSource _sfxAudioSource;
    public AudioClip[] flashlightSFX;
    public float flashlightVolume = 0.6f; 

    [Header("Head Bob")]
    public float bobRate;
    public float bobStrength;
    public float stepStage = -0.5f;
    public float stepVolume = 0.25f;
    public Transform cameraContainer;
    public float cameraHeightOffset;
    public AudioClip stepSFX;
    private bool _canStep;
    private float _sinTime;
    private float _bobTimer;

    private void Awake() => instance = this;

    private void OnEnable()
    {
        _velocityGroup = new VelocityGroup();
        _camera = Camera.main.transform;
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponentInChildren<Animator>(); //Might be null
        _flashlight = GetComponentInChildren<Light>();
        _sfxAudioSource = GetComponent<AudioSource>();
        _wasGrounded = true;
        _canStep = true;
    }

    private void Update() {
        noise = 0;
        UgokuUgoku();
        HandleInput();
        CastInteractableRay();
        PlayerPosition = transform.position;
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
        BobHead();
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
                _currentInteractable.SendMessage("OnUse");
        }

        if (Physics.SphereCast(_camera.transform.position, interactableRadius, _camera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, interactableDistance, interactableLayer))
        {
            if (!_hitInteractables.Contains(hit.transform))
            {
                _hitInteractables.Add(hit.transform);
                _currentInteractable = hit.transform;
                hit.transform.SendMessage("OnInteractableSelectedInternal");
            }
            _distance = hit.distance;
        }
        else
        {
            if (_currentInteractable)
            {
                _currentInteractable.SendMessage("OnInteractableDeselectedInternal");
                _currentInteractable = null;
            }
            _distance = interactableDistance;
        }
        for (int i = 0; i < _hitInteractables.Count; i++)
        {
            Transform t = _hitInteractables[i];
            if (!t || t == _currentInteractable) continue;
            t.SendMessage("OnInteractableDeselectedInternal");
            _hitInteractables.RemoveAt(i);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(cameraContainer.TransformPoint(new Vector3(cameraContainer.localPosition.x, _sinTime + cameraHeightOffset, cameraContainer.localPosition.z)), 0.25f);
        Gizmos.color = Grounded ? groundCheckGizmoColor : new Color(0.25f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position + groundCheckCenter, groundCheckRadius);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(PlayerPosition + Vector3.up * 2f, $"T: {_sinTime} | Timer: {_bobTimer}");
#endif

        if (!_camera) return;
        Vector3 direction = _camera.transform.TransformDirection(Vector3.forward) * _distance;
        Gizmos.DrawRay(_camera.transform.position, direction);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_camera.transform.position + direction, interactableRadius);
    }

    public void Blink() {
        if (!_blinking) {
            _blinking = true;
            StartCoroutine(nameof(StartBlink));
        }
    }

    public void StopBlink()
    {
        if (_blinking)
        {
            _blinking = false;
            StopCoroutine(nameof(StartBlink));
            if (_flashlight)
                _flashlight.enabled = true;
            if (_sfxAudioSource)
                _sfxAudioSource.PlayOneShot(flashlightSFX[0], flashlightVolume);
        }
    }

    private IEnumerator StartBlink() {
        if (_flashlight)
        {
            while (_blinking)
            {
                yield return new WaitForSeconds(Random.Range(flashlightBlinkRateRange.low, flashlightBlinkRateRange.high));
                _flashlight.enabled = !_flashlight.enabled;
                if (_sfxAudioSource)
                    _sfxAudioSource.PlayOneShot(_flashlight.enabled ? flashlightSFX[0] : flashlightSFX[1], flashlightVolume);
            }

            _flashlight.enabled = true;
            if (_sfxAudioSource)
                _sfxAudioSource.PlayOneShot(flashlightSFX[0], flashlightVolume);
        }
    }

    private void BobHead() {
        if (IsMoving)
        {
            float unscaledSinTime = Mathf.Sin(_bobTimer);
            _sinTime = Mathf.Sin(_bobTimer) * bobStrength;
            _bobTimer += bobRate * Time.deltaTime;
            cameraContainer.localPosition = new Vector3(cameraContainer.localPosition.x, _sinTime + cameraHeightOffset, cameraContainer.localPosition.z);
            if (unscaledSinTime <= stepStage)
            {
                if (!_canStep) return;
                _canStep = false;
                if (_sfxAudioSource && !_sfxAudioSource.isPlaying)
                    _sfxAudioSource.PlayOneShot(stepSFX, stepVolume);
            }
            else
                _canStep = true;
        }
        else
            _bobTimer = 0f;
    }
}

public class VelocityGroup {
    public Vector3 horizontal;
    public Vector3 vertical;
}

[System.Serializable]
public struct Range {
    public float low;
    public float high;
}