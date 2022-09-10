namespace pax.BlazorChartJs;
/// <summary>
/// Represents an object that can be either a single value or an IList of values. This is used for type safe js-interop.
/// Modified version of <see href="https://github.com/mariusmuntean/ChartJs.Blazor/blob/master/src/ChartJs.Blazor/Common/IndexableOption.cs">ChartJs.Blazor</see>. All credits!
/// </summary>
/// <typeparam name="T">The type of data this <see cref="IndexableOption{T}"/> is supposed to hold.</typeparam>
public class IndexableOption<T>
{
    private IList<T>? _indexedValues;

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

    internal object GetJsonObject()
    {
        return IsIndexed ?
              IndexedValues ?? throw new ArgumentNullException()
            : SingleValue ?? throw new ArgumentNullException();
    }
}
