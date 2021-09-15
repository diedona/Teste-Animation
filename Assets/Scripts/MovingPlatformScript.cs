using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{

    [SerializeField]
    private float _Speed = 1;

    private void Awake()
    {
        
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x + _Speed * Time.deltaTime, transform.position.y);
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
