using System;
using System.Collections.Generic;
using System.Linq;
namespace SuffixTreeLib
{
    public class ST {
    class SuffixTree {
        const int oo = int.MaxValue/2;
        Node [] nodes;
        char [] text;
        int root;
        static int position = -1;
        
        int currentNode;
        int needSuffixLink;
        int remainder;

        int active_node, active_length, active_edge;

        class Node {

            /*
               There is no need to create an "Edge" class.
               Information about the edge is stored right in the node.
               [start; end) interval specifies the edge,
               by which the node is connected to its parent node.
            */

            public int start, end = oo, link;
            public SortedDictionary<char, int> next = new SortedDictionary<char, int>();

            public Node(int start, int end) {
                this.start = start;
                this.end = end;
            }

            public int edgeLength() {
                return Math.Min(end, position + 1) - start;
            }
        }
        public SuffixTree(){}
        public SuffixTree(int length) {
            nodes = new Node[2* length + 2];
            text = new char[length];
            root = active_node = newNode(-1, -1);
        }

        private void addSuffixLink(int node) {
            if (needSuffixLink > 0)
                nodes[needSuffixLink].link = node;
            needSuffixLink = node;
        }

        char ActiveEdge() {
            return text[active_edge];
        }

        bool walkDown(int next) {
            if (active_length >= nodes[next].edgeLength()) {
                active_edge += nodes[next].edgeLength();
                active_length -= nodes[next].edgeLength();
                active_node = next;
                return true;
            }
            return false;
        }

        int newNode(int start, int end) {
            nodes[++currentNode] = new Node(start, end);
            return currentNode;
        }

        public void addChar(char c) {
            text[++position] = c;
            needSuffixLink = -1;
            remainder++;
            while(remainder > 0) {
                if (active_length == 0) active_edge = position;
                if (!nodes[active_node].next.ContainsKey(ActiveEdge())){
                    int leaf = newNode(position, oo);
                    nodes[active_node].next.Add(ActiveEdge(), leaf);
                    addSuffixLink(active_node); //rule 2
                } else {
                    int next = nodes[active_node].next[ActiveEdge()];
                    if (walkDown(next)) continue;   //observation 2
                    if (text[nodes[next].start + active_length] == c) { //observation 1
                        active_length++;
                        addSuffixLink(active_node); // observation 3
                        break;
                    }
                    int split = newNode(nodes[next].start, nodes[next].start + active_length);
                    nodes[active_node].next[ActiveEdge()] = split;
                    int leaf = newNode(position, oo);
                    nodes[split].next.Add(c, leaf);
                    nodes[next].start += active_length;
                    nodes[split].next.Add(text[nodes[next].start], next);
                    addSuffixLink(split); //rule 2
                }
                remainder--;

                if (active_node == root && active_length > 0) {  //rule 1
                    active_length--;
                    active_edge = position - remainder + 1;
                } else
                    active_node = nodes[active_node].link > 0 ? nodes[active_node].link : root; //rule 3
            }
        }

        /*
            printing the Suffix Tree in a format understandable by graphviz. The outAdd is written into
            st.dot file. In order to see the suffix tree as a PNG image, run the following commandin
            dot -Tpng -O st.dot
         */

        string edgeString(int node) {
            //return new String(Array.copyOfRange(text, nodes[node].start, Math.min(position + 1, nodes[node].end)));
            return new string(text.Skip(nodes[node].start).Take(Math.Min(position + 1, nodes[node].end)).ToArray());
        }

        public void printTree() {
            System.Console.WriteLine("digraph {");
            System.Console.WriteLine("\trankdir = LR;");
            System.Console.WriteLine("\tedge [arrowsize=0.4,fontsize=10]");
            System.Console.WriteLine("\tnode1 [label=\"\",style=filled,fillcolor=lightgrey,shape=circle,width=.1,height=.1];");
            System.Console.WriteLine("//------leaves------");
            printLeaves(root);
            System.Console.WriteLine("//------internal nodes------");
            printInternalNodes(root);
            System.Console.WriteLine("//------edges------");
            printEdges(root);
            System.Console.WriteLine("//------suffix links------");
            printSLinks(root);
            System.Console.WriteLine("}");
        }

        void printLeaves(int x) {
            if (nodes[x].next.Count == 0)
                System.Console.WriteLine("\tnode"+x+" [label=\"\",shape=point]");
            else {
                foreach (int child in nodes[x].next.Values)
                    printLeaves(child);
            }
        }

        void printInternalNodes(int x) {
            if (x != root && nodes[x].next.Count > 0)
                System.Console.WriteLine("\tnode"+x+" [label=\"\",style=filled,fillcolor=lightgrey,shape=circle,width=.07,height=.07]");

            foreach (int child in nodes[x].next.Values)
                printInternalNodes(child);
        }

        void printEdges(int x) {
            foreach (int child in nodes[x].next.Values) {
                System.Console.WriteLine("\tnode"+x+" -> node"+child+" [label=\""+edgeString(child)+"\",weight=3]");
                printEdges(child);
            }
        }

        void printSLinks(int x) {
            if (nodes[x].link > 0)
                System.Console.WriteLine("\tnode"+x+" -> node"+nodes[x].link+" [label=\"\",weight=1,style=dotted]");
            foreach (int child in nodes[x].next.Values)
                printSLinks(child);
        }
    }

    public ST(string line){
        
        SuffixTree st = new SuffixTree(line.Length);
        for (int i = 0; i < line.Length; i++)
            st.addChar(line[i]);
        st.printTree();
    }
}
}