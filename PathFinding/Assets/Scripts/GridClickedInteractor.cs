using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClickedInteractor : MonoBehaviour
{
    private Node currentSelectedNode = null;

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                var go = hit.collider.gameObject;
                var node = go.GetComponent<Node>();
                OnSelectedNode(node);
            }
        }
    }

    private void OnSelectedNode(Node node)
    {
        if (node != currentSelectedNode)
        {
            currentSelectedNode = node;
            node.OnSelect();
        }
    }
}
