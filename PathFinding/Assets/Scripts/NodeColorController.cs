using System;
using UnityEngine;

public class NodeColorController
{
    public bool IsCreatingMaze = false;
    public Color GetColor(Node node)
    {
        switch (node.State)
        {
            case NodeState.Wall:
                return new Color(37.0f / 255.0f, 50.0f / 255.0f, 55.0f / 255.0f, 1);
            case NodeState.Free:
                // dont change color on start and end pos
                if (node.Visited && !IsCreatingMaze) // different color scheme depending on appstate
                    return new Color(0.7f, 0.7f, 0.7f, 1);
                return new Color(0.9f, 0.9f, 0.9f, 1);
            case NodeState.Start:
                return new Color(0.0f / 255.0f, 188.0f / 255.0f, 212.0f / 255.0f, 1);
            case NodeState.End:
                return new Color(76.0f / 255.0f, 175.0f / 255.0f, 80.0f / 255.0f, 1);
            case NodeState.Active:
                if (node.Visited && !IsCreatingMaze) // // different color scheme depending on appstate
                    return new Color(0.7f, 0.7f, 0.7f, 1);
                return new Color(0.8f, 0.5f, 0.5f, 1);
            case NodeState.InShortestPath:
                return new Color(156.0f / 255.0f, 39.0f / 255.0f, 176.0f / 255.0f, 1);
        }

        return new Color(1, 1, 1, 1);
    }
}

