using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject winPopup;
    public Camera mainCamera;
    public Camera playerCamera;
    public Transform player;
    public int width = 21;
    public int height = 21;

    private int[,] mazeGrid;  // 0 = path, 1 = wall
    private Stack<Vector2Int> stack;  // Stack untuk DFS traversal
    private List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, 2),  // UP
        new Vector2Int(2, 0),  // RIGHT
        new Vector2Int(0, -2), // DOWN
        new Vector2Int(-2, 0)  // LEFT
    };

    void Start()
    {
        GenerateMaze();
        playerCamera.enabled = false;
    }

    void GenerateMaze()
    {
        mazeGrid = new int[width, height];

        // Inisialisasi walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mazeGrid[x, y] = 1; // 1 = wall

                Vector3 position = new Vector3(x, 0, y);
                Instantiate(wallPrefab, position, Quaternion.identity).name = $"{x},{y}";
            }
        }

        StartCoroutine(GenerateMazeDFS());
    }

    IEnumerator GenerateMazeDFS()
    {
        stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);                // bottom-left
        Vector2Int end = new Vector2Int(width - 2, height - 2); // top-right

        stack.Push(start);
        mazeGrid[start.x, start.y] = 0; // clear start/entrance, 0 = path
        DestroyWall(start);

        // DFS LOGIC
        while (stack.Count > 0)
        {
            // 1. 1st current -> start (1, 1), get unvisited neighbours: (1, 2), (2, 1).
            // 2. dari unvisited neighbours yang ditemukan, choose at random.
            Vector2Int current = stack.Peek();
            List<Vector2Int> unvisitedNeighbours = GetUnvisitedNeighbours(current);

            if (unvisitedNeighbours.Count > 0)
            {
                Vector2Int chosenNeighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];

                // 3. Remove wall antara current & chosenNeighbour.
                // Contoh: wallPosition = (1, 1) + ((2, 1) - (1, 1)) / 2
                //                      = (1, 1) + ((1,0) /2) = (1.5, 1)
                // -> DestroyWall((1,5 1))
                Vector2Int wallPosition = current + (chosenNeighbour - current) / 2;
                mazeGrid[wallPosition.x, wallPosition.y] = 0;
                DestroyWall(wallPosition);

                mazeGrid[chosenNeighbour.x, chosenNeighbour.y] = 0;
                DestroyWall(chosenNeighbour);
                stack.Push(chosenNeighbour);
            }
            else
            {
                stack.Pop();
            }

            yield return null;
        }

        DestroyWall(new Vector2Int(1, 0)); // Clear entrance
        DestroyWall(new Vector2Int(width - 2, height - 1)); // Clear exit
                                                            // Clear the exit
        mazeGrid[width-2, height-1] = 0;
        DestroyWall(new Vector2Int(width - 2, height - 1));


        Debug.Log("Maze generation complete! Game started.");

        mainCamera.enabled = false;
        playerCamera.enabled = true;
    }

    List<Vector2Int> GetUnvisitedNeighbours(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbour = cell + direction;

            if (neighbour.x > 0 && neighbour.x < width - 1 &&
                neighbour.y > 0 && neighbour.y < height - 1 &&
                mazeGrid[neighbour.x, neighbour.y] == 1)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    void DestroyWall(Vector2Int position)
    {
        GameObject wall = GameObject.Find($"{position.x},{position.y}");
        if (wall != null)
        {
            Destroy(wall);
        }
    }

    public bool IsCellOpen(Vector2Int position)
    {
        // Check bounds
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
        {
            return false; // Out of bounds
        }

        // Special case: Exit position
        if (position == new Vector2Int(19, 20))
        {
            return mazeGrid[position.x, position.y] == 0;
        }

        // Normal case: Check the maze grid
        return mazeGrid[position.x, position.y] == 0; // Path is open
    }

}
