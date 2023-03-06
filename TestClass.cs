using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace LZSS
{
    internal class TestClass
    {
        /*  static void data(string data)
          {
              char[] chara = data.ToCharArray();
              for(int i = 0; i < chara.Length; i++) { 
                 System.Console.Write(chara[i]);
              }
          }*/
        public const string inputFileForEncode = @"D:\c# vp\books\LZSS\one\inputforENcodecode.txt";
        private const string outputFileForEncode = @"D:\c# vp\books\LZSS\one\outputforencoding.txt";
        private const string inputFileForDecode = @"D:\c# vp\books\LZSS\one\outputforencoding.txt";
        private const string outputFileForDecode = @"D:\c# vp\books\LZSS\one\outputfordecoding.txt";
        public static void Main(string[] args)
        {

            //string data1 = "ali is a good boy";
            //  data(data1);
            CompressClass comObj = new CompressClass();
            Decompress decompressObj = new Decompress();
            
               
                    comObj.EncodeFile(inputFileForEncode, outputFileForEncode);
                //  Console.WriteLine("Time elapsed: " + (double)watch.ElapsedMilliseconds / 1000 + " s");
                    Console.WriteLine("file is encoded");
                    
                       
                    decompressObj.Decode(inputFileForDecode, outputFileForDecode);
                       
                       
                    
                
           
        }


    }
}   
