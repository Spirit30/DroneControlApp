using UnityEngine;

namespace Simulator.View
{
    public class DroneView : MonoBehaviour
    {
        [SerializeField]
        ServoView leftBack;

        [SerializeField]
        ServoView leftForward;

        [SerializeField]
        ServoView rightBack;

        [SerializeField]
        ServoView rightForward;

        [SerializeField]
        MeshRenderer connectionIndicator;

        [SerializeField]
        Color offColor = Color.red;

        [SerializeField]
        Color onColor = Color.green;

        [SerializeField]
        Rigidbody body;

        [SerializeField]
        Vector3 gravity = Vector3.down * 100;

        [SerializeField]
        float force = 1000.0f;

        [SerializeField]
        float yawForce = 10.0f;

        [SerializeField]
        float pitchForce = 20.0f;

        [SerializeField]
        float rollForce = 20.0f;

        [SerializeField]
        float pitchAngleForce = 5.0f;

        [SerializeField]
        float rollAngleForce = 5.0f;

        [SerializeReference]
        float maxAngle = 2.0f;

        [SerializeField]
        BoxCollider boundsLimit;

        Vector3 initialPosition;
        Vector3 movement;
        Vector3 angles;
        bool isCollide;
        bool isArmed;

        public void UpdateConnected(bool flag)
        {
            connectionIndicator.material.color = flag ? onColor : offColor;
        }

        public void UpdateInput(float yaw, float throttle, float roll, float pitch)
        {
            leftBack.Amount =
            leftForward.Amount =
            rightBack.Amount =
            rightForward.Amount = throttle;

            movement = gravity + Vector3.up * Time.fixedDeltaTime * force * throttle;
            movement += transform.right * rollForce * roll * throttle;
            movement += transform.forward * pitchForce * pitch * throttle;

            if (!boundsLimit.bounds.Contains(transform.position + movement))
            {
                movement = -transform.position.normalized * gravity.magnitude;
            }

            angles.x = Mathf.Clamp(pitchAngleForce * pitch * throttle, -maxAngle, maxAngle);
            angles.y += yawForce * yaw * throttle;
            angles.z = -Mathf.Clamp(rollAngleForce * roll * throttle, -maxAngle, maxAngle);

            isArmed = !Mathf.Approximately(throttle, 0.0f);
        }

        void Start()
        {
            initialPosition = transform.position;
            movement = gravity;
            angles = body.transform.eulerAngles;
        }

        void FixedUpdate()
        {
            body.velocity = movement;

            if (isArmed && !isCollide)
            {
                body.MoveRotation(Quaternion.Euler(angles));
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            isCollide = true;
        }

        void OnCollisionExit(Collision collision)
        {
            isCollide = false;
        }
    }
}