using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalMove : MonoBehaviour
{
    public Vector3 Center = Vector3.zero;
    public Vector3 Axis = Vector3.up;
    public float Radius = 0.5f;
    public float AngularVelocityDegrees = 100.0f;

    Vector3 _StartPosition;

    // Start is called before the first frame update
    void Start()
    {
        float dotR = Mathf.Abs( Vector3.Dot(Axis, Vector3.right) );
        float dotU = Mathf.Abs( Vector3.Dot(Axis, Vector3.up) );

        if( dotR > dotU )
        {
            _StartPosition = Vector3.Cross(Axis, Vector3.up).normalized * Radius;
        }
        else
        {
            _StartPosition = Vector3.Cross(Axis, Vector3.right).normalized * Radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion Rotation = Quaternion.AngleAxis(Time.time* AngularVelocityDegrees, Axis);

        Vector3 Position = Rotation * _StartPosition;
        transform.localPosition = Position;
    }
}
