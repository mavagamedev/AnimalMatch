using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    private Board _board;

    [Header("Piece information")]
    public int posX, posY;
    public Animal animalType;
    
    public enum Animal
    {
        Elephant, Giraffe, Hippo, Monkey, Panda, Parrot, Penguin, Pig, Rabbit, Snake
    }

    private void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    public void SetInformation(int positionX, int positionY)
    {
        posX = positionX;
        posY = positionY;
    }

    public void MovePiece(int desX, int desY)
    {
        transform.DOMove(new Vector3(desX, desY, transform.position.z), 0.25f)
            .SetEase(Ease.InOutCubic).onComplete = () =>
        {
            posX = desX;
            posY = desY;
        };
    }

    public void RemovePiece()
    {
        transform.DORotate(new Vector3(0, 0, -120f), 0.4f);
        transform.DOScale(Vector3.one * 1.25f, 0.2f).onComplete = 
            () => transform.DOScale(Vector3.zero, 0.2f).onComplete = 
                () => Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if(!_board._swappingPieces) _board.startTile = this;
    }

    private void OnMouseOver()
    {
        if(!_board._swappingPieces) _board.endTile = this;
    }

    private void OnMouseUp()
    {
        StartCoroutine(_board.SwapPieces());
    }
}