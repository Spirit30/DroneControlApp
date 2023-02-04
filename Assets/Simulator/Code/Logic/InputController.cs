using UnityEngine;

namespace Simulator.Logic
{
    class InputController : MonoBehaviour
    {
        public static bool Aim { get; private set;}
        public static Vector2 AimDelta { get; private set; }

        Vector3 previousMousePosition;

        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                previousMousePosition = Input.mousePosition;
            }

            Aim = Input.GetMouseButton(0);

            if(Aim)
            {
                AimDelta = Input.mousePosition - previousMousePosition;
                previousMousePosition = Input.mousePosition;
            }
        }
    }
}