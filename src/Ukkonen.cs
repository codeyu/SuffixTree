using System;
using System.Collections.Generic;
using System.Text;
public class Ukkonen {
	public class SuffixNode {		
		public StringBuilder sb;
		
	    public List<SuffixNode> children = new List<SuffixNode>();
	    
	    public SuffixNode link;
	    public int start;
	    public int end;
	    public int pathlen;
	    
	    public SuffixNode(StringBuilder sb,int start,int end,int pathlen){	
	    	this.sb = sb;
	    	this.start = start;
	    	this.end = end;
	    	this.pathlen = pathlen;
	    }
	    public SuffixNode(StringBuilder sb){	    
	    	this.sb = sb;
	    	this.start = -1;
	    	this.end = -1;	    
	    	this.pathlen = 0;
	    }
	    public int getLength(){
	    	if(start == -1) return 0;
	    	else return end - start + 1;
	    }
	    public String getString(){
	    	if(start != -1){
	    		return this.sb.ToString().Substring(start,end+1);
	    	}else{
	    		return "";
	    	}
	    }
	    public bool isRoot(){
	    	return start == -1;
	    }
	    public String getCoordinate(){
	    	return "[" + start+".." + end + "/" + this.pathlen + "]";
	    }
	    public String toString(){	    	
	    	return getString() + "(" + getCoordinate() 
	    		+ ",link:" + ((this.link==null)?"N/A":this.link.getCoordinate()) 
	    		+ ",children:" + children.Count +")";
	    }	   
	}
	public class State{
		public SuffixNode u; //parent(v)
		//public SuffixNode w;  
		public SuffixNode v;  
		//public int k; //the global index of text starting from 0 to text.length()
		//public boolean finished;  
	}
	
	public SuffixNode root;
	public StringBuilder sb = new StringBuilder();
	
	public Ukkonen(){
		
	}
	
