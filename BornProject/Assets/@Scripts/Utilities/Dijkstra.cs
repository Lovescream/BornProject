using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dijkstra {
    public class Dijkstra {

    }

    public class Node {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int Index => new(X, Y);

        public int F => G + H;
        public int G { get; set; }
        public int H { get; set; }


        public Node(Vector2Int index) {
            this.X = index.x;
            this.Y = index.y;
        }
    }
    public class Graph {
        
        public Node Start { get; private set; }

        private HashSet<Node> _nodes;

        public Graph(HashSet<Vector2Int> indexList, Vector2Int start) {
            _nodes = indexList.Select(index => new Node(index)).ToHashSet();
            Start = _nodes.Where(node => node.X == start.x && node.Y == start.y).FirstOrDefault();
        }

        public Dictionary<Vector2Int, int> GetCost() {
            List<Node> openList = new();
            List<Node> closeList = new();

            openList.Add(Start);

            while (openList.Count > 0) {
                float min = openList.Min(node => node.F);
                Node node = openList.First(node => Mathf.Abs(node.F - min) < float.Epsilon);
                openList.Remove(node);
                closeList.Add(node);

                HashSet<Node> neighbours = new();
                for (int i = 0; i <(int)Direction.COUNT; i++) {
                    Vector2Int index = node.Index.GetDirectionIndex((Direction)i);
                    Node neighbourNode = _nodes.Where(node => node.X == index.x && node.Y == index.y).FirstOrDefault();
                    if (neighbourNode == null) continue;
                    if (closeList.Contains(neighbourNode)) continue;

                    int newCost = node.G + 1;

                    if (newCost < neighbourNode.G || !openList.Contains(neighbourNode)) {
                        neighbourNode.G = newCost;
                        if (!openList.Contains(neighbourNode)) openList.Add(neighbourNode);
                    }
                }
            }

            Dictionary<Vector2Int, int> dictionary = new();
            foreach(Node node in _nodes) {
                dictionary[node.Index] = node.F;
            }
            return dictionary;
        }
    }
    public enum Direction {
        Top,
        Right,
        Bottom,
        Left,
        COUNT
    }
}