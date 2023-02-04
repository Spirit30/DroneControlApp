using System;

namespace Drone.Network.Message
{
    class MessageListener
    {
        public event Action<MessageReceived> OnMessage = delegate { };

        MessageReceived message;

        public void Add(byte @byte)
        {
            if(message.IsEmpty())
            {
                message = new MessageReceived(@byte);
            }
            else
            {
                message.AddByte(@byte);

                if(message.IsComplete())
                {
                    OnMessage(message);
                    message = default;
                }
            }
        }
    }
}