using System;
using System.Numerics;
using System.Net;
using System.Net.Sockets;
using StereoKit;
using System.Collections.Generic;

namespace SKRecording
{
    // Utility functions used by other classes
    class Utils
    {
        // Gets the ip address this program is running on
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

        // Sums up an arrat of integers
        public static int sum(int[] summands)
        {
            int sum = 0;
            foreach(int summand in summands)
            {
                sum += summand;
            }
            return sum;
        }


        // Displays an annotation according to the provided RecordingData
        // Note: These are purely the annotation wihtout the option to delete them. This is usually used
        // when doing playback, as we don't want the viewer deleting annotations
        // TODO: Maybe move into its own VRAnnotation class for standardization
        public static void showAnnotation(RecordingData d)
        {
            UI.WindowBegin("", ref d.pose, UIWin.Body, UIMove.None);
            UI.Label(d.text);
            UI.WindowEnd();

        }

        // Displays an annotation according to the provided RecordingData along with an x button to delete them
        // from the current list of annotations.
        // TODO: Maybe move into its own VRAnnotation class for standardization
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
