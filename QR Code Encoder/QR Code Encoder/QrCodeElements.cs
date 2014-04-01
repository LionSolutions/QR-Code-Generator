using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012

namespace QR_Code_Encoder
{
    /// <summary>
    /// this class contains all things that are needed to draw the elements of the QrCode
    /// i recently removed this from the class QrEncode as it will be needed for the masking process aswell
    /// so i thought that having a class for it would be a better idea
    /// </summary>
    class QrCodeElements
    {
        //elements that are always needed
        static int squareSize = 0;
        static int versionNum = 0;

        BchErrorCorrection newBCH = new BchErrorCorrection(versionNum);

        //although this may be ridiculous, i think it is nessesary
        static bool[,] trackerSquare = { { false, false, false, false, false, false, false, false, false }, 
                                       { false, true, true, true, true, true, true, true, false }, 
                                       { false, true, false, false, false, false, false, true, false }, 
                                       { false, true, false, true, true, true, false, true, false },
                                       { false, true, false, true, true, true, false, true, false },//good thing it was only 9x9..
                                       { false, true, false, true, true, true, false, true, false },
                                       { false, true, false, false, false, false, false, true, false },
                                       { false, true, true, true, true, true, true, true, false },
                                       { false, false, false, false, false, false, false, false, false } };

        public QrCodeElements(int version, int sizeOfSquare)
        {
            versionNum = version;
            squareSize = sizeOfSquare;
        }

        /// <summary>
        /// draws the three large squares that are found in the corners
        /// these not only determine orientation of the matrix but help determine size of the code
        /// </summary>
        /// <param name="theData">the matrix that will have the tracker squares added to it</param>
        /// <returns>the same matrix with the squares added to them</returns>
        public bool[,] DrawTrackerSquares(bool[,] theData)
        {
            //x and y MAY be flipped in this section.. but since we are dealing with a square it does not matter

            //draws all squares
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    //because there is a reflection when x=y, the x and y can be swapped
                    theData[x, y] = trackerSquare[x, y];
                    theData[squareSize - x - 1, y] = trackerSquare[x, y];
                    theData[y, squareSize - x - 1] = trackerSquare[x, y];
                }
            }

            return theData;
        }

        /// <summary>
        /// an alternating row and collumn of bits that in corelation with the tracker squares
        /// help the decoder determine size so the bits can be properly decoded
        /// </summary>
        /// <param name="theData">the matrix that will have the lines added to it</param>
        /// <returns>the same matrix with the lines added to them</returns>
        public bool[,] DrawTimingLines(bool[,] theData)
        {
            //multiX because X can be used multiple times, since the timing lines are a reflection
            for (int multiX = 9; multiX < squareSize - 9; multiX++)
            {
                bool pixelColour;
                //changes proper
                if (multiX % 2 == 0)
                    pixelColour = false;
                else
                    pixelColour = true;

                theData[multiX, 7] = pixelColour;
                theData[7, multiX] = pixelColour;
            }

            return theData;
        }

        public bool[,] DrawAlignmentSquares(bool[,] theData)
        {
            //alignment square looks something like this
            bool[,] alignmentSquare = { { true, true, true, true, true, },
                                      { true, false, false, false, true, },
                                      { true, false, true, false, true, },
                                      { true, false, false, false, true, },
                                      { true, true, true, true, true, }};

            //formula to determine center point of the squares location (my own creation)
            int centerPoint1 = 4 * versionNum + 10;
            int centerPoint2 = 2 * versionNum + 8;

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    //version 1 has no square, versions 2-6 have 1 square
                    if (versionNum > 1)
                    {
                        //one is added due to the border
                        theData[centerPoint1 + x + 1, centerPoint1 + y + 1] = alignmentSquare[x + 2, y + 2];

                        //versions 7-13 have 6 squares
                        if (versionNum > 6 && versionNum < 14)
                        {
                            //draws all of the squares using all combinations of centerpoints and set points 
                            //(excluding ones that would be under tracker squares)
                            theData[centerPoint2 + x + 1, centerPoint2 + y + 1] = alignmentSquare[x + 2, y + 2];
                            theData[centerPoint2 + x + 1, centerPoint1 + y + 1] = alignmentSquare[x + 2, y + 2];
                            theData[centerPoint1 + x + 1, centerPoint2 + y + 1] = alignmentSquare[x + 2, y + 2];
                            theData[7 + x, centerPoint2 + y + 1] = alignmentSquare[x + 2, y + 2];
                            theData[centerPoint2 + x + 1, 7 + y] = alignmentSquare[x + 2, y + 2];
                        }
                    }

                }
            }

            return theData;
        }

        public bool[,] DrawVersionData(bool[,] theData)
        {
            if (versionNum >= 7)
            {
                Queue<bool> boolQueue = newBCH.VersionInformation();
                for (int y = 0; y < 6; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        if (boolQueue.Count > 0)
                        {
                            bool tempBool = boolQueue.Dequeue();
                            theData[squareSize - 12 + x, y + 1] = tempBool;
                            theData[y + 1, squareSize - 12 + x] = tempBool;
                        }

                        else
                        {
                            theData[squareSize - 12 + x, y + 1] = false;
                            theData[y + 1, squareSize - 12 + x] = false;
                        }
                    }
                }
            }
            return theData;
        }

        //due to formatting locations being more delicate, it is less for-loopy and more hardcoded
        public bool[,] DrawFormatData(bool[,] theData, int errorCorrectionActual, int maskNumber)
        {
            Queue<bool> boolQueue = newBCH.FormatInformation(errorCorrectionActual, maskNumber);

            //blocks 0-5
            for (int i = 0; i < 6; i++)
            {
                bool forBool = boolQueue.Dequeue();
                theData[9, i + 1] = forBool;
                theData[squareSize - i - 2, 9] = forBool;
            }

            bool tempBool = boolQueue.Dequeue();

            //block 6
            theData[9, 8] = tempBool;
            theData[squareSize - 8, 9] = tempBool;

            //next bool in the format data
            tempBool = boolQueue.Dequeue();

            //block 7
            theData[9, 9] = tempBool;
            theData[squareSize - 9, 9] = tempBool;

            tempBool = boolQueue.Dequeue();

            //block 8
            theData[8, 9] = tempBool;
            theData[9, squareSize - 8] = tempBool;

            //blocks 9-14
            for (int i = 6; i > 0; i--)
            {
                bool forBool = boolQueue.Dequeue();
                theData[9, squareSize - 1 - i] = forBool;
                theData[i, 9] = forBool;
            }

            return theData;
        }

        //there is always a black spot in this location, its codenamed "Dark Module" 
        public bool[,] DarkModule(bool[,] theData)
        {
            theData[9, squareSize - 9] = true;
            return theData;
        }
    }
}
