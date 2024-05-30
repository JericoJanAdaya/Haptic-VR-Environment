using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysicsPresence : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;

    // Damping parameters
    public float positionDamping = 10f;
    public float rotationDamping = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocityTarget = (target.position - transform.position) / Time.fixedDeltaTime;
        Vector3 velocityError = velocityTarget - rb.velocity;
        rb.AddForce(velocityError * positionDamping, ForceMode.Acceleration);

        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;
        Vector3 angularVelocityTarget = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
        Vector3 angularVelocityError = angularVelocityTarget - rb.angularVelocity;
        rb.AddTorque(angularVelocityError * rotationDamping, ForceMode.Acceleration);
    }
}
