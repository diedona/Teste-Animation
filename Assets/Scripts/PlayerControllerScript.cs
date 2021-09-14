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
    private Transform _GroundCheck;

    [SerializeField]
    private float _SlopeCheckDistance = 1f;

    [SerializeField]
    LayerMask _GroundLayer;

    private readonly float _GroundCheckRadius = 0.2f;

    private SpriteRenderer _SpriteRenderer;
    private Rigidbody2D _RigidBody;
    private Animator _Animator;
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
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _Direction = Input.GetAxisRaw("Horizontal");
        _JumpWasPressed = Input.GetButtonDown("Jump");
        _JumpIsReleased = Input.GetButtonUp("Jump");
        _JumpIsBeingHold = Input.GetButton("Jump");

        HandleFlip();
        HandleAnimation();

        //Debug.Log($"Update === _JumpWasPressed: {_JumpWasPressed} - _IsGrounded: {_IsGrounded}");
    }

    private void FixedUpdate()
    {
        SlopeCheck();
        HandleHorizontalMovement();
        HandleJump();
    }

    private void HandleFlip()
    {
        if (_Direction < 0)
            _SpriteRenderer.flipX = true;
        else if (_Direction > 0)
            _SpriteRenderer.flipX = false;
    }

    private void HandleAnimation()
    {
        if (_RigidBody.velocity.x == 0)
            _Animator.SetBool("walking", false);
        else
            _Animator.SetBool("walking", true);
    }

    private void HandleHorizontalMovement()
    {
        //_RigidBody.velocity = new Vector2(_Direction * _Speed, _RigidBody.velocity.y);

        // Debug.Log($"_IsGrounded: {_IsGrounded} - _IsOnSlop: {_IsOnSlop}");
        if(_IsGrounded && !_IsOnSlop)
        {
            _RigidBody.velocity = new Vector2(_Direction * _Speed, 0f);
        }
        else if(_IsGrounded && _IsOnSlop)
        {
            _RigidBody.velocity = new Vector2(
                _Speed * _SlopeNormalPerpendicular.x * (-_Direction), 
                _Speed * _SlopeNormalPerpendicular.y * (-_Direction));
        }
        else if(!_IsGrounded)
        {
            _RigidBody.velocity = new Vector2(_Direction * _Speed, _RigidBody.velocity.y);
        }
    }

    private void HandleJump()
    {
        GroundCheck();

        if (_JumpWasPressed && _IsGrounded)
        {
            _IsJumping = true;
            _JumpTimerCounter = _JumpTimer;
            Jump();
        }

        if (_JumpIsBeingHold && _IsJumping)
        {
            if (_JumpTimerCounter > 0)
            {
                Jump();
                _JumpTimerCounter -= Time.deltaTime;
            }
            else
            {
                _IsJumping = false;
            }
        }

        if (_JumpIsReleased)
            _IsJumping = false;
    }

    private void GroundCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_GroundCheck.position, _GroundCheckRadius, _GroundLayer);
        _IsGrounded = colliders.Any();
    }

    private void Jump()
    {
        _RigidBody.velocity = new Vector2(_RigidBody.velocity.x, Vector2.up.y * _JumpPower);
    }

    private void SlopeCheck()
    {
        SlopeCheckHorizontal();
        SlopeCheckVertical();
    }

    private void SlopeCheckHorizontal()
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(_GroundCheck.position, Vector2.right, _SlopeCheckDistance, _GroundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(_GroundCheck.position, -Vector2.right, _SlopeCheckDistance, _GroundLayer);

        if(slopeHitFront)
        {
            _IsOnSlop = true;
            _SlopSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            _IsOnSlop = true;
            _SlopSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            _IsOnSlop = false;
            _SlopSideAngle = 0;
        }
    }

    private void SlopeCheckVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(_GroundCheck.position, Vector2.down, _SlopeCheckDistance, _GroundLayer);
        if(hit)
        {
            _SlopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            _SlopDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(_SlopDownAngle != _SlopDownAngleOld)
            {
                _IsOnSlop = true;
            }

            _SlopDownAngleOld = _SlopDownAngle;

            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawRay(hit.point, _SlopeNormalPerpendicular, Color.red);
        }
    }
}
