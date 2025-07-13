using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class PogoJump : Singleton<PogoJump>
{

    [SerializeField] private LineRenderer _arrowLineRenderer;
    [SerializeField] private Vector3 _arrowOffset;
    [SerializeField] private SpriteRenderer _arrowHeadSprite;
    [SerializeField] private Transform _tappy;
    [SerializeField] private CinemachineCamera _vcam;
    [SerializeField] private ParticleSystem _smokeFx;
    [SerializeField] private ParticleSystem _launchSmokeFx;

    [Header("Stats")]
    [SerializeField] private float _poggingThreshold;
    [SerializeField] private float _poggingMaxDistance;
    [SerializeField] private float _maxPogForce;
    [SerializeField] private Vector2 _restrictPogAngle;
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
    [SerializeField] private float _rayDistance = 0.5f;

    [Header("Pog Zoom Out")]
    [SerializeField] private float _pogZoomOutSize;
    [SerializeField] private float _flyingZoomOutSize;
    [SerializeField] private float _zoomOutDuration;

    private bool _holding;
    private Rigidbody2D _rb;
    private float _jumpForcePerc;
    private InputData _inputData = new InputData();
    private bool _isGrounded;
    private int _jumpAmount;
    private float _recheckGrounding;
    private Vector3 _velocity;
    private float _defaultLensOutzoom;
    private float _targetLensOutzoom;

    private ContactFilter2D _filter = new ContactFilter2D();
    
    public int JumpAmount { get => _jumpAmount; }

    public Vector3 Ref_Velocity;

    private void Awake()
    {
        _arrowHeadSprite.enabled = false;
        _rb = GetComponent<Rigidbody2D>();
        _jumpAmount = _jumpResetAmount;
        _defaultLensOutzoom = _vcam.Lens.OrthographicSize;
        _targetLensOutzoom = _defaultLensOutzoom;

        _filter.useTriggers = false;
        _filter.SetLayerMask(_groundLayerMask);
        _filter.useLayerMask = true;
    }

    private void Start()
    {
        TappyAnimations.Instance.ChangeToAnimation("IDLE");
    }


    void Update()
    {
        HandleGrounding();

        if (_jumpAmount == 0)
        {
            GameOver();
            return;
        }
        else if (_jumpAmount < 0)
            return;

        else
        {
            HandleStartingEndInput();

            HandlePogoRotation();

        }

        HandleArrowRenderer();

        HandleMovement();

        HandleRerotation();

        HandleZoom();

        Ref_Velocity = _rb.linearVelocity;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        SoundManager.Instance.PlaySound(new SoundVariationizer("sfx_hit_thud_", 0.25f, 0, 3));
    }
    private void GameOver()
    {
        if (_isGrounded && _jumpAmount == 0)
        {
            _rb.angularVelocity = 0;

            Util.WaitForSeconds(this, () =>
            {
                ExitUI.Instance.GameEnd();
            }, 3.0f);
        }
    }

    private void HandleZoom()
    {
        float currentSize = _vcam.Lens.OrthographicSize;
        float zoomOutSpeed = _zoomOutDuration;
        //Zooming out
        if (currentSize == _flyingZoomOutSize)
        {
            zoomOutSpeed /= 2;
        }

        float newSize = Mathf.Lerp(currentSize, _targetLensOutzoom, Time.deltaTime * zoomOutSpeed);
        _vcam.Lens.OrthographicSize = newSize;
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

            float currentZ = _rb.rotation;
            float targetZ = 0f;

            float newZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * _rerotateSpeed);
            _rb.MoveRotation(newZ);
        }
    }
        

    private void HandleGrounding()
    {
        if (Time.time > _recheckGrounding)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + _groundingOffset, _groundingSizing, _groundLayerMask);

            if (colliders.Length > 0)
            {
                // Raycast from above the player's feet into the ground to get surface normal
                Vector2 rayOrigin = transform.position;
                Vector2 rayDirection = -transform.up;

                RaycastHit2D[] hits = new RaycastHit2D[1];
                int hitCount = Physics2D.Raycast(rayOrigin, rayDirection, _filter, hits, _rayDistance);
                RaycastHit2D hit = new RaycastHit2D();

                if (hitCount > 0) hit = hits[0];


                if ((hit.normal.y > 0.85) || (hit.normal.x == 0 && hit.normal.y == 0))
                {
                    if (!_isGrounded)
                    {
                        _smokeFx.Play();
                        TappyAnimations.Instance.ChangeToAnimation("IDLE");
                        SoundManager.Instance.PlaySound(new SoundVariationizer("sfx_hit_thud_", 0.25f, 0, 3));
                     }

                    _isGrounded = true;
                    _rb.bodyType = RigidbodyType2D.Kinematic;
                    _rb.linearVelocity = Vector2.zero;
                    _jumpForcePerc = 1;

                    
                    _rb.angularVelocity = 0;

                    if (_targetLensOutzoom == _flyingZoomOutSize)
                        _targetLensOutzoom = _defaultLensOutzoom;
                }
            }
            else
            {
                _isGrounded = false;
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

    }

    private void HandleStartingEndInput()
    {
        if (!_holding)
        {
            Ray ray;


            if (Input.touchCount > 0 )
            {
                
                Touch touch = Input.GetTouch(0);
                Vector2 inputPosition = touch.position;

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 0));

                Vector3 offsetHeadPointer = transform.position + _arrowOffset;
                Vector3 dirToInput = worldPos - offsetHeadPointer;


                float angle = Mathf.Atan2(dirToInput.y, dirToInput.x) * Mathf.Rad2Deg;
                if (angle < _restrictPogAngle.x || angle > _restrictPogAngle.y)
                {
                    return;
                }


                _holding = true;
                _arrowHeadSprite.enabled = true;

                _inputData = new InputData(true, InputData.INPUT_PHASE.HOLDING, inputPosition, Vector2.negativeInfinity);
                _inputData.StartingHoldPos = worldPos;
                _inputData.CurrentHoldPos = worldPos;
                TappyAnimations.Instance.ChangeToAnimation("POG");
                SoundManager.Instance.PlaySound("sfx_spring_load");

            }
            //else if (Input.GetMouseButtonDown(0))
            //{
            //    _holding = true;
            //    _arrowHeadSprite.enabled = true;

            //    Vector2 inputPosition = Input.mousePosition;
            //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, 0));

            //    _inputData = new InputData(false, InputData.INPUT_PHASE.HOLDING, inputPosition, Vector2.negativeInfinity);
            //    _inputData.StartingHoldPos = worldPos;
            //    _inputData.CurrentHoldPos = worldPos;
            //}

        }
        else if (_holding)
        {

            Vector2 inputPosition;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                worldPos.z = 0;
                HandlePogging(worldPos);
                //Debug.Log("pogging mobile");
            }
            //else if (Input.GetMouseButton(0))
            //{
            //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            //    worldPos.z = 0;
            //    Debug.Log("pogging PC");

            //    HandlePogging(worldPos);

            //}
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Touch touch = Input.GetTouch(0);
                _inputData.Phase = InputData.INPUT_PHASE.END;
                _holding = false;
                PogOut(touch.position);

            }
            //else if (Input.GetMouseButtonUp(0)) 
            //{
            //    _inputData.Phase = InputData.INPUT_PHASE.END;
            //    _holding = false;
            //    PogOut(Input.mousePosition);


            //}


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

        float angle = Mathf.Atan2(dirToInput.y, dirToInput.x) * Mathf.Rad2Deg;
        if (angle < _restrictPogAngle.x || angle  > _restrictPogAngle.y)
        {
            return;
        }


        _targetLensOutzoom = _pogZoomOutSize;

        _arrowLineRenderer.enabled = true;
        _arrowHeadSprite.enabled = true;

        // Clamp the distance to be between threshold and max
        float poggingDistance = Mathf.Clamp(distance, _poggingThreshold, _poggingMaxDistance);
        _inputData.pogDistance = poggingDistance;

        // Calculate the final touch point (clamped)
        Vector3 finalTouchPoint = offsetHeadPointer + dirToInput.normalized * poggingDistance;
        // Save it
        _inputData.CurrentHoldPos = finalTouchPoint;
         
        

        // Rotate the arrow to point from offsetHeadPointer to the finalTouchPoint
        Vector3 aimDirection = finalTouchPoint - offsetHeadPointer;
        _arrowHeadSprite.transform.rotation = Quaternion.Euler(0, 0, angle-90f);

        _tappy.DOLocalMoveY(-0.4f, 0.25f).SetEase(Ease.Linear);

        //DOTween.To(() => _vcam.Lens.OrthographicSize,
        //       x => _vcam.Lens.OrthographicSize = x,
        //       _targetZoomOutSize,
        //       _zoomOutDuration)
        //   .SetEase(Ease.InOutQuad);
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
                // Move arrow to that point
                Vector3 arrowPos1 = _inputData.CurrentHoldPos;
                arrowPos1.z = 0;
                _arrowHeadSprite.transform.position = arrowPos1;

                return;
            }

            _arrowLineRenderer.SetPosition(0, transform.position + _arrowOffset);
            _arrowLineRenderer.SetPosition(1, _inputData.CurrentHoldPos);
            // Move arrow to that point
            Vector3 arrowPos = _inputData.CurrentHoldPos;
            arrowPos.z = 0;
            _arrowHeadSprite.transform.position = arrowPos;
        }
        else if (_inputData.Phase == InputData.INPUT_PHASE.END)
        {
            _arrowLineRenderer.SetPosition(0, Vector3.zero);
            _arrowLineRenderer.SetPosition(1, Vector3.zero);

        }

    }

    private void PogOut(Vector3 currentInputPos)
    {
        _rb.angularVelocity = 0;

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
        _tappy.DOKill();
        _tappy.DOLocalMoveY(0.5f, 0.15f).SetEase(Ease.InOutQuad);

        _targetLensOutzoom = _flyingZoomOutSize;
        SoundManager.Instance.PlaySound("sfx_spring_unload");
        _launchSmokeFx.Play();
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