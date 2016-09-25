namespace SuffixTreeLib
{
    public class SuffixTreeNode
    {
        const int MaxChar = 256;
        public SuffixTreeNode(SuffixTreeNode node, int start, ref int end) 
        {
            Children = new SuffixTreeNode[MaxChar]; 
            this.Start = start;
            this.End = end;    
            this.suffixIndex = -1;
            this.SuffixLink = node;
        }
        public SuffixTreeNode[] Children { get; }

        //pointer to other node via suffix link
        public SuffixTreeNode SuffixLink { get; set; }

        /*(start, end) interval specifies the edge, by which the
        node is connected to its parent node. Each edge will
        connect two nodes,  one parent and one child, and
        (start, end) interval of a given edge  will be stored
        in the child node. Lets say there are two nods A and B
        connected by an edge with indices (5, 8) then this
        indices (5, 8) will be stored in node B. */
        public int Start { get; set; }
        
        
        public int End { get; set; }
        
        
    
        /*for leaf nodes, it stores the index of suffix for
        the path  from root to leaf*/
        public int suffixIndex { get; set; }
    }
}