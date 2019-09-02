using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public int maxX = 10;
    public int maxY = 10;

    public LayerMask unWalkableLayer;

    public float offsetX = 1;
    public float offsetY = 1;

    public Node[,] grid;

    public Node Node;
    public bool start;

    public Vector2 startNodePosition;
    public Vector2 endNodePosition;

    private List<Node> openNodes;
    private List<Node> closedNodes;
    private Node currentNode;

    public static Grid instance;
    public static Grid GetInstance()
    {
        return instance;
    }
    
    void Awake()
    {
        instance = this;

        openNodes = new List<Node>();
        closedNodes = new List<Node>();
        grid = new Node[maxX, maxY];

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                float posX = x * offsetX - maxX / 2 + offsetX / 2;
                float posY = y * offsetY - maxY / 2 + offsetY / 2;
                Node node = Instantiate(Node, new Vector3(posX , 0.5f, posY), Quaternion.identity);
                node.transform.name = x.ToString() + " " + y.ToString();
                node.transform.parent = transform;
                
                node.x = x;
                node.y = y;

                RaycastHit[] hits = Physics.BoxCastAll(node.transform.position, new Vector3(0.4f, 0.4f, 0.4f), Vector3.up, Quaternion.identity,0.4f, unWalkableLayer);

                for (int i = 0; i < hits.Length; i++)
                {
                    //print(node.gameObject.name + " hits " + hits[i].collider.gameObject.name);
                    node.isWalkable = false;
                }

                grid[x, y] = node;
            }
        }
    }

    void Update()
    {
        if (start)
        {
            start = false;

            Node startNode = GetNodeFromVector2(startNodePosition);
            Node end = GetNodeFromVector2(endNodePosition);

            startNode.SetGCost(end.transform.position);
            startNode.SetHCost(end.transform.position);

            openNodes.Add(startNode);

            int timesToBleh = 0;

            while (timesToBleh < 150 && openNodes.Count > 0)
            {
                currentNode = openNodes[0];

                timesToBleh++;
                for (int i = 0; i < openNodes.Count; i++)
                {
                    //print(openNodes[i].gameObject.name + " fcost: " + openNodes[i].fCost + " VS " + currentNode.gameObject.name + " fcost" + currentNode.fCost);
                    if (openNodes[i].fCost < currentNode.fCost)
                    {
                        if (openNodes[i].fCost == currentNode.fCost)
                        {
                            if (openNodes[i].hCost < currentNode.hCost)
                                currentNode = openNodes[i];
                            else if (openNodes[i].hCost == currentNode.hCost)
                            {
                                int chance = Random.Range(0, 2);
                                currentNode = chance == 0 ? currentNode : openNodes[i];
                            }
                        }
                        else
                            currentNode = openNodes[i];
                    }
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
                if (currentNode == end)
                {
                    var actualNode = end;
                    List<Node> path = new List<Node>();
                    int countbleh = 0;

                    while (actualNode != startNode && countbleh < 50)
                    {
                        countbleh++;
                        path.Add(actualNode);
                        actualNode = actualNode.parentNode;
                    }
                    path.Add(startNode);

                    for (int i = path.Count - 1; i >= 0; i--)
                    {
                        print(path[i].gameObject.name);
                    }

                    return;
                }

                foreach (var node in currentNode.availableNeighbors)
                {
                    if (closedNodes.Contains(node)) continue;

                    if (!openNodes.Contains(node))
                    {
                        node.SetGCost(startNode.transform.position);
                        node.SetHCost(end.transform.position);
                        node.parentNode = currentNode;
                        openNodes.Add(node);
                    }
                }
            }
            
            
        }
    }

    public Node GetNode(int x, int y)
    {
        Node retVal = null;

        if (x < maxX && x >= 0 &&
            y >= 0 && y < maxY)
        {
            retVal = grid[x, y];
        }

        return retVal;
    }

    public Node GetNodeFromVector2(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        Node retVal = GetNode(x, y);
        return retVal;
    }
}