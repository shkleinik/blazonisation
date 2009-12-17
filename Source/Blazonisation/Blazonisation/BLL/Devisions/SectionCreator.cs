using System.Collections.Generic;
using System.Drawing;

namespace Blazonisation.BLL.Devisions
{
    /// <summary>
    /// 1 step
    /// division to 4 massives
    /// ____________
    /// |     |     |
    /// |  0  |  1  |
    /// |_____|_____|
    /// |     |     |
    /// |  2  |  3  |
    /// ____________
    /// 
    /// 2 step
    /// division every part by diagonal
    /// calculate X coordinate by defining on which line of y coord we are now
    /// 
    /// example
    /// if y=0
    /// koef=y/YLENGTH=0
    /// 
    /// x=koef*XLENGTH=0
    /// 
    /// if y=YL/2;
    /// koef=(YL/2)/YL=1/2;
    /// x=koef*XL=1/2XL
    /// 
    /// </summary>
    public class SectionCreator
    {
        static int[] widthes = new int[4];

        /// <summary>
        /// zero index is in 1 section after 1 step
        /// and in upper diagonal
        /// </summary>
        /// <returns></returns>
        public static List<Color>[] GetSectionsArray(Bitmap bitmap)
        {
            List<Color>[] fourSections = DivideOnFourSections(bitmap);
            List<Color[,]> four2DSections = new List<Color[,]>();

            for (int i = 0; i < fourSections.Length; i++)
            {
                four2DSections.Add(CreateTwoDimensionalArray(fourSections[i], widthes[i]));
            }

            List<Color[,]> tempFour2DSections = new List<Color[,]>();
            tempFour2DSections.Add(four2DSections[1]);
            tempFour2DSections.Add(four2DSections[3]);
            tempFour2DSections.Add(four2DSections[2]);
            tempFour2DSections.Add(four2DSections[0]);

            List<Color>[] eightSections = new List<Color>[8];
            for (int i = 0; i < tempFour2DSections.Count; i++)
            {
                int firstAddIndex = i < 2 ? 1 : 0;
                int secondAddIndex = i < 2 ? 0 : 1;
                bool isStartFromUpLeft = i % 2 == 0 ? false : true;
                List<Color>[] diagonalSections = DivideOnDiagonalSections(tempFour2DSections[i], isStartFromUpLeft);
                eightSections[i * 2] = diagonalSections[firstAddIndex];
                eightSections[i * 2 + 1] = diagonalSections[secondAddIndex];
            }
            return eightSections;
        }

        private static List<Color>[] DivideOnFourSections(Bitmap bitmap)
        {
            int averageX = bitmap.Width / 2;
            int averageY = bitmap.Height / 2;
            widthes[0] = averageX;
            widthes[1] = bitmap.Width - averageX;
            widthes[2] = widthes[0];
            widthes[3] = widthes[1];

            List<Color>[] fourSections = new List<Color>[4];
            fourSections[0] = new List<Color>();
            fourSections[1] = new List<Color>();
            fourSections[2] = new List<Color>();
            fourSections[3] = new List<Color>();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (x < averageX)
                    {
                        if (y < averageY)//0
                        {
                            fourSections[0].Add(bitmap.GetPixel(x, y));
                        }
                        else//2
                        {
                            fourSections[2].Add(bitmap.GetPixel(x, y));
                        }
                    }
                    else
                    {
                        if (y < averageY)//1
                        {
                            fourSections[1].Add(bitmap.GetPixel(x, y));
                        }
                        else//3
                        {
                            fourSections[3].Add(bitmap.GetPixel(x, y));
                        }
                    }
                }
            }
            return fourSections;
        }

        private static Color[,] CreateTwoDimensionalArray(List<Color> colors, int arrayWidth)
        {
            int height = colors.Count / arrayWidth;
            Color[,] array = new Color[arrayWidth, height];
            for (int i = 0; i < colors.Count; i++)
            {
                int xIndex = i % arrayWidth;
                int yIndex = i / arrayWidth;
                array[xIndex, yIndex] = colors[i];
            }
            return array;
        }



        /// <summary>
        /// \             /
        ///  \  1     1  /
        ///   \         /
        ///  0 \       /  0
        ///     \     /
        ///     upper diagonal array always at 1 index in out array
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="isFromUpLeftDivision"></param>
        /// <returns></returns>
        private static List<Color>[] DivideOnDiagonalSections(Color[,] colors, bool isFromUpLeftDivision)
        {
            List<Color>[] diagonalSections = new List<Color>[2];
            diagonalSections[0] = new List<Color>();
            diagonalSections[1] = new List<Color>();

            int averageX;
            int height = colors.GetLength(1);
            int width = colors.GetLength(0);

            int rightSection = isFromUpLeftDivision ? 1 : 0;
            int leftSection = isFromUpLeftDivision ? 0 : 1;

            for (int y = 0; y < height; y++)
            {
                double yDouble = (double)y;
                averageX = (int)(width * (yDouble / (height - 1)));
                for (int x = 0; x < width; x++)
                {
                    if (x >= averageX)//up array
                    {
                        diagonalSections[rightSection].Add(colors[x, y]);
                    }
                    else
                    {
                        diagonalSections[leftSection].Add(colors[x, y]);
                    }
                }
            }

            return diagonalSections;
        }
    }
}