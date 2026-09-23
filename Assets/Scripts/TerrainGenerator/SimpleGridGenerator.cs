using UnityEngine;

public class SimpleGridGenerator : MonoBehaviour
{
    private const int RotationSteps = 1;
    private const int AnyRotation = -1;

    [SerializeField]
    private GameObject[] tiles;

    [SerializeField]
    private int gridSize = 100;

    [SerializeField]
    private float tileSize = 30f;

    [SerializeField, Min(0)]
    [Tooltip("How many tiles to look back along the row and the column when checking for repeats. " +
             "A tile that repeats inside that range is only allowed with a rotation it has not used there.")]
    private int noRepeatDistance = 2;

    private int[,] placedTiles;
    private int[,] placedRotations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError($"{nameof(SimpleGridGenerator)} has no tiles assigned.", this);
            return;
        }

        int halfGridSize = gridSize / 2;

        for (int column = -halfGridSize; column < halfGridSize; column++)
        {
            for (int row = -halfGridSize; row < halfGridSize; row++)
            {
                // Shuffling both orders keeps the choice random while the checks below stay a simple scan.
                //    Shuffle(tileOrder);
                //   Shuffle(rotationOrder);
                //   Pick(column, row, tileOrder, rotationOrder, out int tileIndex, out int rotationIndex);

                //   placedTiles[column, row] = tileIndex;
                //   placedRotations[column, row] = rotationIndex;

                int tileIndex = Random.Range(0, tiles.Length);

                Vector3 position = new Vector3((column) * tileSize, 0, (row) * tileSize);
                GameObject tile = Instantiate(tiles[tileIndex], position, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(20, 20, 20);
            }
        }

        return;

        // Each cell is checked against noRepeatDistance cells back along its row and its column.
        int neighboursInRange = noRepeatDistance * 2;

        if (neighboursInRange > tiles.Length * RotationSteps)
        {
            Debug.LogWarning($"{nameof(SimpleGridGenerator)} cannot always keep {neighboursInRange} neighbouring tiles apart with " +
                             $"{tiles.Length} tile(s) and {RotationSteps} rotations. Lower {nameof(noRepeatDistance)} or add more tiles.", this);
        }

        int cellCount = gridSize / 2 * 2;
        int halfCount = cellCount / 2;

        placedTiles = new int[cellCount, cellCount];
        placedRotations = new int[cellCount, cellCount];

        int[] tileOrder = CreateOrder(tiles.Length);
        int[] rotationOrder = CreateOrder(RotationSteps);

        for (int column = 0; column < cellCount; column++)
        {
            for (int row = 0; row < cellCount; row++)
            {
                // Shuffling both orders keeps the choice random while the checks below stay a simple scan.
            //    Shuffle(tileOrder);
             //   Shuffle(rotationOrder);
             //   Pick(column, row, tileOrder, rotationOrder, out int tileIndex, out int rotationIndex);
             
             //   placedTiles[column, row] = tileIndex;
             //   placedRotations[column, row] = rotationIndex;

                int tileIndex = Random.Range(0, tiles.Length);

                Vector3 position = new Vector3((column) * tileSize, 0, (row) * tileSize);
                GameObject tile = Instantiate(tiles[tileIndex], position, Quaternion.identity, transform);
                tile.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                tile.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    /// <summary>
    /// Picks the tile and rotation for a cell, preferring a tile that does not repeat along its
    /// row or column, and falling back to a repeat that at least spins differently.
    /// </summary>
    private void Pick(int column, int row, int[] tileOrder, int[] rotationOrder, out int tileIndex, out int rotationIndex)
    {
        foreach (int candidateTile in tileOrder)
        {
            if (IsUsedNearby(column, row, candidateTile, AnyRotation) == false)
            {
                tileIndex = candidateTile;
                rotationIndex = rotationOrder[0];
                return;
            }
        }

        // Every tile already shows up in range, so repeat one with a rotation it has not used there.
        foreach (int candidateTile in tileOrder)
        {
            foreach (int candidateRotation in rotationOrder)
            {
                if (IsUsedNearby(column, row, candidateTile, candidateRotation) == false)
                {
                    tileIndex = candidateTile;
                    rotationIndex = candidateRotation;
                    return;
                }
            }
        }

        // More tiles in range than there are tile and rotation combinations, so a repeat is unavoidable.
        tileIndex = tileOrder[0];
        rotationIndex = rotationOrder[0];
    }

    /// <summary>
    /// Checks the already placed cells within <see cref="noRepeatDistance"/> along the same row and
    /// column. Pass <see cref="AnyRotation"/> to match the tile whatever its rotation is.
    /// </summary>
    private bool IsUsedNearby(int column, int row, int tileIndex, int rotationIndex)
    {
        for (int distance = 1; distance <= noRepeatDistance; distance++)
        {
            if (Matches(column - distance, row, tileIndex, rotationIndex))
            {
                return true;
            }

            if (Matches(column, row - distance, tileIndex, rotationIndex))
            {
                return true;
            }
        }

        return false;
    }

    private bool Matches(int column, int row, int tileIndex, int rotationIndex)
    {
        if (column < 0 || row < 0)
        {
            return false;
        }

        if (placedTiles[column, row] != tileIndex)
        {
            return false;
        }

        return rotationIndex == AnyRotation || placedRotations[column, row] == rotationIndex;
    }

    private static int[] CreateOrder(int length)
    {
        int[] order = new int[length];

        for (int i = 0; i < length; i++)
        {
            order[i] = i;
        }

        return order;
    }

    private static void Shuffle(int[] order)
    {
        for (int i = order.Length - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            (order[i], order[swapIndex]) = (order[swapIndex], order[i]);
        }
    }
}
