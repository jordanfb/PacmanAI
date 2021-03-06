﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualDeath : IndividualState {
    // Store necessary pathfinding variables
    [HideInInspector]
    public List<List<bool>> Graph;
    [HideInInspector]
    public int levelWidth;
    [HideInInspector]
    public int levelHeight;
    private List<List<int>> paths = new List<List<int>>();
    private List<int> pathIndex = new List<int>();
    private List<Vector2Int> ghostSpawns = new List<Vector2Int>();
    int ghostIndex;

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
        paths[index] = PathFind(new Vector2Int((int)transform.position.x, (int)transform.position.y),
                ghostSpawns[index]);
        pathIndex[index] = 0;
    }

    // Called by the levelmanager on reload
    public void OnReload(List<List<bool>> graph, int levelwidth, int levelheight, Vector2[] spawns, int ghostIndexAssignment) {
        ghostIndex = ghostIndexAssignment;
        Graph = graph;
        levelWidth = levelwidth;
        levelHeight = levelheight;
        paths.Clear();
        pathIndex.Clear();
        ghostSpawns.Clear();
        for (int i = 0; i < spawns.Length; i++) {
            ghostSpawns.Add(new Vector2Int((int)Mathf.Floor(spawns[i].x), (int)Mathf.Floor(spawns[i].y)));
        }
        for (int i = 0; i < 4; i++) {
            // only add one path for this one ghost
            paths.Add(null);
            pathIndex.Add(0);
        }
    }

    // Called by the levelmanager to trigger blue ghost behavior
    public void BigDotTrigger() {
        GetComponent<GhostMovement>().FlipDirection();
        GetComponent<GhostMovement>().SetAlmighty(false);
    }

    // On initialization
    override protected void Start() {
        //system = GetComponent<IndividualFSMSystem>();
    }

    // When active
    override public void Active()
    {
        int i = ghostIndex; // this is just using the first slot for this ghost only
        if (GetComponent<GhostMovement>().Ded())
        {
            if (paths[i] == null)
            {
                SetPath(i);
            }
            else if (pathIndex[i] < paths[i].Count && GetComponent<GhostMovement>().atDecisionPoint)
            {
                GetComponent<GhostMovement>().SetGoalDirection(paths[i][pathIndex[i]]);
                pathIndex[i]++;
            }
            else if (pathIndex[i] >= paths[i].Count)
            {
                GetComponent<GhostMovement>().DedGhost(false);
                paths[i] = null;
            }
        }
        else
        {
            GetComponent<GhostMovement>().Wander();
        }
    }
}
