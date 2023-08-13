using UnityEngine;

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
    public Piece startPiece;
    public Piece endPiece;
    private Piece[,] _pieces;

    private void Start()
    {
        _pieces = new Piece[sizeColumn, sizeRow];
        TravelBoard(true);
        ConfigCamera();
        TravelBoard();
    }

    private void TravelBoard(bool isEmpty=false)
    {
        for (var x = 0 ; x < sizeColumn ; x++)
        {
            for (var y = 0 ; y < sizeRow ; y++)
            {
                var pieceObject = isEmpty ? emptyObject : animalPieces[Random.Range(0, animalPieces.Length)];
                
                var piece = Instantiate(pieceObject, new Vector2(x, y+verticalOffset), Quaternion.identity);
                piece.transform.parent = transform;
                
                if (isEmpty) continue;
                _pieces[x, y] = piece.GetComponent<Piece>();
                _pieces[x, y]?.SetInformation(x,y);
            }
        }
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

    public void SwapPieces()
    {
        if (startPiece && endPiece)
        {
            _pieces[startPiece.positionX, startPiece.positionY] = endPiece;
            _pieces[endPiece.positionX, endPiece.positionY] = startPiece;
            
            var firstPiece = startPiece;
            
            startPiece.MovePiece(endPiece.positionX, endPiece.positionY);
            endPiece.MovePiece(firstPiece.positionX, firstPiece.positionY);
        }
        startPiece = null;
        endPiece = null;
    }
}
