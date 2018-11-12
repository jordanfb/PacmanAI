using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy pathfinds Pacman and attempts to not cross each other's paths
public class FlankState : FSMState {
    // Adjustable variables
    [SerializeField]
    private float distance = 5;

    // Store necessary pathfinding variables
    [HideInInspector]
    public List<List<bool>> Graph;
    [HideInInspector]
    public int levelWidth;
    [HideInInspector]
    public int levelHeight;
    private List<List<int>> paths;
    private List<Vector2Int> ghostPos;

    // For direction calculations
    private enum direction {
        Up, Right, Down, Left
    }
    private Vector2Int[] allDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    // Helper function for path finding
    private List<int> createPath(Dictionary<Vector2Int, direction> paths, Vector2Int current) {
        List<int> path = new List<int>();
        while (paths.ContainsKey(current)) {
            path.Add((int)paths[current]);
            current += allDirections[((int)paths[current] + 2) % 4];
        }
        path.Reverse();
        return path;
    }

    // Path Finding algorithm
    private List<int> PathFind(List<List<bool>> graph, Vector2Int start, Vector2Int goal) {
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> evaluate = new Queue<Vector2Int>();
        evaluate.Enqueue(start);

        Dictionary<Vector2Int, direction> paths = new Dictionary<Vector2Int, direction>();

        while (evaluate.Count != 0) {
            Vector2Int current = evaluate.Dequeue();
            if (Vector2Int.Distance(current, goal) < distance) {
                return createPath(paths, current);
            }
            visited.Add(current);

            if (current.y + 1 < levelHeight && graph[current.y + 1][current.x] &&
                        !visited.Contains(current + Vector2Int.up) && !evaluate.Contains(current + Vector2Int.up)) {
                evaluate.Enqueue(current + Vector2Int.up);
                paths[current + Vector2Int.up] = direction.Up;
            }
            if (current.x + 1 < levelWidth && graph[current.y][current.x + 1] &&
                        !visited.Contains(current + Vector2Int.right) && !evaluate.Contains(current + Vector2Int.right)) {
                evaluate.Enqueue(current + Vector2Int.right);
                paths[current + Vector2Int.right] = direction.Right;
            }
            if (current.y - 1 >= 0 && graph[current.y - 1][current.x] &&
                        !visited.Contains(current + Vector2Int.down) && !evaluate.Contains(current + Vector2Int.down)) {
                evaluate.Enqueue(current + Vector2Int.down);
                paths[current + Vector2Int.down] = direction.Down;
            }
            if (current.x - 1 >= 0 && graph[current.y][current.x - 1] &&
                        !visited.Contains(current + Vector2Int.left) && !evaluate.Contains(current + Vector2Int.left)) {
                evaluate.Enqueue(current + Vector2Int.left);
                paths[current + Vector2Int.left] = direction.Left;
            }
        }
        return null;
    }

    // Sets the path of a ghost that tries not to overlap with the path of another ghost
    private void SetPath(int index) {
        List<List<bool>> tempGraph = new List<List<bool>>();
        foreach (List<bool> row in Graph) {
            tempGraph.Add(new List<bool>(row));
        }
        List<int> result;
        ghostPos[index] = new Vector2Int((int)system.Ghosts[index].transform.position.x, (int)system.Ghosts[index].transform.position.y);
        for (int i = 0; i < paths.Count; i++) {
            result = PathFind(tempGraph, ghostPos[index], new Vector2Int((int)system.Pacman.transform.position.x, (int)system.Pacman.transform.position.y));
            if (result != null) {
                paths[index] = result;
            }
            if (i != index) {
                Vector2Int currentPos = ghostPos[i];
                for (int j = 0; j < paths[i].Count; j++) {
                    currentPos += allDirections[paths[i][j]];
                    tempGraph[currentPos.x][currentPos.y] = false;
                }
            }
        }
    }

    // Called by the levelmanager on reload
    public void OnReload(List<List<bool>> graph, int levelwidth, int levelheight) {
        Graph = graph;
        levelWidth = levelwidth;
        levelHeight = levelheight;
        paths.Clear();
        for (int i = 0; i < system.Ghosts.Count; i++) {
            paths.Add(null);
            ghostPos.Add(Vector2Int.zero);
            SetPath(i);
        }
    }

    // On initialization
    override protected void Start() {
        system = GetComponent<FSMSystem>();
        paths = new List<List<int>>();
        ghostPos = new List<Vector2Int>();
    }

    // When active
    override public void Active() {
        for (int i = 0; i < system.Ghosts.Count; i++) {
            if (Vector2Int.Distance(new Vector2Int((int)system.Ghosts[i].transform.position.x, (int)system.Ghosts[i].transform.position.y),
                new Vector2Int((int)system.Pacman.transform.position.x, (int)system.Pacman.transform.position.y)) <= distance) {
                system.Transition(1);
            }
            if (system.Ghosts[i].GetComponent<GhostMovement>().atDecisionPoint) {
                SetPath(i);
                system.Ghosts[i].GetComponent<GhostMovement>().SetGoalDirection(paths[i][0]);
            }
        }
    }
}
