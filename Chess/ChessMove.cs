using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ChessMove
    {
        /*Array Structures
         * 0: Rook
         * 1: Knight
         * 2: Bishop
         * 3: Queen
         * 4: King
         * 5: Pawn
         */

        private static char[] ChessPieces_Notations = { 'R', 'N', 'B', 'Q', 'K' };
        private static int[] ChessPieces_Count = { 2, 2, 2, 1, 1, 8 };

        //gives all valid moves for each chess piece
        private static int[][] validMoves = new int[][]
        {
          new int[] { 1, 0, 0, 1},
          new int[] { 2, 1, 2, -1, 1, 2, 1, -2},
          new int[] { 1, 1, -1 , 1 },
          new int[] { 1, 0, 0, 1, 1, 1, -1, 1},
          new int[] {  1, 0, 0, 1, 1, 1, -1, 1},
          new int[] {1, 0, 2, 0, 1, 1, 1, -1}
        };

        //gives the scalar for the valid moves (scalar = 0 => not limited)
        private static int[][] scalar = new int[][]
        {
            new int[] {0},
            new int[] {1, -1},
            new int[] {0},
            new int[] {0},
            new int[] {1, -1},
            new int[] {1}
        };

        //Converts the used Chess Notation to a 8x8 board, which is usable for the LSTM network
        public static void CheckChessMove(int moveCount, byte[] bState, int color)
        {            
            if (Variables.InputState[moveCount] == null)
            {
                Variables.InputState[moveCount] = new float[64];
                Array.Copy(Variables.InputState[moveCount - 1], Variables.InputState[moveCount], 64);
            }

            char[] cState = Encoding.ASCII.GetChars(bState);

            //Checks for valid moves (O-O is the notation for castling)
            if (!cState.Contains<char>('-') || cState.Contains<char>('O'))
            {
                int[] newPos = new int[2];
                //Checks Castling
                if ((cState[2] & cState[4]) == 'O' && cState[3] == '-')
                {
                    int count = 0;
                    int min = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        int delta = Math.Abs((int)Variables.InputState[moveCount][color * 32 + 1 + i * 2] - (int)Variables.InputState[moveCount][color * 32 + 15]);
                        if (min == 0 || delta < min)
                        {
                            count = i;
                            min = delta;
                        }
                    }
                    //Sets the Rock and the King to the specified position
                    //Note that O-O-O is also a valid move used in Chess notation, even though it is not implemented yet
                    Variables.InputState[moveCount][color * 32 + 15] = 6;
                    Variables.InputState[moveCount][color * 32 + 1 + count * 2] = 5;
                }
                //Every other move except castling and leveling up the Pawn
                else
                {
                    newPos[0] = int.Parse(cState[4].ToString()) - 1;
                    newPos[1] = cState[3] - 'a';

                    //Captures
                    if (cState[2] == 'x')
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            if (Variables.InputState[moveCount][(1 - color) * 32 + i * 2] == newPos[0] && Variables.InputState[moveCount][(1 - color) * 32 + i * 2 + 1] == newPos[1])
                            {
                                Variables.InputState[moveCount][(1 - color) * 32 + i * 2] = -1;
                                Variables.InputState[moveCount][(1 - color) * 32 + i * 2 + 1] = -1;
                            }
                        }
                    }

                    bool Pawn = true;

                    //Checks all chess pieces
                    for (int i = 0; i < 5; i++)
                    {
                        if (cState.Contains<char>(ChessPieces_Notations[i]))
                        {
                            int piece = i;
                            Pawn = false;
                            //offset of InputState
                            int offset = color * 32;
                            for (int j = 0; j < i; j++)
                            {
                                offset += ChessPieces_Count[j] * 2;
                            }

                            //Chechs if all of the multiply chess pieces of its kind
                            for (int j = 0; j < ChessPieces_Count[piece]; j++)
                            {
                                //differenc of new position to the old position
                                int[] delta = { newPos[0] - (int)Variables.InputState[moveCount][offset + 2 * j], newPos[1] - (int)Variables.InputState[moveCount][offset + 1 + 2 * j] };

                                //checks if the above calculated delta is a valid move
                                for (int k = 0; k < validMoves[piece].Length / 2; k++)
                                {
                                    float factor = 0;
                                    bool legit = true;
                                    //vertical and horizontal movement
                                    for (int l = 0; l < 2; l++)
                                    {
                                        if (delta[l] == 0 && validMoves[i][k * 2 + l] != 0 || delta[l] != 0 && validMoves[i][k * 2 + l] == 0)
                                        {
                                            legit = false;
                                            l = 2;
                                        }
                                        else if (delta[l] != 0 && validMoves[i][k * 2 + l] != 0)
                                        {
                                            //if the movement is limited (e.g king)
                                            if (scalar[i][0] != 0)
                                            {
                                                int value = delta[l] / validMoves[i][k * 2 + l];
                                                legit = false;
                                                for (int m = 0; m < scalar[i].Length; m++)
                                                {
                                                    if (scalar[i][m] == value)
                                                    {
                                                        legit = true;
                                                    }
                                                }
                                                if (!legit)
                                                {
                                                    l = 2;
                                                }
                                            }
                                            else
                                            {
                                                if (factor == 0)
                                                {
                                                    factor = delta[l] / validMoves[i][k * 2 + l];
                                                }
                                                else
                                                {
                                                    if (Math.Abs(factor / delta[l] * validMoves[i][k * 2 + l]) != 1)
                                                    {
                                                        legit = false;
                                                        factor = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if the variant is a legitimate move
                                    if (legit)
                                    {
                                        if (!cState.Contains<char>('x') && new string(cState).Split('\0')[1].Length == 4)
                                        {
                                            if (cState[2] - 'a' != Variables.InputState[moveCount][offset + 2 * j + 1])
                                            {
                                                legit = false;
                                            }
                                        }
                                        else if (new string(cState).Split('\0').Length > 1)
                                        {
                                            if (new string(cState).Split('\0')[1].Length == 5)
                                            {
                                                if (cState[1] - 'a' != Variables.InputState[moveCount][offset + 2 * j + 1])
                                                {
                                                    legit = false;
                                                }
                                            }
                                        }
                                        if (piece == 0 && legit)
                                        {
                                            for (int m = 0; m < 32; m++)
                                            {
                                                if (Variables.InputState[moveCount][2 * m] == Variables.InputState[moveCount][offset + 2 * j])
                                                {
                                                    int delta1 = newPos[1] - (int)Variables.InputState[moveCount][2 * m + 1];

                                                    if (Math.Abs(delta1) < Math.Abs(delta[1]) && (float)delta1 / delta[1] > 0)
                                                    {
                                                        legit = false;
                                                        m = 32;
                                                    }
                                                }
                                            }
                                        }
                                        //updates movement
                                        if (legit)
                                        {
                                            Variables.InputState[moveCount][offset + 2 * j] = newPos[0];
                                            Variables.InputState[moveCount][offset + 2 * j + 1] = newPos[1];

                                            k = validMoves[i].Length / 2;
                                            j = ChessPieces_Count[i];
                                            i = 5;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    if (Pawn)
                    {
                        for (int i = 0; i < ChessPieces_Count[5]; i++)
                        {
                            int[] delta = new int[] { newPos[0] - (int)Variables.InputState[moveCount][color * 32 + 16 + i * 2], newPos[1] - (int)Variables.InputState[moveCount][color * 32 + 16 + i * 2 + 1] };

                            bool legit = true;

                            //Checks for valid capture moves
                            if (cState.Contains<char>('x'))
                            {
                                if (delta[0] != (color - 0.5f) * 2 || delta[1] != cState[3] - cState[1])
                                {
                                    legit = false;
                                }
                            }                            
                            else
                            {
                                if ((int)Variables.InputState[moveCount][color * 32 + 16 + i * 2] != 1)
                                {
                                    if (delta[0] >= 2 || delta[1] != 0)
                                    {
                                        legit = false;
                                    }
                                }
                                else if (delta[0] > 2 || delta[1] != 0)
                                {
                                    legit = false;
                                }
                            }

                            if (legit)
                            {
                                //Checks for overlapping movement, which is not allowed
                                for (int j = 0; j < 32; j++)
                                {
                                    if (Variables.InputState[moveCount][color * 32 + 16 + i * 2 + 1] == Variables.InputState[moveCount][j * 2 + 1])
                                    {
                                        if ((newPos[0] - Variables.InputState[moveCount][color * 32 + 16 + i * 2]) * (color - 0.5f) * 2 > (newPos[0] - Variables.InputState[moveCount][j * 2]) * (color - 0.5f) * 2)
                                        {
                                            if ((newPos[0] - Variables.InputState[moveCount][j * 2]) * (color - 0.5f) * 2 > 0)
                                            {
                                                legit = false;
                                            }
                                        }
                                    }
                                }

                                if (legit)
                                {
                                    Variables.InputState[moveCount][color * 32 + 16 + i * 2] = newPos[0];
                                    Variables.InputState[moveCount][color * 32 + 16 + i * 2 + 1] = newPos[1];

                                    i = ChessPieces_Count[5];
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
