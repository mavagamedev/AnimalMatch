using UnityEngine;

public class Piece : MonoBehaviour
{
    public float positionX, positionY;
    public Animal animalPiece;
    private Board _board;
    
    public enum Animal
    {
        elephant, giraffe, hippo, monkey, panda, parrot, penguin, pig, rabbit, snake
    }

    public void SetInformation(float x, float y, Board board)
    {
        positionX = x;
        positionY = y;
        _board = board;
    }
}