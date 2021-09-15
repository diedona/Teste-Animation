using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [SerializeField]
    private float _DistanceForward = 5;

    [SerializeField]
    private float _Speed = 1;

    private float _StartingX;

    private void Awake()
    {
        _StartingX = transform.position.x;
    }

    private void Update()
    {
        //transform.position = new Vector2(transform.position.x + _Speed * Time.deltaTime, transform.position.y);
        Debug.Log($"currentPosition.x: {transform.position.x}");

        if(!IsAtDestiny())
            MoveToDestiny();        
        else
            RotateDirection();
    }

    private bool IsAtDestiny()
    {
        if(_Speed > 0)
            return (transform.position.x >= _StartingX + _DistanceForward);
        else
            return (transform.position.x <= _StartingX + _DistanceForward);
    }

    private void MoveToDestiny()
    {
        transform.position = new Vector2(transform.position.x + _Speed * Time.deltaTime, transform.position.y);
    }

    private void RotateDirection()
    {
        _StartingX = transform.position.x;
        _DistanceForward = (-_DistanceForward);
        _Speed = (-_Speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
            other.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
            other.transform.parent = null;
    }
}
