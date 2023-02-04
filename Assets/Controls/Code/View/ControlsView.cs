using UnityEngine;
using UnityEngine.UI;

namespace Controls.View.UI
{
    public class ControlsView : MonoBehaviour
    {
        [SerializeField]
        Text leftOutput;

        [SerializeField]
        Text rightOutput;

        [SerializeField]
        Image connectionIndicator;

        public void SetConnectionIndicator(bool flag)
        {
            connectionIndicator.gameObject.SetActive(flag);
        }

        public void UpdateInput(Vector2 left, Vector2 right)
        {
            leftOutput.text = $"LX: {left.x:0.0}\nLY: {left.y:0.0}";
            rightOutput.text = $"RX: {right.x:0.0}\nRY: {right.y:0.0}";
        }
    }
}
