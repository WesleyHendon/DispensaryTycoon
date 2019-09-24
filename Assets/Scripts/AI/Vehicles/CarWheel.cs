using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Vector3 wheelPosition = new Vector3();
    public Quaternion wheelRotation = new Quaternion();

    public bool leftSide;

    void Update()
    {
        wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        Vector3 eulers = wheelRotation.eulerAngles;
        transform.position = wheelPosition;
        transform.eulerAngles = eulers;
    }
}
