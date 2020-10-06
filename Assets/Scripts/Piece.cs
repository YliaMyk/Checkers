using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    public bool ValidMove(Piece[,] board, int startX, int startY, int endX, int endY)
    {
        if (board[endX, endY] != null)
            return false;

        int deltaMoveX = (int)Mathf.Abs(startX - endX);
        int deltaMoveY = endY - startY;

        if (isWhite || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == 2)
                {
                    Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                    
                }
            }
        }

        if (!isWhite || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null && p.isWhite != isWhite)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsForcedToMove(Piece[,] board, int x, int y)
    {
        if (isWhite || isKing)
        {
            //Top Left
            if (x >=2 && y <= 5)
            {
                Piece p = board[x - 1, y + 1];
                //If there is a piece and it is not the same color as ours
                if (p != null && p.isWhite != this.isWhite)
                {
                    //Check if its possible to land after jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            //Top Right
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];
                //If there is a piece and it is not the same color as ours
                if (p != null && p.isWhite != this.isWhite)
                {
                    //Check if its possible to land after jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if (!isWhite || isKing)
        {
            //Bottom Left
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                //If there is a piece and it is not the same color as ours
                if (p != null && p.isWhite != this.isWhite)
                {
                    //Check if its possible to land after jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            //Bottom Right
            if (x <= 5 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                //If there is a piece and it is not the same color as ours
                if (p != null && p.isWhite != this.isWhite)
                {
                    //Check if its possible to land after jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}
