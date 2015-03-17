using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientsSimulator.AppModels
{
    public class AsyncClientsCreatorTask
    {
        #region Variables
        private IPAddress serverIp;
        private int serverPort;
        private UInt32 clientType1Count;
        private UInt32 clientType2Count;
        private UInt32 clientType3Count;
        private UInt32 clientType4Count;
        private Form1 guiHandle;

        #endregion

        #region Constructor
        public AsyncClientsCreatorTask(Form1 guiHandle, IPAddress serverIp, int serverPort, UInt32 clientType1Count, UInt32 clientType2Count, UInt32 clientType3Count, UInt32 clientType4Count)
        {
            this.guiHandle = guiHandle;
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.clientType1Count = clientType1Count;
            this.clientType2Count = clientType2Count;
            this.clientType3Count = clientType3Count;
            this.clientType4Count = clientType4Count;
        } 
        #endregion

        public async Task<Boolean> startClientsCreation()
        {
            return await Task.Run(() => {
                Boolean result = false;
                int clientID = 0;
                try
                {
                    for (int i = 0; i < clientType1Count; i++, clientID++)
                    {         
                        AsyncClientHandlerTask asyncClientHandlerTask = new AsyncClientHandlerTask(guiHandle, new IPEndPoint(serverIp, serverPort), TcpClientType.Normal, clientID);
                        asyncClientHandlerTask.runTcpClientProcess();
                    }
                    for (int i = 0; i < clientType2Count; i++, clientID++)
                    {
                        AsyncClientHandlerTask asyncClientHandlerTask = new AsyncClientHandlerTask(guiHandle, new IPEndPoint(serverIp, serverPort), TcpClientType.StopAfterSync, clientID);
                        asyncClientHandlerTask.runTcpClientProcess();
                    }
                    for (int i = 0; i < clientType3Count; i++, clientID++)
                    {
                        AsyncClientHandlerTask asyncClientHandlerTask = new AsyncClientHandlerTask(guiHandle, new IPEndPoint(serverIp, serverPort), TcpClientType.StopAfterPush, clientID);
                        asyncClientHandlerTask.runTcpClientProcess();
                    }
                    for (int i = 0; i < clientType4Count; i++, clientID++)
                    {
                        AsyncClientHandlerTask asyncClientHandlerTask = new AsyncClientHandlerTask(guiHandle, new IPEndPoint(serverIp, serverPort), TcpClientType.StopAfterAck, clientID);
                        asyncClientHandlerTask.runTcpClientProcess();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    result = false;
                }
                return result;
            });
        }
    }

    public enum TcpClientType {Normal, StopAfterSync, StopAfterPush, StopAfterAck }
}
