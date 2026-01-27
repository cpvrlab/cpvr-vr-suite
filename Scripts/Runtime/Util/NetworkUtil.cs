using System.Net;
using System.Net.Sockets;

namespace cpvr_vr_suite.Scripts.Runtime.Util
{
    /// <summary>
    /// Network helper methods.
    /// </summary>
    public static class NetworkUtil {
        /// <summary>
        /// Get the local ip address of the machine.
        /// </summary>
        /// <returns>The current IP address as a string or '?'</returns>
        public static string GetLocalIpAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "?";
        }
    }
}