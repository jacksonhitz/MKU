using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public LayerMask moveableLayer;  // Layer for identifying moveable areas
    public Vector3 cubeOrigin;       // Origin point of the cube
    public Vector3 cubeSize = new Vector3(10, 10, 10); // Size of the cube in units
    public float nodeSpacing = 1f;   // Distance between nodes

    public List<Node> nodes = new List<Node>();

    void Start()
    {
        GenerateCube();
    }

    void GenerateCube()
    {
        Vector3 halfSize = cubeSize / 2f;
        Vector3 startPoint = cubeOrigin - halfSize;

        int countX = Mathf.RoundToInt(cubeSize.x / nodeSpacing);
        int countZ = Mathf.RoundToInt(cubeSize.z / nodeSpacing);

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 worldPoint = startPoint + new Vector3(x * nodeSpacing, 0, z * nodeSpacing) + Vector3.one * (nodeSpacing / 2);

                RaycastHit hit;
                if (Physics.Raycast(worldPoint + Vector3.up * 50f, Vector3.down, out hit, 100f, moveableLayer))
                {
                    Vector3 nodePosition = new Vector3(worldPoint.x, hit.point.y, worldPoint.z);
                    nodes.Add(new Node(true, nodePosition));
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(cubeOrigin, cubeSize);

        if (nodes != null)
        {
            foreach (Node node in nodes)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSpacing * 0.9f));
            }
        }
    }

    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;

        public Node(bool walkable, Vector3 worldPosition)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
        }
    }
}
