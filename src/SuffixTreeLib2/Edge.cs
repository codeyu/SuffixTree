/* Edge.cs
 * To Do: add comments
 * I need a better hashing system for large data sets. 
 * 
 * 
 * This is a suffix tree algorithm for .NET written in C#. Feel free to use it as you please!
 * This code was derived from Mark Nelson's article located here: http://marknelson.us/1996/08/01/suffix-trees/
 * Have Fun 
 * 
 * Zikomo A. Fields 2008
 * 
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;

namespace SuffixTreeLib2
{
    public struct Edge<TValue> where TValue : IComparable<TValue>
    {
        public int indexOfFirstCharacter;
        public int indexOfLastCharacter;
        public int startNode;
        public int endNode;        

        public const int HASH_TABLE_SIZE = 306785407;        
        
        public Edge(int startNode)
        {            
            this.startNode = -1;
            this.indexOfFirstCharacter = 0;
            this.indexOfLastCharacter = 0;
            this.endNode = 0;
        }

        public Edge(int indexOfFirstCharacter, int indexOfLastCharacter, int parentNode)
        {            
            this.indexOfFirstCharacter = indexOfFirstCharacter;
            this.indexOfLastCharacter = indexOfLastCharacter;
            this.startNode = parentNode;
            this.endNode = Node<TValue>.Count++;
        }

        public Edge(Edge<TValue> edge)
        {
            this.startNode = edge.startNode;
            this.endNode = edge.endNode;
            this.indexOfFirstCharacter = edge.indexOfFirstCharacter;
            this.indexOfLastCharacter = edge.indexOfLastCharacter;            
        }

        public void Copy(Edge<TValue> edge)
        {
            this.startNode = edge.startNode;
            this.endNode = edge.endNode;
            this.indexOfFirstCharacter = edge.indexOfFirstCharacter;
            this.indexOfLastCharacter = edge.indexOfLastCharacter;            
        }

        static public void Insert(SuffixTree<TValue> tree, Edge<TValue> edge)
        {
            int i = Hash(edge.startNode, tree.m_Source[edge.indexOfFirstCharacter]);
            if (!tree.Edges.ContainsKey(i))
            {
                tree.Edges.Add(i, new Edge<TValue>(-1));
            }
            while (tree.Edges[i].startNode != -1)
            {
                i = ++i % HASH_TABLE_SIZE;
                if (!tree.Edges.ContainsKey(i))
                {
                    tree.Edges.Add(i, new Edge<TValue>(-1));
                }

            }
            tree.Edges[i] = edge;
        }

        static public void Remove(SuffixTree<TValue> tree, Edge<TValue> edge)
        {            
            int i = Hash(edge.startNode, tree.m_Source[edge.indexOfFirstCharacter]);
            while (tree.Edges[i].startNode != edge.startNode || tree.Edges[i].indexOfFirstCharacter != edge.indexOfFirstCharacter)
            {
                i = ++i % HASH_TABLE_SIZE;
            }
            for (; ; )
            {
                
                Edge<TValue> tempEdge = tree.Edges[i];
                tempEdge.startNode = -1;
                tree.Edges[i] = tempEdge;
                int j = i;
                for (; ; )
                {
                    i = ++i % HASH_TABLE_SIZE;
                    if (!tree.Edges.ContainsKey(i))
                    {
                        tree.Edges.Add(i, new Edge<TValue>(-1));
                    }
                    if (tree.Edges[i].startNode == -1)
                    {
                        return;
                    }

                    int r = Hash(tree.Edges[i].startNode, tree.m_Source[tree.Edges[i].indexOfFirstCharacter]);
                    if (i >= r && r > j)
                    {
                        continue;
                    }
                    if (r > j && j > i)
                    {
                        continue;
                    }
                    if (j > i && i >= r)
                    {
                        continue;
                    }
                    break;
                }
                tree.Edges[j].Copy(tree.Edges[i]);
            }
        }

        static public int SplitEdge(SuffixTree<TValue> tree, Suffix<TValue> s, ref Edge<TValue> edge)
        {
            Remove(tree, edge);
            int last = edge.indexOfFirstCharacter + s.m_IndexOfLastCharacter - s.m_IndexOfFirstCharacter;
            Edge<TValue> newEdge = new Edge<TValue>(edge.indexOfFirstCharacter, last, s.m_OriginNode);
            Edge<TValue>.Insert(tree, newEdge);
            if (tree.Nodes.ContainsKey(newEdge.endNode))
            {
                tree.Nodes[newEdge.endNode].suffixNode = s.m_OriginNode;
            }
            else
            {
                Node<TValue> newNode = new Node<TValue>();
                newNode.suffixNode = s.m_OriginNode;
                tree.Nodes.Add(newEdge.endNode, newNode);
            }

            edge.indexOfFirstCharacter += s.m_IndexOfLastCharacter - s.m_IndexOfFirstCharacter + 1;
            edge.startNode = newEdge.endNode;
            Edge<TValue>.Insert(tree, edge);            
            return newEdge.endNode;
           
        }

        static public Edge<TValue> Find(IEnumerable<TValue> theString, Dictionary<int, Edge<TValue>> edges, int node, TValue c)
        {
            int i = Hash(node, c);
            for (; ; )
            {
                if (!edges.ContainsKey(i))
                {
                    edges.Add(i,new Edge<TValue>(-1));
                }
                if (edges[i].startNode == node)
                {
                    if (theString.ElementAt(edges[i].indexOfFirstCharacter).CompareTo(c) == 0)
                    {
                        return edges[i];
                    }                   
                }
                if (edges[i].startNode == -1)
                {
                    return edges[i];
                }
                i = ++i % HASH_TABLE_SIZE;
            }            
        }

        public static int Hash(int node, TValue c)
        {
            int rtnValue = ((node << 8) + c.GetHashCode()) % (int)HASH_TABLE_SIZE;
            return rtnValue;
        }
    }
}