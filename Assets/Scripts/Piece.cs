using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    private Board _board;

    [Header("Piece information")]
    public int positionX, positionY;
    public Animal animalPiece;
    
    public enum Animal
    {
        Elephant, Giraffe, Hippo, Monkey, Panda, Parrot, Penguin, Pig, Rabbit, Snake
    }

    private void Start()
    {
        _board = FindObjectOfType<Board>();
    }

    public void SetInformation(int posX, int posY)
    {
        positionX = posX;
        positionY = posY;
    }

    public void MovePiece(int desX, int desY)
    {
        transform.DOMove(new Vector3(desX, desY, transform.position.z), 0.25f)
            .SetEase(Ease.InOutCubic).onComplete = () =>
        {
            positionX = desX;
            positionY = desY;
        };
    }

    private void OnMouseDown()
    {
        _board.startPiece = this;
    }

    private void OnMouseOver()
    {
        _board.endPiece = this;
    }

    private void OnMouseUp()
    {
        _board.SwapPieces();
    }
}