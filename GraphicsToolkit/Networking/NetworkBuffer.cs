using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsToolkit.Networking
{
    /// <summary>
    /// This class helps organize the data required for
    /// reading and writing to a network stream
    /// </summary>
    public class NetworkBuffer
    {
        public byte[] WriteBuffer;
        public byte[] ReadBuffer;
        public int CurrentWriteByteCount;
    }
}
