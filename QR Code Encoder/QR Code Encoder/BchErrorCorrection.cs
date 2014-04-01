using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012
///The purpose of this class is to handle all things involving error correction
///From the Format information, to Data Stream Blocks, this class handles it all
///References have to go out to www.thonky.com for their tutorial on Error correction for the main data stream
///Otherwise I may have been left in the dark. They explained the theory in greater detail and then I was able to implement a solution.
///I was able to figure out the error correction of the Format and Version data on my own though,
///but as you will notice they are not exactly the same thing.
///for fancyess..

namespace QR_Code_Encoder
{
    class BchErrorCorrection
    {
        static int version;
        static int squareSize;

        static int[,] BchErrorLibrary = { { 17, 13, 10, 7 }, { 28, 22, 16, 10 }, { 44, 36, 26, 15 }, { 64, 52, 36, 20 }, { 88, 72, 48, 26 }, { 112, 96, 64, 36 }, { 130, 108, 72, 40 }, { 156, 132, 88, 48 }, { 192, 160, 110, 60 }, { 224, 192, 130, 72 } };
        
        static Dictionary<int, List<int>> PolynomialsInAntiLog = new Dictionary<int, List<int>>();

        QrEncodeHelper qrAssistance = new QrEncodeHelper(version);

        //partly taken from galois field modulo 2 arithmetic GF[256] reference PDF
        //then converted from C to C#
        const int GF = 256;// define the Size & Prime Polynomial of this Galois field (prime polynomial really has no "prime" properties to it..)
        const int PP = 285; // modified from 301 to fit the particular case used in QR Code
        int[] Log = new int[GF];
        int[] ALog = new int[GF]; // establish global Log and Antilog arrays
        // fill the Log[] and ALog[] arrays with appropriate integer values
        //also notice that the term "logs" really has nothing to do with your idea of a logarithm (go figure)

        public BchErrorCorrection(int versionIn)
        {
            version = versionIn;
            squareSize = version * 4 + 17 + 2;
        }

