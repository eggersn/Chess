using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class DataBase
    {
        private static float[] defaultInput = { 7, 0, 7, 7, 7, 1, 7, 6, 7, 2, 7, 5, 7, 3, 7, 4, 6, 0, 6, 1, 6, 2, 6, 3, 6, 4, 6, 5, 6, 6, 6, 7,
                                                0, 0, 0, 7, 0, 1, 0, 6, 0, 2, 0, 5, 0, 3, 0, 4, 1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7};
        private static char[] notations = { 'R', 'N', 'B', 'Q', 'K' };
        private static string url = "http://www.chessgames.com/perl/chessgame?gid=";
        private static int count = 1000000;

        public static void ConvertdefaultInput()
        {
            for (int i = 0; i < defaultInput.Length; i++)
            {
                defaultInput[i] = defaultInput[i] / 3.5f - 1;
            }
        }

        public static void GetChessGames(int sampleSize, int offset)
        {
            string filepath = Program.folderpath + "\\DataSet.txt";
            Console.Write("Progress:\t 0%");
            int temp = sampleSize;
            int progress = 0;

            HttpWebRequest request;
            HttpWebResponse response;
            count += offset;

            while (sampleSize > 0)
            {
                if ((float)(temp - sampleSize) / temp > progress * 0.01f)
                {
                    progress += (temp - sampleSize) / temp * 100;
                    if (progress < 10)
                    {
                        Console.Write("\b\b\b\b " + Math.Round((float)progress) + " %");
                    }
                    else
                    {
                        Console.Write("\b\b\b\b" + Math.Round((float)progress) + " %");
                    }
                }

                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url + count.ToString());
                    response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;
                        bool validGame = true;

                        if (response.CharacterSet == null)
                        {
                            readStream = new StreamReader(receiveStream);
                        }
                        else
                        {
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        string data = readStream.ReadToEnd();

                        response.Close();
                        readStream.Close();

                        if (data.Contains("Result \"0-1\"") || data.Contains("Result \"1-0\""))
                        {
                            string[] moves = data.Split('\n')[55].Split('.');
                            byte[] buffer = new byte[(moves.Length - 1) * 10 + 3];

                            buffer[0] = 0;
                            if (data.Contains("Result \"1-0\""))
                            {
                                buffer[0] = 1;
                            }

                            buffer[1] = (byte)Math.Floor((float)moves.Length / 256);
                            buffer[2] = (byte)(moves.Length - 256 * buffer[1]);

                            for (int i = 1; i < moves.Length; i++)
                            {
                                string[] colorMoves = moves[i].Split(' ');

                                for (int j = 0; j < 2; j++)
                                {
                                    if (colorMoves[j].Length > 5 || colorMoves[j] == "O-O-O")
                                    {
                                        validGame = false;
                                        i = moves.Length;
                                        j = 2;
                                    }
                                    else
                                    {
                                        if (colorMoves[j][colorMoves[j].Length - 1] == '+')
                                        {
                                            colorMoves[j] = colorMoves[j].Split('+')[0];
                                        }

                                        for (int k = 5 - colorMoves[j].Length; k < 5; k++)
                                        {
                                            buffer[3 + ((i - 1) * 2 + j) * 5 + k] = Encoding.ASCII.GetBytes(new char[] { colorMoves[j][k - 5 + colorMoves[j].Length] })[0];
                                        }
                                    }

                                }
                            }

                            if (validGame)
                            {
                                using (var stream = new FileStream(filepath, FileMode.Append))
                                {
                                    stream.Write(buffer, 0, buffer.Length);
                                }

                                sampleSize--;
                            }

                        }
                        count++;
                    }
                }
                catch
                {
                    count++;
                }
            }

            Console.WriteLine();
        }

        public static void ConvertChessNotation(int offset)
        {
            string filepath = Program.folderpath + "\\DataSet.txt";
            byte[] buffer = File.ReadAllBytes(filepath);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (offset > 0)
                {
                    i += (buffer[i + 1] * 256 + buffer[i + 2] - 1) * 10 + 2;
                    offset--;
                }
                else
                {
                    Variables.winningColor = buffer[i];
                    Variables.stateCount = (buffer[i + 1] * 256 + buffer[i + 2]);
                    Program.Dimensions[0] = Variables.stateCount;
                    Variables.InputState = new float[Variables.stateCount][];
                    Variables.InputState[0] = new float[64];
                    Array.Copy(defaultInput, Variables.InputState[0], 64);
                    byte[] data = new byte[5];

                    for (int j = 0; j < Variables.stateCount - 1; j++)
                    {
                        Array.Copy(buffer, 3 + i + j * 10, data, 0, 5);
                        ChessMove.CheckChessMove(j + 1, data, 1);
                        Array.Copy(buffer, 8 + i + j * 10, data, 0, 5);
                        ChessMove.CheckChessMove(j + 1, data, 0);
                    }

                    i = buffer.Length;
                }
            }
        }

        public static void GetWorkspaceSize(int offset, int size)
        {
            string filepath = Program.folderpath + "\\DataSet.txt";
            byte[] buffer = File.ReadAllBytes(filepath);
            int tempSize = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (offset > 0)
                {
                    int s = buffer[i + 1] * 256 + buffer[i + 2] - 1;
                    i += s * 10 + 2;
                    offset--;
                }
                else if (size > 0)
                {
                    int s = (buffer[i + 1] * 256 + buffer[i + 2]);
                    if (s > tempSize)
                    {
                        tempSize = s;
                    }
                    i += (s - 1) * 10 + 2;
                    size--;
                }
            }

            Program.Dimensions[0] = tempSize;
        }
    }
}
