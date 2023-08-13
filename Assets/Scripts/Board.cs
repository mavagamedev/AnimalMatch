using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private float sizeOffset;
    [SerializeField] private float verticalOffset;

    [SerializeField] private float sizeColumn, sizeRow;
    [SerializeField] private GameObject pieceObject;
    
    private void Start()
    {
        SetBoard();
        ResizeCamera();
    }

    private void SetBoard()
    {        
        var halfRow  = (sizeRow/2)-0.5f ;
        var halfColumn  = (sizeColumn/2)-0.5f ;
        
        for (var x = -halfColumn ; x <= halfColumn ; x++)
        {
            for (var y = -halfRow+verticalOffset ; y <= halfRow+verticalOffset ; y++)
            {
                var piece = Instantiate(pieceObject, new Vector2(x, y), Quaternion.identity);
                piece.transform.parent = transform;
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
