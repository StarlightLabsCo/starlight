using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class GraphUpdater : MonoBehaviour
{
    [SerializeField]
    public Tilemap pathTilemap;

    public string tagName = "Path";
    private int pathTag;

    // Start is called before the first frame update
    void Start()
    {
        pathTag = 1;

        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
        {
            var graph = AstarPath.active.data.gridGraph;
            for (int x = 0; x < graph.width; x++)
            {
                for (int z = 0; z < graph.depth; z++)
                {
                    var node = graph.GetNode(x, z);
                    if (node != null)
                    {
                        // Convert the node position back to world coordinates
                        Vector3 nodePosition = (Vector3)node.position;
                        Vector3Int tilePosition = pathTilemap.WorldToCell(nodePosition);

                        // If there's a tile at the node position, then we update the tag of the node.
                        if (pathTilemap.GetTile(tilePosition))
                        {
                            node.Tag = (uint)pathTag;
                        } else
                        {
                            node.Tag = 0;
                        }
                    }
                }
            }
        }));
    }
}
