using System.Collections.Generic;
using System.Linq;
using TbsFramework.Pathfinding.DataStructs;

/// <summary>
/// This is the class that finds the path between two nodes on the hexagonal grid.
/// The graph edges are dictionaries (nested) where outer has all nodes and the inner are their neighbours with the weights of the edges to find the path
/// If a path exists between the hexagon and the player, the nodes in the path are returned, otherwise an empty list is returned
/// </summary>
namespace TbsFramework.Pathfinding.Algorithms
{
    public abstract class IPathfinding
    {
        public abstract List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode) where T : IGraphNode;

        protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, float>> edges, T node) where T : IGraphNode
        {
            if (!edges.ContainsKey(node))
            {
                return new List<T>();
            }
            return edges[node].Keys.ToList();
        }
    }
}