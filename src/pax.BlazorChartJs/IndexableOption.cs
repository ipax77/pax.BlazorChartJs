using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace pax.BlazorChartJs;
/// <summary>
/// Represents an object that can be either a single value or an IList of values. This is used for type safe js-interop.
/// </summary>
/// <typeparam name="T">The type of data this <see cref="IndexableOption{T}"/> is supposed to hold.</typeparam>
[CollectionBuilder(typeof(IndexableOptionBuilder), "Create")]
public class IndexableOption<T> : IEnumerable<T>
{
    private List<T>? _indexedValues;

    /// <summary>
    /// The indexed values represented by this instance.
    /// </summary>
    public IList<T>? IndexedValues
    {
        get
        {
            return _indexedValues;
        }
    }

    private T? _singleValue;

    /// <summary>
    /// The single value represented by this instance.
    /// </summary>
    public T? SingleValue
    {
        get
        {
            return _singleValue;
        }
    }

    /// <summary>
    /// Gets the value indicating whether the option wrapped in this <see cref="IndexableOption{T}"/> is indexed.
    /// <para>True if the wrapped value represents an IList of <typeparamref name="T"/>, false if it represents a single value of <typeparamref name="T"/>.</para>
    /// </summary>
    public bool IsIndexed { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="IndexableOption{T}"/> which represents a single value.
    /// </summary>
    /// <param name="singleValue">The single value this <see cref="IndexableOption{T}"/> should represent.</param>
    public IndexableOption(T singleValue)
    {
        _singleValue = singleValue;
        IsIndexed = false;
    }

    /// <summary>
    /// Creates a new instance of <see cref="IndexableOption{T}"/> which represents an IList of values.
    /// </summary>
    /// <param name="indexedValues">The IList of values this <see cref="IndexableOption{T}"/> should represent.</param>
    public IndexableOption(IList<T> indexedValues)
    {
        _indexedValues = new List<T>(indexedValues);
        IsIndexed = true;
    }

    public IndexableOption(ICollection<T> values)
    {
        _indexedValues = new List<T>(values);
        IsIndexed = true;
    }

    public IndexableOption(ReadOnlySpan<T> values)
    {
        _indexedValues = [.. values];
        IsIndexed = true;
    }

    public int Count => _indexedValues == null ? 0 : _indexedValues.Count;

    public void Insert(int index, T item)
    {
        _indexedValues?.Insert(index, item);
    }

    public void Add(T item)
    {
        _indexedValues?.Add(item);
    }

    public void RemoveAt(int index)
    {
        _indexedValues?.RemoveAt(index);
    }

    public void Remove(T item)
    {
        _indexedValues?.Remove(item);
    }

    public IndexableOption<T> FromT(T value)
        => new(value);

    public static implicit operator IndexableOption<T>(T value)
        => new(value);

#pragma warning disable CA1002 // Do not expose generic lists
    public IndexableOption<T> FromList(List<T> value)
        => new(value);

    public static implicit operator IndexableOption<T>(List<T> value)
        => new(value);
#pragma warning restore CA1002 // Do not expose generic lists

    public IndexableOption<T> FromCollection(Collection<T> value)
    => new(value);

    public static implicit operator IndexableOption<T>(Collection<T> value)
        => new(value);

    internal object GetJsonObject()
    {
        return IsIndexed ?
              IndexedValues ?? throw new ArgumentNullException()
            : SingleValue ?? throw new ArgumentNullException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_indexedValues is not null)
        {
            return _indexedValues.GetEnumerator();
        }
        else if (_singleValue is not null)
        {
            return new List<T>() { _singleValue }.GetEnumerator();
        }
        else
        {
            throw new ArgumentNullException(nameof(IndexedValues));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}


public static class IndexableOptionBuilder
{
    public static IndexableOption<T> Create<T>(ReadOnlySpan<T> values) => new IndexableOption<T>(values);
}

