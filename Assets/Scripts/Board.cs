using System;
using System.Linq;
using System.Collections;
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
    [SerializeField] private float timeSetupPieces = 2.0f;
    
    [Header("Swap Pieces")] 
    public Piece startTile;
    public Piece endTile;
    private Piece[,] _pieces;
    public bool _swappingPieces;

    private void Start()
    {
        _pieces = new Piece[sizeColumn, sizeRow];
        SetupBoard();
        ConfigCamera();
        StartCoroutine(SetupPieces(true));
    }
    
    private void ConfigCamera()
    {
        // Position Camera
        var posCameraX = (sizeColumn / 2f)-0.5f;
        var posCameraY = (sizeRow / 2f)-0.5f;
        Camera.main!.transform.position = new Vector3(posCameraX,posCameraY,-10);
        
        // Resize Camera
        var verticalSize = (sizeRow / 2f)+2f;
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
    
    private IEnumerator SetupPieces(bool timeWait)
    {
        for (var x = 0 ; x < sizeColumn ; x++)
        {
            for (var y = 0 ; y < sizeRow ; y++)
            {
                yield return new WaitForSeconds(timeWait ? timeSetupPieces/(sizeColumn*sizeRow):0f);

                if (_pieces[x, y]) continue;
                CreatePieceAt(x,y);

                var currIntento = 0;
                const int maxAttempts = 5;

                while (HasPreviousMatches(x,y))
                {
                    if(currIntento>=maxAttempts) break;
                   
                    RemovePieceAt(_pieces[x,y]);
                    CreatePieceAt(x, y);
                    currIntento++;
                }
            }
        }
    }

    private bool HasPreviousMatches(int posX, int posY)
    {
        var leftMatchs = GetMatchByDirection(posX, posY, Vector2.left, 2);
        var downMatchs = GetMatchByDirection(posX, posY, Vector2.down, 2);
        
        return leftMatchs.Count > 0 || downMatchs.Count > 0;
    }

    private void CreatePieceAt(int posX, int posY)
    {
        var pieceObject =  animalPieces[Random.Range(0, animalPieces.Length)];
        var piece = Instantiate(
            pieceObject, new Vector2(posX, posY+2+verticalOffset), Quaternion.identity, transform);

        _pieces[posX, posY] = piece.GetComponent<Piece>();
        _pieces[posX, posY]?.SetInformation(posX,posY);
        _pieces[posX, posY].MovePiece(posX,posY);
    }

    private void ClearPieceAt(Piece piece)
    {
        piece.RemovePiece();
        _pieces[piece.posX, piece.posY] = null;
    }

    private void RemovePieceAt(Piece piece)
    {
        Destroy(piece.gameObject);
        _pieces[piece.posX, piece.posY] = null;
    }
    
    private bool IsPosibleMove()
    {
        var isPosible = (Math.Abs(startTile.posX - endTile.posX) == 1 && startTile.posY == endTile.posY) ||
                        (Math.Abs(startTile.posY - endTile.posY) == 1 && startTile.posX == endTile.posX);
        
        return isPosible;
    }

    public IEnumerator SwapPieces()
    {
        if (!startTile || !endTile || !IsPosibleMove() || _swappingPieces) yield break;
        _swappingPieces = true;

        var startPiece = _pieces[startTile.posX, startTile.posY];
        var endPiece = _pieces[endTile.posX, endTile.posY];
            
        startPiece.MovePiece(endTile.posX, endTile.posY);
        endPiece.MovePiece(startTile.posX, startTile.posY);
            
        _pieces[startTile.posX, startTile.posY] = endPiece;
        _pieces[endTile.posX, endTile.posY] = startPiece;
        
        yield return new WaitForSeconds(0.6f);

        var startPieceMatchs = GetMatchByPiece(startPiece.posX, startPiece.posY);
        var endPieceMatchs = GetMatchByPiece(endPiece.posX, endPiece.posY);
            
        var allMatches = startPieceMatchs.Union(endPieceMatchs).ToList();
        
        if (allMatches.Count>0)
        {    
            StartCoroutine(ClearPieces(allMatches));
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

    private IEnumerator ClearPieces(List<Piece> piecesToClear)
    {
        piecesToClear.ForEach(ClearPieceAt);
        var columnsMatches = GetColumnsMatches(piecesToClear);
        yield return new WaitForSeconds(0.4f);
        
        var collapsedPieces = CollapseColumns(columnsMatches);
        StartCoroutine(FindMatchsRecursively(collapsedPieces));
    }

    private IEnumerator FindMatchsRecursively(List<Piece> collapsedPieces)
    {
        yield return new WaitForSeconds(1f);

        var newMatches = new List<Piece>();
        collapsedPieces.ForEach(piece =>
        {
            var pieceMatchs = GetMatchByPiece(piece.posX, piece.posY);
            
            if (pieceMatchs.Count <= 0) return;
            newMatches = newMatches.Union(pieceMatchs).ToList();
            StartCoroutine(ClearPieces(pieceMatchs));
        });
        
        if (newMatches.Count>0)
        {
            var newCollapsedPieces = CollapseColumns(GetColumnsMatches(newMatches));
            yield return FindMatchsRecursively(newCollapsedPieces);
        }
        else
        {
            StartCoroutine(SetupPieces(false));
        }
        yield return null;
    }

    private List<int> GetColumnsMatches(List<Piece> piecesOfColumns)
    {
        var columns = new List<int>();
        piecesOfColumns.ForEach(piece =>
        {
            if (columns.Contains(piece.posX)) return;
            columns.Add(piece.posX);
        });
        return columns;
    }

    private List<Piece> CollapseColumns(List<int> columns)
    {
        var movingPieces = new List<Piece>();
        foreach (var posColumn in columns)
        {
            for (var posRow = 0; posRow < sizeRow; posRow++)
            {
                if (_pieces[posColumn, posRow]) continue;
                
                for (var posRowNext = posRow+1; posRowNext < sizeRow; posRowNext++)
                {
                    if (!_pieces[posColumn, posRowNext]) continue;

                    _pieces[posColumn, posRowNext].MovePiece(posColumn,posRow);
                    _pieces[posColumn, posRow] = _pieces[posColumn, posRowNext];
                    _pieces[posColumn, posRowNext] = null;
                    
                    if (!movingPieces.Contains(_pieces[posColumn, posRow]))
                    {
                        movingPieces.Add(_pieces[posColumn,posRow]);
                    }
                    break;
                }
            }
        }
        return movingPieces;
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
            if (nextPiece && nextPiece.animalType == startPieceMatch.animalType) matches.Add(nextPiece);
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
