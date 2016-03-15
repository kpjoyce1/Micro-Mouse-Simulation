using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroMouseSimulation
{
    class MazeGraph
    {
        private MazeNode _head;

        public MazeNode Head
        {
            get { return _head; }
            set { _head = value; }
        }

        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public MazeGraph()
        {
            _head = new MazeNode(new Vector2(0, 0));
            Size++;   
            MazeNode current = _head;
            _head.ScanValue = 14;

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        MazeNode temp = new MazeNode(new Vector2(x, y));
                        temp.ScanValue = Math.Abs(7 - x) +  Math.Abs(7 - y);                   
                        while (current.Next != null)
                        {
                            current = current.Next;
                        }
                        current.Next = temp;
                        _size++;
                    }
                }
            }
        }

        public MazeNode GetNode(Vector2 Location)
        {
            MazeNode current = _head;
            
            while(current.Next != null)
            {
                if(current.Location == Location)
                {
                    return current;
                }
                current = current.Next;
            }

            return null;   
        }

        public void AddEdge(Vector2 currentLocation, Vector2 connectedLocation)
        {
            MazeNode current = _head;

            while (current.Next != null)
            {
                if (current.Location == currentLocation)
                {
                    break;
                }
                current = current.Next;
            }
                MazeNode connected = _head;

                while (connected.Next != null)
                {
                    if (connected.Location == connectedLocation)
                    {
                        break;
                    }
                    connected = connected.Next;
                }

                if (current.EdgeHead == null)
                {
                    current.EdgeHead = new MazeEdge(connected);
                }
                else
                {
                    MazeEdge tempEdge = current.EdgeHead;

                    while (tempEdge.Next != null)
                    {
                        if(tempEdge.Target.Location == connectedLocation) // already in edgelist
                        {
                            return;
                        }
                        tempEdge = tempEdge.Next;
                    }

                    tempEdge.Next = new MazeEdge(connected);

                }

        }


        public MazeNode [] shortestPath(Vector2 source, Vector2 destination)
        {
            //Dijkstra's Algorithm
            /*
             * 1. Assign source node distance of zero, and all other values to unattainable 
             * 2. Mark the current node and set all other nodes as unmarked
             * 3. Consider all unvisited neighbors to current node and calculate the distance
             *    If the distance is smaller than the current distance choose the smaller
             * 4. Mark the current node, and move on to unmarked node
             */

            //initialze distances
            MazeNode current = _head;

            List<MazeNode> UnivisitedNodes = new List<MazeNode>();  
                        
            while(current.Next != null)
            {
                current.Parent = null;
                current.Color = Color.Red;
                current.Mark = false;
                if (current.Location != source)
                {
                    current.Distance = -1;                    
                }
                else
                {
                    current.Distance = 0; //starting node has distance 0                    
                }
                UnivisitedNodes.Add(current);
                current = current.Next;
            }

            //dijstra finds all shortest paths to each node
            while (UnivisitedNodes.Count != 0)
            {
                MazeNode minNode = minDistance(UnivisitedNodes);
                UnivisitedNodes.Remove(minNode);

                MazeEdge edge = minNode.EdgeHead;

                while(edge != null)
                {
                    int distanceToNextNode = 1 + minNode.Distance;
                    if (distanceToNextNode <= edge.Target.Distance || edge.Target.Distance == -1)
                    {
                        edge.Target.Distance = distanceToNextNode;                                                                   
                        edge.Target.Parent = minNode;                        
                    }
                    edge = edge.Next;
                }                               
            }

            //get target node
            current = _head;
            while(current.Next != null)
            {
                if(current.Location == destination)
                {
                    break;
                }
                current = current.Next;
            }

            //create PathStack
            List<MazeNode> PathStack = new List<MazeNode>();

            while (current != null)
            {
                current.Color = Color.White;
                current = current.Parent;
            }            

            return PathStack.ToArray();

        }

        private MazeNode minDistance(List<MazeNode> list)
        {

            MazeNode minNode = list[0];            
            foreach(MazeNode node in list)
            { 
                if((node.Distance > minNode.Distance && minNode.Distance == -1) || (node.Distance < minNode.Distance && node.Distance >= 0 && minNode.Distance >= 0))
                {
                    minNode = node;
                }
            }

            return minNode;
        }
       
        public Vector2 nextNodeInScan(Vector2 currentLocation)
        {
            MazeNode current = _head;

            while (current.Next != null)
            {
                if (current.Location == currentLocation)
                {
                    break;
                }
                current = current.Next;
            }

            MazeEdge edge = current.EdgeHead;

            while(edge != null)
            {
                if(edge.Target.ScanValue <= current.ScanValue)
                {
                    return edge.Target.Location;
                }
                edge = edge.Next;
            }

            return Vector2.One * -1f;

        }

        public static Vector2 toMazeCoorinates(Vector2 Position)
        {
            return new Vector2((int)((Position.X - 8) / (Game1.MapUnit + 8)), (int)((Position.Y - 8) / (Game1.MapUnit + 8)));
        }

        public static Vector2 toScreenCoordinates(Vector2 Location)
        {
            return Location * Game1.MapUnit + Location * 8f + Vector2.One * (28 + 6);
        }

        public  void Draw(SpriteBatch spriteBatch)
        {
            MazeNode current = _head;

            while (current.Next != null)
            {
                current.Draw(spriteBatch);
                current = current.Next;
            }
        }

    }
}