        //main function that handles almost all things BCH (reed solomon would be proud)
        //from all the testing i have performed this algorithm appears to be perfect
        public List<int> BCHmain(List<int> messageListIn, int generatorPolynomialNumber)
        {
            //these are needed only for this function
            DeclareLogsAndAntiLogs();
            DeclarePolynomials();

            int numberErrorCorrectionCodeWords = generatorPolynomialNumber;//BchErrorLibrary[version - 1, errorCorrectionLevel];
            int totalCodeWords = numberErrorCorrectionCodeWords + messageListIn.Count;
            //creates the message polynomial with the correct number of terms
            List<int> messagePolynomial = new List<int>();
            List<int> generatorPolynomial = new List<int>();

            //add the incoming message data into the form of a polynomial
            for (int i = 0; i < messageListIn.Count; i++)
                messagePolynomial.Add(messageListIn[i]);

            //add remaining zero's that are needed
            for (int i = 0; i < totalCodeWords - messageListIn.Count; i++)
                messagePolynomial.Add(0);

            //the array index is now the exponent >:D this is colin's way of cheating
            messagePolynomial.Reverse(0, totalCodeWords);

            //the first exponent has to be left blank!
            generatorPolynomial.Add(0);

            //add the polynomials
            for (int i = 1; i <= numberErrorCorrectionCodeWords; i++)
                generatorPolynomial.Add(PolynomialsInAntiLog[numberErrorCorrectionCodeWords][i - 1]);
            //add the zeros
            for (int i = 0; i < (totalCodeWords - numberErrorCorrectionCodeWords) - 1; i++)
                generatorPolynomial.Add(0);

            //generator polynomials exponents now line up too >:D
            generatorPolynomial.Reverse(0, totalCodeWords);

            //the generator polynomial is needed for every cycle
            //so a copy of it in the proper format is needed
            List<int> tempGenList = new List<int>();
            //copy the array, this is the only way sadly..
            foreach (int tempInt in generatorPolynomial)
                tempGenList.Add(tempInt);

            /// <summary>
            /// this next bit of code is not only the single most advanced program/algorithm i have ever developed
            /// but the most insane and mind bending problem i have tackled in my life
            /// finishing it is not only a triumph for Grade 12 Computer Science
            /// but a triumph for myself.
            /// so ask yourself, is this madness? or is it organized madness?
            /// either way i actually understand it, so does that make me insane?
            /// well i'll let you be the judge, but on a side note i feel like it when i try to explain this to anyone so...
            /// anyways, you can now continue on. enjoy the rest of the code.
            /// </summary>
            /// end of the summary? more like end of the speech after winning an oscar..

            //looping starts from the exponent with the highest value all the way down to the last exponent
            //this is accompished by looping while the messagepolynomial is still larger than the number of codewords
            //also, keep in mind that the purpose of this operation is to get the remainder when the message polynomial is divided by g(x) aka the generator polynoimials
            for (int currentPosition = totalCodeWords - 1; messagePolynomial.Count > numberErrorCorrectionCodeWords; currentPosition--)
            {
                //this will eliminate the case where the previous XORing creates two zeros and only one is removed
                //see the else statement for the solution to this problem
                if (messagePolynomial[currentPosition] != 0)
                {
                    //this is the exponent of "a" which will be multiplied by the exponents of "a" in the generator polynomial
                    int tempInt = Log[messagePolynomial[currentPosition]];

                    //cycles through all elements of the generator polynomial applying the value of the antiLog 
                    for (int k = currentPosition; k >= generatorPolynomial.Count - numberErrorCorrectionCodeWords - 1; k--)
                    {
                        //the "a"'s are multiplied, as we are determining the new exponents we can simply add them
                        generatorPolynomial[k] += tempInt;
                        //if the new exponent is larger than 255 it must be modded by 255
                        generatorPolynomial[k] %= 255;
                    }

                    //convert BACK to real integers
                    for (int i = generatorPolynomial.Count - 1; i >= generatorPolynomial.Count - numberErrorCorrectionCodeWords - 1; i--)
                        generatorPolynomial[i] = ALog[generatorPolynomial[i]];

                    //XOR's the converted generator polynomial with the message polynomial
                    //effectively eliminating the first term of the generator polynomial because they are (or should be) the same
                    for (int i = generatorPolynomial.Count - 1; i >= generatorPolynomial.Count - numberErrorCorrectionCodeWords; i--)
                        generatorPolynomial[i] ^= messagePolynomial[i];

                    /// <summary>
                    /// this next section of code is kinda a switch-a-roo that prepares all of the lists
                    /// for the next round. i tried to optimize this code as i think there has to be a simplier way
                    /// but my attempts have failed. i end up with crazy negative numbers so i will just let it be
                    /// </summary>

                    //the first term of the generator polynomial is removed because it is a zero
                    generatorPolynomial.RemoveAt(currentPosition);
                    //the messagepolynomial is reset so it can be filled with the contents of the generator polynomial
                    messagePolynomial.Clear();
                    //the generator polynomial is then copied into the message polynomial for the next rotation
                    foreach (int thisInt in generatorPolynomial)
                        messagePolynomial.Add(thisInt);

                    //the "0" element is removed from the generator polynomial shifting the entire contents down by one and 
                    //preserving the exponent on x as the array index >:D
                    tempGenList.RemoveAt(0);
                    //reset the generator polynomial so it can be replaced for the next round
                    generatorPolynomial.Clear();

                    //the modified generator polynomial is replaced with the original
                    foreach (int thisInt in tempGenList)
                        generatorPolynomial.Add(thisInt);

                }
                //double zero case
                else
                {
                    //removes the zero and sets all of the lists as if nothing happened
                    messagePolynomial.RemoveAt(currentPosition);
                    generatorPolynomial.RemoveAt(0);
                    tempGenList.RemoveAt(0);
                }
            }

            //since i have been using the array in "reverse" i need to reverse it so it can be exported
            messagePolynomial.Reverse();

            //add the integers to the queue
            //outQueue = qrAssistance.AddCodewordsToQueue(outQueue, messagePolynomial);

            //return that queue, like a B4W5
            return messagePolynomial;
        }

