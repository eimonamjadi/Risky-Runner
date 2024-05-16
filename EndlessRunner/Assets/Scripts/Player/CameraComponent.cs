using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
	public PlayerController pc;

	private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - pc.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z+pc.transform.position.z);
    }
}
