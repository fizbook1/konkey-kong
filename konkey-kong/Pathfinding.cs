using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pakeman
{
    public class Graph
    {
        Node[,] nodes = new Node[36, 27];
        public Graph(Tile[,] bräde)
        {
            for (int i = 0; i < 27; i++)
            {
                for (int j = 0; j < 36; j++)
                {
                    nodes[j, i] = new Node(new Point(j, i), bräde);
                }
            }
        }

        private Stack<Node> Pathfinding(Point start, Point target)
        {
            //bool[,] visited = new bool[9, 9];
            //Node[,] parents = new Node[9, 9];

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(nodes[start.X, start.Y]);
            Stack<Node> fullPath = new Stack<Node>();

            while (queue.Count > 0)
            {
                Node currentNode = queue.Dequeue();
                //visited[start.X, start.Y] = true;
                currentNode.visited = true;
                if (currentNode.coordinate == target)
                { 
                    fullPath.Push(currentNode);
                    break;
                }

                List<Node> adjs = GetAdjacent(currentNode);
                foreach (Node n in adjs)
                {
                    n.parent = currentNode;
                    //parents[n.coordinate.X, n.coordinate.Y] = currentNode;
                    n.visited = true;
                    //visited[n.coordinate.X, n.coordinate.Y] = true;
                    queue.Enqueue(n);

                }
            }


            if (fullPath.Count == 0)
            {
                return fullPath;
            }

            while (fullPath.Peek().parent != null)
            {
                fullPath.Push(fullPath.Peek().parent);
            }
            fullPath.Pop();

            ResetPathfinding();

            return fullPath;
        }

        public Point NextMove(Point start, Point player)
        {

            Stack<Node> fullPath = Pathfinding(start, player);

            if(fullPath.Count > 0)
            {
                return fullPath.Pop().coordinate;
            }

            return start;
        }

        private List<Node> GetAdjacent(Node n)
        {
            List<Node> adjacents = new List<Node>();
            foreach (Point p in n.edges)
            {
                if (!p.Equals(new Point(-1, -1)) && !nodes[p.X, p.Y].visited)
                {
                    adjacents.Add(nodes[p.X, p.Y]);
                }
            }
            return adjacents;
        }

        public void RebuildNodes(Tile[,] bräde)
        {
            foreach (Node n in nodes)
            {
                n.EvaluateEdges(bräde);
            }
        }

        public void ResetPathfinding()
        {
            foreach (Node n in nodes)
            {
                n.parent = null;
                n.visited = false;
            }
        }
    }

    public class Node
    {
        public Node parent = null;
        public Point coordinate;
        public bool visited = false;
        public Point[] edges = new Point[4];


        public Node(Point coordinate, Tile[,] bräde)
        {
            this.coordinate = coordinate;
            EvaluateEdges(bräde);
        }
        public void EvaluateEdges(Tile[,] tiles)
        {
            if (coordinate.X > 0 && tiles[coordinate.X - 1, coordinate.Y].type != TileType.Wall)
            {
                edges[0] = new Point(coordinate.X - 1, coordinate.Y);
            }
            else
            {
                edges[0] = new Point(-1, -1);
            }
            if (coordinate.X < 35 && tiles[coordinate.X + 1, coordinate.Y].type != TileType.Wall)
            {
                edges[2] = new Point(coordinate.X + 1, coordinate.Y);
            }
            else
            {
                edges[2] = new Point(-1, -1);
            }
            if (coordinate.Y > 0 && tiles[coordinate.X, coordinate.Y - 1].type != TileType.Wall)
            {
                edges[1] = new Point(coordinate.X, coordinate.Y - 1);
            }
            else
            {
                edges[1] = new Point(-1, -1);
            }
            if (coordinate.Y < 26 && tiles[coordinate.X, coordinate.Y + 1].type != TileType.Wall)
            {
                edges[3] = new Point(coordinate.X, coordinate.Y + 1);
            }
            else
            {
                edges[3] = new Point(-1, -1);
            }
        }
    }
}
