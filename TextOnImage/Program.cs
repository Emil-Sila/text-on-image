using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Text;
using AForge.Imaging.Filters;

namespace TextOnImage
{
    class Program
    {
        static string fontLocation = @"D:\Faks\Diplomski\Fonts";

        static int targetPixels = 100;//(int)targetHeight * Dpi;
        static int Dpi = 100;
        static float tSize;
        static Bitmap glyphImage;
        static PrivateFontCollection fontCollection = new PrivateFontCollection();
        static string[] glyphs = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G",
            "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1",
            "2", "3", "4", "5", "6", "7", "8", "9", "0" };


        static void Main(string[] args)
        {
            ProcessDirectory(fontLocation);
        }

        // Process all files in the directory passed in
        public static void ProcessDirectory(string targetDirectory)
        {
            // Find all directories in the main path
            string[] directoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string directory in directoryEntries)
            { 
                // Check if the font folder has already been processed
                string[] glyphImages = Directory.GetFiles(directory, "*.png");

                /*
                // DEVELPOMENT ONLY: delete all images 
                if (glyphImages.Length > 0)
                {
                    foreach (string glyphImge in glyphImages)
                    {
                        File.Delete(glyphImge);
                    }
                }
                */

                if (true)//glyphImages.Length == 0)
                {
                    // Process the list of files found in the directory. Process only regular style fonts
                    string[] fontEntries = Directory.GetFiles(directory, "*Regular.ttf");
                    foreach (string fontName in fontEntries)
                    {
                        // Add unprocessed fonts to the collection
                        fontCollection.AddFontFile(fontName);
                        // Process font from the collection
                        MakeFontExamples(fontCollection.Families.Last(), (int)FontStyle.Regular, directory);
                    }
                }
            }
        }

        public static void MakeFontExamples(FontFamily ff, int fs, string fontDirPath)
        {
            foreach (string glyph in glyphs)
            {
                /*
                if (char.IsUpper(glyph[0]))
                {
                    string glyphLoc = Directory.GetFiles(fontDirPath, "*" + glyph + "_.png")[0];
                    glyphImage = (Bitmap)Image.FromFile(glyphLoc);
                }
                else
                {
                    string glyphLoc = Directory.GetFiles(fontDirPath, "*" + glyph + ".png")[0];
                    glyphImage = (Bitmap)Image.FromFile(glyphLoc);
                }

                GaussianBlur filter = new GaussianBlur(2, 3);
                filter.ApplyInPlace(glyphImage);

                string fontDirName = new DirectoryInfo(fontDirPath).Name;

                if (char.IsUpper(glyph[0]))
                    glyphImage.Save(fontDirPath + "\\" + fontDirName + "_" + glyph + "_BL_.png", ImageFormat.Png);
                else
                    glyphImage.Save(fontDirPath + "\\" + fontDirName + "_" + glyph + "BL.png", ImageFormat.Png);
                */
                
                using (Bitmap bmp = new Bitmap(targetPixels, targetPixels))
                {
                    // either set the resolution here
                    // or get and use it above from the Graphics!
                    bmp.SetResolution(Dpi, Dpi);
                    using (Graphics G = Graphics.FromImage(bmp))
                    {
                        G.SmoothingMode = SmoothingMode.AntiAlias;
                        G.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        // target position (in pixels)
                        PointF DrawingPoint = new PointF(0, 0);
                        GraphicsPath CharGraphicsPath = new GraphicsPath();
                        // first try:
                        CharGraphicsPath.AddString(glyph, ff, fs, targetPixels, DrawingPoint,
                                     StringFormat.GenericTypographic);
                        // this is the 1st result
                        RectangleF CharArea = CharGraphicsPath.GetBounds();
                        // now we correct the size (max dimesion is equal to targetPixels)
                        if (CharArea.Width > CharArea.Height)
                        {
                            tSize = targetPixels * targetPixels / CharArea.Width;
                        }
                        else
                        {
                            tSize = targetPixels * targetPixels / CharArea.Height;
                        }

                        // Redraw
                        CharGraphicsPath.Reset();
                        CharGraphicsPath.AddString(glyph, ff, fs, tSize, DrawingPoint, StringFormat.GenericTypographic);
                        // Check the area of the charachter again
                        CharArea = CharGraphicsPath.GetBounds();

                        // Correct the location of the charachter
                        //MessageBox.Show(CharArea.ToString());
                        DrawingPoint = new PointF(DrawingPoint.X - CharArea.X, DrawingPoint.X - CharArea.Y);

                        // Final drawing
                        CharGraphicsPath.Reset();
                        CharGraphicsPath.AddString(glyph, ff, fs, tSize, DrawingPoint, StringFormat.GenericTypographic);

                        // Draw the path on the image
                        G.Clear(Color.White);
                        G.FillPath(Brushes.Black, CharGraphicsPath);

                        string fontDirName = new DirectoryInfo(fontDirPath).Name;

                        //now we save the image 
                        if (char.IsUpper(glyph[0]))
                            bmp.Save(fontDirPath + "\\" + fontDirName + "_" + glyph + "_AA_.png", ImageFormat.Png); // "_NH_.png"
                        else
                            bmp.Save(fontDirPath + "\\" + fontDirName + "_" + glyph + "AA.png", ImageFormat.Png); // "NH.png"

                        bmp.Dispose();
                        CharGraphicsPath.Dispose();
                        G.Dispose();
                    }           
                }

            }
        }
    }
}
