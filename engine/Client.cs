using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Assembly
{
    public class Client
    {
        static private int praiseEventId;
        static private Thread thread_RecieveResult;

        public Client()
        {
            praiseEventId = 0;
        }

        static public void Create_Client_Networking()
        {
            Valve.Sockets.Library.Initialize();
            Client_Assembly.Client_Networking.CreateNetworkingClient();
            thread_RecieveResult = new Thread(Client_Assembly.Client.Thread_RecieveResult);
            thread_RecieveResult.Start();
        }

        static public void Generate_InputAction()
        {
            Client_Assembly.Client_Networking.CreateAndSendNewMessage(69);
        }

        static public void Thread_RecieveResult()
        {
            while (true)
            {
                Client_Assembly.Client_Networking.CopyPayloadFromMessage();

            }
        }
    }
}