using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustColliderPosition : MonoBehaviour
{
    struct CurveCollider
    {
        public CurveCollider(BoxCollider box, float x, Mesh renderer)
        {
            _boxCollider = box;
            _originalCenterX = x;
            _renderer = renderer;
        }
        public BoxCollider _boxCollider;
        public float _originalCenterX;
        public Mesh _renderer;
    };

    private BoxCollider[] _boxColliders;
    private List<CurveCollider> _curveColliders;
    public float CurvedX;
    public float OriginalCenterX;

    private void Awake()
    {
        _boxColliders = GetComponentsInChildren<BoxCollider>();
        _curveColliders = new List<CurveCollider>();
    }

    private void Start()
    {
        CurveCollider curveCollider;
        foreach (BoxCollider col in _boxColliders)
        {
            curveCollider = new CurveCollider(col, col.center.x, col.GetComponent<MeshFilter>().mesh);
            _curveColliders.Add(curveCollider);
        }
        AdjustCollidersOffset();
    }

    /// <summary>
    /// Tried to draw red dots on the eight vertices of a cube
    /// </summary>
    [ContextMenu("DrawGizmos")]
    public void DrawGizmos()
    {

        _boxColliders = GetComponentsInChildren<BoxCollider>();
        _curveColliders = new List<CurveCollider>();
        CurveCollider curveCollider;
        foreach (BoxCollider col in _boxColliders)
        {
            curveCollider = new CurveCollider(col, col.center.x, col.GetComponent<MeshFilter>().mesh);
            _curveColliders.Add(curveCollider);
        }

        TestGizmos g = Camera.main.GetComponent<TestGizmos>();
        if (g)
        {
            g.Trans = new List<Vector3>();
            foreach (Vector3 vec in _curveColliders[4]._renderer.vertices)
            {
                g.Trans.Add(_curveColliders[4]._boxCollider.transform.TransformPoint(vec));
            }
        }
    }

    [ContextMenu("AdjustColliderOffset")]
    public void AdjustCollidersOffset()
    {
        _boxColliders = new BoxCollider[12];
        _boxColliders = GetComponentsInChildren<BoxCollider>();
        _curveColliders = new List<CurveCollider>();
        CurveCollider curveCollider;
        foreach (BoxCollider col in _boxColliders)
        {
            curveCollider = new CurveCollider(col, col.center.x, col.GetComponent<MeshFilter>().mesh);
            _curveColliders.Add(curveCollider);
        }

        //while (true)
        //{
        //    yield return new WaitForSeconds(0.5f);
        foreach (CurveCollider col in _curveColliders)
        {
            Matrix4x4 localToWorld = col._boxCollider.transform.localToWorldMatrix;
            Vector3 world_v = localToWorld.MultiplyPoint3x4(col._boxCollider.center);
            Vector3 worldPos = col._boxCollider.transform.position;
            float _pow = Mathf.Pow(world_v.z, 2f);
            float res = _pow * CurvedX;
            world_v.x += Mathf.Pow(world_v.z, 2f) * CurvedX;
            Vector3 adjustment = localToWorld.MultiplyPoint3x4(Vector3.zero);
            world_v -= adjustment;
            //Vector3 meshPos_right = col._renderer.vertices[0]; //local
            //Vector3 meshPos_left = col._renderer.vertices[1];
            //Vector3 meshPos = col._boxCollider.transform.TransformPoint((meshPos_right + meshPos_left)/2f);
            //float distToCam = col._boxCollider.transform.TransformPoint((meshPos_right + meshPos_left) / 2f - Camera.main.transform.localPosition).z;
            //if (distToCam > 0f)
            //{
            //    float offsetX = (CurvedX / 100f) * Mathf.Pow(distToCam, 1.8f);
            //    col._boxCollider.center = world_v;
            //}
            col._boxCollider.center = world_v;
        }
        //}
    }
}
