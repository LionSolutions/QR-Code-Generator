using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

///By: Colin Gagich - Grade 12, Late Semester 1, 2011/2012

namespace QR_Code_Encoder
{
    public partial class frmMain : Form
    {
        Image myCanvas;

        static int pixelDensity = 5; //e.g. if pixelDensity =3, 1 boolian value from the array represents a 3x3 block of pixels in the final image (a 3-5 density is ideal)
        static int versionNum; //version number is 1-40 (but only versions 1-13 are functional)
        //static int maskVersion = 8; //changes mask pattern. 0-8 can be used (8 for no mask)
        static int squareSize;
        static int encodingType = 2;

        public frmMain()
        {
            InitializeComponent();
        }

        private void SetPixel(int x, int y, bool val)
        {
            int blockColour = 1;
            if (val) blockColour = 0;
            {
                //adds the pixel into the final image WITH the new pixel density for export
                for (int tempX = 0; tempX < pixelDensity; tempX++)
                {
                    for (int tempY = 0; tempY < pixelDensity; tempY++)
                    {
                        //starts at the target pixel from the original data, multiplies it by the pixel density to find its starting point
                        //then draws all of the pixels that will need to be drawn according to the density
                        ((Bitmap)myCanvas).SetPixel(x * pixelDensity + tempX, y * pixelDensity + tempY, Color.FromArgb(255, blockColour * 255, blockColour * 255, blockColour * 255));
                    }
                }
            }
        }

        private void qrStringIn_TextChanged(object sender, EventArgs e)
        {
            if (dynamicCheckBox.Checked == true)
                UpdateQrCode();
        }

        private bool DetermineErrorCorrection()
        {
            if (bestErrorCMode.Checked == true)
                return true;
            else
                return false;
        }

        public void UpdateQrCode()
        {
            string tempString = tempInvalidQrWorkAround();
            QrEncodeHelper qrAssistance = new QrEncodeHelper(tempString);

            int errorCorrectionLevel;

            versionNum = qrAssistance.DetermineBestVersionAndErrorCorrection(encodingType, DetermineErrorCorrection(), out errorCorrectionLevel);

            bool[,] dataArray;

            //17 is the base + 2 for the border plus 4 times the version number as the square increases by 4 for each version increase
            squareSize = 17 + 2 + 4 * versionNum;

            QrEncode newQr = new QrEncode(versionNum, errorCorrectionLevel);

            myCanvas = new Bitmap(squareSize * pixelDensity, squareSize * pixelDensity);
            Graphics g = Graphics.FromImage(myCanvas);
            g.Clear(Color.White);

            dataArray = newQr.EncodeTheData(tempString);

            //draws all of the pixels that are to be filled in
            for (int x = 0; x < dataArray.GetLength(0); x++)
            {
                for (int y = 0; y < dataArray.GetLength(0); y++)
                {
                    if (dataArray[x, y] == true)
                        SetPixel(x, y, true);
                }
            }

            //LABELS ARE NICE - also provides IMPORTANT data to me as i use it
            messageLengthLBL.Text = "Message Length: " + qrStringIn.Text.Length;
            versionLBL.Text = "Version: " + versionNum;
            maskLBL.Text = "Mask: " + newQr.mask;

            //appropriate error correction tag is set in the form
            switch (errorCorrectionLevel)
            {
                case 2:
                    bchLVL.Text = "EC: H";
                    break;
                case 3:
                    bchLVL.Text = "EC: Q";
                    break;
                case 0:
                    bchLVL.Text = "EC: M";
                    break;
                case 1:
                    bchLVL.Text = "EC: L";
                    break;

            }
            if (System.IO.File.Exists(System.Environment.CurrentDirectory.ToString() + "\\magic.png"))
            {
                mainPicBox.Load(System.Environment.CurrentDirectory.ToString() + "\\magic2.png");
                System.IO.File.Delete(System.Environment.CurrentDirectory.ToString() + "\\magic.png");
            }
            myCanvas.Save(System.Environment.CurrentDirectory.ToString() + "\\magic.png", System.Drawing.Imaging.ImageFormat.Png);
            mainPicBox.Load(System.Environment.CurrentDirectory.ToString() + "\\magic.png");
            
        }

        private void dynamicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //enabling / disabling dynamic mode changes whether or not you can press the generate button
            //by default dynamic mode is turned on. why? because it just looks so cool.
            if (dynamicCheckBox.Checked == true)
            {
                //if you switch dynamic mode on after making changes it just UPDATES. #kewlprogramy-ish-things
                generateCodeBtn.Enabled = false;
                UpdateQrCode();
            }
            else
                generateCodeBtn.Enabled = true;
        }

        private void generateCodeBtn_Click(object sender, EventArgs e)
        {
            //what else would a generate button do? like seriously...
            UpdateQrCode();
        }

        //this is a lovely fix that for the meantime shall fix the glitches of my program xD
        private string tempInvalidQrWorkAround()
        {
            string tempString = qrStringIn.Text;

            if (debugModeChkBox.Checked == false)
            {
                switch (qrStringIn.Text.Length)
                {
                    case 9:
                        //tempString += " ";
                        break;
                    case 48:
                        tempString += " ";
                        break;
                    case 66:
                        //tempString += " ";
                        break;
                    case 106:
                       tempString += "  ";
                        break;
                }
            }
            return tempString;
        }

        private void debugModeChkBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateQrCode();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
