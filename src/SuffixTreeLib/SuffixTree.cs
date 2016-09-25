using System;
namespace SuffixTreeLib
{
    //fork from http://www.geeksforgeeks.org/generalized-suffix-tree-1/

    public class SuffixTree 
    {
        const int MaxChar = 256;
        string text; //Input string
        SuffixTreeNode Root = null; //Pointer to root node

        /*lastNewNode will point to newly created internal node,
        waiting for it's suffix link to be set, which might get
        a new suffix link (other than root) in next extension of
        same phase. lastNewNode will be set to NULL when last
        newly created internal node (if there is any) got it's
        suffix link reset to new internal node created in next
        extension of same phase. */
        SuffixTreeNode lastNewNode = null;
        SuffixTreeNode activeNode = null;

        /*activeEdge is represeted as input string character
        index (not the character itself)*/
        int activeEdge = -1;
        int activeLength = 0;

        // remainingSuffixCount tells how many suffixes yet to
        // be added in tree
        int remainingSuffixCount = 0;
        int leafEnd = -1;
        int rootEnd = -1;
        int splitEnd = -1;
        int size = -1; //Length of input string

        int EdgeLength(SuffixTreeNode n) {
            if(n == Root)
                return 0;
            return n.End - n.Start + 1;
        }
        void addSuffixLink(SuffixTreeNode node) {
            if (lastNewNode != null)
                lastNewNode.SuffixLink = node;
            lastNewNode = node;
        }
        int walkDown(SuffixTreeNode currNode)
        {
            /*activePoint change for walk down (APCFWD) using
            Skip/Count Trick  (Trick 1). If activeLength is greater
            than current edge length, set next  internal node as
            activeNode and adjust activeEdge and activeLength
            accordingly to represent same activePoint*/
            if (activeLength >= EdgeLength(currNode))
            {
                activeEdge += EdgeLength(currNode);
                activeLength -= EdgeLength(currNode);
                activeNode = currNode;
                return 1;
            }
            return 0;
        }

        void ExtendSuffixTree(int pos)
        {
            /*Extension Rule 1, this takes care of extending all
            leaves created so far in tree*/
            leafEnd = pos;
        
            /*Increment remainingSuffixCount indicating that a
            new suffix added to the list of suffixes yet to be
            added in tree*/
            remainingSuffixCount++;
        
            /*set lastNewNode to NULL while starting a new phase,
            indicating there is no internal node waiting for
            it's suffix link reset in current phase*/
            lastNewNode = null;
        
            //Add all suffixes (yet to be added) one by one in tree
            while(remainingSuffixCount > 0) {
        
                if (activeLength == 0)
                    activeEdge = pos; //APCFALZ
        
                // There is no outgoing edge starting with
                // activeEdge from activeNode
                if (activeNode.Children[(int)text[activeEdge]] == null)
                {
                    //Extension Rule 2 (A new leaf edge gets created)

                   activeNode.Children[(int)text[activeEdge]] =
                                                new SuffixTreeNode(Root, pos, ref leafEnd);
                    
                    
        
                    /*A new leaf edge is created in above line starting
                    from  an existng node (the current activeNode), and
                    if there is any internal node waiting for it's suffix
                    link get reset, point the suffix link from that last
                    internal node to current activeNode. Then set lastNewNode
                    to NULL indicating no more node waiting for suffix link
                    reset.*/
                    //if (lastNewNode != null)
                    //{
                        //lastNewNode.SuffixLink = activeNode;
                        //lastNewNode = null;
                    //}
                    addSuffixLink(activeNode);
                }
                // There is an outgoing edge starting with activeEdge
                // from activeNode
                else
                {
                    // Get the next node at the end of edge starting
                    // with activeEdge
                    SuffixTreeNode next = activeNode.Children[(int)text[activeEdge]];
                    if (walkDown(next) > 0)//Do walkdown
                    {
                        //Start from next node (the new activeNode)
                        continue;
                    }
                    /*Extension Rule 3 (current character being processed
                    is already on the edge)*/
                    if (text[next.Start + activeLength] == text[pos])
                    {
                        //If a newly created node waiting for it's 
                        //suffix link to be set, then set suffix link 
                        //of that waiting node to curent active node
                        //if(lastNewNode != null && activeNode != Root)
                        //{
                            //lastNewNode.SuffixLink = activeNode;
                            //lastNewNode = null;
                        //}
                        addSuffixLink(activeNode);
                        //APCFER3
                        activeLength++;
                        /*STOP all further processing in this phase
                        and move on to next phase*/
                        break;
                    }
        
                    /*We will be here when activePoint is in middle of
                    the edge being traversed and current character
                    being processed is not  on the edge (we fall off
                    the tree). In this case, we add a new internal node
                    and a new leaf edge going out of that new node. This
                    is Extension Rule 2, where a new leaf edge and a new
                    internal node get created*/
                    
                    splitEnd = next.Start + activeLength - 1;
        
                    //New internal node
                    SuffixTreeNode split = new SuffixTreeNode(Root, next.Start, ref splitEnd);
                    activeNode.Children[text[activeEdge]] = split;
        
                    //New leaf coming out of new internal node
                    split.Children[(int)text[pos]] = new SuffixTreeNode(Root, pos, ref leafEnd);
                    next.Start += activeLength;
                    split.Children[(int)text[next.Start]] = next;
        
                    /*We got a new internal node here. If there is any
                    internal node created in last extensions of same
                    phase which is still waiting for it's suffix link
                    reset, do it now.*/
                    //if (lastNewNode != null)
                    //{
                    /*suffixLink of lastNewNode points to current newly
                    created internal node*/
                        //lastNewNode.SuffixLink = split;
                    //}
        
                    /*Make the current newly created internal node waiting
                    for it's suffix link reset (which is pointing to root
                    at present). If we come across any other internal node
                    (existing or newly created) in next extension of same
                    phase, when a new leaf edge gets added (i.e. when
                    Extension Rule 2 applies is any of the next extension
                    of same phase) at that point, suffixLink of this node
                    will point to that internal node.*/
                    //lastNewNode = split;
                    addSuffixLink(split);
                }
        
                /* One suffix got added in tree, decrement the count of
                suffixes yet to be added.*/
                remainingSuffixCount--;
                if (activeNode == Root && activeLength > 0) //APCFER2C1
                {
                    activeLength--;
                    activeEdge = pos - remainingSuffixCount + 1;
                }
                else //APCFER2C2
                {
                    activeNode = activeNode.SuffixLink != null ? activeNode.SuffixLink : Root;   
                }
            }
        }

