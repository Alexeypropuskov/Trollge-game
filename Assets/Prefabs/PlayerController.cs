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
    public Animator anim;
    public Transform trform;
    public bool faceRight = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Walk();
        Reflect();
    }

    void Walk()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        anim.SetFloat("MoveX", Mathf.Abs(moveVector.x));
        rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
        ///rb.AddForce(moveVector * speed);
    }

    void Reflect()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x <0 && faceRight))
        {
            trform.localScale *= new Vector2(-1, 1);
            faceRight = !faceRight;
        }
    }

}
