using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;

/*
unitask so webgl works properly
no optimization for proper builds
create git repo & github page */

public class AppManager : MonoBehaviour
{
    [SerializeField]
    GameObject EntityPrefab = null;

    [SerializeField]
    public int GridWidth = 40;

    [SerializeField]
    public int GridHeight = 25;

    List<Node> nodes;
    private bool IsBusy = false;
    NodeColorController nodeColorController = new NodeColorController();

    void Start()
    {
        // Get ECS Loop.
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        // Setup UniTask's PlayerLoop.
        PlayerLoopHelper.Initialize(ref playerLoop);

        CreateGrid();
    }

    public void CreateGrid()
    {
        if (IsBusy)
            return;
        IsBusy = true;
        // clear any existing nodes
        nodes = new List<Node>();

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var go = Instantiate(EntityPrefab, new Vector3(x, y, transform.position.z), Quaternion.identity);
                go.transform.parent = transform;
                var node = go.GetComponent<Node>();
                var gridPosition = new GridPosition(x, y);
                node.GridPosition = gridPosition;
                node.colorController = nodeColorController;

                // frame it
                if (x == 0 || y == 0 || x == GridWidth - 1 || y == GridHeight - 1)
                {
                    node.State = NodeState.Wall;
                }

                nodes.Add(node);
            }
        }

        var startPosition = GeneratePosition();
        nodes.Where(n => n.GridPosition.X == startPosition.X && n.GridPosition.Y == startPosition.Y).First().State = NodeState.Start;
        var endPosition = GeneratePosition();
        while (endPosition.X == startPosition.X && endPosition.Y == startPosition.Y)
        {
            endPosition = GeneratePosition();
        }

        nodes.Where(n => n.GridPosition.X == endPosition.X && n.GridPosition.Y == endPosition.Y).First().State = NodeState.End;
        IsBusy = false;
    }

    public void SetupMaze()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        var mazeCreator = new MazeCreator(nodes, GridWidth, GridHeight);
        var start = GeneratePosition();
        nodeColorController.IsCreatingMaze = true;
        mazeCreator.CreateMaze(start);
        
        mazeCreator.OnCreated += (s, e) => {
            IsBusy = false;
            nodeColorController.IsCreatingMaze = false;
        };
    }

    public void CalculatePath()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        var solver = new MazeSolver(nodes, GridWidth, GridHeight);
        solver.SolveMaze();
        solver.OnSolved += (s, e) => {
            IsBusy = false;
        };
    }

    // used for start and end point
    private GridPosition GeneratePosition()
    {
        int x = UnityEngine.Random.Range(0, GridWidth - 1);
        int y = UnityEngine.Random.Range(0, GridHeight - 1);

        if (x % 2 == 0)
            x += 1;

        if (y % 2 == 0)
            y += 1;
        return new GridPosition(x, y);
    }
}

public class MazeActionArgs { }