        /// <summary>
        /// Takes the error correction level and the mask pattern number and creates 
        /// the format data which is then masked and can be put directly into the matrix
        /// </summary>
        /// <param name="errorCorrectionLevel">the error correction level (0-3)</param>
        /// <param name="maskPattern">the mask pattern (0-7)</param>
        /// <returns>the queue that is masked and can be directly placed into the matrix</returns>
        public Queue<bool> FormatInformation(int errorCorrectionLevel, int maskPattern)
        {
            //generator polynomial is a typo, it is really the "message" polynomial
            //gOfX is the generator polynomial xD oh well, my mistake

            Queue<bool> outQueue = new Queue<bool>();
            bool[] generatorPolynomial = new bool[15];
            //i really hate using a COUNTER, but for this case i will just have to deal with it
            int generatorBoolCounter = 0;
            //this is gOfX, this is what divides the generator polynomial
            bool[] gOfX = { true, false, true, false, false, true, true, false, true, true, true };
            //reversing it allows the actual numbers of expontents to be used
            Array.Reverse(gOfX);

            //add the first two bits to the out queue and the generator polynomial
            foreach (bool thisBool in qrAssistance.BitsWithPadding(errorCorrectionLevel, 2))
            {
                //add to the queue for later use
                outQueue.Enqueue(thisBool);
                //add to the generator polynomial for later use
                generatorPolynomial[generatorBoolCounter] = thisBool;
                generatorBoolCounter++;
            }

            //and now for the next 3 bits, the data that represents the masking data
            foreach (bool thisBool in qrAssistance.BitsWithPadding(maskPattern, 3))
            {
                //add to the queue for later use
                outQueue.Enqueue(thisBool);
                //add to the generator polynomial for later use
                generatorPolynomial[generatorBoolCounter] = thisBool;
                generatorBoolCounter++;
            }

            //effectively raising the polynomials by x^10 xD (it works..)
            Array.Reverse(generatorPolynomial);

            //and this is what i call murder, have fun mr. trink..
            //cycles through the elements of the generator polynomial (ten is added so it doesnt have to be added at every step later)
            //also starting at 14 alows me to use the same array for when it is exported into the queue
            for (int i = 14; i >= 10; i--)
            {
                //then if the polynomial that is currently being looked at ISNT zero
                if (generatorPolynomial[i] == true)
                {
                    int quotientExponent = i - 10;
                    for (int k = 10; k >= 0; k--)
                        //XOR is magical, it took me about 4 minutes exactly to realize the operation that was actually being done 
                        //while getting a remained was XOR in this case
                        generatorPolynomial[k + quotientExponent] ^= gOfX[k];

                }
            }
            //after getting the above i wanted to cry, as it is only 9 LINES OF CODE and the amount of time i spent is pityfull........ 

            //add the new bits to the out stream
            for (int i = 9; i >= 0; i--)
                outQueue.Enqueue(generatorPolynomial[i]);

            //XOR stream
            bool[] XORMaskStream = { true, false, true, false, true, false, false, false, false, false, true, false, false, true, false };
            //all of the outgoing bits have to be XOR'd with a mask that apparently prevents the rare case of an ALL zero string from being outputted
            //im just going to leave the understanding of this one to the people with the PhD's (or just accept that this bit stream does what they say it does)
            for (int i = 0; i <= 14; i++)
                //cycles through the whole queue XORIN' it all with the bit stream
                outQueue.Enqueue(outQueue.Dequeue() ^ XORMaskStream[i]);

            outQueue = qrAssistance.ReverseQueue(outQueue);

            return outQueue;
        }

        //this is much like the function for format information, refer to it for any questions you may have 
        public Queue<bool> VersionInformation()
        {
            Queue<bool> outQueue = new Queue<bool>();
            bool[] generatorPolynomial = new bool[18];
            int generatorPolynomialCounter = 0;
            bool[] gOfX = { true, true, true, true, true, false, false, true, false, false, true, false, true };
            Array.Reverse(gOfX);

            //add the first 6 bits to the queue and the polynomial
            foreach (bool thisBool in qrAssistance.BitsWithPadding(version, 6))
            {
                outQueue.Enqueue(thisBool);
                generatorPolynomial[generatorPolynomialCounter] = thisBool;
                generatorPolynomialCounter++;
            }

            Array.Reverse(generatorPolynomial);

            for (int i = 17; i >= 12; i--)
            {
                //then if the polynomial that is currently being looked at ISNT zero
                if (generatorPolynomial[i] == true)
                {
                    int quotientExponent = i - 12;
                    for (int k = 12; k >= 0; k--)
                        //XOR is magical, it took me about 4 minutes exactly to realize the operation that was actually being done 
                        //while getting a remained was XOR in this case
                        generatorPolynomial[k + quotientExponent] ^= gOfX[k];
                }
            }

            generatorPolynomial.Reverse();

            //add new bits to the queue
            for (int i = 11; i >= 0; i--)
                outQueue.Enqueue(generatorPolynomial[i]);

            return outQueue;
        }

