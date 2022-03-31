using System;
using System.Numerics;
using System.Net;
using System.Net.Sockets;
using StereoKit;

namespace SKRecording
{
    class Utils
    {
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

        public static Vector3 SubtractVec3s(Vector3 isSubtractedFrom, Vector3 isSubtracted)
        {
            Vector3 res = isSubtractedFrom;
            res.X = res.X - isSubtracted.X;
            res.Y = res.Y - isSubtracted.Y;
            res.Z = res.Z - isSubtracted.Z;

            return res;

        }
    }
}
