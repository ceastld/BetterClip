// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.Specialized;
using BetterClip.Extension;
using Wpf.Ui.Controls;

namespace BetterClip.Helpers;

public enum ListOperationEnum
{
    Insert,
    InsertRange,
    Remove,
    RemoveAt,
    Add,
    AddRange,
    Update,
    Move,
}

public static class ListOperation
{
    public static ListOperation<TItem> Insert<TItem>(TItem item, int index, object? source = null) => new(ListOperationEnum.Insert, item, index, source);
    public static ListOperation<TItem> InsertRange<TItem>(IList<TItem> items, int index, object? source = null) => new(ListOperationEnum.InsertRange, default!, index, source: source) { Items = items };
    public static ListOperation<TItem> RemoveAt<TItem>(TItem item, int index, object? source = null) => new(ListOperationEnum.Remove, item, index, source: source);
    public static ListOperation<TItem> Remove<TItem>(TItem item, object? source = null) => new(ListOperationEnum.Remove, item, source: source);
    public static ListOperation<TItem> Add<TItem>(TItem item, object? source = null) => new(ListOperationEnum.Add, item, source: source);
    public static ListOperation<TItem> AddRange<TItem>(IList<TItem> items, object? source = null) => new(ListOperationEnum.AddRange, default!, source: source) { Items = items };
    public static ListOperation<TItem> Update<TItem>(TItem item, int index, object? source = null) => new(ListOperationEnum.Update, item, index, source: source);
    public static ListOperation<TItem> Move<TItem>(int oldIndex, int newIndex, object? source = null) => new(ListOperationEnum.Move, default!, source: source) { OldIndex = oldIndex, NewIndex = newIndex };

    public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list), "The list cannot be null.");
        }

        if (oldIndex < 0 || oldIndex >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(oldIndex), "The old index is out of range.");
        }

        if (newIndex < 0 || newIndex >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newIndex), "The new index is out of range.");
        }

        if (oldIndex == newIndex) return;

        T item = list[oldIndex];
        list.RemoveAt(oldIndex);

        // Adjust the index for insertion after removal
        if (newIndex > oldIndex) newIndex--;

        list.Insert(newIndex, item);
    }

    public static void Apply<TTarget>(IList<TTarget> collection, NotifyCollectionChangedEventArgs e)
    {
        
    }
}

public class ListOperation<TItem>(ListOperationEnum optype, TItem item, int index = 0, object? source = null)
{
    public ListOperationEnum OpType { get; set; } = optype;
    public TItem NewItem { get; set; } = item;
    public IList<TItem> Items { get; set; } = [];
    public int OldIndex { get; set; }
    public int NewIndex { get; set; } = index;
    public object? Source { get; set; } = source;
    public void Apply(IList<TItem> list, Action<TItem>? insertAction = null, Action<TItem>? removeAction = null)
    {
        Apply(list, x => x, insertAction, removeAction);
    }
    public void Apply<TTarget>(IList<TTarget> collection,
                               Func<TItem, TTarget> converter,
                               Action<TTarget>? insertAction = null,
                               Action<TTarget>? removeAction = null)
    {
        var item = converter(NewItem);
        var index = NewIndex.Limit(0, collection.Count);
        switch (OpType)
        {
            case ListOperationEnum.Insert:
                collection.Insert(index, item);
                insertAction?.Invoke(item);
                break;
            case ListOperationEnum.InsertRange:
                foreach (var _item in Items.Select(converter).Reverse().ToList())
                {
                    collection.Insert(index, _item);
                    insertAction?.Invoke(_item);
                }
                break;
            case ListOperationEnum.Remove:
                collection.Remove(item);
                removeAction?.Invoke(item);
                break;
            case ListOperationEnum.RemoveAt:
                collection.RemoveAt(index);
                removeAction?.Invoke(item);
                break;
            case ListOperationEnum.Add:
                collection.Add(item);
                insertAction?.Invoke(item);
                break;
            case ListOperationEnum.AddRange:
                foreach (var _item in Items.Select(converter).ToList())
                {
                    collection.Add(_item);
                    insertAction?.Invoke(_item);
                }
                break;
            case ListOperationEnum.Update:
                collection[NewIndex] = item;
                insertAction?.Invoke(item);
                break;
            case ListOperationEnum.Move:
                collection.Move(OldIndex, NewIndex);
                break;
            default:
                break;
        }
    }
}