        private void DeclareLogsAndAntiLogs()
        {
            int i;
            Log[0] = 1 - GF; ALog[0] = 1;
            for (i = 1; i < GF; i++)
            {
                ALog[i] = ALog[i - 1] * 2;
                if (ALog[i] >= GF) ALog[i] ^= PP;
                Log[ALog[i]] = i;
            }
        }

        private void DeclarePolynomials()
        {
            if (PolynomialsInAntiLog.ContainsKey(7) != true)
            {
                PolynomialsInAntiLog.Add(7, new List<int>(new int[] { 87, 229, 146, 149, 238, 102, 21 }));
                PolynomialsInAntiLog.Add(10, new List<int>(new int[] { 251, 67, 46, 61, 118, 70, 64, 94, 32, 45 }));
                PolynomialsInAntiLog.Add(13, new List<int>(new int[] { 74, 152, 176, 100, 86, 100, 106, 104, 130, 218, 206, 140, 78 }));
                PolynomialsInAntiLog.Add(15, new List<int>(new int[] { 8, 183, 61, 91, 202, 37, 51, 58, 58, 237, 140, 124, 5, 99, 105 }));
                PolynomialsInAntiLog.Add(16, new List<int>(new int[] { 120, 104, 107, 109, 102, 161, 76, 3, 91, 191, 147, 169, 182, 194, 225, 120 }));
                PolynomialsInAntiLog.Add(17, new List<int>(new int[] { 43, 139, 206, 78, 43, 239, 123, 206, 214, 147, 24, 99, 150, 39, 243, 163, 136 }));
                PolynomialsInAntiLog.Add(18, new List<int>(new int[] { 215, 234, 158, 94, 184, 97, 118, 170, 79, 187, 152, 148, 252, 179, 5, 98, 96, 153 }));
                PolynomialsInAntiLog.Add(20, new List<int>(new int[] { 17, 60, 79, 50, 61, 163, 26, 187, 202, 180, 221, 225, 83, 239, 156, 164, 212, 212, 188, 190 }));
                PolynomialsInAntiLog.Add(22, new List<int>(new int[] { 210, 171, 247, 242, 93, 230, 14, 109, 221, 53, 200, 74, 8, 172, 98, 80, 219, 134, 160, 105, 165, 231 }));
                PolynomialsInAntiLog.Add(24, new List<int>(new int[] { 229, 121, 135, 48, 211, 117, 251, 126, 159, 180, 169, 152, 192, 226, 228, 218, 111, 0, 117, 232, 87, 96, 227, 21 }));
                PolynomialsInAntiLog.Add(26, new List<int>(new int[] { 173, 125, 158, 2, 103, 182, 118, 17, 145, 201, 111, 28, 165, 53, 161, 21, 245, 142, 13, 102, 48, 227, 153, 145, 218, 70 }));
                PolynomialsInAntiLog.Add(28, new List<int>(new int[] { 168, 223, 200, 104, 224, 234, 108, 180, 110, 190, 195, 147, 205, 27, 232, 201, 21, 43, 245, 87, 42, 195, 212, 119, 242, 37, 9, 123 }));
                PolynomialsInAntiLog.Add(32, new List<int>(new int[] { 10, 6, 106, 190, 249, 167, 4, 67, 209, 138, 138, 32, 242, 123, 89, 27, 120, 185, 80, 156, 38, 69, 171, 60, 28, 222, 80, 52, 254, 185, 220, 241 }));
                PolynomialsInAntiLog.Add(36, new List<int>(new int[] { 200, 183, 98, 16, 172, 31, 246, 234, 60, 152, 115, 0, 167, 152, 113, 248, 238, 107, 18, 63, 218, 37, 87, 210, 105, 177, 120, 74, 121, 196, 117, 251, 113, 233, 30, 120 }));
            }
        }
    }
}
