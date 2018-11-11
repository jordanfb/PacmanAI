using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : FSMState {
    [HideInInspector]
    public List<List<bool>> Graph;
    [HideInInspector]
    public int levelWidth;
    [HideInInspector]
    public int levelHeight;
    private List<List<Vector2Int>> paths;
    private List<int> pathIndex;

    override protected void Start() {
        system = GetComponent<FSMSystem>();
        paths = new List<List<Vector2Int>>();
        pathIndex = new List<int>();
        for (int i = 0; i < system.Ghosts.Count; i++) {
            paths.Add(null);
            pathIndex.Add(0);
        }
    }

    // When active
    override public void Active() {
        /*for (int i = 0; i < system.Ghosts.Count; i++) {

            system.Ghosts[i].GetComponent<GhostMovement>().SetGoalDirection(0);
        }*/
        if (Input.GetKeyDown(KeyCode.B)) {
            List<Vector2Int> lis = PathFind(new Vector2Int(1, 1), new Vector2Int(1, 29));
            if (lis != null) {
                foreach (Vector2Int a in lis) {
                    print(a);
                }
            } else {
                print(null);
            }
        }
    }

    private void Level1AI() {
        for (int i = 0; i < system.Ghosts.Count; i++) {
            Vector2Int currentPos = new Vector2Int((int)system.Ghosts[i].transform.position.x, (int)system.Ghosts[i].transform.position.y);
            Vector2Int direction = paths[i][pathIndex[i]] - currentPos;
            system.Ghosts[i].GetComponent<GhostMovement>().SetGoalDirection(0);
        }
    }

    private void SetPath(int index) {
        paths[index] = PathFind(new Vector2Int((int)system.Ghosts[index].transform.position.x, (int)system.Ghosts[index].transform.position.y),
                new Vector2Int((int)system.Pacman.transform.position.x, (int)system.Pacman.transform.position.y));
        pathIndex[index] = 1;
    }
    // Helper function for path finding
    private List<Vector2Int> createPath(Dictionary<Vector2Int, Vector2Int> paths, Vector2Int current) {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(current);
        while (paths.ContainsKey(current)) {
            current = paths[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    // Path Finding algorithm
    private List<Vector2Int> PathFind(Vector2Int start, Vector2Int goal) {
        List<Vector2Int> visited = new List<Vector2Int>();
        Queue<Vector2Int> evaluate = new Queue<Vector2Int>();
        evaluate.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> paths = new Dictionary<Vector2Int, Vector2Int>();

        while (evaluate.Count != 0) {
            Vector2Int current = evaluate.Dequeue();
            if (current == goal) {
                return createPath(paths, current);
            }
            visited.Add(current);

            if (current.y + 1 < levelHeight && Graph[current.y + 1][current.x] &&
                        !visited.Contains(current + Vector2Int.up) && !evaluate.Contains(current + Vector2Int.up)) {
                evaluate.Enqueue(current + Vector2Int.up);
                paths[current + Vector2Int.up] = current;
            }
            if (current.x + 1 < levelWidth && Graph[current.y][current.x + 1] &&
                        !visited.Contains(current + Vector2Int.right) && !evaluate.Contains(current + Vector2Int.right)) {
                evaluate.Enqueue(current + Vector2Int.right);
                paths[current + Vector2Int.right] = current;
            }
            if (current.y - 1 >= 0 && Graph[current.y - 1][current.x] &&
                        !visited.Contains(current + Vector2Int.down) && !evaluate.Contains(current + Vector2Int.down)) {
                evaluate.Enqueue(current + Vector2Int.down);
                paths[current + Vector2Int.down] = current;
            }
            if (current.x - 1 >= 0 && Graph[current.y][current.x - 1] &&
                        !visited.Contains(current + Vector2Int.left) && !evaluate.Contains(current + Vector2Int.left)) {
                evaluate.Enqueue(current + Vector2Int.left);
                paths[current + Vector2Int.left] = current;
            }
        }
        return null;
    }
}
