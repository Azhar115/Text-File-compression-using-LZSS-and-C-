using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace LZSS
{
    internal class CompressClass 
    {
        //other class objects assosiation 

        public int[] slidingWindow = new int[Constants.WindowSize];
        public CircularQueue LookAhead;

        private static readonly int[] HashTable = new int[Constants.HashSize]; // List head for each hask key
        private static readonly int[] Next = new int[Constants.WindowSize]; // Indices of next elements in the hash list



        public CompressClass()
        {
            
            LookAhead = new CircularQueue(Constants.LookAheadArraySize);
        }
        public void EncodeFile(string inputFileNameForEncode, string outputFileNameForEncode)
        {
            using (FileStream readerFile = new FileStream(inputFileNameForEncode, FileMode.Open, FileAccess.Read))
            {
                using (var InputFileReader = new StreamReader(readerFile))
                {
                    using (FileStream writerFile = new FileStream(outputFileNameForEncode, FileMode.Append, FileAccess.Write))
                    {
                        using (var OutputEncodeFileWriter = new StreamWriter(writerFile))
                        {

                            var readCharacter = 0;
                            int uncodedlength = 0;
                            int slidingWindowCurrentIndex=0;
                            //store 18 char data into lookAhead
                            for (; uncodedlength < Constants.LookAheadArraySize; uncodedlength++)
                            {
                                readCharacter = InputFileReader.Read();
                                if (readCharacter == -1)
                                    break;
                                LookAhead.enQueue((char)(readCharacter));  //explicit type cast into char to store
                            }
                            //file was already empity and could not be encoded 
                            if (uncodedlength == 0)
                            {
                                return;
                            }
                            int j = initialzeHashTable();
                            // if tree initialization was not done corrrectly then do not encode
                            if (j != 0)
                            {
                                return;
                            }
                            var flags = 0;       //used to get the number of unmatched char in lookahead to be written on file without encoding.
                            var flagposition = 1;   //show the indexes in writen file after encoding that are not encoded and stored.
                            var encodedData = new char[16];  // each time 16 indexes to be encoded from lookaheadarray
                            var encodedDataIndex = 0;
                            var i = 0;  //local variable to be used
                            Constants.matchData matchData1 = FindMatch(LookAhead.getDeQueueIndex());

                            //terminates when all data from file is read and encoded and nothing is in Lookahead
                            while (uncodedlength > 0)
                            {
                                //if match length exceeds the char string length in lookahead then it is garbagen so remove garbage
                                if (matchData1.matchLength > uncodedlength)
                                {
                                    matchData1.matchLength = uncodedlength;
                                }
                                //if length of match data is 2 or 1 or less then we will not encode it, we will directly store it 
                                if (matchData1.matchLength <= Constants.MaxUncoded)
                                {
                                    matchData1.matchLength = 1;
                                    flags |= flagposition; // here we store the index of uncoded char so that we can use in decoding.
                                    encodedData[encodedDataIndex] =(char)LookAhead.getiThIndexElement(LookAhead.getDeQueueIndex()); //char at dequeue position
                                    encodedDataIndex++;      //index increme

                                }
                                else
                                {    // change int offset into char offset
                                     // as     (10bits)512 ->>changes->32(8bits)
                                     //binery   1110000000 ->>changes->00111000 
                                    encodedData[encodedDataIndex] = (char)((matchData1.offset & 0x0FFF) >> 4);
                                    encodedDataIndex++;
                                    //now suppose   match offset is 12  and match length is 7
                                    //then converting into char becomes 0xC7 
                                    encodedData[encodedDataIndex] = (char)(((matchData1.offset & 0x000F) << 4) |
                                         (matchData1.matchLength - (Constants.MaxUncoded + 1)));
                                    encodedDataIndex++;
                                   


                                }

                                //now if flagposition reaches to 128 (0x80) the encodedData array becomes full therefore we have to first save data into output file
                                // at flagposition 128(0x80) the encodedData will have 8X2(16) char data
                                if (flagposition == 0x80)
                                {
                                    System.Console.WriteLine("Offset: " + matchData1.offset + " Length: " + matchData1.matchLength);
                                    Console.ReadKey();
                                    // this stores at the start of each 16 characters which tells which ones out of 16 char are uncoded character(char<2)length stored.
                                    OutputEncodeFileWriter.Write((char)flags);
                                    System.Console.WriteLine("encoding data");
                                    for (i = 0; i < encodedDataIndex; i++)
                                    {
                                        //writing each char into output file of encoded
                                        OutputEncodeFileWriter.Write((char)encodedData[i]);
                                        System.Console.Write(encodedData[i] + ", ");
                                       
                                    }
                                    System.Console.WriteLine();
                                    // now reUse back the array for 16 char storage
                                    encodedDataIndex = 0;
                                    flags = 0;
                                    flagposition = 1;
                                }
                                else
                                {
                                    flagposition <<= 1;  //1,2,4,16...128
                                }
                                //after encoding 16 chars we have to do two works
                                //1st)  add/replace new total mathlength of characters from lookahead into sliding window 
                                //2nd) add/replace new matchlength of chararcters from input file  into back lookAhead array
                                i = 0;
                                while (i < matchData1.matchLength && (InputFileReader.Read() == -1))
                                {
                                    readCharacter = InputFileReader.Read();
                                    
                                    Char VALUE = LookAhead.deQueue();
                                    System.Console.WriteLine("LOOKAHEAD DEQUEU   " + VALUE);
                                    ReplaceChar(slidingWindowCurrentIndex,VALUE);
                                    slidingWindowCurrentIndex = (slidingWindowCurrentIndex + 1) % Constants.WindowSize;
                                    LookAhead.enQueue((char)readCharacter); //add character into lookahead
                                    i++;
                                }
                                // as we reach to end of of file means no more input remaining data pushed into sliding window without taking file input

                                while (i < matchData1.matchLength )
                                {

                                    ReplaceChar(slidingWindowCurrentIndex,LookAhead.deQueue());
                                    slidingWindowCurrentIndex = (slidingWindowCurrentIndex + 1) % Constants.WindowSize;

                                    i++;
                                    uncodedlength--;  // decreasing the length of lookahead window so the main loop terminates as input ends and lookahead array too.
                                }
                                matchData1 = FindMatch(LookAhead.getDeQueueIndex());
                            }
                            // data still in encodedData array after termination should be written on output file
                            if (encodedDataIndex != 0)
                            {
                                OutputEncodeFileWriter.Write((char)flags);
                                for (i = 0; i < encodedDataIndex; i++)
                                {
                                    //writing each char into output file of encoded
                                    OutputEncodeFileWriter.Write(encodedData[i]);
                                }
                            }
                        }
                    }
                }
            }
        }

    

        private int initialzeHashTable()
         {
            // Initializing the sliding window with same values which means 
            // there is only 1 hash key for the entier sliding window
            
            for (var i = 0;  i < Constants.WindowSize; i++)
            {
                slidingWindow[i] =' '; // filling the sliding value with null value 
                Next[i] = i+1;            // headNode ->childNode->nextChildNode->NUll_index
            }

            // There is no next for the last character
            Next[Constants.WindowSize-1] = Constants.NullIndex;

            // The only list now is the list with spaces
            for (var i = 0; i < Constants.HashSize; i++)
            {
                HashTable[i] = Constants.NullIndex;
            }

            HashTable[GetHashKey(0, false)] = 0;
            return 0; // initializing is successfull
        }
        /*
         * hash key is generated of consective three elements of taking exa_or (+)
         */
        private  int GetHashKey(int offset, bool isInLookahead)
        {
            var hashKey = 0;
            //sign (^) shows exa or
            if (isInLookahead)   // if  isInLookahead =true, it means element is of lookahead and generate for it
            {
                for (var i = 0; i < Constants.MaxUncoded + 1; i++)
                {
                    hashKey = (hashKey << 5) ^ LookAhead.getiThIndexElement(offset);
                    hashKey %= Constants.HashSize;
                    offset = (offset + 1) % Constants.LookAheadArraySize;
                }
            }
            else
            {
                for (var i = 0; i < Constants.MaxUncoded + 1; i++)
                {
                    hashKey = (hashKey << 5) ^ slidingWindow[offset];
                    hashKey %= Constants.HashSize;
                    offset = (offset + 1) % Constants.WindowSize;
                }
            }

            return hashKey;    //generated hashkey
        }

        public void ReplaceChar( int SlidingWindowindex,char newChar)
        {

            int firstIndex=0;
            if (SlidingWindowindex < Constants.MaxUncoded)
            {
                firstIndex = SlidingWindowindex + Constants.WindowSize - Constants.MaxUncoded-1;
               
            }
            else
            {
                firstIndex = SlidingWindowindex - Constants.MaxUncoded-1;
            }
            

            // Remove all hash entries containing character at charIndex
            for (var i = 0; i < Constants.MaxUncoded + 1; i++)
            {
                System.Console.WriteLine("FIRST VALUE in remove" + firstIndex+i);
                RemoveString((firstIndex + i) % Constants.WindowSize);
            }

            slidingWindow[SlidingWindowindex]  = newChar;
           

            for (var i = 0; i < Constants.MaxUncoded + 1; i++)
            {
                System.Console.WriteLine("FIRST VALUE in add" + firstIndex+i);
                AddString((firstIndex + i) % Constants.WindowSize);
            }
        }
        /*
         *  removing string from the hashtable as well next linear probe array
         */
        private  void RemoveString(int charIndex)
        {
            var nextIndex = Next[charIndex];
            Next[charIndex] = Constants.NullIndex;

            // getting hash key for sliding window elements
            var hashKey = GetHashKey(charIndex, false);
            System.Console.WriteLine("HASKEY"+hashKey);
            // We're deleting a list head
            // the key pointing to charIndex should be replaced with charIndex next element 
            if (HashTable[hashKey] == charIndex)
            {
                HashTable[hashKey] = nextIndex;
                System.Console.WriteLine("remove if "+ nextIndex);
                return;
            }
            // if the key does not pointing in hastable to charindex then find in charindex chain
            // Find character pointing to ours
            
                var i = HashTable[hashKey];
            System.Console.WriteLine("else remove i" + i);
                System.Console.WriteLine("I" + i);
                while (Next[i] != charIndex)
                {
                    i = Next[i];
                }


                Next[i] = nextIndex;  //found in chain linear probe
            
        }
        /*
         * adding string into sliding window while first assigning the hashkey to the element
         */
        private void AddString(int charIndex)
        {
            // Inserted character will be at the end of the list
            Next[charIndex] = Constants.NullIndex;
            //getting for that element in sliding window
            var hashKey = GetHashKey(charIndex, false);
            System.Console.WriteLine("HASKEY IN ADD"+ hashKey);
            // This is the only character in the list
            //if there is no element in hastable with same key, then add that in table
            if (HashTable[hashKey] == Constants.NullIndex)
            {
                System.Console.WriteLine("if add hashkey"+hashKey);
                HashTable[hashKey] = charIndex;
                return;
            }
             // if in hashtable element exist with same key then add in linear probe or chain of array
            // Find the end of the list
            var i = HashTable[hashKey];
            while (Next[i] != Constants.NullIndex)
            {
                i = Next[i];
            }

            // Add new character to the list end
            Next[i] = charIndex;
        }
        private  Constants.matchData FindMatch(int uncodedHead)
        {
            var matchData1 = new Constants.matchData();

            var i = HashTable[GetHashKey(uncodedHead, true)];
            var j = 0;

            while (i != Constants.NullIndex)
            {
                // We've matched the first symbol
                if (slidingWindow[i] == LookAhead.getiThIndexElement(uncodedHead))
                {
                    j = 1;

                    while ((slidingWindow[i + j] % Constants.WindowSize) ==
                        LookAhead.getiThIndexElement((uncodedHead + j) % Constants.LookAheadArraySize))
                    {
                        if (j >= Constants.LookAheadArraySize)
                        {
                            break;
                        }

                        j++;
                    }

                    if (j > matchData1.matchLength)
                    {
                        matchData1.matchLength = j;
                        matchData1.offset = i;
                    }
                }

                if (j >= Constants.LookAheadArraySize)
                {
                    matchData1.matchLength = Constants.LookAheadArraySize;
                    break;
                }

                i = Next[i];
            }

            return matchData1;
        }
    }

}
