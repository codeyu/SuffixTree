/* Suffix.cs
 * To Do: Add comments
 * 
 * This is a suffix tree algorithm for .NET written in C#. Feel free to use it as you please!
 * This code was derived from Mark Nelson's article located here: http://marknelson.us/1996/08/01/suffix-trees/
 * Have Fun 
 * 
 * Zikomo A. Fields 2008
 *  
 */


using System;

namespace SuffixTreeLib2
{
    public class Suffix<TValue> where TValue : IComparable<TValue>
    {
        public int m_OriginNode = 0;
        public int m_IndexOfFirstCharacter;
        public int m_IndexOfLastCharacter;

        public Suffix(int node, int start, int stop)
        {
            this.m_OriginNode = node;
            this.m_IndexOfFirstCharacter = start;
            this.m_IndexOfLastCharacter = stop;
        }

        public Suffix(Suffix<TValue> suffix)
        {
            this.m_OriginNode = suffix.m_OriginNode;
            this.m_IndexOfFirstCharacter = suffix.m_IndexOfFirstCharacter;
            this.m_IndexOfLastCharacter = suffix.m_IndexOfLastCharacter;
        }

        public bool IsExplicit
        {
            get
            {
                return m_IndexOfFirstCharacter > m_IndexOfLastCharacter;
            }        
        }

        public void Canonize(SuffixTree<TValue> tree)
        {
            if (!IsExplicit)
            {
                Edge<TValue> edge = Edge<TValue>.Find(tree.m_Source, tree.Edges, m_OriginNode, tree.m_Source[m_IndexOfFirstCharacter]);
                int edgeSpan = edge.indexOfLastCharacter - edge.indexOfFirstCharacter;
                while (edgeSpan <= (this.m_IndexOfLastCharacter - this.m_IndexOfFirstCharacter))
                {
                    this.m_IndexOfFirstCharacter = this.m_IndexOfFirstCharacter + edgeSpan + 1;
                    this.m_OriginNode = edge.endNode;
                    if (this.m_IndexOfFirstCharacter <= this.m_IndexOfLastCharacter)
                    {
                        edge = Edge<TValue>.Find(tree.m_Source, tree.Edges, edge.endNode, tree.m_Source[this.m_IndexOfFirstCharacter]);
                        edgeSpan = edge.indexOfLastCharacter - edge.indexOfFirstCharacter;
                    }
                }
            }
        }
    }
}