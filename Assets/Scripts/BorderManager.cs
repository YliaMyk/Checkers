using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BorderManager : MonoBehaviour
{
    private Piece[,] pieces = new Piece[8, 8];

    public GameObject whiteObj;
    public GameObject blackObj;

    private int blackCount;
    private int whiteCount;

    private Piece selectedPiece;

    //proper moving additionals
    private Vector3 boardOffset = new Vector3(-4.0f, 0f, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0f, 0.5f);

    //drags
    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    public bool isWhite;
    private bool isWhiteTurn;
    private bool hasKilled;

    //UI fields
    public bool isStarted = false;
    public Text sideInText;
    public Text endText;
    public GameObject canvas;
    public GameObject endCanvas;

    private List<Piece> forcedPieces;

    // Start is called before the first frame update
    void Start()
    {
        spawnCheckers(0, 3);
        spawnCheckers(5, 8);
        forcedPieces = new List<Piece>();
        isWhiteTurn = true;
        isWhite = true;
        whiteCount = 12; 
        blackCount = 12;
    }

    public void setSide()
    {
        isWhite = !isWhite;
        isWhiteTurn = !isWhiteTurn;
        if (isWhite) sideInText.text = "Your choise is White";
        else sideInText.text = "Your choise is Black";
    }

    public void StartGame()
    {
        canvas.SetActive(false);
        isStarted = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }

    private void spawnCheckers(int yStart, int yEnd)
    {
        for (int y = yStart; y < yEnd; y++)
        {
            bool isOdd = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece(isOdd ? x : x + 1, y);
            }
        }
    }


    private void GeneratePiece(int x, int y)
    {
        bool yIsOver3 = y > 3;
        GameObject go = Instantiate(yIsOver3 ? blackObj : whiteObj) as GameObject; //spawn on scene
        go.transform.SetParent(transform); //Set Board as parent
        Piece p = go.GetComponent<Piece>(); //get piece using Generic 
        pieces[x, y] = p; //set piece to pieces array
        Move(p, x, y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted) return;
            if (Input.touchCount > 0)
            {
                UpdateTouchOver();

                if (isWhite ? isWhiteTurn : !isWhiteTurn)
                {
                int x = (int)mouseOver.x;
                int y = (int)mouseOver.y;

                Touch touch = Input.GetTouch(0);

                if (selectedPiece != null)
                    UpdatePieceDragUp(selectedPiece);

                if (touch.phase == TouchPhase.Began)
                    SelectPiece(x, y);

                /*if (Input.GetMouseButtonDown(0)) //Left click of mouse
                    SelectPiece(x, y);*/

                int dragX = (int)startDrag.x;
                int dragY = (int)startDrag.y;

                if (touch.phase == TouchPhase.Ended)
                    MoveToPosition(dragX, dragY, x, y);

                /*if (Input.GetMouseButtonUp(0)) //release mouse
                    MoveToPosition(dragX, dragY, x, y);*/
                }
       }
    }
    

    private void Move(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }

    private void UpdateTouchOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find");
            return;
        }

        RaycastHit hit;
        
        Touch touch = Input.GetTouch(0);

        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int) (hit.point.x - boardOffset.x);
            mouseOver.y = (int) (hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    /**
     * For upping a piece while dragging
     **/
    private void UpdatePieceDragUp(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find");
            return;
        }

        RaycastHit hit;
        Touch touch = Input.GetTouch(0);
        //Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private bool checkBorder(int coord)
    {
        return coord < 0 || coord >= 8;
    }

    private void SelectPiece(int x, int y)
    {

        if (checkBorder(x) || checkBorder(y))
            return;

        Piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }
    
    private void MoveToPosition(int startX, int startY, int endX, int endY)
    {

        forcedPieces = GetPossibleMoves();

        startDrag = new Vector2(startX, startY);
        endDrag = new Vector2(endX, endY);
        selectedPiece = pieces[startX, startY];

        if(checkBorder(endX) || checkBorder(endY))
        {
            if (selectedPiece != null)
                Move(selectedPiece, startX, startY);

            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if (selectedPiece != null)
        {
            if (endDrag == startDrag)
            {
                Move(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            if (selectedPiece.ValidMove(pieces, startX, startY, endX, endY))
            {
                //If this is a jump
                if (Mathf.Abs(startX - endX) == 2)
                {
                    Piece p = pieces[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null)
                    {
                        pieces[(startX + endX) / 2, (startY + endY) / 2] = null;
                        if (p.isWhite) whiteCount--;
                        else blackCount--;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                //Were we supposed to kill anything
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    Move(selectedPiece, startX, startY);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[endX, endY] = selectedPiece;
                pieces[startX, startY] = null;
                Move(selectedPiece, endX, endY);
                EndTurn();
            } 
            else
            {
                Move(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }

    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        //Transform to a King
        if(selectedPiece != null)
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            } 
        }

        selectedPiece = null;
        startDrag = Vector2.zero;

        if (GetPossibleMoves(selectedPiece, x, y).Count != 0 && hasKilled)
            return;

        isWhiteTurn = !isWhiteTurn;
        isWhite = !isWhite;
        hasKilled = false;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if (whiteCount == 0) Victory(false);
        if (blackCount == 0) Victory(true);
    }

    private void Victory(bool isWhiteLocal)
    {
        if (isWhiteLocal)
            endText.text = "White wons";
        else endText.text = "Black wons";
        endCanvas.SetActive(true);

        isStarted = false;
    }

    private List<Piece> GetPossibleMoves()
    {
        forcedPieces = new List<Piece>();

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsForcedToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);
        return forcedPieces;
    }

    private List<Piece> GetPossibleMoves(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();
        
        if (pieces[x, y].IsForcedToMove(pieces, x, y))
            forcedPieces.Add(pieces[x, y]);
      
        return forcedPieces;
    }
}
