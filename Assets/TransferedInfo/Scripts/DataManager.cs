using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{

    //A linked list containing all created nodes
    LinkedList<Node> nodes = new LinkedList<Node>();

    /// <summary>
    /// Searches the entire node map (all existing nodes) for a node containing a certain file name.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>Returns node of found node; or if a node doesn't exist then it is </returns>
    Node FindNodeWithinNodemap(string fileName, string productName, int addedSugar)
    {
        foreach (Node node in nodes)
        {
            if (node.filename.Equals(fileName))
            {
                //Node Found and it Exists
                return node;
            }
        }

        //Node not found, thus a new node is created.
        return CreateNewNode(fileName, productName, addedSugar);
    }

    //Creates a new node, automatically called when a node is searched and not found.
    Node CreateNewNode(string fileName, string productName, int addedSugar)
    {
        Node newNode = new Node(fileName, productName, addedSugar);
        nodes.AddLast(newNode);
        return newNode;
    }
}

class Node
{
    /// <summary>
    /// Represents all the connections from this node to other nodes.
    /// </summary>
    LinkedList<NodeLinker> connections;

    public string filename, productname;
    public int addedSugar;

    public Node(string filename, string productname, int addedSugar)
    {
        this.filename = filename;
        this.productname = productname;
        this.addedSugar = addedSugar;
        connections = new LinkedList<NodeLinker>();
    }

    /*
    public void AddConnection(Node otherNode)
    {
        //If a link to othernode doesn't already exist.
        if (!FindNodeLinkFromConnections(otherNode))
        {
            NodeLinker link = new NodeLinker(otherNode, 1);
            otherNode.AddConnection(this);
        }
    }

    */

    /// <summary>
    /// Finds a node link to a node.
    /// </summary>
    /// <param name="node">node we're looking for a link with</param>
    /// <returns>returns true if a node is found, false if not.</returns>
    public bool FindNodeLinkFromConnections(Node node)
    {
        NodeLinker foundNodeLink = null;
        foreach (NodeLinker nodeLinker in connections)
        {
            //A Connection Between Nodes Already Exists
            if (nodeLinker.linkedNodeA.Equals(node) || nodeLinker.linkedNodeB.Equals(node))
            {
                foundNodeLink = nodeLinker;
                foundNodeLink.frequency++;
                return true;
            }
        }

        //Nodes are not connected
        return false;
    }

    /// <summary>
    /// Gets all the paths that can be accessed from a node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// 
    /*
    public LinkedList<NodeLinker> GetAllConnectedNodeLinks(Node node)
    {
        //TODO
        LinkedList<NodeLinker> links = new LinkedList<NodeLinker>();
        LinkedList<Node> checkedNodes = new LinkedList<Node>();
    }

    */

}

/// <summary>
/// A Node Linker represents a connection between two different nodes within a node map.
/// </summary>
class NodeLinker
{
    /// <summary>
    /// the linker is connected to this node.
    /// </summary>
    public Node linkedNodeA, linkedNodeB;
    /// <summary>
    /// number of times the node has appeared alongside the linkedNode.
    /// </summary>
    public int frequency;

    public NodeLinker(Node nodeA, Node nodeB, int freq)
    {
        linkedNodeA = nodeA;
        linkedNodeB = nodeB;

        frequency = freq;
    }
}



