using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR_Code_Encoder
{
    ///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012
    ///this class had more function when i started the project, as the project progressed functions were moved from this class to other classes
    ///the main purpose though is still important, to connect the raw placed unmasked data with its masks, getting the best one, 
    ///and then returning it to the form to get displayed to the user
    ///for fancyness..
    
    class QrEncode
    {
        static int squareSize;
        static int versionNum;
        public int mask = 3;
        //this is the number that is really used by a qr code for the error correction level
        static int errorCorrectionActual = 0;
        //this is the logical number that i am going to use to make things understandable to a human
        static int errorCorrectionLogical = 0;

        public QrEncode(int versionNumIn)
        {
            squareSize = versionNumIn * 4 + 17 + 2;
            versionNum = versionNumIn;
        }

        public QrEncode(int versionNumIn, int errorCorrection)
        {
            //17 is the base, and each version increase adds 4 to the size (then an additional 2 for the whitespace around the sides)
            squareSize = versionNumIn * 4 + 17 + 2;
            versionNum = versionNumIn;

            //this fixes how the index codes for error correction levels are not in a logical order
            errorCorrectionActual = errorCorrection;
            switch (errorCorrectionActual)
            {
                case 2:
                    errorCorrectionLogical = 0;
                    break;
                case 3:
                    errorCorrectionLogical = 1;
                    break;
                case 0:
                    errorCorrectionLogical = 2;
                    break;
                case 1:
                    errorCorrectionLogical = 3;
                    break;
             }
        }

        //calls all functions that are NOT related to the data or error correction
        //used to redraw all of those important pieces after it gets masked
        public bool[,] DrawAllBasics(bool[,] theData)
        {
            QrCodeElements qrElements = new QrCodeElements(versionNum, squareSize);
            
            theData = qrElements.DrawTrackerSquares(theData);
            theData = qrElements.DrawTimingLines(theData);
            theData = qrElements.DrawAlignmentSquares(theData);
            theData = qrElements.DarkModule(theData);

            return theData;
        }

        public bool[,] EncodeTheData(string message)
        {
            bool[,] theData;
            Mask thisMask = new Mask(versionNum, squareSize, errorCorrectionActual);
            DataEncodeQR thisQr = new DataEncodeQR(versionNum, errorCorrectionLogical);

            theData = thisQr.DataEncodeSwitch(message, 3);

            //apply the best mask and return it
            theData = thisMask.BestMask(theData, out mask);

            return theData;
        }
    }
}