	//build a suffix-tree for a string of text
	public void  buildSuffixTree(String text) {	
		int m = text.Length;
		
		if(m==0)
			return;
		
		if(root==null){
			root = new SuffixNode(sb);		
			root.link = root; //link to itself
		}
		
		List<SuffixNode> leaves =  new List<SuffixNode>();
		
		//add first node
		sb.Append(text[0]);
		SuffixNode node = new SuffixNode(sb,0,0,1);
		leaves.Add(node);
		root.children.Add(node);	
		int j_star = 0; //j_{i-1}
		
		SuffixNode u = root;
		SuffixNode v = root;			
		for(int i=1;i<=m-1;i++){			
			//do phase i
			sb.Append(text[i]);
			
			//step 1: do implicit extensions 
			foreach(SuffixNode leafnode in leaves){
				leafnode.end++;
				leafnode.pathlen++;
			}
			
			//step 2: do explicit extensions until rule #3 is applied			
			State state = new State();	
			
			//for the first explicit extension, we reuse the last phase's u and do slowscan
			//also note: suffix link doesn't span two phases.
			int j=j_star+1;
			SuffixNode s = u;		 
			int k = s.pathlen + j;		
			state.u = s;			
			state.v = s;  
			SuffixNode newleaf = slowscan(state,s,k);
			if(newleaf == null){
				//if rule #3 is applied, then we can terminate this phase
				j_star = j - 1;
				//Note: no need to update state.v because it is not going to be used
				//at the next phase
				u = state.u;
				continue;
			}else{			
				
				j_star = j;
				leaves.Add(newleaf);
				
				u = state.u;
				v = state.v;
			}		
			j++;
			
			//for other explicit extensions, we start with fast scan.
			for(;j<=i;j++){
				s = u.link;
				
				int uvLen=v.pathlen - u.pathlen;  		
				if(u.isRoot() && !v.isRoot()){
					uvLen--;
				}
				//starting with index k of the text
				k = s.pathlen + j;		
				
				
				//init state
				state.u = s;			
				state.v = s; //if uvLen = 0 
				
				//execute fast scan
				newleaf = fastscan(state,s,uvLen,k);				
				//establish the suffix link with v		
				v.link = state.v;
				
				if(newleaf == null){
					//if rule #3 is applied, then we can terminate this phase
					j_star = j - 1;
					u = state.u;
					break;
				}else{
					
					j_star = j;
					leaves.Add(newleaf);
					
					u = state.u;
					v = state.v;
				}			
			}
		}
	}
	//slow scan from currNode until state.v is found
	//return the new leaf if a new one is created right after v;
	//return null otherwise (i.e. when rule #3 is applied)
	public SuffixNode slowscan(State state,SuffixNode currNode,int k){
		SuffixNode newleaf = null;
		
		bool done = false;		
		int keyLen = sb.Length- k;
		for(int i=0;i<currNode.children.Count;i++){
			SuffixNode child = currNode.children[i];
			
			//use min(child.key.length, key.length)			
			int childKeyLen = child.getLength();
			int len = childKeyLen<keyLen?childKeyLen:keyLen;
			int delta = 0;
			for(;delta<len;delta++){
				if(sb[k+delta] != sb[child.start+delta]){
					break;
				}
			}
			if(delta==0){//this child doesn't match	any character with the new key			
				//order keys by lexi-order
				if(sb[k] < sb[child.start]){
					//e.g. child="e" (currNode="abc")
					//	   abc                     abc
					//    /  \    =========>      / | \
					//   e    f   insert "c"     c  e  f
					int pathlen = sb.Length - k + currNode.pathlen;
					SuffixNode node = new SuffixNode(sb,k,sb.Length-1,pathlen);
					currNode.children.Insert(i,node);		
					//state.u = currNode; //currNode is already registered as state.u, so commented out
					state.v = currNode;
					newleaf = node;
					done = true;
					break;					
				}else{ //key.charAt(0)>child.key.charAt(0)
					//don't forget to add the largest new key after iterating all children
					continue;
				}
			}else{//current child's key partially matches with the new key	
				if(delta==len){
					if(keyLen==childKeyLen){						
						//e.g. child="ab"
						//	   ab                    ab
						//    /  \    =========>    /  \
						//   e    f   insert "ab"  e    f
						//terminate this phase  (implicit tree with rule #3)		
						state.u = child;
						state.v = currNode;
					}else if(keyLen>childKeyLen){ 
						//TODO: still need an example to test this condition
						//e.g. child="ab"
						//	   ab                      ab
						//    /  \    ==========>     / | \ 							
						//   e    f   insert "abc"   c e  f		
						//recursion
						state.u = child;
						state.v = child;
						k += childKeyLen;
						//state.k = k;
						newleaf = slowscan(state,child,k);
					}
					else{ //keyLen<childKeyLen
						//e.g. child="abc"
						//	   abc                      abc
						//    /   \      =========>     /  \ 
						//   e     f     insert "ab"   e   f	   
						//					          
						//terminate this phase  (implicit tree with rule #3)
						//state.u = currNode;
						state.v = currNode;
					}
				}else{//0<delta<len 
			
					//e.g. child="abc"
					//	   abc                     ab
					//    /  \     ==========>     / \
					//   e    f   insert "abd"    c   d 
					//                           /  \
					//                          e    f					
					//insert the new node: ab 
					int nodepathlen = child.pathlen 
							- (child.getLength()-delta);
					SuffixNode node = new SuffixNode(sb,
							child.start,child.start + delta - 1,nodepathlen); 
					node.children = new List<SuffixNode>();
					
					int leafpathlen = (sb.Length - (k + delta)) + nodepathlen;
					SuffixNode leaf = new SuffixNode(sb,
							k+delta,sb.Length-1,leafpathlen);
					
					//update child node: c
					child.start += delta;
					if(sb[k+delta]<sb[child.start]){
						node.children.Add(leaf);
						node.children.Add(child);
					}else{
						node.children.Add(child);
						node.children.Add(leaf);							
					}
					//update parent
					currNode.children.Insert(i, node);
					
					//state.u = currNode; //currNode is already registered as state.u, so commented out
					state.v = node;
					newleaf = leaf;			
				}
				done = true;
				break;
			}
		}
		if(!done){
			int pathlen = sb.Length - k + currNode.pathlen;
			SuffixNode node = new SuffixNode(sb,k,sb.Length-1,pathlen);
			currNode.children.Add(node);
			//state.u = currNode; //currNode is already registered as state.u, so commented out
			state.v = currNode;	
			newleaf = node;
		}
		
		return newleaf;
	}
	
	
	//fast scan until state.v is found;
	//return the new leaf if a new one is created right after v;
	//return null otherwise (i.e. when rule #3 is applied)
	public SuffixNode fastscan(State state,SuffixNode currNode,int uvLen,int k){				
		if(uvLen==0){
			//state.u = currNode; //currNode is already registered as state.u, so commented out
			//continue with slow scan
			return slowscan(state,currNode,k);	
		}
		
		SuffixNode newleaf = null;
		bool done  = false;
		for(int i=0;i<currNode.children.Count;i++){
			SuffixNode child = currNode.children[i];
			
			if(sb[child.start] == sb[k]){				
				int len = child.getLength();
				if(uvLen==len){
					//then we find v			
					//uvLen = 0;					
					state.u = child;	
					//state.v = child;
					k += len;
					//state.k = k;
					
					//continue with slow scan
					newleaf = slowscan(state,child,k);					
				}else if(uvLen<len){
					//we know v must be an internal node; branching	and cut child short								
					//e.g. child="abc",uvLen = 2
					//	   abc                          ab
					//    /  \    ================>     / \
					//   e    f   suffix part: "abd"   c   d 
					//                                /  \
					//                               e    f				
					
					//insert the new node: ab; child is now c 
					int nodepathlen = child.pathlen 
							- (child.getLength()-uvLen);
					SuffixNode node = new SuffixNode(sb,
							child.start,child.start + uvLen - 1,nodepathlen); 
					node.children = new List<SuffixNode>();
					
					int leafpathlen = (sb.Length - (k + uvLen)) + nodepathlen;
					SuffixNode leaf = new SuffixNode(sb,
							k+uvLen,sb.Length-1,leafpathlen);
					
					//update child node: c
					child.start += uvLen;
					if(sb[k+uvLen]<sb[child.start]){
						node.children.Add(leaf);
						node.children.Add(child);
					}else{
						node.children.Add(child);
						node.children.Add(leaf);							
					}
			
					//update parent
					currNode.children.Insert(i, node);
					
					//uvLen = 0;
					//state.u = currNode; //currNode is already registered as state.u, so commented out
					state.v = node;				
					newleaf = leaf;
				}else{//uvLen>len
					//e.g. child="abc", uvLen = 4
					//	   abc                          
					//    /  \    ================>      
					//   e    f   suffix part: "abcde"   
					//                                
					//                  
					//jump to next node
					uvLen -= len;
					state.u = child;
					//state.v = child;
					k += len;
					//state.k = k;
					newleaf = fastscan(state,child,uvLen,k);
				}
				done = true;
				break;
			}
		}		
		if(!done){			
			//TODO: still need an example to test this condition
			//add a leaf under the currNode
			int pathlen = sb.Length - k + currNode.pathlen;
			SuffixNode node = new SuffixNode(sb,k,sb.Length-1,pathlen);
			currNode.children.Add(node);
			//state.u = currNode; //currNode is already registered as state.u, so commented out
			state.v = currNode;	
			newleaf = node;
		}
		
		return newleaf;
	}
	
	//for test purpose only
	public void printTree(){
		Console.WriteLine("The suffix tree for S = {0} is: \n",this.sb);
		this.print(0, this.root);
	}
	public void print(int level, SuffixNode node){
		for (int i = 0; i < level; i++) {
            Console.Write(" ");
        }
		Console.Write("|");
        for (int i = 0; i < level; i++) {
        	Console.Write("-");
        }
        //Console.WriteLine("%s(%d..%d/%d)%n", node.getString(),node.start,node.end,node.pathlen);
        Console.Write("({0},{1})\n", node.start,node.end);
        foreach (SuffixNode child in node.children) {
        	print(level + 1, child);
        }		
	}
	
}
