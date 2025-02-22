using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client_Assembly
{
    public class Client
    {
        static private byte in_praiseEventId, out_praiseEventId;
        static private Thread thread_RecieveResult;

        public Client()
        {
            in_praiseEventId = 0;
            out_praiseEventId = 0;
        }

        static public void Create_Client_Networking()
        {
            Valve.Sockets.Library.Initialize();
            //Client_Assembly.Client_Networking.CreateNetworkingClient();
            //thread_RecieveResult = new Thread(Client_Assembly.Client.Thread_RecieveResult);
            //thread_RecieveResult.Start();
            //Console.WriteLine("Press ENTER to generate input event.");
            //Console.ReadLine();
            //Client_Assembly.Client.Generate_InputAction(69);
        }

/*       static public void Generate_InputAction(int value)
        {
            Set_in_praiseEventId(value);
            Client_Assembly.Client_Networking.CreateAndSendNewMessage();
        }
*/
        static public void Initialise_ThisClientWithServer()
        {
            string ip_Address = GetLocalIPAddress();
            Console.WriteLine("IP Address is " + ip_Address);
            Client_Assembly.Client_Networking.CreateAndSendNewMessage(0);
        }

        static public void Thread_RecieveResult()
        {
            while (true)
            {
                //Client_Assembly.Client_Networking.CopyPayloadFromMessage();

            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        static public byte Get_in_praiseEventId()
        {
            return in_praiseEventId;
        }
        static public byte Get_out_praiseEventId()
        {
            return out_praiseEventId;
        }

        static private void Set_in_praiseEventId(byte value)
        {
            in_praiseEventId = value;
        }
        static private void Set_out_praiseEventId(byte value)
        {
            out_praiseEventId = value;
        }
    }
}