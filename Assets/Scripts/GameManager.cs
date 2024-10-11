using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int BOARD_SIZE = 7;

    [SerializeField] private GameObject player1;
    [SerializeField] private TextMeshProUGUI player1Score;
    [SerializeField] private GameObject player2;
    [SerializeField] private TextMeshProUGUI player2Score;

    [SerializeField] private Renderer currentPlayerTurn;

    private bool isBoardRotating = false;
    private bool isGameOver = false;


    [SerializeField] private GameObject[] spawnLocations;
    [SerializeField] private RigidBodyChildManager anchorLocation;
    [SerializeField] private SmoothRotation boardRotationScript;

    private bool player1Turn = true;
    private bool?[,] boardState;

    [SerializeField]
    private int player1Wins = 0;
    [SerializeField]
    private int player2Wins = 0;

    //TODO
    //IF LAST FIVE MOVES WERE TURNING THE TABLE, COUNT IT AS A TIE, BONUS, CHECK IF ADDING A PIECE ANYWHERE FOR EITHER SIDE WOULD CAUSE ONE PLAYER TO WIN, AND ONLY COUNT THAT AS A POINT FOR WHETHER A TIE SHOULD HAPPEN

    private void Start()
    {
        boardState = new bool?[BOARD_SIZE, BOARD_SIZE];
    }

    public void ResetBoard()
    {
        boardState = new bool?[BOARD_SIZE, BOARD_SIZE];
        anchorLocation.DeleteChildren();
        isGameOver = false;
        ResetScores();
        UpdateScore();
    }

    public void SelectColumn(int column) 
        => TakeTurn(column);

    public void RotateBoard(bool clockwise) 
        => RotateBoardHandler(clockwise ? RotationOptions.Clockwise : RotationOptions.CounterClockwise);

    public void Rotate180() 
        => RotateBoardHandler(RotationOptions.DoubleClockwise);

    private void RotateBoardHandler(RotationOptions rotation)
    {
        if (!IsAllowedTurn())
            return;

        isBoardRotating = true;

        ChangeTurn();

        anchorLocation.SetChildState(false);

        int degrees = 0;
        switch (rotation)
        {
            case RotationOptions.DoubleClockwise:
                degrees = 180;
                boardState = MatrixManipulator.Rotate180(boardState);
                break;
            case RotationOptions.Clockwise:
                degrees = 90;
                boardState = MatrixManipulator.RotateMatrix(boardState, true);
                break;
            case RotationOptions.CounterClockwise:
                degrees = -90;
                boardState = MatrixManipulator.RotateMatrix(boardState, false);
                break;
        }

        boardRotationScript.RotateDegrees(degrees, onRotationComplete: () =>
        {
            anchorLocation.RotateChildren(-degrees);

            anchorLocation.SetChildState(true);

            CountFourInRowColumnOrDiagonal();

            isBoardRotating = false;
        });
    }

    private void ChangeTurn()
    {
        player1Turn = !player1Turn;
        currentPlayerTurn.material = (player1Turn ? player1 : player2).GetComponent<SpriteRenderer>().sharedMaterial;
    }

    private bool IsAllowedTurn(bool ignoreFallingPiece = false)
    {
        var isAllowed = !isGameOver && !isBoardRotating;

        if (ignoreFallingPiece || !isAllowed)
            return isAllowed;

        foreach (Transform trans in anchorLocation.transform)
            if (trans.TryGetComponent<Rigidbody2D>(out var piece) && piece.velocity != Vector2.zero)
                return false;

        return isAllowed;
    }

    private void TakeTurn(int column)
    {
        if (!IsAllowedTurn(true) || !UpdateBoardState(column))
            return; //INVALID PLACEMENT OR PIECE IS CURRENTLY IN AIR

        Instantiate(player1Turn ? player1 : player2, spawnLocations[column].transform.position, Quaternion.identity, anchorLocation.transform);
        ChangeTurn();

        var isBoardFilled = true;
        for (int col = 0; col < BOARD_SIZE; col++)
        {
            if (boardState[col, BOARD_SIZE - 1] == null)
            {
                isBoardFilled = false;
                break;
            }
        }

        CountFourInRowColumnOrDiagonal();

        if (player1Wins == 0 && player2Wins == 0)
            return;

        var isTie = player1Wins == player2Wins;

        if (isBoardFilled && isTie)
        {
            //TIE AND GAME OVER
            isGameOver = true;
            //Display game tie and over

            return;
        }

        if (isTie)
        {
            //Game tie but no winner just yet, theres more places on the board you may fill with pieces
        }
        else
        {
            //We have a singular winner!
            var winnerWinCount = Mathf.Max(player1Wins, player2Wins);
            var isPlayer1Win = winnerWinCount == player1Wins;
        }
    }

    private bool UpdateBoardState(int column)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            if (boardState[column, i] == null)
            {
                boardState[column, i] = player1Turn;
                return true;
            }
        }
        return false;
    }

    private void CountFourInRowColumnOrDiagonal()
    {
        ResetScores();

        HasFourInRow();
        HasFourInColumn();
        HasFourInDiagonal();

        UpdateScore();

        MatrixManipulator.PrintMatrix(boardState);
    }

    private void ResetScores()
    {
        player1Wins = 0;
        player2Wins = 0;
    }

    private void UpdateScore()
    {
        player1Score.text = player1Wins.ToString();
        player2Score.text = player2Wins.ToString();
    }


    private void HasFourInRow()
    {
        Enumerable.Range(0, BOARD_SIZE).ToList().ForEach(row =>
        {
            Enumerable.Range(0, BOARD_SIZE - 3).ToList().ForEach(col =>
            {
                bool? firstVal = boardState[row, col];
                if (firstVal != null && Enumerable.Range(0, 4).All(offset => boardState[row, col + offset] == firstVal))
                {
                    if (firstVal == true) // Player 1
                        player1Wins++;
                    else // Player 2
                        player2Wins++;
                }
            });
        });
    }

    private void HasFourInColumn()
    {
        Enumerable.Range(0, BOARD_SIZE).ToList().ForEach(col =>
        {
            Enumerable.Range(0, BOARD_SIZE - 3).ToList().ForEach(row =>
            {
                bool? firstVal = boardState[row, col];
                if (firstVal != null && Enumerable.Range(0, 4).All(offset => boardState[row + offset, col] == firstVal))
                {
                    if (firstVal == true) // Player 1
                        player1Wins++;
                    else // Player 2
                        player2Wins++;
                }
            });
        });
    }

    private void HasFourInDiagonal()
    {
        // Check downward diagonal (\) and upward diagonal (/)
        Enumerable.Range(0, BOARD_SIZE).ToList().ForEach(row =>
        {
            Enumerable.Range(0, BOARD_SIZE).ToList().ForEach(col =>
            {
                bool? firstVal = boardState[row, col];
                if (firstVal == null)
                    return;

                // Check downward diagonal (\ direction)
                if (row + 3 < BOARD_SIZE && col + 3 < BOARD_SIZE)
                {
                    bool downward = Enumerable.Range(0, 4).All(offset => boardState[row + offset, col + offset] == firstVal);
                    if (downward)
                    {
                        if (firstVal == true)
                            player1Wins++;
                        else
                            player2Wins++;
                    }
                }

                // Check upward diagonal (/ direction)
                if (row - 3 >= 0 && col + 3 < BOARD_SIZE)
                {
                    bool upward = Enumerable.Range(0, 4).All(offset => boardState[row - offset, col + offset] == firstVal);
                    if (upward)
                    {
                        if (firstVal == true)
                            player1Wins++;
                        else
                            player2Wins++;
                    }
                }
            });
        });
    }

}


public enum RotationOptions
{
    Clockwise = 0,
    CounterClockwise = 1,
    DoubleClockwise = 2,
}