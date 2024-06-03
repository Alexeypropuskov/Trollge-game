using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponglue : MonoBehaviour
{
    public Transform place;
    public Transform weapon;

    // Start is called before the first frame update
    void Start()
    {
        place = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Glue();
    }

    void Glue()
    {
        weapon.position = place.position;
    }
}
