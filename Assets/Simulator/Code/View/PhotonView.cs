using Controls.Logic.Input;
using Photon.Pun;
using Simulator.Logic;

namespace Simulator.View
{
    public class PhotonView : MonoBehaviourPun, IPunObservable
    {
        PhotonSender sender;
        PhotonReceiver receiver;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(sender.LX);
                stream.SendNext(sender.LY);
                stream.SendNext(sender.RX);
                stream.SendNext(sender.RY);
            }
            else if(stream.IsReading && receiver)
            {
                float lx = (float)stream.ReceiveNext();
                float ly = (float)stream.ReceiveNext();
                float rx = (float)stream.ReceiveNext();
                float ry = (float)stream.ReceiveNext();

                receiver.UIview.UpdateInput(lx, ly, rx, ry);
                receiver.DroneView.UpdateInput(lx, ly, rx, ry);
            }
        }

        void Awake()
        {
            if (photonView.IsMine)
            {
                sender = FindObjectOfType<PhotonSender>();
            }
            else
            {
                receiver = FindObjectOfType<PhotonReceiver>();
            }
        }
    }
}