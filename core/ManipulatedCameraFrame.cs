using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class ManipulatedCameraFrame : ManipulatedFrame
    {
        public Vector3f RevolveAroundPoint { get; set; }
    }
}
