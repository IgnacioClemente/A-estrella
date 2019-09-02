using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public int x;
    public int y;
    public float hCost;
    public float gCost;

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node parentNode;
    public bool isWalkable = true;
    public List<Node> availableNeighbors;

    private void Awake()
    {
        availableNeighbors = new List<Node>();
    }

    private void Start()
    {
        GetAvailableNeighbors();      
    }

    public void SetHCost(Vector3 endPos)
    {
        hCost = Mathf.Abs(endPos.x - transform.position.x) + Mathf.Abs(endPos.z - transform.position.z);
        //print(gameObject.name + " hcost = " + hCost);
    }

    public void SetGCost(Vector3 startPos)
    {
        gCost = Mathf.Abs(startPos.x - transform.position.x) + Mathf.Abs(startPos.z - transform.position.z);
        //print(gameObject.name + " gcost = " + gCost);
    }

    public void GetAvailableNeighbors()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1 ; y++)
            {
                var node = Grid.GetInstance().GetNode(this.x + x, this.y + y);
                if (node != null && node.isWalkable)
                    availableNeighbors.Add(node);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(isWalkable ? 0 : 1, 0, isWalkable ? 1 : 0, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}