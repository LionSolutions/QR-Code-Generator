using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR_Code_Encoder
{
    ///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012
    ///this class handles the main data stream creation, from the raw data being input by the user, to the placement in the grid
    class DataEncodeQR
    {
        static int version;
        static int squareSize;
        static int errorCorrectionLevel = 0;
        //qr helper contains useful tools that are needed for qr production 
        QrEncodeHelper qrAssistance = new QrEncodeHelper(version);

        //dictionary used for alpha numeric encoding
        static Dictionary<string, int> alphaNumericDictionary = new Dictionary<string, int>();

        //the following three data arrays are filled with important information that related to version number and the error correction levels
        //the pattern is as follows. ex, [i,k] i = the version number minus one and k = the logical error correction level, 0-3, 0 being the highest and 3 being the lowest
        static int[,] numberOfCodeWords = { { 9, 13, 16, 19 }, { 16, 22, 28, 34 }, { 26, 34, 44, 55 }, { 36, 48, 64, 80 }, 
                                          { 46, 62, 86, 108 }, { 60, 76, 108, 136 }, { 66, 88, 124, 156 }, { 86, 110, 154, 194 },
                                          {100,132,182,232},{122,154,216,274},{140,180,254,324},{158,206,290,370},
                                          {180,244,334,428},{197,261,365,461},{223,295,415,523},{253,325,453,589}};

        static int[,] NumBchBlocks = { { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 2, 2, 1, 1 }, { 4, 2, 2, 1 }, { 4, 4, 2, 1 }, { 4, 4, 4, 2 }, { 5, 6, 4, 2 }, { 6, 6, 4, 2 }, { 8, 8, 5, 2 }, { 8, 8, 5, 4 }, { 11, 8, 5, 4 }, { 11, 10, 8, 4 } };
        static int[,] BchErrorLibrary = { { 17, 13, 10, 7 }, { 28, 22, 16, 10 }, { 44, 36, 26, 15 }, { 64, 52, 36, 20 }, { 88, 72, 48, 26 }, { 112, 96, 64, 36 }, { 130, 108, 72, 40 }, { 156, 132, 88, 48 }, { 192, 160, 110, 60 }, { 224, 192, 130, 72 } };

        public DataEncodeQR(int qrVersion, int errorCorrection)
        {
            version = qrVersion;
            squareSize = qrVersion * 4 + 17 + 2;
            errorCorrectionLevel = errorCorrection;
        }

        public bool[,] DataEncodeSwitch(string message, int ecodementType)
        {
            Queue<bool> tempBoolQueue = new Queue<bool>();
            //BchErrorCorrection newBCH = new BchErrorCorrection(version);
            List<int> outList = new List<int>();

            //there is the possibility that i will later add other encodement types (you never know)
            if (ecodementType == 3)
                tempBoolQueue = AlphaNumericEncoding(message);

            tempBoolQueue = AddExtraCodeWords(tempBoolQueue);

            outList = MultipleBlockSequencer(qrAssistance.QueueToDataCodeWords(tempBoolQueue));
            tempBoolQueue.Clear();
            //tempBoolQueue = newBCH.BCHmain(tempBoolQueue, qrAssistance.QueueToDataCodeWords(tempBoolQueue), errorCorrectionLevel);

            return QrSequencer(qrAssistance.AddCodewordsToQueue(tempBoolQueue, outList));
        }

        //deals with codewords blocks and when they start to get split up
        //this is nasty..
        private List<int> MultipleBlockSequencer(List<int> outInList)
        {
            BchErrorCorrection newBCH = new BchErrorCorrection(version);
            int numBlocks = NumBchBlocks[version - 1, errorCorrectionLevel];
            List<int> tempIntList = new List<int>();

            if (numBlocks == 1)
            {
                tempIntList = newBCH.BCHmain(outInList, BchErrorLibrary[version - 1, errorCorrectionLevel]);
                foreach (int tempInt in tempIntList)
                {
                    outInList.Add(tempInt);
                }
            }
            else
            {
                List<int>[] codeWordBlocks = new List<int>[numBlocks];

                List<int>[] BCHblocks = new List<int>[numBlocks];

                //initialize the arrays
                for (int i = 0; i < numBlocks; i++)
                {
                    codeWordBlocks[i] = new List<int>();
                    BCHblocks[i] = new List<int>();
                }
                int tempCountInt = outInList.Count;

                //convert the data codewords into groups for error correction

                for (int k = 0; k < numBlocks; k++)
                {
                    for (int i = 0; i < tempCountInt / numBlocks; i++)
                    {
                        //somtimes the amount of codewords can be uneven to the amount of blocks, leaving some blocks with more than others
                        if (outInList.Count > 0)
                        {
                            codeWordBlocks[k].Add(outInList[0]);
                            outInList.RemoveAt(0);
                        }
                    }
                }

                //gets the corresponding blocks of error correction
                for (int i = 0; i < codeWordBlocks.Length; i++)
                {
                    //while cycling through each codeword block, calculate each error correction block and add it to the block collection
                    foreach (int thisInt in newBCH.BCHmain(codeWordBlocks[i], BchErrorLibrary[version - 1, errorCorrectionLevel] / numBlocks))
                    {
                        BCHblocks[i].Add(thisInt);
                    }
                }

                //now that blocks have been sorted and error correction blocks have been calculated
                //the blocks can be put into their new and exciting order!
                tempCountInt = codeWordBlocks[0].Count;

                for (int k = 0; k < tempCountInt; k++)
                {
                    for (int i = 0; i < numBlocks; i++)
                    {
                        outInList.Add(codeWordBlocks[i][k]);
                    }
                }

                tempCountInt = BCHblocks[0].Count;
                for (int k = 0; k < tempCountInt; k++)
                {
                    for (int i = 0; i < numBlocks; i++)
                    {
                        outInList.Add(BCHblocks[i][k]);
                    }
                }

            }

            return outInList;
        }

        private Queue<bool> AlphaNumericEncoding(string message)
        {
            //creates dictionary of characters
            SetUpDictionary();

            //stores the decoded numbers
            List<int> messageDecoded = new List<int>();
            //the queue to be output for data placement
            Queue<bool> outQueue = new Queue<bool>();

            //message length is needed here and there
            //also i need the starting length constantly
            int messageLength = message.Length;

            //must be all in caps to be decoded
            message = message.ToUpper();

            //add the mode indicator to the queue
            //requires total of 4 bits and the mode number is 2
            foreach (bool thisBool in qrAssistance.BitsWithPadding(2, 4))
            {
                outQueue.Enqueue(thisBool);
            }

            //determines padding for characters data block according to version 
            int characterLengthIndex;
            if (version < 10)
                characterLengthIndex = 9;
            else if (version < 27)
                characterLengthIndex = 11;
            else
                characterLengthIndex = 13;

            //adds the character count data to the array
            foreach (bool thisBool in qrAssistance.BitsWithPadding(messageLength, characterLengthIndex))
            {
                outQueue.Enqueue(thisBool);
            }

            //convert message to corresponding integer values
            for (int i = 0; i < messageLength; i++)
            {
                //if the character being input is actually in the dictionary..
                if (alphaNumericDictionary.ContainsKey(message.Substring(0, 1)))
                    messageDecoded.Add(alphaNumericDictionary[message.Substring(0, 1)]);

                //if the character was not found in the dictionary, a space in put in its place (no rhyme intended)
                else
                    messageDecoded.Add(36);
                message = message.Substring(1);
            }

            //converts the decoded chacter stream into bools and adds them to the queue
            for (int i = 0; i < messageDecoded.Count; i += 2)
            {
                int currentStream;
                int streamLength;
                //two characters are converted into 11 bit streams, sometimes there is an odd character left at the end
                if (i == messageDecoded.Count - 1)
                {
                    currentStream = messageDecoded[i];
                    //stream changes to 6 when there is only one character being converted
                    streamLength = 6;
                }
                else
                {
                    //the stream = the current character x 45 + the next character
                    currentStream = messageDecoded[i] * 45 + messageDecoded[i + 1];
                    streamLength = 11;
                }

                //adds the current data value into the data stream
                foreach (bool thisBool in qrAssistance.BitsWithPadding(currentStream, streamLength))
                {
                    outQueue.Enqueue(thisBool);
                }
            }

            ///adds some terminator bits - four bits that are zeros which are added
            ///these bits are used to signal the end of a message to the decoder
            ///as usual, there are specific cases that need to be looked out for, for example when the capacity of the symbol
            ///is almost filled by the data stream (within 4 from the capacity), only the amount needed to fill the stream length are added
            ///in general, 4 bits are added at this step
            if (outQueue.Count < numberOfCodeWords[version - 1, errorCorrectionLevel] * 8 - 4)
            {
                //adding of the four bits that are mentioned above
                for (int i = 0; i < 4; i++)
                    outQueue.Enqueue(false);
            }
            
            //the queue must be a multiple of 8, otherwise when it is divided into codewords the operation would fail
            outQueue = qrAssistance.ModTo8(outQueue);
            return outQueue;
        }

        /// <summary>
        /// adds the alternating 236 and 17 8-bit values to the queue which are called "padding bits"
        /// </summary>
        /// <param name="outQueue">the data stream</param>
        /// <returns></returns>
        private Queue<bool> AddExtraCodeWords(Queue<bool> outQueue)
        {
            bool switchBool = false;
            int outQueueCount = outQueue.Count;
            for (int i = 0; i < numberOfCodeWords[version - 1, errorCorrectionLevel] - outQueueCount / 8; i++)
            {
                //XOR switching, gotta love it ;)
                switch (switchBool)
                {
                        //236 is added
                    case false:
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(false);
                        break;
                        //17 is added
                    case true:
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(true);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(false);
                        outQueue.Enqueue(true);
                        break;
                }
                switchBool ^= true;
            }

            return outQueue;
        }

        /// <summary>
        /// "writes" the data into the grid using an ingenious method of for loops
        /// </summary>
        /// <param name="theQueue">this is the queue that is ready to be put into the grid</param>
        /// <returns>the grid with all of the data put in the correct place, it does not have tracking
        /// data yet or anything like that so that should be added.</returns>
        private bool[,] QrSequencer(Queue<bool> theQueue)
        {
            bool[,] theData = new bool[squareSize, squareSize];

            bool[,] voidSpace = DrawVoidSpace();

            //starting in the furthest right collumn, and each collumn is 2 pixels wide
            for (int collumn = squareSize - 2; collumn > 1; collumn -= 2)
            {

                //this determines the direction of the collumn, up or down
                if ((collumn + 1) % 4 != 0)
                {
                    //up sequence
                    //row start counting from the bottom to top
                    for (int row = squareSize - 1; row > 0; row--)
                    {
                        //multiplier is a fancy term for move over one in the collumn that is 2 wide so each pixel can be assessed as the row changes
                        for (int collumMultiplier = 0; collumMultiplier < 2; collumMultiplier++)
                        {
                            //confirms there is still data to still write, otherwise BAIL.
                            if (theQueue.Count == 0)
                                return theData;

                            //there was a glitch where the sequencer would determine the horizontal alternating pattern as an obstical
                            //this lead to it only drawing one collumn making all of the collumns after it
                            //simple fix is to shift it when its evaluating this collumn by one to the left
                            if (collumn <= 8 && collumn > 1)
                            {
                                //confirms the current space is writable in the adjusted collumn (possibily of tracker squares being there in higher versions
                                if (voidSpace[collumn - collumMultiplier - 1, row] == false)
                                    //writes the data in the corrected collumn
                                    theData[collumn - collumMultiplier - 1, row] = theQueue.Dequeue();

                            }
                            else
                                //confirms the current space is writable
                                if (voidSpace[collumn - collumMultiplier, row] == false)
                                    //writes the data in the correct spot
                                    theData[collumn - collumMultiplier, row] = theQueue.Dequeue();

                        }
                    }
                }
                //for when the collumn is going down
                else
                {
                    //only thing changed here is the row loop, it counts up, not down
                    for (int row = 1; row < squareSize; row++)
                    {
                        for (int collumnMultiplier = 0; collumnMultiplier < 2; collumnMultiplier++)
                        {
                            if (theQueue.Count == 0)
                                return theData;

                            if (collumn <= 8 && collumn > 1)
                            {
                                if (voidSpace[collumn - collumnMultiplier - 1, row] == false)
                                {
                                    theData[collumn - collumnMultiplier - 1, row] = theQueue.Dequeue();
                                }
                            }
                            else
                                //see? the same.
                                if (voidSpace[collumn - collumnMultiplier, row] == false)
                                {

                                    theData[collumn - collumnMultiplier, row] = theQueue.Dequeue();
                                    //i actually spelt collumnMultiplier different. did you notice?
                                }
                        }
                    }
                }
            }

            return theData;
        }

        //declares the dictionary for use with alpha numeric encoding
        private void SetUpDictionary()
        {
            //there was a glitch that the dictionary would try to recall itself, this is the fix
            if (!alphaNumericDictionary.ContainsKey("0"))
            {
                alphaNumericDictionary.Add("0", 0);
                alphaNumericDictionary.Add("1", 1);
                alphaNumericDictionary.Add("2", 2);
                alphaNumericDictionary.Add("3", 3);
                alphaNumericDictionary.Add("4", 4);
                alphaNumericDictionary.Add("5", 5);
                alphaNumericDictionary.Add("6", 6);
                alphaNumericDictionary.Add("7", 7);
                alphaNumericDictionary.Add("8", 8);
                alphaNumericDictionary.Add("9", 9);
                alphaNumericDictionary.Add("A", 10);
                alphaNumericDictionary.Add("B", 11);
                alphaNumericDictionary.Add("C", 12);
                alphaNumericDictionary.Add("D", 13);
                alphaNumericDictionary.Add("E", 14);
                alphaNumericDictionary.Add("F", 15);
                alphaNumericDictionary.Add("G", 16);
                alphaNumericDictionary.Add("H", 17);
                alphaNumericDictionary.Add("I", 18);
                alphaNumericDictionary.Add("J", 19);
                alphaNumericDictionary.Add("K", 20);
                alphaNumericDictionary.Add("L", 21);
                alphaNumericDictionary.Add("M", 22);
                alphaNumericDictionary.Add("N", 23);
                alphaNumericDictionary.Add("O", 24);
                //SOMEONE P'D IN THE DICTIONARY..
                alphaNumericDictionary.Add("P", 25);
                //-____________________-
                alphaNumericDictionary.Add("Q", 26);
                alphaNumericDictionary.Add("R", 27);
                alphaNumericDictionary.Add("S", 28);
                alphaNumericDictionary.Add("T", 29);
                alphaNumericDictionary.Add("U", 30);
                alphaNumericDictionary.Add("V", 31);
                alphaNumericDictionary.Add("W", 32);
                alphaNumericDictionary.Add("X", 33);
                alphaNumericDictionary.Add("Y", 34);
                alphaNumericDictionary.Add("Z", 35);
                alphaNumericDictionary.Add(" ", 36);
                alphaNumericDictionary.Add("$", 37);
                alphaNumericDictionary.Add("%", 38);
                alphaNumericDictionary.Add("*", 39);
                alphaNumericDictionary.Add("+", 40);
                alphaNumericDictionary.Add("-", 41);
                alphaNumericDictionary.Add(".", 42);
                alphaNumericDictionary.Add("/", 43);
                alphaNumericDictionary.Add(":", 44);
            }
        }

        //used for when encoding data, the "void" space is used as a reference by the fuction encoding data
        //enabling proper data encodation. is also added to as data is encoded
        //i basically just took all of my drawing functions, complied them into one and made it so they always place true in the grid
        private bool[,] DrawVoidSpace()
        {
            ///you may recognize some of these pattern from earlier in the code
            ///if you do, you are totally right, its from there
            ///the only difference is instead of drawing patterns, just void space is drawn inplace of the complex patterns

            bool[,] theData = new bool[squareSize, squareSize];

            //draws border
            for (int i = 0; i < squareSize; i++)
            {
                theData[i, 0] = true;
                theData[0, i] = true;
                theData[i, squareSize - 1] = true;
                theData[squareSize - 1, i] = true;
            }

            //draws tracker squares
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    //top left square
                    theData[x, y] = true;

                    //because there is a reflection when x=y, the x and y can be swapped
                    //other two squares
                    theData[squareSize - x - 1, y] = true;
                    theData[y, squareSize - x - 1] = true;
                }
            }

            //multiX because X can be used multiple times, since the timing lines are a reflection
            for (int multiX = 9; multiX < squareSize - 9; multiX++)
            {
                theData[multiX, 7] = true;
                theData[7, multiX] = true;
            }

            //formulas to determine center point of the squares location (my own creation)
            int centerPoint1 = 4 * version + 10;
            int centerPoint2 = 2 * version + 8;

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    //version 1 has no square, versions 2-6 have 1 square
                    if (version > 1)
                    {
                        //one is added due to the border
                        theData[centerPoint1 + x + 1, centerPoint1 + y + 1] = true;

                        //versions 7-13 have 6 squares
                        if (version > 6 && version < 14)
                        {
                            //draws all of the squares using all combinations of centerpoints and set points 
                            //(excluding ones that would be under tracker squares)
                            theData[centerPoint2 + x + 1, centerPoint2 + y + 1] = true; //set to true for the blank void space
                            theData[centerPoint2 + x + 1, centerPoint1 + y + 1] = true;
                            theData[centerPoint1 + x + 1, centerPoint2 + y + 1] = true;
                            theData[7 + x, centerPoint2 + y + 1] = true;
                            theData[centerPoint2 + x + 1, 7 + y] = true;
                        }
                    }

                }
            }

            //draws dark version data
            if (version > 6)
            {
                for (int y = 0; y < 6; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        theData[squareSize - 12 + x, y + 1] = true;
                        theData[y + 1, squareSize - 12 + x] = true;
                    }
                }
            }

            //draws dark format information
            for (int i = 1; i < 9; i++)
            {
                theData[9, i] = true;
                theData[i, 9] = true;

                theData[9, squareSize - i] = true;
                theData[squareSize - i - 1, 9] = true;
            }
            theData[9, 9] = true;

            //draws dark module
            theData[9, squareSize - 9] = true;

            return theData;
        }
    }
}
