using System;
using System.Collections.Generic;

namespace MyTree{

    public class Tree{

    }

    public class Node{
        public Node Parent {get; set;}
        public string Name {get; set;} 
        public List<Node> Children {get; set;} = new();

        public Node(Node p, string n){
            Name = n;
            if (p is not null) Parent = p;
        }

        public Node(string str, string n){
            Name = n;
            
            var strs = str.Split(" ");
            foreach(var s in strs){
                Children.Add(new Node(this, s));
            }
        }
    }
}