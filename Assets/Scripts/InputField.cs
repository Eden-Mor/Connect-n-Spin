using UnityEngine;

public class InputField : MonoBehaviour
{
    public int column;

    public GameManager gameManager;

    void OnMouseDown()
    {
        gameManager.SelectColumn(column);
    }
}
