
using System.Text.Json.Serialization;

namespace pax.BlazorChartJs;
#pragma warning disable CA2227
/// <summary>
/// Dataset
/// </summary>    
public class Dataset
{
    /// <summary>
    /// Dataset
    /// </summary>  
    public Dataset()
    {
        Data = new List<double>();
        BackgroundColor = new List<string>();
        BorderColor = new List<string>();
    }

    /// <summary>
    /// Id
    /// </summary>        
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Label
    /// </summary>        
    public string Label { get; set; } = "pax.BlazorChartJs";
    /// <summary>
    /// Data
    /// </summary>            
    public ICollection<double> Data { get; set; }
    /// <summary>
    /// BackgroundColor (css color)
    /// </summary>                
    public ICollection<string> BackgroundColor { get; set; }
    /// <summary>
    /// BorderColor (css color)
    /// </summary>                
    public ICollection<string> BorderColor { get; set; }
    /// <summary>
    /// BorderWidth
    /// </summary>                
    public int BorderWidth { get; set; }
    /// <summary>
    /// Fill
    /// </summary>                
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Fill { get; set; }
    /// <summary>
    /// PointBackgroundColor
    /// </summary>                
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PointBackgroundColor { get; set; }
    /// <summary>
    /// PointBorderColor
    /// </summary>                
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PointBorderColor { get; set; }
    /// <summary>
    /// PointRadius
    /// </summary>                
    public int PointRadius { get; set; }
    /// <summary>
    /// PointBorderWidth
    /// </summary>                
    public int PointBorderWidth { get; set; }
    /// <summary>
    /// PointHitRadius
    /// </summary>                
    public int PointHitRadius { get; set; }
    /// <summary>
    /// Tension
    /// </summary>   
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]             
    public double? Tension { get; set; }
}
#pragma warning restore CA2227