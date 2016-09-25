/* cs
 * To Do: add comments
 * 
 * 
 * This is a suffix tree algorithm for .NET written in C#. Feel free to use it as you please!
 * This code was derived from Mark Nelson's article located here: http://marknelson.us/1996/08/01/suffix-trees/
 * Have Fun 
 * 
 * Zikomo A. Fields 2008
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuffixTreeLib2
{

    public class SuffixTree<TValue> where TValue : IComparable<TValue>
    {        
        public TValue [] m_Source = null;       
        public Dictionary<int, Edge<TValue>> Edges = null;
        public Dictionary<int, Node<TValue>> Nodes = null;
        public SuffixTree(IEnumerable<TValue> source)
        {
            m_Source = source.ToArray<TValue>();
            Nodes = new Dictionary<int, Node<TValue>>();
            Edges = new Dictionary<int, Edge<TValue>>();            
        }

        public void BuildTree()
        {
            Suffix<TValue> active = new Suffix<TValue>(0, 0, -1);
            for (int i = 0; i <= m_Source.Count() - 1; i++)
            {
                AddPrefix(active, i);
            }
        }

        public bool Search(IEnumerable<TValue> searchPattern)
        {
            bool found = false;
            if (searchPattern.Count() > 0)
            {
                int index = 0;
                Edge<TValue> edge;
                if (!Edges.TryGetValue((int)Edge<TValue>.Hash(0, searchPattern.ElementAt(0)), out edge))
                {
                    return false;
                }

                if (edge.startNode == -1)
                {
                    return false;
                }
                else
                {
                    for (;;)
                    {
                        for (int j = edge.indexOfFirstCharacter; j <= edge.indexOfLastCharacter; j++)
                        {
                            if (index >= searchPattern.Count())
                            {
                                return true;
                            }
                            if (m_Source[j].CompareTo(searchPattern.ElementAt(index++)) != 0)
                            {
                                return false;
                            }
                        }
                        if (index < searchPattern.Count())
                        {
                            Edge<TValue> value;
                            if (Edges.TryGetValue(Edge<TValue>.Hash(edge.endNode, searchPattern.ElementAt(index)), out value))
                            {
                                edge = value;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return found;
        }

        private void AddPrefix(Suffix<TValue> active, int indexOfLastCharacter)
        {
            int parentNode;
            int lastParentNode = -1;

            for (; ; )
            {
                Edge<TValue> edge = new Edge<TValue>(-1);
                parentNode = active.m_OriginNode;

                if (active.IsExplicit)
                {
                    edge = Edge<TValue>.Find(m_Source, Edges, active.m_OriginNode, m_Source[indexOfLastCharacter]);
                    if (edge.startNode != -1)
                    {
                        break;
                    }
                }
                else
                {
                    edge = Edge<TValue>.Find(m_Source, Edges, active.m_OriginNode, m_Source[active.m_IndexOfFirstCharacter]);
                    int span = active.m_IndexOfLastCharacter - active.m_IndexOfFirstCharacter;
                    if (m_Source[edge.indexOfFirstCharacter + span + 1].CompareTo(m_Source[indexOfLastCharacter]) == 0)
                    {
                        break;
                    }
                    parentNode = Edge<TValue>.SplitEdge(this, active, ref edge);
                }

                Edge<TValue> newEdge = new Edge<TValue>(indexOfLastCharacter, m_Source.Length - 1, parentNode);                
                Edge<TValue>.Insert(this, newEdge);
                if (lastParentNode > 0)
                {
                    Nodes[lastParentNode].suffixNode = parentNode;                   
                }
                lastParentNode = parentNode;

                if (active.m_OriginNode == 0)
                {
                    active.m_IndexOfFirstCharacter++;
                }
                else
                {
                    active.m_OriginNode = Nodes[active.m_OriginNode].suffixNode;
                }                
                active.Canonize(this);
            }
            if (lastParentNode > 0)
            {
                Nodes[lastParentNode].suffixNode = parentNode;
            }
            active.m_IndexOfLastCharacter++;
            active.Canonize(this);
        }
    }
}