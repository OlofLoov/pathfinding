using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : MonoBehaviour
{   
    private bool _visited = false;
    public bool Visited
    {
        get => _visited;
        set
        {
            _visited = value;
            UpdateNodeColor();
        }
    }

    public GridPosition GridPosition;
    public int GCost = 0; // Distance from start to this node
    public int HCost; // estimated distance from the current node to the end node
    public int FCost => GCost + HCost; // f is the total cost for the node
    public Node PreviousNode;
    public NodeColorController colorController;

    public void SetHCost(Node endNode)
    {
        this.HCost = Math.Abs(endNode.GridPosition.X - GridPosition.X) + Math.Abs(endNode.GridPosition.Y - GridPosition.Y);
    }

    private void Start()
    {
        UpdateNodeColor();
    }

    private NodeState _state = NodeState.Free;
    public NodeState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                UpdateNodeColor();
            }
        }
    }

    public void OnSelect()
    {
        if (State == NodeState.Wall)
            State = NodeState.Free;
        else if (State == NodeState.Free)
            State = NodeState.Wall;
    }

    private void UpdateNodeColor()
    {
        GetComponent<SpriteRenderer>().color = colorController.GetColor(this);
    }
}
