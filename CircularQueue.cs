using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZSS
{
    internal class CircularQueue
    {
        int enQueuePtr;
        int deQueuePtr;
        int maxMatchIteratorPtr;
        int maxmatchLength;
        char[] Queue;
        public CircularQueue(int queueSize)
        {
            Queue = new char[queueSize];
             enQueuePtr = -1;
             deQueuePtr = -1;
        }
        public bool IsFull()
        {
            if(deQueuePtr == enQueuePtr + 1 )
            {
                return true;
            }
            return false;
        }
        public bool isEmpity()
        {
            if(deQueuePtr ==enQueuePtr )
            {
                return true;
            }
            return false;
        }
        public void enQueue(char character)
        {
            if (IsFull())
            {
                return ;  
            }
            else if(enQueuePtr ==-1 && deQueuePtr == -1)
            {
                enQueuePtr++;
                deQueuePtr++;
                Queue[enQueuePtr] = character;
                
            }
            else
            {
                enQueuePtr = (enQueuePtr + 1) % Queue.Length;
                Queue[enQueuePtr] = character;
                
            }
        }
        public char deQueue()
        {
            if (isEmpity())
            {
                return '|';
            }
            else
            {
                char temChar =Queue[deQueuePtr];
                deQueuePtr = ((deQueuePtr+1) %Queue.Length);
                return temChar;
            }
        }
        public int getDeQueueIndex()
        {
            
                return deQueuePtr;
            
        }
        
       public int getEnQueueIndex()
        {
            return enQueuePtr;
        }
        public char getiThIndexElement(int i)
        {
            
            return Queue[i];
        }
        
    }
}
