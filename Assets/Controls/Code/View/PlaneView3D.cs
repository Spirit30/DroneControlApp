using UnityEngine;

namespace Controls.View
{
    class PlaneView3D : MonoBehaviour
    {
        [SerializeField]
        Transform plane;

        [SerializeField]
        MeshRenderer top;

        [SerializeField]
        MeshRenderer bottom;

        [SerializeField]
        MeshRenderer left;

        [SerializeField]
        MeshRenderer right;

        [SerializeField]
        MeshRenderer front;

        Material Top => top.material;
        Material Bottom => bottom.material;
        Material Left => left.material;
        Material Right => right.material;
        Material Front => front.material;

        Vector3 previousPosition;

        public void Show(bool flag)
        {
            gameObject.SetActive(flag);
        }

        public void Rotate(float x, float y, float z)
        {
            plane.rotation = Quaternion.Euler(x, y, z);
        }

        public void Move(float x, float y, float z)
        {
            var newPosition = new Vector3(x, y, z);
            var deltaPosition = newPosition - previousPosition;
            previousPosition = newPosition;

            float showSpeed = 10.0f * Time.deltaTime;

            Top.mainTextureOffset -= new Vector2(-deltaPosition.x, deltaPosition.z) * showSpeed;
            Bottom.mainTextureOffset += new Vector2(deltaPosition.x, deltaPosition.z) * showSpeed;
            Left.mainTextureOffset += new Vector2(deltaPosition.y, -deltaPosition.z) * showSpeed;
            Right.mainTextureOffset += new Vector2(deltaPosition.y, deltaPosition.z) * showSpeed;
            Front.mainTextureOffset += new Vector2(deltaPosition.x, deltaPosition.y) * showSpeed;

            //Debug position.
            //plane.transform.position = new Vector3(x, y, z);
        }
    }
}