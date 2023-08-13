using JetBrains.Annotations;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Config Camera Offset")]
    [SerializeField] private float sizeOffset;
    [SerializeField] private float verticalOffset;

    [Header("Config Board")]
    [SerializeField] private float sizeColumn, sizeRow;
    [SerializeField] private GameObject emptyObject;
    [SerializeField] private GameObject[] animalPieces;
    
    private void Start()
    {
        TravelBoard(true);
        ResizeCamera();
        TravelBoard();
    }

    private void TravelBoard(bool isEmpty=false)
    {
        var halfRow  = (sizeRow/2)-0.5f ;
        var halfColumn  = (sizeColumn/2)-0.5f ;

        for (var x = -halfColumn ; x <= halfColumn ; x++)
        {
            for (var y = -halfRow+verticalOffset ; y <= halfRow+verticalOffset ; y++)
            {
                var pieceObject = isEmpty ? emptyObject : animalPieces[Random.Range(0, animalPieces.Length)];
                var piece = Instantiate(pieceObject, new Vector2(x, y), Quaternion.identity);
                piece.transform.parent = transform;
                piece.GetComponent<Piece>()?.SetInformation(x+2.5f,y+2.5f,this);
            }
        }
    }

    private void ResizeCamera()
    {
        var verticalSize = (sizeRow / 2)+0.5f;
        Camera.main!.orthographicSize =
            (sizeColumn > verticalSize ? sizeColumn : verticalSize)+sizeOffset;
    }
}
