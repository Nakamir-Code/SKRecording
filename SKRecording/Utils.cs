using System;
using System.Numerics;
using System.Net;
using System.Net.Sockets;
using StereoKit;
using System.Collections.Generic;

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

        public static void showAnnotation(RecordingData d)
        {
            UI.WindowBegin("", ref d.pose, UIWin.Body, UIMove.None);
            UI.Label(d.text);
            UI.WindowEnd();

        }

        public static int sum(int[] summands)
        {
            int sum = 0;
            foreach(int summand in summands)
            {
                sum += summand;
            }
            return sum;
        }

        public static void showDeletableAnnotation(RecordingData d, List<RecordingData> annots)
        {
            UI.WindowBegin("", ref d.pose, UIWin.Body, UIMove.None);
            if (UI.Button("X")) 
            {
                annots.Remove(d);
            }
            
            UI.Label(d.text);
            UI.WindowEnd();

        }

    }
}
