using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTest : MonoBehaviour
{
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderBR;
    public WheelCollider wheelColliderBL;

    //public RoadPath testPath;

    float minSteerAngle = 50f;
    float maxSteerAngle = 90f;

    float maxMotorTorque = 650f;
    float maxBrakeTorque = 800f;
    float maxSpeed = 3.5f;

    public bool turning = false;

    void Start()
    {
        wheelColliderBL.steerAngle = 90;
        wheelColliderBR.steerAngle = 90;
        //OnSpawn(testPath);
    }

    void FixedUpdate()
    {
        CheckSensors();
        ApplySteer();
        CheckWaypointDistance();
    }

    private void CheckSensors()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.right);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 10f))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance > 3f && GetSpeed() == 0)
            {
                DriveForward(true);
            }
            else
            {
                Brake(hit.point);
            }
        }
        else
        {
            DriveForward(false);
        }
        Debug.DrawRay(ray.origin, ray.direction*10);
    }

    private void ApplySteer()
    {
        //Vector3 relativeVector = gameObject.transform.InverseTransformPoint(currentPath.waypoints[currentPathIndex].transform.position);
        //float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        //if (relativeVector.z < 0)
        //{
        //    newSteer *= -1;
        //}
        //if (Mathf.Abs(newSteer) < 70 && Mathf.Abs(newSteer) > 55)
        //{
            turning = true;
            Vector3 eulers = transform.eulerAngles;
            /*if (newSteer < 0)
            {
                transform.eulerAngles = new Vector3(eulers.x, eulers.y + .18f, eulers.z);
            }
            else
            {
                transform.eulerAngles = new Vector3(eulers.x, eulers.y - .18f, eulers.z);
            }*/
        //}
        //else
       // {
        //    turning = false;
        //}
        //wheelColliderFR.steerAngle = newSteer;
        //wheelColliderFL.steerAngle = newSteer;
    }

    int counter = 0;
    private void DriveForward(bool quickly)
    {
        float currentSpeed = GetSpeed();
        if (currentSpeed < maxSpeed)
        {
            wheelColliderBL.brakeTorque = 0;
            wheelColliderBR.brakeTorque = 0;
            wheelColliderBL.motorTorque = (turning) ? maxMotorTorque / 6 : (quickly) ? maxMotorTorque * 4 : maxMotorTorque;
            wheelColliderBR.motorTorque = (turning) ? maxMotorTorque / 6 : (quickly) ? maxMotorTorque * 4 : maxMotorTorque;
        }
        else
        {
            wheelColliderBL.brakeTorque = 0;
            wheelColliderBR.brakeTorque = 0;
            wheelColliderBL.motorTorque = 0;
            wheelColliderBR.motorTorque = 0;
        }
        //PathWaypoint currentWaypoint = currentPath.waypoints[currentPathIndex];
        //if (currentWaypoint.waypointType == PathWaypoint.WaypointType.crosswalk || currentWaypoint.waypointType == PathWaypoint.WaypointType.split)
        //{
        //    Brake();
        //}
    }

    private void Brake(Vector3 stopPosition)
    {
        /*float distanceFromStop = Vector3.Distance(transform.position, stopPosition);
        print(distanceFromStop);
        if (distanceFromStop > 3.5f && GetSpeed() > 0)
        {
            float torque = CalculateBrakeForce(stopPosition);
            wheelColliderBL.motorTorque = 0;
            wheelColliderBR.motorTorque = 0;
            wheelColliderBL.brakeTorque = torque;
            wheelColliderBR.brakeTorque = torque;
        }
        else if (distanceFromStop > 3.5f)
        {
            DriveForward();
        }*/
        float torque = CalculateBrakeForce(stopPosition);
        wheelColliderBL.motorTorque = 0;
        wheelColliderBR.motorTorque = 0;
        wheelColliderBL.brakeTorque = maxBrakeTorque;
        wheelColliderBR.brakeTorque = maxBrakeTorque;
    }

    private void Brake()
    {
        wheelColliderBL.brakeTorque = maxBrakeTorque/15;
        wheelColliderBR.brakeTorque = maxBrakeTorque/15;
    }

    public void CheckWaypointDistance()
    {
        //float distance = Vector3.Distance(transform.position, currentPath.waypoints[currentPathIndex].transform.position);
        //PathWaypoint waypoint = currentPath.waypoints[currentPathIndex];
        //if (distance < .5f)
        //{
            /*if (currentPathIndex == currentPath.waypoints.Count - 1)
            {
                if (waypoint.waypointType == PathWaypoint.WaypointType.split)
                {
                    //currentPath = waypoint.splitRight.GetComponent<RoadPath>();
                }
                currentPathIndex = 0;
            }
            else
            {
                currentPathIndex++;
            }*/
        //}
        //else if (distance > .5f && distance < 4f && waypoint.waypointType == PathWaypoint.WaypointType.split)
        //{
            /*if (waypoint.splitLeft == null)
            {
                //currentPath = waypoint.splitRight.GetComponent<RoadPath>();
            }
            else
            {
                //currentPath = waypoint.splitLeft.GetComponent<RoadPath>();
            }*/
        //    currentPathIndex = 0;
        //}
    }

    public float CalculateBrakeForce(Vector3 stopPos)
    {
        float distance = Vector3.Distance(transform.position, stopPos);
        float brakeForce = 0.5f * GetComponent<Rigidbody>().mass * Mathf.Pow(GetSpeed(), 2) / distance;
        //print(brakeForce);
        return brakeForce;
    }

    public float GetSpeed()
    {
        //float speed = 2 * Mathf.PI * wheelColliderFL.radius * wheelColliderFL.rpm * 60 / 1000;
        Rigidbody carBody = GetComponent<Rigidbody>();
        Vector3 velocity = carBody.velocity;
        if (velocity.magnitude < 1)
        {
            return Mathf.Round(velocity.magnitude);
        }
        return velocity.magnitude;
    }

    //public RoadPath currentPath;

    public void OnSpawn(/*RoadPath path*/)
    {
        //currentPath = path;
        driving = true;
       // StartCoroutine(FollowCarPath());
    }

    bool driving = false;
    int currentPathIndex = 0;

/*IEnumerator FollowCarPath()
    {
        while (driving)
        {
            if (currentPathIndex < currentPath.waypoints.Count-1)
            {
                ApplySteer(currentPath.waypoints[currentPathIndex]);
                RaycastHit hit;
                Ray carRay = new Ray(new Vector3(transform.position.x, transform.position.y + .25f, transform.position.z), Vector3.forward * 12);
                Debug.DrawRay(carRay.origin, carRay.direction * 12);
                if (Physics.Raycast(carRay.origin, carRay.direction, out hit, 12))
                {
                    float distance = Vector3.Distance(transform.position, hit.point);
                    //float brakeTorque = MapValue(distance, 0, 15, maxSpeed * 5, maxSpeed);
                    Brake(hit.point);
                }
                else
                {
                    print("Driving forward");
                    DriveForward();
                }
                float distanceToWaypoint = Vector3.Distance(transform.position, currentPath.waypoints[currentPathIndex].transform.position);
                if (distanceToWaypoint < 1)
                {
                    if (currentPath.waypoints[currentPathIndex].waypointType == PathWaypoint.WaypointType.split)
                    {
                        currentPath = currentPath.waypoints[currentPathIndex].splitRight.GetComponent<RoadPath>();
                        currentPathIndex = 0;
                        yield return null;
                    }
                    currentPathIndex++;
                }
            }
            yield return null;
        }
        print("Ending coroutine");
    }*/

    float MapValue(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}