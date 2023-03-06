
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LZSS
{

    public static class Constants
    {
        private const int windowSize = 4096; // 0X0FFF (size of sliding window)
        private const int nullIndex = windowSize + 1;
        private const int hashSize = 1024;
        private  const int maxUncoded = 2;
        private const int lookAheadArraySize = 16 + maxUncoded; //size of lookahead window
        private const int breakEven = 1;
        private const int endOFStream = 0;
        private const int unUsed = 0;
       

        //properties of attributes
       
        public struct matchData
        {
            public int matchLength { set; get; }
            public int offset { set; get; }
        }
        
        public static int NullIndex { get { return nullIndex; } }
        public static int HashSize { get { return hashSize; } }
        public static int WindowSize { get { return windowSize; } }
        public static int MaxUncoded { get { return maxUncoded; } }
        public static int LookAheadArraySize { get { return lookAheadArraySize; } }
        public static int BreakEven { get { return breakEven; } }
        public static int inputFileEnds { get { return endOFStream; } }
        public static int Null { get { return unUsed; } }
       

    }}

