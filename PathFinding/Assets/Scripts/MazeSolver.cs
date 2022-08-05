using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Net;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class MazeSolver
{
    public event EventHandler<MazeActionArgs> OnSolved;
    List<Node> activeNodes, nodes;
    int GridWidth, GridHeight;

    public MazeSolver(List<Node> nodes, int gridWidth, int gridHeight)
    {
        this.nodes = nodes;
        this.GridHeight = gridHeight;
        this.GridWidth = gridWidth;
    }

    public async void SolveMaze()
    {
        activeNodes = new List<Node>();
        var startNode = nodes.First(n => n.State == NodeState.Start);
        activeNodes.Add(startNode);
        var endNode = nodes.First(n => n.State == NodeState.End);

        while (activeNodes.Any())
        {
            var nodeToCheck = activeNodes.OrderBy(x => x.FCost).First();
            await UniTask.Delay(10);
            nodeToCheck.Visited = true;

            if (nodeToCheck.State == NodeState.End)
            {
                var prevNode = nodeToCheck.PreviousNode;

                // reverse taken path
                while (prevNode != startNode)
                {
                    prevNode.State = NodeState.InShortestPath;
                    await UniTask.Delay(20);
                    prevNode = prevNode.PreviousNode;
                }

                break;
            }

            var possibleNodes = GetNeighbours(nodeToCheck);
            foreach (var node in possibleNodes)
            {
                if (node.State == NodeState.End)
                {
                    activeNodes.Add(node);
                    node.PreviousNode = nodeToCheck;
                    continue;
                }

                if (activeNodes.Any(n => n.GridPosition.X == node.GridPosition.X && n.GridPosition.Y == node.GridPosition.Y))
                {
                    var existingNode = activeNodes.First(x => x.GridPosition.X == node.GridPosition.X && x.GridPosition.Y == node.GridPosition.Y);

                    // might revisit a node from a different path thus might be a more cost effective route
                    if (existingNode.FCost > node.FCost)
                    {
                        activeNodes.Remove(existingNode);
                    }
                }

                if (!activeNodes.Contains(node))
                {
                    node.GCost = nodeToCheck.GCost + 1; // cost comp to previous is one up, since we have now moved one step more from start node
                    node.PreviousNode = nodeToCheck;
                    node.SetHCost(endNode);
                    if (node.State != NodeState.Start)
                        node.State = NodeState.Active;
                    activeNodes.Add(node);
                }
            }

            activeNodes.Remove(nodeToCheck);
        }

        OnSolved?.Invoke(this, new MazeActionArgs());
    }

    private List<Node> GetNeighbours(Node nodeToCheck)
    {
        var possibleNodes = new List<Node> { };

        if (nodeToCheck.GridPosition.X + 1 < GridWidth)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X + 1 == n.GridPosition.X && nodeToCheck.GridPosition.Y == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.X - 1 >= 0)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X - 1 == n.GridPosition.X && nodeToCheck.GridPosition.Y == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.Y + 1 < GridHeight)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X == n.GridPosition.X && nodeToCheck.GridPosition.Y + 1 == n.GridPosition.Y).First());

        if (nodeToCheck.GridPosition.Y - 1 >= 0)
            possibleNodes.Add(nodes.Where(n => nodeToCheck.GridPosition.X == n.GridPosition.X && nodeToCheck.GridPosition.Y - 1 == n.GridPosition.Y).First());

        return possibleNodes
            .Where(n => n.Visited == false)
            .Where(n => n.State != NodeState.Wall)
            .ToList();
    }
}

