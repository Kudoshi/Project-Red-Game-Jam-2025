using System;
using UnityEngine;

public class PogoJump : MonoBehaviour
{
    [SerializeField] private LineRenderer _arrowLineRenderer;
    [SerializeField] private Vector3 _arrowOffset;
    [SerializeField] private SpriteRenderer _arrowHeadSprite;

    [Header("Stats")]
    [SerializeField] private float _poggingThreshold;
    [SerializeField] private float _poggingMaxDistance;
    [SerializeField] private float _maxPogForce;
    [SerializeField] private int _jumpResetAmount;
    [SerializeField] private float _rerotateSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _drag;
    [SerializeField] private float _velocityMinThresholdGravity;
    [SerializeField] private float _decreaseDoubleJumpForceDecrease;

    [Header("Grounding Stats")]
    [SerializeField] private Vector3 _groundingOffset;
    [SerializeField] private float _groundingSizing;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundCheckNextTime;

    private bool _holding;
    private Rigidbody2D _rb;
    private float _jumpForcePerc;
    private InputData _inputData = new InputData();
    private bool _isGrounded;
    private int _jumpAmount;
    private float _recheckGrounding;
    private Vector3 _velocity;

    private void Awake()
    {
        _arrowHeadSprite.enabled = false;
        _rb = GetComponent<Rigidbody2D>();
        _jumpAmount = _jumpResetAmount;
    }
    void Update()
    {
        
        HandleGrounding();

        if (_jumpAmount == 0)
            return;
        else
        {
            HandleStartingEndInput();

            HandlePogoRotation();

        }

        HandleArrowRenderer();

        HandleMovement();

        HandleRerotation();


    }

    private void HandleMovement()
    {
        _velocity = _rb.linearVelocity;

        // Apply gravity if airborne and falling fast enough
        if (!_isGrounded && _velocity.y < _velocityMinThresholdGravity)
        {
            _velocity.y -= _gravity * Time.deltaTime;
        }

        // Apply drag to horizontal movement only (don't drag y / gravity)
        _velocity.x = Mathf.Lerp(_velocity.x, 0f, _drag * Time.deltaTime);

        // Apply velocity to custom 2D movement system
        _rb.linearVelocity = _velocity;

    }

