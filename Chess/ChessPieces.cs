using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ChessPieces
    {
        public static string CompareChessPiece(int x, int y, Bitmap chessboard, string folderpath)
        {
            int[,,] chesspiece = new int[18, 18, 3];

            for(int i = 0; i < 18; i++)
            {
                for(int j = 0; j < 18; j++)
                {
                    int[] avg = new int[3];

                    for(int k = 0; k < 5; k++)
                    {
                        for(int l = 0; l < 5; l++)
                        {
                            chesspiece[i, j, 0] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).R;
                            chesspiece[i, j, 1] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).G;
                            chesspiece[i, j, 2] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).B;
                        }
                    }

                    for(int k = 0; k < 3; k++)
                    {
                        chesspiece[i, j, k] = (int)Math.Round((double)chesspiece[i, j, k] / 25);
                    }
                }
            }

            string[] files = Directory.GetFiles(folderpath);
            double max = 0;
            string cat = "";

            for(int i = 0; i < 26; i++)
            {
                Bitmap b_chesspiece = new Bitmap(files[i]);
                double res = 0;

                for(int j = 0; j < 18; j++)
                {
                    for(int k = 0; k < 18; k++)
                    {
                        int[] avg = new int[3];
                        avg[0] = b_chesspiece.GetPixel(j, k).R;
                        avg[1] = b_chesspiece.GetPixel(j, k).G;
                        avg[2] = b_chesspiece.GetPixel(j, k).B;

                        for(int l = 0; l < 3; l++)
                        {
                            res += Math.Abs(avg[l] - chesspiece[j, k, l]);
                        }
                    }
                }

                if(res < max || i == 0)
                {
                    max = res;
                    cat = files[i].Split('\\')[files[i].Split('\\').Length - 1].Split('.')[0];
                }
            }

            return cat;
        }

        public static void FindPieces(string imagepath, string folderpath)
        {
            Bitmap chessboard = new Bitmap(imagepath);

            ChessPiece(0, 0, 182, chessboard, folderpath);
            ChessPiece(1, 0, 272, chessboard, folderpath);

            for(int i = 0; i < 8; i++)
            {
                int x = i * 91;

                if(i > 1)
                {
                    x--;
                    if(i > 6)
                    {
                        x--;
                    }
                }

                for(int j = 0; j < 2; j++)
                {
                    ChessPiece(32 + i * 4 + j, x, j * 91, chessboard, folderpath);
                    ChessPiece(32 + i * 4 + j + 2, x, (6 + j) * 91 - 2, chessboard, folderpath);
                }
            }

            chessboard.Dispose();
        }

        private static void ChessPiece(int counter, int x, int y, Bitmap chessboard, string folderpath)
        {
            Bitmap chesspiece = new Bitmap(18, 18);

            for(int i = 0; i < 18; i++)
            {
                for(int j = 0; j < 18; j++)
                {
                    int[] avg = new int[3];

                    for(int k = 0; k < 5; k++)
                    {
                        for(int l = 0; l < 5; l++)
                        {
                            avg[0] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).R;
                            avg[1] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).G;
                            avg[2] += chessboard.GetPixel(x + i * 5 + k, y + j * 5 + l).B;
                        }
                    }

                    for(int k = 0; k < 3; k++)
                    {
                        avg[k] = (int)Math.Round((double)avg[k] / 25);
                    }

                    chesspiece.SetPixel(i, j, Color.FromArgb(avg[0], avg[1], avg[2]));
                }
            }

            chesspiece.Save(folderpath + "\\ChessPiece_" + counter + ".jpg");
            chesspiece.Dispose();
        }
    }
}
