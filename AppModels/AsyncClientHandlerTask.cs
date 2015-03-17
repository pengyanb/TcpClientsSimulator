using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpClientsSimulator.AppUtilities;

namespace TcpClientsSimulator.AppModels
{
    public class AsyncClientHandlerTask
    {
        #region Variables
        private IPEndPoint ipEndpoint;
        private Form1 guiHandle;
        private TcpClientType tcpClientType;
        private int clientId;
        private String clientTypeString;
        public Boolean keepGoing;
        private String messageToSend = "peter,peng,test,18160201";
        #endregion

        #region Constructor
        public AsyncClientHandlerTask(Form1 guiHandle, IPEndPoint ipEndpoint, TcpClientType tcpClientType, int clientId)
        {
            keepGoing = true;
            this.guiHandle = guiHandle;
            this.ipEndpoint = ipEndpoint;
            this.tcpClientType = tcpClientType;
            this.clientId = clientId;
            switch(tcpClientType)
            {
                case TcpClientType.Normal: clientTypeString = "normal";break;
                case TcpClientType.StopAfterSync: clientTypeString = "stop after sync"; break;
                case TcpClientType.StopAfterPush: clientTypeString = "stop after client push"; break;
                case TcpClientType.StopAfterAck: clientTypeString = "stop after server ack"; break;
                default: clientTypeString = "unknown"; break;
            }
            messageToSend = String.Format("{0}{1:000}", messageToSend, clientId);
            lock(guiHandle.asyncClientHandlerTaskList)
            {
                guiHandle.asyncClientHandlerTaskList.Add(this);
            }
        } 
        #endregion

        public async void runTcpClientProcess()
        {
            await Task.Run(async () => { 
                TcpClient tcpClient = null;

                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipEndpoint);
                    updateGui("[Client " + clientId + " @ " + StaticUtilities.getNowString() + "], type: " + clientTypeString + ", created.");
                    
                    if(tcpClientType == TcpClientType.Normal 
                        || tcpClientType == TcpClientType.StopAfterAck
                        || tcpClientType == TcpClientType.StopAfterPush)  //push tcp Message
                    {
                        NetworkStream netStream = tcpClient.GetStream();
                        
                        if (netStream.CanWrite)
                        {
                            byte[] outBuffer = Encoding.GetEncoding(28591).GetBytes(messageToSend);
                            await netStream.WriteAsync(outBuffer, 0, outBuffer.Length);
                            updateGui("[Client " + clientId + " @ " + StaticUtilities.getNowString() + "] write: " + messageToSend);
                        }
                        
                    }
                    if(tcpClientType == TcpClientType.Normal || 
                        tcpClientType == TcpClientType.StopAfterAck)      //read server ack - echo message
                    {
                        NetworkStream netStream = tcpClient.GetStream();
                        
                        if (netStream.CanRead)
                        {
                            byte[] inBuffer = new byte[2048];
                            int bytesRead = await netStream.ReadAsync(inBuffer, 0, inBuffer.Length);
                            if(bytesRead > 0)
                            {
                                String inString = Encoding.GetEncoding(28591).GetString(inBuffer, 0, bytesRead);
                                updateGui("[Client " + clientId + " @ " + StaticUtilities.getNowString() + "] read: " + messageToSend);
                            }
                        }  
                    }
                    if(tcpClientType == TcpClientType.Normal)               //close socket properly
                    {
                        keepGoing = false;
                    }
                    while(keepGoing)
                    {
                        Tuple<Boolean, TcpState> tcpConnectionState = checkTcpConnectionState(tcpClient);
                        if(!tcpConnectionState.Item1)
                        {
                            keepGoing = false;
                            updateGui("[Client " + clientId + " @ " + StaticUtilities.getNowString() + "] - connnection state: " + tcpConnectionState.Item2.ToString());
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                if(tcpClient != null)
                {
                    try
                    {
                        if (tcpClient.GetStream() != null)
                        {
                            tcpClient.GetStream().Close();
                        }
                        tcpClient.Close();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    updateGui("[Client " + clientId + " @ " + StaticUtilities.getNowString() + "] - closed");
                }
                lock(guiHandle.asyncClientHandlerTaskList)
                {
                    guiHandle.asyncClientHandlerTaskList.Remove(this);
                    guiHandle.BeginInvoke(new Action(()=>{
                        switch (tcpClientType)
                        {
                            case TcpClientType.Normal:
                                guiHandle.countClientType1Complete++;
                                guiHandle.progressBar1.Value = (int)(guiHandle.countClientType1Complete * 100.0 / guiHandle.countClientType1Total);
                                Console.WriteLine("Progress: " + guiHandle.progressBar1.Value);
                                break;
                            case TcpClientType.StopAfterSync:
                                guiHandle.countClientType2Complete++;
                                guiHandle.progressBar2.Value = (int)(guiHandle.countClientType2Complete * 100.0 / guiHandle.countClientType2Total);
                                break;
                            case TcpClientType.StopAfterPush:
                                guiHandle.countClientType3Complete++;
                                guiHandle.progressBar3.Value = (int)(guiHandle.countClientType3Complete * 100.0 / guiHandle.countClientType3Total);
                                break;
                            case TcpClientType.StopAfterAck:
                                guiHandle.countClientType4Complete++;
                                guiHandle.progressBar4.Value = (int)(guiHandle.countClientType4Complete * 100.0 / guiHandle.countClientType4Total);
                                break;
                        }
                        if (guiHandle.asyncClientHandlerTaskList.Count == 0)
                        {
                            guiHandle.button1.Text = "Start Simulation";
                            guiHandle.button1.Enabled = true;
                        }
                    }));
                }
            });
        }

        #region checkTcpConnectionState
        public Tuple<Boolean, TcpState> checkTcpConnectionState(TcpClient tcpClient)
        {
            Boolean isConnnected = false;
            TcpState stateOfConnection = TcpState.Unknown;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections()
                .Where(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint))
                .ToArray();
            if (tcpConnections != null && tcpConnections.Length > 0)
            {
                stateOfConnection = tcpConnections.First().State;
                if (stateOfConnection == TcpState.Established)
                {
                    isConnnected = true;
                }
            }
            return Tuple.Create(isConnnected, stateOfConnection);
        } 
        #endregion

        #region updateGui
        public void updateGui(String updateString)
        {
            guiHandle.BeginInvoke(new Action(() =>
            {
                guiHandle.richTextBox1.Text += updateString + "\n";
            }));
        } 
        #endregion
    }
}
