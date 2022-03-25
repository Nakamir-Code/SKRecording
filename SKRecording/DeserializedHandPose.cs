using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKRecording
{
    public class DeserializedHandPose
    {
        public float[][] Quats { get; set; }
        public float[][] Tvecs { get; set; }
        public float[] Radii { get; set;}
    }
}
