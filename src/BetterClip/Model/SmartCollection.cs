using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BetterClip.Model;


public class SmartCollection<T> : ObservableCollection<T>
{
    public SmartCollection()
    {
    }

    public SmartCollection(IEnumerable<T> collection) : base(collection)
    {
    }

    public SmartCollection(List<T> list) : base(list)
    {
    }

    public void AddRange(IEnumerable<T> range)
    {
        foreach (T obj in range)
            Items.Add(obj);
        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void Reset(IEnumerable<T> range)
    {
        ClearItems();
        if (range == null)
            return;
        AddRange(range);
    }
}
