using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Valve.Sockets;

namespace Client_Assembly
{
    public class Client_Networking
    {
        static private uint connection;
        static private NetworkingSockets sockets = null;
        static private NetworkingMessage netMessage;
        
        public Client_Networking()
        {
            connection = 0;
            sockets = new NetworkingSockets();
            netMessage = new NetworkingMessage();
        }
        static private int BoolToInt(bool value)
        {
            int temp = 0;
            if (value) temp = 1;
            if (!value) temp = 0;
            return temp;
        }

        static private int BitArrayToInt(bool[] arr, int count)
        {
            int ret = 0;
            int tmp;
            for (int i = 0; i < count; i++)
            {
                tmp = BoolToInt(arr[i]);
                ret |= tmp << count - i - 1;
            }
            return ret;
        }


        static public void CreateNetworkingClient()
        {
            NetworkingSockets client = new NetworkingSockets();
            
            StatusCallback status = (ref StatusInfo info) =>
            {
                switch (info.connectionInfo.state)
                {
                    case ConnectionState.None:
                        break;

                    case ConnectionState.Connected:
                        Console.WriteLine("Client connected to server - ID: " + connection);
                        CopyPayloadFromMessage();
                        break;

                    case ConnectionState.ClosedByPeer:
                    case ConnectionState.ProblemDetectedLocally:
                        client.CloseConnection(connection);
                        Console.WriteLine("Client disconnected from server");
                        break;
                }
            };

            NetworkingUtils utils = new NetworkingUtils();
            utils.SetStatusCallback(status);

            Address address = new Address();

            address.SetAddress(Client_Assembly.Client.GetLocalIPAddress(), 3074);//ToDo

            connection = client.Connect(ref address);

#if VALVESOCKETS_SPAN
	MessageCallback message = (in NetworkingMessage netMessage) => {
		Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
	};
#else
            const int maxMessages = 20;

            NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif

            while (!Console.KeyAvailable)
            {
                client.RunCallbacks();

#if VALVESOCKETS_SPAN
		client.ReceiveMessagesOnConnection(connection, message, 20);
#else
                int netMessagesCount = client.ReceiveMessagesOnConnection(connection, netMessages, maxMessages);

                if (netMessagesCount > 0)
                {
                    for (int i = 0; i < netMessagesCount; i++)
                    {
                        ref NetworkingMessage netMessage = ref netMessages[i];

                        Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                        CopyPayloadFromMessage();//added

                        netMessage.Destroy();
                    }
                }
#endif

                Thread.Sleep(15);
            }
            Client_Assembly.Client.Initialise_ThisClientWithServer();
        }

        public static void CreateAndSendNewMessage(byte praiseEventId)
        {
            byte[] data = new byte[64];
            byte intBytes = new byte();
            string IP_Address_String = Client_Assembly.Client.GetLocalIPAddress();
            switch(praiseEventId)
            {
            case 0:
                intBytes = praiseEventId;
                data[0] = intBytes;
                data[1] = byte.Parse(IP_Address_String.Substring(0, 3));
                data[2] = byte.Parse(IP_Address_String.Substring(4, 7));
                data[3] = byte.Parse(IP_Address_String.Substring(8, 11));
                data[4] = byte.Parse(IP_Address_String.Substring(13, 16));

                intBytes = 0;
                for (ushort index = 5; index < data.Length; index++)
                {
                    data[index] = intBytes;
                }
                break;
            
            case 1:
                    
                break;
            }
            sockets.SendMessageToConnection(connection, data);//ToDo
        }

        public static void CopyPayloadFromMessage()
        {
            byte[] buffer = new byte[1024];
            netMessage.CopyTo(buffer);

            int praiseEventId = 0;
            bool[] temp_bool_array;
            temp_bool_array = new bool[16];
            for (short i = 0; i < 16; i++)
            {
                temp_bool_array[i] = Convert.ToBoolean(buffer[i]);
            }
            praiseEventId = BitArrayToInt(temp_bool_array, 16);
        }

        public static void SetA_HookForDebugInformation()
        {
            DebugCallback debug = (type, message) =>
            {
                Console.WriteLine("Debug - Type: " + type + ", Message: " + message);
            };

            NetworkingUtils utils = new NetworkingUtils();

            utils.SetDebugCallback(DebugType.Everything, debug);
        }
    }
}