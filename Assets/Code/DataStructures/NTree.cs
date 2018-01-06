using System.Collections;
using System.Collections.Generic;
using UnityEngine;

delegate void TreeVisitor<T>(T nodeData);

/** implemented via  linked list */
[System.Serializable]
class NTreeNode<T>
{
    private T data;
    private LinkedList<NTreeNode<T>> children;

    public NTreeNode(T data)
    {
        this.data = data;
        children = new LinkedList<NTreeNode<T>>();
    }

    public void AddChild(T child)
    {
        children.AddFirst(new NTreeNode<T>(child));
    }

    public NTreeNode<T> GetChild(int i)
    {
        foreach (NTreeNode<T> n in children)
            if (--i == 0)
                return n;
        return null;
    }

    public void Traverse(NTreeNode<T> node, TreeVisitor<T> visitor)
    {
        visitor(node.data);
        foreach (NTreeNode<T> kid in node.children)
            Traverse(kid, visitor);
    }
}

