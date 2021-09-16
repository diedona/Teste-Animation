using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{

    [SerializeField]
    private float _Speed = 5;

    [SerializeField]
    private float _JumpPower = 6;

    [SerializeField]
    private float _JumpTimer = 1.2f;

    [SerializeField]
    private float _SlopeCheckDistance = 0.1f;

    [SerializeField]
    LayerMask _GroundLayer;

    private readonly float _GroundCheckRadius = 0.2f;

    private SpriteRenderer _SpriteRenderer;
    private Rigidbody2D _RigidBody;
    private Animator _Animator;
    private CapsuleCollider2D _CapsuleCollider;
    private float _Direction;
    private bool _JumpWasPressed;
    private bool _JumpIsBeingHold;
    private bool _JumpIsReleased;
    private bool _IsGrounded = false;
    private float _JumpTimerCounter = 0;
    private bool _IsJumping = false;

    private bool _IsOnSlop;
    private float _SlopDownAngle;
    private float _SlopDownAngleOld;    
    private float _SlopSideAngle;
    private Vector2 _SlopeNormalPerpendicular;

    private void Awake()
    {
        _JumpTimerCounter = _JumpTimer;
        _RigidBody = GetComponent<Rigidbody2D>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _Animator = GetComponent<Animator>();
        _CapsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _Direction = Input.GetAxisRaw("Horizontal");
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
    }

    private void HandleHorizontalMovement()
    {
        _RigidBody.velocity = new Vector2(_Direction * _Speed, _RigidBody.velocity.y);
    }

    private void HandleJump()
    {
        _JumpWasPressed = Input.GetButtonDown("Jump");
        _JumpIsBeingHold = Input.GetButton("Jump");
        _JumpIsReleased = Input.GetButtonUp("Jump");

        GroundCheck();

        if(_JumpWasPressed && _IsGrounded)
        {
            Jump();
            _IsJumping = true;
            _JumpTimerCounter = _JumpTimer;
        }

        if(_JumpIsBeingHold && _IsJumping)
        {
            if(_JumpTimerCounter > 0)
            {
                Jump();
                _JumpTimerCounter -= Time.deltaTime;
            }
            else
            {
                _IsJumping = false;
            }
        }

        if(_JumpIsReleased)
        {
            _IsJumping = false;
        }
    }

    private void GroundCheck()
    {
        var bounds = _CapsuleCollider.bounds;
        var size = new Vector3(bounds.size.x - 0.1f, bounds.size.y, bounds.size.z);

        RaycastHit2D rayCastHit = Physics2D.BoxCast(bounds.center, size, 0f, Vector2.down, _GroundCheckRadius, _GroundLayer);
        _IsGrounded = (rayCastHit.collider != null);
    }

    private void Jump()
    {
        _RigidBody.velocity = new Vector2(_RigidBody.velocity.x, Vector2.up.y * _JumpPower);
    }

}
