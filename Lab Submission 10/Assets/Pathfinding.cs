using System.Collections.Generic;
using TMPro;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int start = new Vector2Int(0, 1);
    private Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int next;
    private Vector2Int current;

    [Header("Generation Settings")]
    public Slider obstacleSlider;
    public int width;
    public int height;

    [Header("Start and End Input")]
    public TMP_InputField startX;
    public TMP_InputField startY;
    public TMP_InputField goalX;
    public TMP_InputField goalY;

    [Header("Obstacle Input")]
    public TMP_InputField obstacleX;
    public TMP_InputField obstacleY;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int[,] grid = new int[,]
    {
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 }
    };

    private void Start()
    {
        // Generate our grid!
        grid = GenerateRandomGrid(width, height, obstacleSlider.value);

        FindPath(start, goal);
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        path.Clear();

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }

    // Allows for us to trigger this with a button.
    public void GenerateRandomGrid()
    {
        grid = GenerateRandomGrid(width, height, obstacleSlider.value);

        FindPath(start, goal);
    }

    // Randomly populate the grid with obstacles based on a probability factor.
    private int[,] GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        int[,] newGrid = new int[width, height];

        // Determine if an obstacle should generate for each location.
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                // If we are the start or goal, do not try to place any obstacle here!
                if(new Vector2Int(j, i) == start || new Vector2Int(j, i) == goal)
                {
                    continue;
                }    

                int index = Random.Range(0, 100);

                // If our index is below the probability, there is an obstacle here!
                if(index < obstacleProbability)
                {
                    newGrid[i, j] = 1;
                }
                else
                {
                    newGrid[i, j] = 0;
                }
            }
        }

        return newGrid;
    }

    // Get a vector2int from two input fields.
    private Vector2Int InputFieldToVector2(TMP_InputField inputX, TMP_InputField inputY)
    {
        // Try to get ints out of the input fields.
        int x, y;
        int.TryParse(inputX.text, out x);
        int.TryParse(inputY.text, out y);

        return new Vector2Int(x, y);
    }

    // Allows us to interact with this with a button.
    public void AddObstacle()
    {
        AddObstacle(InputFieldToVector2(obstacleX, obstacleY));
    }

    // Add an obstacle at this location.
    private void AddObstacle(Vector2Int position)
    {
        // Make sure this position is in bounds.
        if(IsInBounds(position))
        {
            grid[position.x, position.y] = 1;

            // Find our path.
            FindPath(start, goal);
            return;
        }

        Debug.LogWarning("Obstacle position is out of bounds!"); 
    }

    // Allows us to interact with this with a button.
    public void SetStartPosition(){
        SetStartPosition(InputFieldToVector2(startX, startY));
    }

    // Set our start position.
    private void SetStartPosition(Vector2Int position){
        // Make sure this position is in bounds.
        if(IsInBounds(position))
        {
            // If this location is an obstacle, remove it.
            if(grid[position.x, position.y] == 1)
            {
                grid[position.x, position.y] = 0;
            }

            // Set our start position.
            start = new Vector2Int(position.y, position.x);

            // Find our path.
            FindPath(start, goal);
            return;
        }

        Debug.LogWarning("Start position is out of bounds!"); 
    }

    // Allows us to interact with this with a button.
    public void SetGoalPosition(){
        SetGoalPosition(InputFieldToVector2(goalX, goalY));
    }

    // Set our goal position.
    private void SetGoalPosition(Vector2Int position){
        // Make sure this position is in bounds.
        if(IsInBounds(position))
        {
            // If this location is an obstacle, remove it.
            if(grid[position.x, position.y] == 1)
            {
                grid[position.x, position.y] = 0;
            }

            // Set our start position.
            goal = new Vector2Int(position.y, position.x);

            // Find our path.
            FindPath(start, goal);
            return;
        }

        Debug.LogWarning("Goal position is out of bounds!"); 
    }
}
