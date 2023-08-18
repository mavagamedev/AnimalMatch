using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [Header("Config Camera Offset")]
    [SerializeField] private float sizeOffset;
    [SerializeField] private float verticalOffset;

    [Header("Config Board")]
    [SerializeField] private int sizeColumn;
    [SerializeField] private int sizeRow;
    [SerializeField] private GameObject emptyObject;
    [SerializeField] private GameObject[] animalPieces;
    
    [Header("Swap Pieces")] 
    public Piece startTile;
    public Piece endTile;
    private Piece[,] _pieces;
    private bool _swappingPieces;

    private void Start()
    {
        _pieces = new Piece[sizeColumn, sizeRow];
        SetupBoard();
        ConfigCamera();
        SetupPieces();
    }
    
    private void ConfigCamera()
    {
        // Position Camera
        var posCameraX = (sizeColumn / 2f)-0.5f;
        var posCameraY = (sizeRow / 2f)-0.5f;
        Camera.main!.transform.position = new Vector3(posCameraX,posCameraY,-10);
        
        // Resize Camera
        var verticalSize = (sizeRow / 2f)+0.5f;
        Camera.main!.orthographicSize =
            (sizeColumn > verticalSize ? sizeColumn : verticalSize)+sizeOffset;
    }

    private void SetupBoard()
    {
        for (var x = 0 ; x < sizeColumn ; x++)
        {
            for (var y = 0 ; y < sizeRow ; y++)
            {
                Instantiate(emptyObject,new Vector2(x, y+verticalOffset),Quaternion.identity,transform);
            }
        }
    }
    private void SetupPieces()
    {
        for (var x = 0 ; x < sizeColumn ; x++)
        {
            for (var y = 0 ; y < sizeRow ; y++)
            {
               CreatePieceAt(x,y);
               
               var currIntento = 0;
               const int maxIntentos = 5;
               while (HasPreviousMatches(x,y))
               {
                   if(currIntento>maxIntentos) break;
                   
                   ClearPieceAt(_pieces[x,y]);
                   CreatePieceAt(x, y);
                   currIntento++;
               }
            }
        }
    }

    private void CreatePieceAt(int posX, int posY)
    {
        var pieceObject =  animalPieces[Random.Range(0, animalPieces.Length)];
        var piece = Instantiate(
            pieceObject, new Vector2(posX, posY+verticalOffset), Quaternion.identity, transform);

        _pieces[posX, posY] = piece.GetComponent<Piece>();
        _pieces[posX, posY]?.SetInformation(posX,posY);
    }

    private void ClearPieceAt(Piece piece)
    {
        Destroy(piece.gameObject);
        _pieces[piece.posX, piece.posY] = null;
    }
    
    private bool HasPreviousMatches(int posX, int posY)
    {
        var leftMatchs = GetMatchByDirection(posX, posY, Vector2.left,2);
        var downMatchs = GetMatchByDirection(posX, posY, Vector2.down,2);
        return leftMatchs.Count > 0 || downMatchs.Count > 0;
    }

    public IEnumerator SwapPieces()
    {
        if (!startTile || !endTile || !IsPosibleMove() || _swappingPieces) yield break;

        var startPiece = _pieces[startTile.posX, startTile.posY];
        var endPiece = _pieces[endTile.posX, endTile.posY];
            
        startPiece.MovePiece(endTile.posX, endTile.posY);
        endPiece.MovePiece(startTile.posX, startTile.posY);
            
        _pieces[startTile.posX, startTile.posY] = endPiece;
        _pieces[endTile.posX, endTile.posY] = startPiece;
        
        yield return new WaitForSeconds(0.6f);

        var startPieceMatchs = GetMatchByPiece(startPiece.posX, startPiece.posY);
        var endPieceMatchs = GetMatchByPiece(endPiece.posX, endPiece.posY);
            
        var foundMatch = startPieceMatchs.Count>0 || endPieceMatchs.Count>0;
        
        if (foundMatch)
        {    
            startPieceMatchs.ForEach(ClearPieceAt);
            endPieceMatchs.ForEach(ClearPieceAt);
        }
        else
        {
            startPiece.MovePiece(endTile.posX, endTile.posY);
            endPiece.MovePiece(startTile.posX, startTile.posY);
            
            _pieces[endTile.posX, endTile.posY] = startPiece;
            _pieces[startTile.posX, startTile.posY] = endPiece;
        }
        
        startTile = null;
        endTile = null;
        _swappingPieces = false;
        
        yield return null; 
    }

    private bool IsPosibleMove()
    {
        var isPosible = (Math.Abs(startTile.posX - endTile.posX) == 1 && startTile.posY == endTile.posY) ||
                        (Math.Abs(startTile.posY - endTile.posY) == 1 && startTile.posX == endTile.posX);
        
        return isPosible;
    }

    private List<Piece> GetMatchByDirection(int piecePosX, int piecePosY, Vector2 direction, int minPieces=3)
    {
        var matches = new List<Piece>();
        var startPieceMatch = _pieces[piecePosX, piecePosY];
        matches.Add(startPieceMatch);

        var maxValue = sizeColumn > sizeRow ? sizeColumn : sizeRow;

        for (var i = 1; i < maxValue; i++)
        {
            var nextX = piecePosX + ((int)direction.x * i);
            var nextY = piecePosY + ((int)direction.y * i);
            if (nextX < 0 || nextX >= sizeColumn || nextY < 0 || nextY >= sizeRow) continue;
            
            var nextPiece = _pieces[nextX, nextY];
            if (nextPiece && nextPiece.animalType == startPieceMatch.animalType)
            {
                print($"MATCH: {startPieceMatch.animalType}[{piecePosX}, {piecePosY}] --- {nextPiece.animalType}[{nextX}, {nextY}]");
                matches.Add(nextPiece);
            }
            else break;
        }

        return matches.Count >= minPieces ? matches : new List<Piece>();
    }

    private List<Piece> GetMatchByPiece(int posX, int posY, int minPieces = 3)
    {
        var upMatchs = GetMatchByDirection(posX, posY, Vector2.up, 2);
        var downMatchs = GetMatchByDirection(posX, posY, Vector2.down, 2);
        var rightMatchs = GetMatchByDirection(posX, posY, Vector2.right, 2);
        var leftMatchs = GetMatchByDirection(posX, posY, Vector2.left, 2);

        var verticalMatchs = upMatchs.Union(downMatchs).ToList();
        var horizontalMatchs = leftMatchs.Union(rightMatchs).ToList();

        var foundMatches = new List<Piece>();

        if (verticalMatchs.Count >= minPieces) foundMatches = foundMatches.Union(verticalMatchs).ToList();
        if (horizontalMatchs.Count >= minPieces) foundMatches = foundMatches.Union(horizontalMatchs).ToList();
        
        return foundMatches;
    }
}