    private void HandleRerotation()
    {
        if (Time.time >= _recheckGrounding && _isGrounded && !_holding)
        {
            Debug.Log("Rotate bang");
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rerotateSpeed);
        }
    }
        

    private void HandleGrounding()
    {
        if (Time.time > _recheckGrounding)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + _groundingOffset, _groundingSizing, _groundLayerMask);
            if (colliders.Length > 0)
            {
                _isGrounded = true;
                _rb.bodyType = RigidbodyType2D.Kinematic;
                _rb.linearVelocity = Vector2.zero;
                _jumpForcePerc = 1;

                //_jumpAmount = _jumpResetAmount;
            }
            else _isGrounded = false;
        }
    }

    private void HandleStartingEndInput()
    {
        if (!_holding)
        {
            Ray ray;


            if (Input.touchCount > 0 )
            {
                _holding = true;
                _arrowHeadSprite.enabled = true;

                Touch touch = Input.GetTouch(0);
                Vector2 inputPosition = touch.position;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 0));

                _inputData = new InputData(true, InputData.INPUT_PHASE.HOLDING, inputPosition, Vector2.negativeInfinity);
                _inputData.StartingHoldPos = worldPos;
                _inputData.CurrentHoldPos = worldPos;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _holding = true;
                _arrowHeadSprite.enabled = true;

                Vector2 inputPosition = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 0));

                _inputData = new InputData(false, InputData.INPUT_PHASE.HOLDING, inputPosition, Vector2.negativeInfinity);
                _inputData.StartingHoldPos = worldPos;
                _inputData.CurrentHoldPos = worldPos;
            }

        }
        else if (_holding)
        {

            Vector2 inputPosition;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                worldPos.z = 0;
                HandlePogging(worldPos);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                worldPos.z = 0;

                HandlePogging(worldPos);

            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Touch touch = Input.GetTouch(0);
                _inputData.Phase = InputData.INPUT_PHASE.END;
                _holding = false;
                PogOut(touch.position);

            }
            else if (Input.GetMouseButtonUp(0)) 
            {
                _inputData.Phase = InputData.INPUT_PHASE.END;
                _holding = false;
                PogOut(Input.mousePosition);


            }


        }
    }

    private void HandlePogging(Vector3 currentInputPos)
    {
        Vector3 offsetHeadPointer = transform.position + _arrowOffset;
        Vector3 dirToInput = currentInputPos - offsetHeadPointer;
        float distance = dirToInput.magnitude;

        // Ignore if too close
        if (distance < _poggingThreshold)
        {
            _inputData.CurrentHoldPos = _inputData.StartingHoldPos;
            _arrowLineRenderer.enabled= false;
            _arrowHeadSprite.enabled= false;
            return;
        }

        _arrowLineRenderer.enabled = true;
        _arrowHeadSprite.enabled = true;

        // Clamp the distance to be between threshold and max
        float poggingDistance = Mathf.Clamp(distance, _poggingThreshold, _poggingMaxDistance);
        _inputData.pogDistance = poggingDistance;

        // Calculate the final touch point (clamped)
        Vector3 finalTouchPoint = offsetHeadPointer + dirToInput.normalized * poggingDistance;
        // Save it
        _inputData.CurrentHoldPos = finalTouchPoint;

        // Move arrow to that point
        Vector3 arrowPos = finalTouchPoint;
        arrowPos.z = 0;
        _arrowHeadSprite.transform.position = arrowPos;

        // Rotate the arrow to point from offsetHeadPointer to the finalTouchPoint
        Vector3 aimDirection = finalTouchPoint - offsetHeadPointer;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        _arrowHeadSprite.transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    private void HandlePogoRotation()
    {
        if (!_holding) return;
        Vector3 currentHoldPos = _inputData.CurrentHoldPos;
        Vector2 direction = (currentHoldPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void HandleArrowRenderer()
    {
        if (_inputData.Phase == InputData.INPUT_PHASE.HOLDING)
        {
            if (_inputData.StartingHoldPos == _inputData.CurrentHoldPos)
            {
                _arrowLineRenderer.SetPosition(0, transform.position + _arrowOffset);
                _arrowLineRenderer.SetPosition(1, transform.position + _arrowOffset);
                return;
            }

            _arrowLineRenderer.SetPosition(0, transform.position + _arrowOffset);
            _arrowLineRenderer.SetPosition(1, _inputData.CurrentHoldPos);
        }
        else if (_inputData.Phase == InputData.INPUT_PHASE.END)
        {
            _arrowLineRenderer.SetPosition(0, Vector3.zero);
            _arrowLineRenderer.SetPosition(1, Vector3.zero);

        }

    }

    private void PogOut(Vector3 currentInputPos)
    {

        float pogForce = _maxPogForce * (_inputData.pogDistance / _poggingMaxDistance);
        //_rb.AddForce(transform.up * pogForce, ForceMode2D.Impulse);
        Vector3 velocity = _rb.linearVelocity;
        velocity += transform.up * (pogForce * _jumpForcePerc);
        _rb.linearVelocity = velocity;
        _rb.bodyType = RigidbodyType2D.Dynamic;


        _inputData.EndingHoldPos = currentInputPos;
        _arrowHeadSprite.enabled = false;

        _jumpAmount--;
        _jumpForcePerc *= _decreaseDoubleJumpForceDecrease;
        

        _recheckGrounding = Time.time + _groundCheckNextTime;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + _groundingOffset, _groundingSizing);

    }
}

//[System.Serializable]
public class InputData
{
    public enum INPUT_PHASE { HOLDING, END, IDLE }

    public bool IsTouchInput;
    public INPUT_PHASE Phase = INPUT_PHASE.IDLE;
    public Vector2 StartingHoldPos;
    public Vector2 CurrentHoldPos;
    public Vector2 EndingHoldPos;

    // Cache. Dangerous
    public Vector3 Direction;
    public float pogDistance;

    public InputData() { }
    public InputData(bool isTouchInput, INPUT_PHASE phase, Vector2 startingHoldPos, Vector2 endingHoldPos)
    {
        IsTouchInput = isTouchInput;
        Phase = phase;
        StartingHoldPos = startingHoldPos;
        EndingHoldPos = endingHoldPos;
    }
}