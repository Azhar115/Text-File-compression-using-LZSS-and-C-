using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZSSHashMap
{
    internal class Constants
    {
        public const int WindowSize = 4096;

        public const int NullIndex = WindowSize + 1;

        public const int MaxUncoded = 2;

        public const int MaxCoded = MaxUncoded + 16;

        public const int HashSize = 1024;
        public struct EncodedString
        {
            // Offset to start of longest match
            public int Offset { get; set; }

            // Length of longest match
            public int Length { get; set; }
        }
    }
}
