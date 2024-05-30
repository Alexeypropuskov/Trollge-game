using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        transform.position += new Vector3(1,0,0) * Input.GetAxis("Horizontal");
    }
}
