using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

/* 
   From wikipedia
   Given a current cell as a parameter
   Mark the current cell as visited
   While the current cell has any unvisited neighbour cells
     Choose one of the unvisited neighbours
     Remove the wall between the current cell and the chosen cell
     Invoke the routine recursively for a chosen cell
*/

public class MazeCreator
{
    public event EventHandler<MazeActionArgs> OnCreated;
    List<Node> activeNodes, nodes;
    int GridWidth, GridHeight;

    public MazeCreator(List<Node> nodes, int gridWidth, int gridHeight)
    {
        this.nodes = nodes;
        this.GridHeight = gridHeight;
        this.GridWidth = gridWidth;
    }

    public void CreateMaze(GridPosition startPosition)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var node = nodes.Where(n => n.GridPosition.X == x && n.GridPosition.Y == y).First();
                // create grid where every even is a block/wall, these walls/doors will then be opened by the maze generator method
                // so walls are distributed on every even grid position, every uneven is will always be open/free
                if (x % 2 == 0)
                    node.State = NodeState.Wall;
                if (y % 2 == 0)
                    node.State = NodeState.Wall;
            }
        }
        GenerateMaze(startPosition);
    }

    private async void GenerateMaze(GridPosition startPosition)
    {
        var startNode = nodes.Where(n => n.GridPosition.X == startPosition.X && n.GridPosition.Y == startPosition.Y).First();
        await VisitCell(startNode);
        foreach (var node in nodes)
        {
            node.Visited = false;
        }

        OnCreated?.Invoke(this, new MazeActionArgs());
    }

    private async Task VisitCell(Node selectedNode)
    {
        // set visited cell to free
        if (selectedNode.State != NodeState.Start && selectedNode.State != NodeState.End)
            selectedNode.State = NodeState.Free;

        selectedNode.Visited = true;
        await UniTask.Delay(15);

        var neighbours = GetNeighbours(selectedNode);

        while (neighbours.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, neighbours.Count);
            var oneOfTheNeighbour = neighbours[index];

            var xDiff = selectedNode.GridPosition.X - oneOfTheNeighbour.GridPosition.X;
            var yDiff = selectedNode.GridPosition.Y - oneOfTheNeighbour.GridPosition.Y;

            // unlocking walls/doors in maze. Half diff since walls are initally distributed on even grid positions.
            if (xDiff != 0)
            {
                var nodeToNotBlock = nodes.Where(n => n.GridPosition.X == -xDiff / 2 + selectedNode.GridPosition.X && n.GridPosition.Y == selectedNode.GridPosition.Y).FirstOrDefault();
                if (nodeToNotBlock != null)
                {
                    nodeToNotBlock.State = NodeState.Free;
                }
            }
            else if (yDiff != 0)
            {
                var nodeToNotBlock = nodes.Where(n => n.GridPosition.X == selectedNode.GridPosition.X && n.GridPosition.Y == -yDiff / 2 + selectedNode.GridPosition.Y).FirstOrDefault();
                if (nodeToNotBlock != null)
                {
                    nodeToNotBlock.State = NodeState.Free;
                }
            }

            await VisitCell(oneOfTheNeighbour); // recursively visit one of the neighbours
            neighbours = neighbours.Where(x => x.Visited == false).ToList();
        }
    }

    private List<Node> GetNeighbours(Node nodeToCheck)
    {
        var possibleNodes = new List<Node> { };

        // just look on grid position + 2 since step size between two walls is at least 2.
        if (nodeToCheck.GridPosition.X + 2 < GridWidth)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X + 2 == n.GridPosition.X && nodeToCheck.GridPosition.Y == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.X - 2 >= 0)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X - 2 == n.GridPosition.X && nodeToCheck.GridPosition.Y == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.Y + 2 < GridHeight)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X == n.GridPosition.X && nodeToCheck.GridPosition.Y + 2 == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.Y - 2 >= 0)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X == n.GridPosition.X && nodeToCheck.GridPosition.Y - 2 == n.GridPosition.Y).First());

        return possibleNodes.Where(n => n.Visited == false).ToList();
    }
}

