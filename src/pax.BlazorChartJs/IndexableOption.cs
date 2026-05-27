using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace pax.BlazorChartJs;

/// <summary>
/// Represents a Chart.js option that can be a single value, an indexed list of values,
/// or a scriptable JavaScript callback reference.
/// </summary>
[CollectionBuilder(typeof(IndexableOptionBuilder), "Create")]
public class IndexableOption<T> : IEnumerable<T>
{
    private readonly List<T>? _indexedValues;

    public IList<T>? IndexedValues => _indexedValues;

    public T? SingleValue { get; }

    public ChartJsFunction? FunctionValue { get; }

    public IndexableOptionKind Kind { get; }

    public bool IsIndexed => Kind == IndexableOptionKind.Indexed;

    public bool IsFunction => Kind == IndexableOptionKind.Function;

    public IndexableOption(T singleValue)
    {
        SingleValue = singleValue;
        Kind = IndexableOptionKind.SingleValue;
    }

    public IndexableOption(IList<T> indexedValues)
    {
        _indexedValues = [.. indexedValues];
        Kind = IndexableOptionKind.Indexed;
    }

    public IndexableOption(ICollection<T> values)
    {
        _indexedValues = [.. values];
        Kind = IndexableOptionKind.Indexed;
    }

    public IndexableOption(ReadOnlySpan<T> values)
    {
        _indexedValues = [.. values];
        Kind = IndexableOptionKind.Indexed;
    }

    public IndexableOption(ChartJsFunction function)
    {
        FunctionValue = function ?? throw new ArgumentNullException(nameof(function));
        Kind = IndexableOptionKind.Function;
    }

    public int Count => _indexedValues?.Count ?? 0;

    public void Insert(int index, T item)
    {
        EnsureIndexed().Insert(index, item);
    }

    public void Add(T item)
    {
        EnsureIndexed().Add(item);
    }

    public void RemoveAt(int index)
    {
        EnsureIndexed().RemoveAt(index);
    }

    public void Remove(T item)
    {
        _ = EnsureIndexed().Remove(item);
    }

    public static implicit operator IndexableOption<T>(T value)
    {
        return new(value);
    }

    public static implicit operator IndexableOption<T>(ChartJsFunction function)
    {
        return new(function);
    }

#pragma warning disable CA1002
    public static implicit operator IndexableOption<T>(List<T> value)
    {
        return [.. value];
    }
#pragma warning restore CA1002

    public static implicit operator IndexableOption<T>(Collection<T> value)
    {
        return [.. value];
    }

    internal object GetJsonObject()
    {
        return Kind switch
        {
            IndexableOptionKind.SingleValue =>
                SingleValue ?? throw new InvalidOperationException("Single value is null."),

            IndexableOptionKind.Indexed =>
                IndexedValues ?? throw new InvalidOperationException("Indexed values are null."),

            IndexableOptionKind.Function =>
                FunctionValue ?? throw new InvalidOperationException("Function value is null."),

            _ => throw new InvalidOperationException($"Unsupported {nameof(IndexableOptionKind)}: {Kind}.")
        };
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Kind switch
        {
            IndexableOptionKind.Indexed when _indexedValues is not null =>
                _indexedValues.GetEnumerator(),

            IndexableOptionKind.SingleValue when SingleValue is not null =>
                new[] { SingleValue }.AsEnumerable().GetEnumerator(),

            IndexableOptionKind.Function =>
                Enumerable.Empty<T>().GetEnumerator(),

            _ => throw new InvalidOperationException("IndexableOption is not initialized correctly.")
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private List<T> EnsureIndexed()
    {
        return _indexedValues is null
            ? throw new InvalidOperationException(
                "This IndexableOption does not contain indexed values.")
            : _indexedValues;
    }

    public IndexableOption<T> ToIndexableOption()
    {
        throw new NotImplementedException();
    }
}

public enum IndexableOptionKind
{
    SingleValue,
    Indexed,
    Function
}

public static class IndexableOptionBuilder
{
    public static IndexableOption<T> Create<T>(ReadOnlySpan<T> values)
    {
        return new IndexableOption<T>(values);
    }
}