        void print(int i, int j)
        {
            int k;
            for (k=i; k<=j && text[k] != '#'; k++)
                System.Console.Write("{0}", text[k]);
            if(k<=j)
                System.Console.Write("#");
        }
        
        //Print the suffix tree as well along with setting suffix index
        //So tree will be printed in DFS manner
        //Each edge along with it's suffix index will be printed
        void setSuffixIndexByDFS(SuffixTreeNode n, int labelHeight)
        {
            if (n == null)  return;
        
            if (n.Start != -1) //A non-root node
            {
                //Print the label on edge from parent to current node
                print(n.Start, n.End);
            }
            int leaf = 1;
            int i;
            for (i = 0; i < MaxChar; i++)
            {
                if (n.Children[i] != null)
                {
                    if (leaf == 1 && n.Start != -1)
                        System.Console.Write(" [{0}]\n", n.suffixIndex);
        
                    //Current node is not a leaf as it has outgoing
                    //edges from it.
                    leaf = 0;
                    setSuffixIndexByDFS(n.Children[i], labelHeight +
                                        EdgeLength(n.Children[i]));
                }
            }
            if (leaf == 1)
            {
                for(i= n.Start; i<= n.End; i++)
                {
                    if(text[i] == '#') //Trim unwanted characters
                    {
                        n.End = i;
                    }
                }
                n.suffixIndex = size - labelHeight;
                System.Console.Write(" [{0}]\n", n.suffixIndex);
            }
        }
        
        void freeSuffixTreeByPostOrder(SuffixTreeNode n)
        {
            if (n == null)
                return;
            int i;
            for (i = 0; i < MaxChar; i++)
            {
                if (n.Children[i] != null)
                {
                    freeSuffixTreeByPostOrder(n.Children[i]);
                }
            }
            //if (n.suffixIndex == -1)
                //free(n->end);
            n = null;
        }

        /*Build the suffix tree and print the edge labels along with
        suffixIndex. suffixIndex for leaf edges will be >= 0 and
        for non-leaf edges will be -1*/
        public void BuildSuffixTree(string s)
        {
            text = s;
            size = s.Length;
            int i;
            rootEnd = - 1;
        
            /*Root is a special node with start and end indices as -1,
            as it has no parent from where an edge comes to root*/
            Root = new SuffixTreeNode(Root, -1, ref rootEnd);
        
            activeNode = Root; //First activeNode will be root
            for (i=0; i<size; i++)
            {
                ExtendSuffixTree(i);
            } 
            int labelHeight = 0;
            setSuffixIndexByDFS(Root, labelHeight);
        
            //Free the dynamically allocated memory
            freeSuffixTreeByPostOrder(Root);
        }

    }
}