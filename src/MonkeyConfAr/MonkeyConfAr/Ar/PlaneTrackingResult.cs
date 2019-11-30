using System;
using System.Collections.Generic;
using System.Text;
using Urho;

namespace MonkeyConfAr.Ar
{
    public class PlaneTrackingResult
    {
        public Vector3 Position { get; private set; }

        public float ExtentsX { get; private set; }

        public float ExtentsZ { get; private set; }

        public PlaneTrackingResult(Vector3 position, float extentsX, float extentsZ)
        {
            Position = position;
            ExtentsX = extentsX;
            ExtentsZ = extentsZ;
        }
    }
}
