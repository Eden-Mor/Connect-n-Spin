using System.Linq;
using System.Text;
using UnityEngine;

public static class MatrixManipulator
{
    public static bool?[,] RotateMatrix(bool?[,] matrix, bool clockwise = true)
    {
        int n = matrix.GetLength(0);
        var rotatedMatrix = new bool?[n, n];

        Enumerable.Range(0, n).ToList().ForEach(i =>
            Enumerable.Range(0, n).ToList().ForEach(j =>
            {
                if (clockwise)
                    rotatedMatrix[j, n - 1 - i] = matrix[i, j]; // 90 degrees clockwise
                else
                    rotatedMatrix[n - 1 - j, i] = matrix[i, j]; // 90 degrees counterclockwise
            })
        );

        ApplyGravity(rotatedMatrix);

        return rotatedMatrix;
    }

    public static void FlipBoard(bool?[,] matrix)
    {
        int n = matrix.GetLength(0);

        for (int col = 0; col < n; col++)
        {
            var columnValues = Enumerable.Range(0, n)
                                         .Select(row => matrix[col, row])
                                         .Reverse()
                                         .ToArray();

            for (int row = 0; row < n; row++)
            {
                matrix[col, row] = columnValues[row];
            }
        }

        ApplyGravity(matrix);
    }

    public static bool?[,] Rotate180(bool?[,] matrix)
    {
        int n = matrix.GetLength(0);
        var rotatedMatrix = new bool?[n, n];

        for (int col = 0; col < n; col++)
        {
            var columnValues = Enumerable.Range(0, n)
                                         .Select(row => matrix[col, row])
                                         .Reverse()
                                         .ToArray();

            for (int row = 0; row < n; row++)
            {
                rotatedMatrix[n - col - 1, row] = columnValues[row];
            }
        }

        ApplyGravity(rotatedMatrix);

        return rotatedMatrix;
    }

    public static void ApplyGravity(bool?[,] matrix)
    {
        int n = matrix.GetLength(0);

        for (int col = 0; col < n; col++)
        {
            var tokens = Enumerable.Range(0, n)
                                   .Select(row => matrix[col, row])
                                   .Where(val => val.HasValue)
                                   .ToList();

            for (int row = n - 1; row >= 0; row--)
                matrix[col, row] = row < tokens.Count 
                                    ? tokens[row] 
                                    : null;
        }
    }

    public static void PrintMatrix(bool?[,] matrix)
    {
        StringBuilder sb = new StringBuilder();
        int n = matrix.GetLength(0);

        for (int row = n - 1; row >= 0; row--)
        {
            for (int col = 0; col < n; col++)
                sb.Append(matrix[col, row] != null ? matrix[col, row].Value ? 'T' : 'F' : '-' + " ");

            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

}
