using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012
///This class deals with the 8 masking patterns that can be applied to the grid
///and evaluates all the maskes on a unique penalty scoring system that determines which
///mask is the best with that particular set of data.
///some challenges are as followed: each version of the grid after masking requires a different
///format information string, as the mask number is contained in it. This means that each mask has to have its own
///format string calculated as its being placed in the grid.
///for fancyness..

namespace QR_Code_Encoder
{
    class Mask
    {
        static int versionNum = 1;
        static int squareSize = 0;
        static bool[][,] maskArray = new bool[9][,];
        static int errorCorrection;
        QrCodeElements QrElements;

        public Mask(int versionNumIn, int squareSizeIn, int errorCorrectionActual)
        {
            versionNum = versionNumIn;
            squareSize = squareSizeIn;
            errorCorrection = errorCorrectionActual;

            QrElements = new QrCodeElements(versionNum, squareSize);
        }

        //determines the best mask and returns the mask number
        public bool[,] BestMask(bool[,] theData, out int BestMask)
        {
            int[] penatlyScores = new int[8];
            BestMask = 0;
            //version data is included in the mask, so it must be added before the mask is applied

            maskArray[8] = (bool[,])theData.Clone();
            maskArray[8] = DrawAllBasics(new bool[squareSize, squareSize], errorCorrection, 0);
            for (int i = 0; i <= 7; i++)
            {
                //mask a copy of the data with the current mask
                maskArray[i] = Masker((bool[,])theData.Clone(), i);
                //draw all of the remaining elements
                maskArray[i] = DrawAllBasics(maskArray[i], errorCorrection, i);
            }

            PenanltyScoreOne(maskArray, penatlyScores);
            PenaltyScoreTwo(maskArray, penatlyScores);
            PenanltyScoreThree(maskArray, penatlyScores);
            PenanltyScoreFour(maskArray, penatlyScores);

            //determines which mask is the BEST one
            for (int i = 1; i < 8; i++)
            {
                if (penatlyScores[i] < penatlyScores[BestMask])
                    BestMask = i;
            }

            return maskArray[BestMask];
        }

        private void PenanltyScoreOne(bool[][,] theData, int[] penaltyScores)
        {
            //cycles through each of the 7 different mask patterns
            for (int i = 0; i <= 7; i++)
            {
                //cycling through the x and y
                for (int x = 1; x < squareSize - 1; x++)
                {
                    string horizontalString = "", verticalString = "";

                    //adds the first element of the line to both strings
                    //the first element had to be added before the loop other wise two elements wouldn't be present for the change detection
                    //since the grid can be viewed as a reflection, x and y can just be flipped
                    switch (theData[i][x, 1])
                    {
                        case true:
                            verticalString += "1";
                            break;
                        case false:
                            verticalString += "0";
                            break;
                    }
                    switch (theData[i][1, x])
                    {
                        case true:
                            horizontalString += "1";
                            break;
                        case false:
                            horizontalString += "0";
                            break;
                    }
                    
                    //this cycles through each line of data
                    for (int y = 2; y < squareSize - 1; y++)
                    {
                        //adds the next element in the sequence
                        switch (theData[i][x, y])
                        {
                            case true:
                                verticalString += "1";
                                break;
                            case false:
                                verticalString += "0";
                                break;
                        }
                        switch (theData[i][y, x])
                        {
                            case true:
                                horizontalString += "1";
                                break;
                            case false:
                                horizontalString += "0";
                                break;
                        }

                        //the purpose to this function is to detect change in the sequence but at the same time keep track of how many bits are the same in a row
                        //as the sequence cycles, it collects like bits, once a different bits appears, it counts how many of the same appeared in a row, and makes
                        //the most recent bit the first bit of the new sequence
                        //scoring goes as followed: if the bits are more than 5 in length, 3 points are added and for each bit after the first five, an addition point is added
                        if (verticalString.Substring(verticalString.Length - 2, 1) != verticalString.Substring(verticalString.Length - 1, 1))
                        {
                            if (verticalString.Length - 1 >= 5)
                            {
                                penaltyScores[i] += 3 + (verticalString.Length - 6);
                            }
                            verticalString = verticalString.Substring(verticalString.Length - 1);
                        }

                        //if the current element is different than the last element
                        if (horizontalString.Substring(horizontalString.Length - 2, 1) != horizontalString.Substring(horizontalString.Length - 1, 1))
                        {
                            //if the sequence is 5 or more in lenth (not including the last bit which is different than the sequence)
                            if (horizontalString.Length - 1 >= 5)
                            {
                                //add the right amount of points
                                penaltyScores[i] += 3 + (horizontalString.Length - 6);
                            }
                            //clear the string and make it the same as the most recently added bit (which is the different one) so the whole process can be started again
                            horizontalString = horizontalString.Substring(horizontalString.Length - 1);
                        }
                    }

                    //obviously at the end of the row, there could be more than 5 bits in row but the previous method may not pick up on it because there is no "change"
                    //this will catch that and add the right amount of points
                    if (horizontalString.Length >= 5)
                        penaltyScores[i] += horizontalString.Length -2;

                    if (verticalString.Length >= 5)
                        penaltyScores[i] += verticalString.Length - 2;
                    

                }
            }
        }

