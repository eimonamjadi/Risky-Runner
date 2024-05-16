using System;

public class BinaryHeap
{
    public Node root;
    public Node insert;
    public int count = 0;

    public BinaryHeap()
    {
        count = 0;
    }

    public virtual void Add(Node node)
    {
        if (root == null)
        {
            root = node;
            insert = node;
            count++;
        }
        else
        {
            if (insert.left == null)
            {
                insert.left = node;
                node.parent = insert;

            }
            else
            {
                insert.right = node;
                node.parent = insert;

                //AdjustInsertPos();
            }
            //Balance(node);
        }
    }

    
}

public class Node
{
    public Node(int data) 
    {
        Number = data;
    }

    public Node left;
    public Node right;
    public Node parent;

    public int Number;
}