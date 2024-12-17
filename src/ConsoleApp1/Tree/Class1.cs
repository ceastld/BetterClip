using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace ConsoleApp1.Tree;

public interface ITreeNode
{
    IList<ITreeNode> Children { get; }
}

public interface ITreeCollection<TNode> where TNode : ITreeNode
{
    ICollection<TNode> Roots { get; }
}



public class TreeSourceCache<TNode, TKey> where TNode : notnull, ITreeNode where TKey : notnull
{
    SourceCache<TNode, TKey> cache = default!;
    public TreeSourceCache(TNode root)
    {

    }
}

