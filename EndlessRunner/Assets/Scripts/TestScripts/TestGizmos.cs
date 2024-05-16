using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGizmos : MonoBehaviour
{
    public List<Vector3> Trans;

    private void Awake()
    {
        Trans = new List<Vector3>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Vector3 t in Trans)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t, 0.5f);
        }
    }
}