        private void PenaltyScoreTwo(bool[][,] theData, int[] penaltyScores)
        {
            //cycles through each of the 7 different mask patterns
            for (int i = 0; i <= 7; i++)
            {
                //cycling through the x and y
                for (int x = 1; x < squareSize - 1; x++)
                {
                    for (int y = 1; y < squareSize - 1; y++)
                    {
                        //the gist of this -> if a 2x2 square of the idential colour is found, 
                        //with the current object being evaluted as the top left corner of the square,
                        //add three to the mask's penatly score
                        //this method is slightly different than the one that is used by the standard but i have found it to have the same result in ALL cases
                        if ((theData[i][x + 1, y] == theData[i][x, y]) && (theData[i][x, y + 1] == theData[i][x, y]) && (theData[i][x + 1, y + 1] == theData[i][x, y]))
                            penaltyScores[i] += 3;
                    }
                }
            }
        }

        private void PenanltyScoreThree(bool[][,] theData, int[] penaltyScores)
        {
            //cycles through each of the 7 different mask patterns
            for (int i = 0; i <= 7; i++)
            {
                //cycling through the x and y
                for (int x = 1; x < squareSize - 1; x++)
                {
                    string horizontalString = "", verticalString = "";

                    //collect the data, in a mirror image this is grabing each row, the top and the bottom
                    for (int y = 1; y < squareSize - 1; y++)
                    {
                        verticalString += theData[i][x, y];
                        horizontalString += theData[i][y, x];
                    }

                    //if the row/collumn contains any of these sequences, 40 points are added to the penatly score
                    //these patterns are that of a tracker square pattern, for obvious reasons we do not want these patterns occuring in the middle of the grid
                    if (verticalString.Contains("FalseFalseFalseFalseTrueFalseTrueTrueTrueFalseTrue"))
                        penaltyScores[i] += 40;
                    if (horizontalString.Contains("FalseFalseFalseFalseTrueFalseTrueTrueTrueFalseTrue"))
                        penaltyScores[i] += 40;
                    if (horizontalString.Contains("TrueFalseTrueTrueTrueFalseTrueFalseFalseFalseFalse"))
                        penaltyScores[i] += 40;
                    if (verticalString.Contains("TrueFalseTrueTrueTrueFalseTrueFalseFalseFalseFalse"))
                        penaltyScores[i] += 40;
                }
            }
        }

        //this is a contrast meter of light vs dark modules
        //this score is not very important, as the highest points i have seen added is 8,
        //in the grande scheme of things when other penalty score are pulling numbers of 200+, this one is almost irrelevant
        private void PenanltyScoreFour(bool[][,] theData, int[] penaltyScores)
        {
            //cycles through each of the 7 different mask patterns
            for (int i = 0; i <= 7; i++)
            {
                double numBlackSquares = 0;
                double pointScore;

                //counts all of the black squares
                foreach (bool thisBool in theData[i])
                {
                    if (thisBool == true)
                        numBlackSquares++;
                }

                //this is the number of black squares divided by the total number of squares in the symbol
                //multiplied by 100, this is a percentage?
                pointScore = (numBlackSquares / Math.Pow(squareSize - 2, 2)) * 100;
                pointScore -= 50;

                //make pointScore ALWAYS positive
                if (pointScore < 0)
                    pointScore *= -1;

                //remove anything after the decimal (yes i know just converting it to an int would work, but i like it this way)
                pointScore -= pointScore % 1;

                //it has to be divided by 5 then multiplied, not times by 2 like logic says
                pointScore = (pointScore / 5) * 10;

                //add the points to the total!
                penaltyScores[i] += (int)pointScore;
            }
        }

        private bool[,] DrawAllBasics(bool[,] theData, int errorCorrection, int mask)
        {
            theData = QrElements.DrawTrackerSquares(theData);
            theData = QrElements.DrawTimingLines(theData);
            theData = QrElements.DrawAlignmentSquares(theData);
            theData = QrElements.DarkModule(theData);
            theData = QrElements.DrawFormatData(theData, errorCorrection, mask);
            theData = QrElements.DrawVersionData(theData);
            return theData;
        }

        public bool[,] Masker(bool[,] theData, int maskMode)
        {
            //cycle through the x and the y
            for (int x = 0; x < squareSize - 2; x++)
            {
                for (int y = 0; y < squareSize - 2; y++)
                {
                    double tempVal = -1;
                    switch (maskMode)
                    {
                        case 0:
                            tempVal = (x + y) % 2;
                            break;
                        case 1:
                            tempVal = y % 2;
                            break;
                        case 2:
                            tempVal = x % 3;
                            break;
                        case 3:
                            tempVal = (x + y) % 3;
                            break;
                        case 4:
                            tempVal = ((y / 2) + (x / 3)) % 2;
                            break;
                        case 5:
                            tempVal = (y * x) % 2 + (x * y) % 3;
                            break;
                        case 6:
                            tempVal = ((y * x) % 2 + (x * y) % 3) % 2;
                            break;
                        case 7:
                            tempVal = ((y + x) % 2 + (x * y) % 3) % 2;
                            break;
                    }

                    if (tempVal == 0)
                        theData[x + 1, y + 1] ^= true;
                    else if (tempVal != -1)
                        theData[x + 1, y + 1] ^= false;
                    //+1 due to the border, -2 was taken off the forLoop for account for the total border

                }
            }
            return theData;
        }

    }
}
