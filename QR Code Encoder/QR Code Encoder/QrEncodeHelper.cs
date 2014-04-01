using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012
///for fancyness..

namespace QR_Code_Encoder
{
    /// <summary>
    /// this is a multipurpose class designed to be used all throughout the QR code
    /// sometimes it is used to determine the best version
    /// sometimes it is used to convert decimals into binary
    /// its just really usefull and it used many times
    /// this is the tool box for a QR Code Generator
    /// </summary>
    class QrEncodeHelper
    {
        static int version;
        static int squareSize;
        //sometimes the string is needed
        //sometimes it is not
        static string theMessage;

        public QrEncodeHelper(int versionIn)
        {
            version = versionIn;
            squareSize = version * 4 + 17 + 2;
        }

        public QrEncodeHelper(string message)
        {
            theMessage = message;
        }

        /// <summary>
        /// converts a decimal value into a bool queue with the right amount of padding (0's) on the front
        /// </summary>
        /// <param name="toBool">the decimal value that will be converted to binary</param>
        /// <param name="padding">the total length of digits the number requires to be after being 
        /// converted to binary. this is commonly known as padding </param>
        /// <returns>a queue that has the right amount of padding. the most significant digit is 
        /// the first item in the queue</returns>
        public Queue<bool> BitsWithPadding(int toBool, int padding)
        {
            Queue<bool> outQueue = new Queue<bool>();

            for (int i = padding - 1; i >= 0; i--)
            {
                if (toBool == 0)
                    outQueue.Enqueue(false);
                else if (Math.Pow(2, i) <= toBool)
                {
                    outQueue.Enqueue(true);
                    toBool -= (int)Math.Pow(2, i);
                }
                else
                    outQueue.Enqueue(false);
            }
            return outQueue;
        }

        //converts a boolean queue to a decimal value
        public int BitsToDemical(Queue<bool> theQueue)
        {
            int intExport = 0;
            //cycles through the queue, adding the values that make up the decimal value
            for (int i = theQueue.Count; i > 0; i--)
            {
                if (theQueue.Peek() == true)
                    intExport += (int)Math.Pow(2, i - 1);

                theQueue.Dequeue();
            }
            return intExport;
        }

        //takes an queue and adds false items to it to make it a multiple of 8
        public Queue<bool> ModTo8(Queue<bool> outQueue)
        {
            int loopUntil = 8 - outQueue.Count % 8;
            for (int i = 0; i < loopUntil; i++)
            {
                outQueue.Enqueue(false);
            }
            return outQueue;
        }

        /// <summary>
        /// takes a queue that is already a multiple of 8 (or should be a multiple of 8)
        /// and converts each 8 bits into an integer
        /// </summary>
        /// <param name="inQueue">the queue that will be converted into 8 bit code words</param>
        /// <returns>a list of integers in order that represent that 8 bit code words</returns>
        public List<int> QueueToDataCodeWords(Queue<bool> inQueue)
        {
            List<int> outIntList = new List<int>();

            //creates boolean list from the queue
            List<bool> tempBoolList = inQueue.ToList();

            //rotate through each group of 8
            for (int i = 0; i < tempBoolList.Count / 8; i++)
            {
                //temp queue for conversion
                Queue<bool> tempBoolQueue = new Queue<bool>();

                //cycle through the 8 in the group
                for (int k = 0; k < 8; k++)
                {
                    //add the current item (using the multiplier) to the queue
                    tempBoolQueue.Enqueue(tempBoolList[i * 8 + k]);
                }

                //add the converted codeword to the list for exportation
                outIntList.Add(BitsToDemical(tempBoolQueue));
            }
            if (outIntList[outIntList.Count - 1] == 0)
            {
                outIntList.RemoveAt(outIntList.Count - 1);
            }
            return outIntList;
        }

