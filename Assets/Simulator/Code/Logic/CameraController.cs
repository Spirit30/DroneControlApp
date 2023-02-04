using UnityEngine;

namespace Simulator.Logic
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        Transform target = null;

        [SerializeField] 
        float distance = 5.0f;

        [SerializeField] 
        float xSpeed = 120.0f;
        [SerializeField]
        float ySpeed = 120.0f;

        [SerializeField]
        float yMinLimit = -20f;
        [SerializeField]
        float yMaxLimit = 80f;

        [SerializeField]
        float distanceMin = .5f;
        [SerializeField]
        float distanceMax = 15f;

        float x = 0.0f;
        float y = 0.0f;

        void Start()
        {
            ResetAndApply();
        }

        void LateUpdate()
        {
            if(Input.GetMouseButtonDown(0))
            {
                ResetAndApply();
            }
            else
            {
                UpdateAndApply();
            }
        }

        void ResetAndApply()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            UpdateAndApply();
        }

        void UpdateAndApply()
        {
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            RaycastHit hit;

            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.SetPositionAndRotation(position, rotation);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}