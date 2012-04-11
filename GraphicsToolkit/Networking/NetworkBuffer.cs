using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsToolkit.Networking
{
    public class NetworkBuffer
    {
        public byte[] WriteBuffer;
        public byte[] ReadBuffer;
        public int CurrentWriteByteCount;
    }
}
