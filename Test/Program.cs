using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Channel.Beidou500;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            List<byte>  m_inputBuffer = new List<byte> {128 ,129,130};

            foreach (byte b in m_inputBuffer)
            {
                char[] s = Encoding.ASCII.GetChars(m_inputBuffer.ToArray<byte>());
                string st = new string(s);
            }
        }
    }
}
