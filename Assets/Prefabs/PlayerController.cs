using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    
    public int Jumpforce;
    public Rigidbody2D rb;
    public Vector2 moveVector;
    public float speed = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        walk();
        
    }

    void walk()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        ///rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
        rb.AddForce(moveVector * speed);
    }


}
