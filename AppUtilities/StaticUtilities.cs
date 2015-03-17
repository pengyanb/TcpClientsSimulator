using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientsSimulator.AppUtilities
{
    public static class StaticUtilities
    {
        #region getHostIpAddress
        public static String getHostIpAddress()
        {
            String hostIpAddress = "";
            NetworkInterface[] networkInterfaceList = NetworkInterface.GetAllNetworkInterfaces();
            Boolean foundHostIp = false;
            foreach (NetworkInterface networkInterface in networkInterfaceList)
            {
                //GatewayIPAddressInformation gatewayAddressInfo = networkInterface.GetIPProperties().GatewayAddresses.First();
                //if (gatewayAddressInfo != null && gatewayAddressInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                //Console.WriteLine("[" + networkInterface.Description+"]");
                if (!networkInterface.Description.Contains("VirtualBox"))
                {
                    foreach (var unicastIp in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        IPAddress ipAddress = unicastIp.Address;
                        if ((networkInterface.OperationalStatus == OperationalStatus.Up) && (ipAddress.AddressFamily == AddressFamily.InterNetwork))
                        {
                            hostIpAddress = ipAddress.ToString();
                            foundHostIp = true;
                            break;
                        }
                    }
                }
                if (foundHostIp) break;
            }
            return hostIpAddress;
        }
        #endregion

        #region isValidIpv4String
        public static Tuple<Boolean, IPAddress> isValidIpv4String(String inputString)
        {
            IPAddress ipAddress = null;
            Boolean result = !String.IsNullOrEmpty(inputString) && IPAddress.TryParse(inputString, out ipAddress);
            return Tuple.Create(result, ipAddress);
        } 
        #endregion

        #region isValidPortNumber
        public static Tuple<Boolean, int> isValidPortNumber(string inputString)
        {
            int portNum = -1;
            Boolean result = false;
            try
            {
                portNum = Convert.ToInt32(inputString);
                if (portNum > 0 && portNum <= 65535)
                    result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                result = false;
            }
            return Tuple.Create(result, portNum);
        } 
        #endregion

        #region isValidUnsignedInt
        public static Tuple<Boolean, UInt32> isValidUnsignedInt(String inputString)
        {
            UInt32 outputNumber = 0;
            Boolean result = false;
            try
            {
                outputNumber = Convert.ToUInt32(inputString);
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Tuple.Create(result, outputNumber);
        } 
        #endregion

        #region getNowString
        public static String getNowString()
        {
            return String.Format("{0:yyyy}/{0:MM}/{0:dd} {0:hh}:{0:mm}:{0:ss}", DateTime.Now);
        } 
        #endregion
    }
}
