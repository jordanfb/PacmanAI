using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy pathfinds Pacman every step and follows the path
public class ChaseState : FSMState {
    // Adjustable variables
    [SerializeField]
    private int chance = 8;

    // Store necessary pathfinding variables
    [HideInInspector]
    public List<List<bool>> Graph;
    [HideInInspector]
    public int levelWidth;
    [HideInInspector]
    public int levelHeight;
    private List<List<int>> paths;
    private List<int> pathIndex;
    private float time = 1;

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
    private List<int> PathFind(Vector2Int start, Vector2Int goal) {
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> evaluate = new Queue<Vector2Int>();
        evaluate.Enqueue(start);

        Dictionary<Vector2Int, direction> paths = new Dictionary<Vector2Int, direction>();

        while (evaluate.Count != 0) {
            Vector2Int current = evaluate.Dequeue();
            if (current == goal) {
                return createPath(paths, current);
            }
            visited.Add(current);

            if (current.y + 1 < levelHeight && Graph[current.y + 1][current.x] &&
                        !visited.Contains(current + Vector2Int.up) && !evaluate.Contains(current + Vector2Int.up)) {
                evaluate.Enqueue(current + Vector2Int.up);
                paths[current + Vector2Int.up] = direction.Up;
            }
            if (current.x + 1 < levelWidth && Graph[current.y][current.x + 1] &&
                        !visited.Contains(current + Vector2Int.right) && !evaluate.Contains(current + Vector2Int.right)) {
                evaluate.Enqueue(current + Vector2Int.right);
                paths[current + Vector2Int.right] = direction.Right;
            }
            if (current.y - 1 >= 0 && Graph[current.y - 1][current.x] &&
                        !visited.Contains(current + Vector2Int.down) && !evaluate.Contains(current + Vector2Int.down)) {
                evaluate.Enqueue(current + Vector2Int.down);
                paths[current + Vector2Int.down] = direction.Down;
            }
            if (current.x - 1 >= 0 && Graph[current.y][current.x - 1] &&
                        !visited.Contains(current + Vector2Int.left) && !evaluate.Contains(current + Vector2Int.left)) {
                evaluate.Enqueue(current + Vector2Int.left);
                paths[current + Vector2Int.left] = direction.Left;
            }
        }
        return null;
    }

    // Sets the path of a ghost
    private void SetPath(int index) {
        paths[index] = PathFind(new Vector2Int((int)system.Ghosts[index].transform.position.x, (int)system.Ghosts[index].transform.position.y),
                new Vector2Int((int)system.Pacman.transform.position.x, (int)system.Pacman.transform.position.y));
        pathIndex[index] = 0;
    }

    // Called by the levelmanager on reload
    public void OnReload(List<List<bool>> graph, int levelwidth, int levelheight) {
        Graph = graph;
        levelWidth = levelwidth;
        levelHeight = levelheight;
        paths.Clear();
        pathIndex.Clear();
        for (int i = 0; i < system.Ghosts.Count; i++) {
            paths.Add(null);
            pathIndex.Add(0);
            SetPath(i);
        }
    }

    // On initialization
    override protected void Start() {
        system = GetComponent<FSMSystem>();
        paths = new List<List<int>>();
        pathIndex = new List<int>();
    }

    // When active
    override public void Active() {
        for (int i = 0; i < system.Ghosts.Count; i++) {
            if (system.Ghosts[i].GetComponent<GhostMovement>().atDecisionPoint) {
                SetPath(i);
                system.Ghosts[i].GetComponent<GhostMovement>().SetGoalDirection(paths[i][0]);
            }
        }
        if (time < 0) {
            if (Random.Range(0, chance) == 0) {
                system.Transition(0);
            }
            time++;
        }
        time -= Time.deltaTime;
    }
}
