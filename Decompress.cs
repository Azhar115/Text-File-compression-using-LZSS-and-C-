
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LZSS
{

    public class Decompress
    {
        Constants.matchData matchdataCode;
        private CircularQueue slidingWindow;
        private CircularQueue LookAhead;
        
        public Decompress()
        {
            slidingWindow = new CircularQueue(Constants.WindowSize);
            LookAhead = new CircularQueue(Constants.LookAheadArraySize);
        }
        public void Decode(string inputFileNameForDecode, string outputFileNameForDecode)
        {
          using(FileStream readerFile = new FileStream(inputFileNameForDecode, FileMode.Open, FileAccess.Read))
          {
            using (var reader = new StreamReader(readerFile))
            {
                    using (FileStream writerFile = new FileStream(outputFileNameForDecode, FileMode.Append, FileAccess.Write))
                    {
                        using (var writer = new StreamWriter(writerFile))
                        {

                            initialzeHashTable();
                            var flags = 0; // Encoded flag
                            var flagsUsed = 7; // Not encoded flag



                            while (true)
                            {
                                flags >>= 1;
                                flagsUsed++;

                                // Shifted out all the flag bits -> read a new flag
                                var readChar = 0;
                                //uncoded flags
                                if (flagsUsed == 8)
                                {
                                    if ((readChar = reader.Read()) == -1)
                                    {
                                        break;
                                    }

                                    flags = readChar & 0xFF;
                                    flagsUsed = 0;
                                }

                                // reading the uncoding characters from input file
                                if ((flags & 1) != 0)
                                {
                                    if ((readChar = reader.Read()) == -1)
                                    {
                                        break;
                                    }

                                    // Write out byte and put it in sliding window
                                    writer.Write((char)readChar);
                                    slidingWindow.enQueue((char)readChar);

                                }
                                else
                                {

                                    if ((matchdataCode.offset = reader.Read()) == -1)
                                    {
                                        break;
                                    }

                                    if ((matchdataCode.matchLength = reader.Read()) == -1)
                                    {
                                        break;
                                    }

                                    // Unpack offset and length
                                    matchdataCode.offset <<= 4;
                                    matchdataCode.offset |= (matchdataCode.matchLength & 0x00F0) >> 4;
                                    matchdataCode.matchLength = (matchdataCode.matchLength & 0x000F) + Constants.MaxUncoded + 1;

                                    // Write out decoded string to file and lookahead
                                    for (var i = 0; i < matchdataCode.matchLength; i++)
                                    {
                                        readChar = slidingWindow.getiThIndexElement(matchdataCode.offset + i);
                                        writer.Write((char)readChar);
                                        LookAhead.enQueue((char)readChar);
                                    }


                                    // Write out decoded string to sliding window
                                    for (var i = 0; i < matchdataCode.matchLength; i++)
                                    {
                                        slidingWindow.enQueue(LookAhead.deQueue());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /*
         * filling sliding window with defaul null values as in compression
         */
        private int initialzeHashTable()
        {
            // Initializing the sliding window with same values  which we did in compression 
            
            for (var i = 0; i < Constants.WindowSize; i++)
            {
                slidingWindow.enQueue(' '); // filling the sliding value with null value as in compression
                         
            }
            return 0;  //sucessfully initialized
        }
    }
}
