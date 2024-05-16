using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        CharacterController a = GetComponent<CharacterController>();
        if (!a.isGrounded)
        {
            pos.y -= 10f * Time.deltaTime;
            a.Move(pos);
        }
        else
        {
            Debug.Log("isGrounded");
        }    
        transform.position = pos;
    }
}
