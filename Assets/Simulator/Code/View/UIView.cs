using UnityEngine;
using UnityEngine.UI;

namespace Simulator.View.UI
{
    public class UIView : MonoBehaviour
    {
        [SerializeField]
        Text leftOutput;

        [SerializeField]
        Text rightOutput;

        public void UpdateInput(float lx, float ly, float rx, float ry)
        {
            leftOutput.text = $"LX: {lx:0.0}\nLY: {ly:0.0}";
            rightOutput.text = $"RX: {rx:0.0}\nRY: {ry:0.0}";
        }
    }
}