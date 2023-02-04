using UnityEngine;

namespace Simulator.View
{
    public class ServoView : MonoBehaviour
    {
        //[SerializeField]
        //Rigidbody body;

        //[SerializeField]
        //float  force = 1000.0f;

        [SerializeField]
        MeshRenderer geometry;

        [SerializeField]
        int direction = 1;

        [SerializeField]
        float spinSpeed = 25.0f;

        public float Amount { get; set; }

        //void FixedUpdate()
        //{
        //    body.velocity = Physics.gravity + Vector3.up * Time.fixedDeltaTime * force * Amount;
        //}

        void Update()
        {
            var angles = geometry.transform.localEulerAngles;
            angles.y += direction * spinSpeed * Time.deltaTime * Amount;
            angles.y = Mathf.Clamp(angles.y, -360.0f, 360.0f);
            geometry.transform.localEulerAngles = angles;
        }
    }
}