        public int DetermineBestVersionAndErrorCorrection(int encodingType, bool encodingSwitch, out int errorCorrectionLevel)
        {
            int messageLength = theMessage.Length;
            errorCorrectionLevel = 0;

            switch (encodingSwitch)
            {
                //this mode gives the most amount of characters available, and does also NOT function so do not use it
                case false:
                    //version 1
                    if (messageLength < 26)
                    {
                        //high
                        if (messageLength < 11)
                            errorCorrectionLevel = 2;
                        //quality
                        else if (messageLength < 17)
                            errorCorrectionLevel = 3;
                        //medium
                        else if (messageLength < 21)
                            errorCorrectionLevel = 0;
                        else//low
                            errorCorrectionLevel = 1;
                        return 1;
                    }
                    else if (messageLength < 48)
                    {
                        //quality
                        if (messageLength < 30)
                            errorCorrectionLevel = 3;
                        //medium
                        else if (messageLength < 39)
                            errorCorrectionLevel = 0;
                        else
                            //low
                            errorCorrectionLevel = 1;
                        return 2;
                    }
                    else if (messageLength < 78)
                    {
                        //medium
                        if (messageLength < 62)
                            errorCorrectionLevel = 0;
                        else
                            //low
                            errorCorrectionLevel = 1;
                        return 3;
                    }
                    else if (messageLength < 115)
                    {
                        //medium
                        //if (messageLength < 91)
                            //errorCorrectionLevel = 0;
                        //else 
                            //low
                            errorCorrectionLevel = 1;
                        return 4;
                    }
                    break;

                //highest or best error correction possible
                //this is also a mess, it works, just accept it and move on
                case true:
                    //version 1
                    if (messageLength < 15)
                    {
                        //high
                        if (messageLength < 9)
                            errorCorrectionLevel = 2;
                        //quality
                        else if (messageLength < 17)
                            errorCorrectionLevel = 3;
                        else
                            //medium
                            errorCorrectionLevel = 0;

                        return 1;
                    }
                    else if (messageLength < 28)
                    {
                        //high
                        if (messageLength < 19)
                            errorCorrectionLevel = 2;
                        //quality
                        else if (messageLength < 30)
                            errorCorrectionLevel = 3;
                        return 2;
                    }
                    else if (messageLength < 45)
                    {
                        //high
                        if (messageLength < 34)
                            errorCorrectionLevel = 2;
                        //quality
                        else 
                            errorCorrectionLevel = 3;
                        return 3;
                    }
                    else if (messageLength < 66)
                    {
                        //high
                        if (messageLength < 51)
                            errorCorrectionLevel = 2;
                        //quality
                        else 
                            errorCorrectionLevel = 3;
                        return 4;
                    }/* this version currently does not work, please come back later or give us a call at BLAHBLAHBLAH-BLAHBLAHBLAH-BLAHBLAHBLAHBLAH
                    else if (messageLength < 84)
                    {
                        //high
                        if (messageLength < 65)
                            errorCorrectionLevel = 2;
                        //quality
                        else
                            errorCorrectionLevel = 3;
                        return 5;
                    }*/
                    else if (messageLength < 109)
                    {
                        //high
                        if (messageLength < 85)
                            errorCorrectionLevel = 2;
                        //quality
                        else if (messageLength < 109)
                            errorCorrectionLevel = 3;
                        else if (messageLength < 150)
                            //medium
                            errorCorrectionLevel = 0;
                        else
                            errorCorrectionLevel = 1;
                        return 6;
                    }
                    else if (messageLength < 224)
                    {
                        //high
                        if (messageLength < 94)
                            errorCorrectionLevel = 2;
                        //quality
                        else
                            errorCorrectionLevel = 1;
                        return 7;
                    }
                    else if (messageLength < 143)
                    {
                        //high
                        if (messageLength < 123)
                            errorCorrectionLevel = 2;
                        //quality
                        else
                            errorCorrectionLevel = 3;
                        return 8;
                    }
                    else if (messageLength < 174)
                    {
                        //high
                        if (messageLength < 144)
                            errorCorrectionLevel = 2;
                        //quality
                        else
                            errorCorrectionLevel = 3;
                        return 9;
                    }
                    else if (messageLength < 200)
                    {
                        //high
                        if (messageLength < 175)
                            errorCorrectionLevel = 2;
                        //quality
                        else
                            errorCorrectionLevel = 3;
                        return 10;
                    }
                    break;

            }
            //i guess this an error return?
            return 1;
        }

        //reverses a queue in what is probably not the most efficient way
        //but still accomplishes the task at hand
        public Queue<bool> ReverseQueue(Queue<bool> outQueue)
        {
            int tempCount = outQueue.Count;
            Stack<bool> tempStack = new Stack<bool>();
            //puts all items into a stack
            for (int i = 0; i < tempCount; i++)
            {
                tempStack.Push(outQueue.Dequeue());
            }
            //takes items out of that stack, effectively in the opposite order
            for (int i = 0; i < tempCount; i++)
            {
                outQueue.Enqueue(tempStack.Pop());
            }

            return outQueue;
        }

        //takes an interger list of "codewords" and converts them to the appropriate 8-bit binary values and
        //then adds them to the queue
        public Queue<bool> AddCodewordsToQueue(Queue<bool> outQueue, List<int> intList)
        {
            //add the integers to the queue
            foreach (int thisInt in intList)
                foreach (bool thisBool in BitsWithPadding(thisInt, 8))
                    outQueue.Enqueue(thisBool);

            return outQueue;
        }
    }
}
