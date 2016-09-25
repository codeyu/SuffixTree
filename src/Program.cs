using System;
using SuffixTreeLib2;
using SuffixTreeLib;
namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {             
            //test suffix-tree
		System.Console.WriteLine("****************************");		
		String text = "xbxb^"; //the last char must be unique!
		Ukkonen stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();
		
		System.Console.WriteLine("****************************");		
		text = "mississippi^";
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();
		
		System.Console.WriteLine("****************************");		
		text = "GGGGGGGGGGGGCGCAAAAGCGAGCAGAGAGAAAAAAAAAAAAAAAAAAAAAA^";
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();
		
		System.Console.WriteLine("****************************");		
		text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ^";
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();

		System.Console.WriteLine("****************************");		
		text = "AAAAAAAAAAAAAAAAAAAAAAAAAA^";
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();
		
		System.Console.WriteLine("****************************");		
		text = "minimize";  //the last char e is different from other chars, so it is ok.
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();
		
				
		System.Console.WriteLine("****************************");		
		//the example from McCreight's: A Space-Economical Suffix Tree Construction Algorithm
		text = "bbbbbababbbaabbbbbc^";
		stree = new Ukkonen();
		stree.buildSuffixTree(text);
		stree.printTree();  
        }
    }
}
