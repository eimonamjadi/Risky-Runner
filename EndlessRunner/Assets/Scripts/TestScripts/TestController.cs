using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestController : MonoBehaviour
{
    private Rigidbody rb;
    private float Speed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.position += Vector3.forward * Speed * Time.deltaTime;
    }
}
