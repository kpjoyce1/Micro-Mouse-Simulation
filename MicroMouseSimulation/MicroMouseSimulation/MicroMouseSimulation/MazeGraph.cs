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

        private Stack<MazeNode> _scanStack;

        public Stack<MazeNode> ScanStack
        {
            get { return _scanStack; }
            set { _scanStack = value; }
        }

        private Queue<MazeNode> _scanQueue;

        public Queue<MazeNode> ScanQueue
        {
            get { return _scanQueue; }
            set { _scanQueue = value; }
        }

        private List<MazeNode> _scanOpenList;

        public List<MazeNode> ScanOpenList
        {
            get { return _scanOpenList; }
            set { _scanOpenList = value; }
        }

        private List<MazeNode> _scanCloseList;

        public List<MazeNode> ScanCloseList
        {
            get { return _scanCloseList; }
            set { _scanCloseList = value; }
        }
        
        public MazeGraph()
        {
            _scanQueue = new Queue<MazeNode>();

            _scanStack = new Stack<MazeNode>();

            _scanOpenList = new List<MazeNode>();
            _scanCloseList = new List<MazeNode>();

            _head = new MazeNode(new Vector2(0, 0));
            Size++;
            MazeNode current = _head;
            _head.ScanValue = 14;

            _scanStack.Push(_head);

            _scanQueue.Enqueue(_head);

            _head.F = 0;
            _scanOpenList.Add(_head);
            
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        MazeNode temp = new MazeNode(new Vector2(x, y));
                        temp.ScanValue = Math.Abs(7 - x) + Math.Abs(7 - y);
                        
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
                if (current.Location == Location)
                {
                    return current;
                }
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

            if (!current.EdgeStack.Contains(connected))
            {
                if (current.EdgeHead == null)
                {
                    MazeEdge temp = new MazeEdge(connected);
                    current.EdgeHead = temp;
                    current.EdgeStack.Push(temp.Target);
                }
                else
                {
                    MazeEdge tempEdge = current.EdgeHead;

                    while (tempEdge.Next != null)
                    {
                        if (tempEdge.Target.Location == connectedLocation) // already in edgelist
                        {
                            return;
                        }
                        tempEdge = tempEdge.Next;
                    }

                    tempEdge.Next = new MazeEdge(connected);
                    current.EdgeStack.Push(tempEdge.Next.Target);

                }
            }
        }

        public void ChangeScanValues(Vector2 position)
        {
            MazeNode temp = _head;
            MazeNode curr = _head;
            while(temp != null)
            {
                if(temp.Location == MazeGraph.toMazeCoorinates(position))
                {
                    curr = temp;
                }
                temp.ScanValue =(int) (Math.Abs(temp.Location.X) + Math.Abs(temp.Location.Y));

                temp = temp.Next;
            }
            _scanCloseList.Clear();
            _scanOpenList.Clear();
            curr.F = 0;
            _scanOpenList.Add(curr);
        }

        public void ClearPathFlag()
        {
            MazeNode temp = _head;
            while (temp != null)
            {
                temp.PathFlag = false;
                temp = temp.Next;
            }
        }

        public Stack<MazeNode> getAPath(Vector2 source, Vector2 destination)
        {
            ClearPathFlag();
            MazeNode temp = _head;
            while(temp != null)
            {
                if(temp.Location == source)
                {
                    break;
                }
                temp = temp.Next;

            }

            Queue<MazeNode> q = new Queue<MazeNode>();

            q.Enqueue(temp);
            while(q.Count != 0)
            {
                MazeNode t = q.Dequeue();
                t.PathFlag = true;
                if (t.Location == destination)
                {
                    q.Clear();
                    q.Enqueue(t);
                    break;
                }
                foreach (var e in t.EdgeStack)
                {
                    if(!e.PathFlag)
                    { 
                        e.PathFlag = true;
                        e.PathParent = t;
                        q.Enqueue(e);
                    }
                }
            }


            MazeNode dest = q.Dequeue();
            Stack<MazeNode> path = new Stack<MazeNode>();
            while (dest.Location != source)
            {
                path.Push(dest);
                dest = dest.PathParent;
            }
            
            return path;
        }
        public Stack<MazeNode> shortestPath(Vector2 source, Vector2 destination)
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

                if (minNode.Location == destination)
                {
                    break;
                }

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
            Stack<MazeNode> PathStack = new Stack<MazeNode>();

            while (current != null)
            {
                current.Color = Color.White;
                PathStack.Push(current);
                current = current.Parent;
            }            

            return PathStack;

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

        public Vector2 nextNodeInScanDepthFirst(Bot bot)
        {

            MazeNode current;
            current = _scanStack.Pop();
            current.Mark = true;

            MazeEdge e = current.EdgeHead;
            while (e != null)
            {
                if (!e.Target.Mark)
                {
                    if(!_scanStack.Contains(e.Target))
                        _scanStack.Push(e.Target);
                }
                e = e.Next;
            }

            return _scanStack.Peek().Location;

        }

        public Vector2 nextNodeInScanBreadthFirst(Bot bot)
        {

            MazeNode current;
            current = _scanQueue.Dequeue();

            if (!current.Mark)
            {
                MazeEdge e = current.EdgeHead;
                while (e != null)
                {
                    if (!e.Target.Mark)
                    {
                        if (!_scanQueue.Contains(e.Target))
                        {
                            _scanQueue.Enqueue(e.Target);
                        }
                    }
                    else
                    {
                        ;
                    }
                    e = e.Next;
                }
                current.Mark = true;
            }

            return _scanQueue.Peek().Location;

        }

        public Vector2 nextNodeInFloodFill(Bot bot)
        {
            /*

             */
            MazeNode q = _scanOpenList[0];
            if(q.ScanValue < 1)
            {
                return q.Location;
            }
            _scanOpenList.RemoveAt(0);

            foreach (var e in q.EdgeStack)
            {
                float g = q.G + 1;
                bool skip = false;
                if(_scanOpenList.Contains(e))
                {
                    //skip
                    skip = true;
                }
                else if(_scanCloseList.Contains(e))
                {
                    if (e.G < g)
                    {
                        //skip
                        skip = true;
                    }
                    else
                    {
                        _scanCloseList.Add(e);
                        _scanOpenList.Remove(e);
                    }
                }
                else
                {
                    _scanOpenList.Add(e);
                }
                if(!skip)
                {
                    e.G = g;
                    e.Parent = q;
                }
                
            }
            _scanCloseList.Add(q);

            for (int i = 0; i < _scanOpenList.Count; i++)
            {
                int index = i;
                for (int j = i; j < _scanOpenList.Count; j++)
                {
                    if(Vector2.Distance(_scanOpenList[index].Location, new Vector2(7, 7)) * 1.0 +  Math.Pow(Vector2.Distance(q.Location, _scanOpenList[index].Location), 2)  >
                       Vector2.Distance(_scanOpenList[j].Location, new Vector2(7, 7)) * 1.0 + Math.Pow(Vector2.Distance(_scanOpenList[j].Location, q.Location), 2))          
                    {
                        index = j;
                    }
                }
                var temp = _scanOpenList[i];
                _scanOpenList[i] = _scanOpenList[index];
                _scanOpenList[index] = temp;

            }

            return _scanOpenList[0].Location;
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
