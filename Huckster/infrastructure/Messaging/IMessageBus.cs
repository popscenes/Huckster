using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Messaging
{
    public interface IMessageBus
    {
        void SendMessage(IMessage message);
    }
}
