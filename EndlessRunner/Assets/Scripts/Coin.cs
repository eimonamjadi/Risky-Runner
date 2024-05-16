using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, 50f*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
    	if(other.tag == "Player") {
    		Destroy(gameObject);
    	}
    }
}
