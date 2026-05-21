using System.Text.Json;

namespace pax.BlazorChartJs.tests;

[TestClass]
public sealed class MissingPropertiesSerializationTests
{
    [TestMethod]
    public void DatasetCommonMissingPropertiesSerializeFromBaseDataset()
    {
        BarDataset dataset = new()
        {
            Animation = false,
            Animations = new Animations()
            {
                X = false
            },
            Normalized = true,
            Transitions = new Dictionary<string, object>()
            {
                ["show"] = false
            }
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.IsFalse(root.GetProperty("animation").GetBoolean());
        Assert.IsFalse(root.GetProperty("animations").GetProperty("x").GetBoolean());
        Assert.IsTrue(root.GetProperty("normalized").GetBoolean());
        Assert.IsFalse(root.GetProperty("transitions").GetProperty("show").GetBoolean());
    }

    [TestMethod]
    public void BubbleAxisAndStackPropertiesSerializeWithChartJsPropertyNames()
    {
        BubbleDataset dataset = new()
        {
            IndexAxis = "y",
            Stack = "revenue",
            XAxisID = "x1",
            YAxisID = "y1"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual("y", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("revenue", root.GetProperty("stack").GetString());
        Assert.AreEqual("x1", root.GetProperty("xAxisID").GetString());
        Assert.AreEqual("y1", root.GetProperty("yAxisID").GetString());
    }

    [TestMethod]
    public void PieArcPropertiesSerializeScalarsAndScriptableMarkers()
    {
        PieDataset dataset = new()
        {
            BorderDash = ChartJsFunction.FromName("resolveArcDash"),
            BorderDashOffset = 2,
            Circular = true,
            HoverBorderDash = ChartJsFunction.FromName("resolveHoverArcDash"),
            HoverBorderDashOffset = 3,
            IndexAxis = "x",
            Label = "Doughnut Dataset",
            Order = 4,
            SelfJoin = false,
            Stack = "rings"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual("resolveArcDash", GetMarkerName(root.GetProperty("borderDash")));
        Assert.AreEqual(2, root.GetProperty("borderDashOffset").GetDouble());
        Assert.IsTrue(root.GetProperty("circular").GetBoolean());
        Assert.AreEqual("resolveHoverArcDash", GetMarkerName(root.GetProperty("hoverBorderDash")));
        Assert.AreEqual(3, root.GetProperty("hoverBorderDashOffset").GetDouble());
        Assert.AreEqual("x", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("Doughnut Dataset", root.GetProperty("label").GetString());
        Assert.AreEqual(4, root.GetProperty("order").GetInt32());
        Assert.IsFalse(root.GetProperty("selfJoin").GetBoolean());
        Assert.AreEqual("rings", root.GetProperty("stack").GetString());
    }

    [TestMethod]
    public void PolarAreaArcPropertiesSerializeScalarsAndScriptableMarkers()
    {
        PolarAreaDataset dataset = new()
        {
            Angle = 90,
            BorderDash = ChartJsFunction.FromName("resolvePolarDash"),
            BorderDashOffset = 1.5,
            BorderJoinStyle = "round",
            BorderRadius = ChartJsFunction.FromName("resolvePolarRadius"),
            Circumference = 180,
            HoverBorderDash = ChartJsFunction.FromName("resolvePolarHoverDash"),
            HoverBorderDashOffset = 2.5,
            HoverOffset = 8,
            IndexAxis = "r",
            Label = "Polar Dataset",
            Offset = 6,
            Order = 7,
            Rotation = 45,
            SelfJoin = true,
            Spacing = 3,
            Stack = "polar",
            Weight = 2
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.AreEqual(90, root.GetProperty("angle").GetDouble());
        Assert.AreEqual("resolvePolarDash", GetMarkerName(root.GetProperty("borderDash")));
        Assert.AreEqual(1.5, root.GetProperty("borderDashOffset").GetDouble());
        Assert.AreEqual("round", root.GetProperty("borderJoinStyle").GetString());
        Assert.AreEqual("resolvePolarRadius", GetMarkerName(root.GetProperty("borderRadius")));
        Assert.AreEqual(180, root.GetProperty("circumference").GetDouble());
        Assert.AreEqual("resolvePolarHoverDash", GetMarkerName(root.GetProperty("hoverBorderDash")));
        Assert.AreEqual(2.5, root.GetProperty("hoverBorderDashOffset").GetDouble());
        Assert.AreEqual(8, root.GetProperty("hoverOffset").GetDouble());
        Assert.AreEqual("r", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual("Polar Dataset", root.GetProperty("label").GetString());
        Assert.AreEqual(6, root.GetProperty("offset").GetDouble());
        Assert.AreEqual(7, root.GetProperty("order").GetInt32());
        Assert.AreEqual(45, root.GetProperty("rotation").GetDouble());
        Assert.IsTrue(root.GetProperty("selfJoin").GetBoolean());
        Assert.AreEqual(3, root.GetProperty("spacing").GetDouble());
        Assert.AreEqual("polar", root.GetProperty("stack").GetString());
        Assert.AreEqual(2, root.GetProperty("weight").GetDouble());
    }

    [TestMethod]
    public void RadarUnprefixedLineAndPointPropertiesSerialize()
    {
        RadarDataset dataset = new()
        {
            CapBezierPoints = true,
            CubicInterpolationMode = "monotone",
            DrawActiveElementsOnTop = false,
            HitRadius = 5,
            HoverRadius = ChartJsFunction.FromName("resolveHoverRadius"),
            IndexAxis = "x",
            Radius = 4,
            Rotation = 45,
            Segment = new { borderColor = "red" },
            ShowLine = true,
            Stack = "radar",
            Stepped = "middle",
            XAxisID = "xRadar",
            YAxisID = "yRadar"
        };

        using var document = SerializeToDocument(dataset);
        var root = document.RootElement;

        Assert.IsTrue(root.GetProperty("capBezierPoints").GetBoolean());
        Assert.AreEqual("monotone", root.GetProperty("cubicInterpolationMode").GetString());
        Assert.IsFalse(root.GetProperty("drawActiveElementsOnTop").GetBoolean());
        Assert.AreEqual(5, root.GetProperty("hitRadius").GetDouble());
        Assert.AreEqual("resolveHoverRadius", GetMarkerName(root.GetProperty("hoverRadius")));
        Assert.AreEqual("x", root.GetProperty("indexAxis").GetString());
        Assert.AreEqual(4, root.GetProperty("radius").GetDouble());
        Assert.AreEqual(45, root.GetProperty("rotation").GetDouble());
        Assert.AreEqual("red", root.GetProperty("segment").GetProperty("borderColor").GetString());
        Assert.IsTrue(root.GetProperty("showLine").GetBoolean());
        Assert.AreEqual("radar", root.GetProperty("stack").GetString());
        Assert.AreEqual("middle", root.GetProperty("stepped").GetString());
        Assert.AreEqual("xRadar", root.GetProperty("xAxisID").GetString());
        Assert.AreEqual("yRadar", root.GetProperty("yAxisID").GetString());
    }

    [TestMethod]
    public void AxisTooltipAndLegendLabelMissingPropertiesSerialize()
    {
        CartesianAxis axis = new()
        {
            Clip = true,
            Labels = ["ON", "OFF"],
            Position = "right"
        };
        Tooltip tooltip = new()
        {
            Animation = false,
            Animations = false,
            Axis = "xy",
            IncludeInvisible = true
        };
        Labels labels = new()
        {
            BorderRadius = 6,
            UseBorderRadius = true
        };

        using var axisDocument = SerializeToDocument(axis);
        using var tooltipDocument = SerializeToDocument(tooltip);
        using var labelsDocument = SerializeToDocument(labels);

        Assert.IsTrue(axisDocument.RootElement.GetProperty("clip").GetBoolean());
        Assert.AreEqual("ON", axisDocument.RootElement.GetProperty("labels")[0].GetString());
        Assert.AreEqual("OFF", axisDocument.RootElement.GetProperty("labels")[1].GetString());
        Assert.AreEqual("right", axisDocument.RootElement.GetProperty("position").GetString());
        Assert.IsFalse(tooltipDocument.RootElement.GetProperty("animation").GetBoolean());
        Assert.IsFalse(tooltipDocument.RootElement.GetProperty("animations").GetBoolean());
        Assert.AreEqual("xy", tooltipDocument.RootElement.GetProperty("axis").GetString());
        Assert.IsTrue(tooltipDocument.RootElement.GetProperty("includeInvisible").GetBoolean());
        Assert.AreEqual(6, labelsDocument.RootElement.GetProperty("borderRadius").GetDouble());
        Assert.IsTrue(labelsDocument.RootElement.GetProperty("useBorderRadius").GetBoolean());
    }

    [TestMethod]
    public void DatasetScriptableOptionsSerializeMarkersAndPreserveUnionShapes()
    {
        LineDataset lineDataset = new()
        {
            BorderCapStyle = ChartJsFunction.FromName("lineCap"),
            BorderDash = new List<double> { 4, 2 },
            BorderDashOffset = ChartJsFunction.FromName("lineDashOffset"),
            BorderJoinStyle = ChartJsFunction.FromName("lineJoin"),
            BorderWidth = ChartJsFunction.FromName("lineWidth"),
            CubicInterpolationMode = ChartJsFunction.FromName("lineInterpolation"),
            Fill = ChartJsFunction.FromName("lineFill"),
            HoverBorderCapStyle = ChartJsFunction.FromName("lineHoverCap"),
            HoverBorderDash = ChartJsFunction.FromName("lineHoverDash"),
            HoverBorderDashOffset = ChartJsFunction.FromName("lineHoverDashOffset"),
            HoverBorderJoinStyle = ChartJsFunction.FromName("lineHoverJoin"),
            HoverBorderWidth = ChartJsFunction.FromName("lineHoverWidth"),
            PointStyle = false,
            Segment = new
            {
                BorderColor = ChartJsFunction.FromName("lineSegmentColor"),
                BorderDash = ChartJsFunction.FromName("lineSegmentDash")
            },
            Stepped = "middle",
            Tension = ChartJsFunction.FromName("lineTension")
        };
        RadarDataset radarDataset = new()
        {
            BorderCapStyle = ChartJsFunction.FromName("radarCap"),
            BorderDash = ChartJsFunction.FromName("radarDash"),
            BorderDashOffset = ChartJsFunction.FromName("radarDashOffset"),
            BorderJoinStyle = ChartJsFunction.FromName("radarJoin"),
            BorderWidth = ChartJsFunction.FromName("radarWidth"),
            Fill = ChartJsFunction.FromName("radarFill"),
            HoverBorderCapStyle = ChartJsFunction.FromName("radarHoverCap"),
            HoverBorderDash = ChartJsFunction.FromName("radarHoverDash"),
            HoverBorderDashOffset = ChartJsFunction.FromName("radarHoverDashOffset"),
            HoverBorderJoinStyle = ChartJsFunction.FromName("radarHoverJoin"),
            HoverBorderWidth = ChartJsFunction.FromName("radarHoverWidth"),
            Tension = ChartJsFunction.FromName("radarTension")
        };
        BarDataset barDataset = new()
        {
            BorderSkipped = ChartJsFunction.FromName("barBorderSkipped")
        };
        PieDataset pieDataset = new()
        {
            BorderRadius = ChartJsFunction.FromName("pieBorderRadius")
        };

        using var lineDocument = SerializeToDocument(lineDataset);
        using var radarDocument = SerializeToDocument(radarDataset);
        using var barDocument = SerializeToDocument(barDataset);
        using var pieDocument = SerializeToDocument(pieDataset);
        var lineRoot = lineDocument.RootElement;
        var radarRoot = radarDocument.RootElement;

        Assert.AreEqual("lineCap", GetMarkerName(lineRoot.GetProperty("borderCapStyle")));
        Assert.AreEqual(4, lineRoot.GetProperty("borderDash")[0].GetDouble());
        Assert.AreEqual(2, lineRoot.GetProperty("borderDash")[1].GetDouble());
        Assert.AreEqual("lineDashOffset", GetMarkerName(lineRoot.GetProperty("borderDashOffset")));
        Assert.AreEqual("lineJoin", GetMarkerName(lineRoot.GetProperty("borderJoinStyle")));
        Assert.AreEqual("lineWidth", GetMarkerName(lineRoot.GetProperty("borderWidth")));
        Assert.AreEqual("lineInterpolation", GetMarkerName(lineRoot.GetProperty("cubicInterpolationMode")));
        Assert.AreEqual("lineFill", GetMarkerName(lineRoot.GetProperty("fill")));
        Assert.AreEqual("lineHoverCap", GetMarkerName(lineRoot.GetProperty("hoverBorderCapStyle")));
        Assert.AreEqual("lineHoverDash", GetMarkerName(lineRoot.GetProperty("hoverBorderDash")));
        Assert.AreEqual("lineHoverDashOffset", GetMarkerName(lineRoot.GetProperty("hoverBorderDashOffset")));
        Assert.AreEqual("lineHoverJoin", GetMarkerName(lineRoot.GetProperty("hoverBorderJoinStyle")));
        Assert.AreEqual("lineHoverWidth", GetMarkerName(lineRoot.GetProperty("hoverBorderWidth")));
        Assert.IsFalse(lineRoot.GetProperty("pointStyle").GetBoolean());
        Assert.AreEqual("lineSegmentColor", GetMarkerName(lineRoot.GetProperty("segment").GetProperty("borderColor")));
        Assert.AreEqual("lineSegmentDash", GetMarkerName(lineRoot.GetProperty("segment").GetProperty("borderDash")));
        Assert.AreEqual("middle", lineRoot.GetProperty("stepped").GetString());
        Assert.AreEqual("lineTension", GetMarkerName(lineRoot.GetProperty("tension")));

        Assert.AreEqual("radarCap", GetMarkerName(radarRoot.GetProperty("borderCapStyle")));
        Assert.AreEqual("radarDash", GetMarkerName(radarRoot.GetProperty("borderDash")));
        Assert.AreEqual("radarDashOffset", GetMarkerName(radarRoot.GetProperty("borderDashOffset")));
        Assert.AreEqual("radarJoin", GetMarkerName(radarRoot.GetProperty("borderJoinStyle")));
        Assert.AreEqual("radarWidth", GetMarkerName(radarRoot.GetProperty("borderWidth")));
        Assert.AreEqual("radarFill", GetMarkerName(radarRoot.GetProperty("fill")));
        Assert.AreEqual("radarHoverCap", GetMarkerName(radarRoot.GetProperty("hoverBorderCapStyle")));
        Assert.AreEqual("radarHoverDash", GetMarkerName(radarRoot.GetProperty("hoverBorderDash")));
        Assert.AreEqual("radarHoverDashOffset", GetMarkerName(radarRoot.GetProperty("hoverBorderDashOffset")));
        Assert.AreEqual("radarHoverJoin", GetMarkerName(radarRoot.GetProperty("hoverBorderJoinStyle")));
        Assert.AreEqual("radarHoverWidth", GetMarkerName(radarRoot.GetProperty("hoverBorderWidth")));
        Assert.AreEqual("radarTension", GetMarkerName(radarRoot.GetProperty("tension")));
        Assert.AreEqual("barBorderSkipped", GetMarkerName(barDocument.RootElement.GetProperty("borderSkipped")));
        Assert.AreEqual("pieBorderRadius", GetMarkerName(pieDocument.RootElement.GetProperty("borderRadius")));
    }

    [TestMethod]
    public void FillerOptionsAndLineFillTargetSerialize()
    {
        ChartJsOptions options = new()
        {
            Plugins = new Plugins
            {
                Filler = new FillerOptions
                {
                    Propagate = false,
                    DrawTime = "beforeDraw"
                }
            }
        };
        LineDataset dataset = new()
        {
            Fill = new ChartJsFillOptions
            {
                Above = "blue",
                Below = "red",
                Target = new ChartJsFillTarget { Value = 350 }
            }
        };

        using var optionsDocument = SerializeToDocument(options);
        using var datasetDocument = SerializeToDocument(dataset);

        var filler = optionsDocument.RootElement.GetProperty("plugins").GetProperty("filler");
        var fill = datasetDocument.RootElement.GetProperty("fill");

        Assert.IsFalse(filler.GetProperty("propagate").GetBoolean());
        Assert.AreEqual("beforeDraw", filler.GetProperty("drawTime").GetString());
        Assert.AreEqual("blue", fill.GetProperty("above").GetString());
        Assert.AreEqual("red", fill.GetProperty("below").GetString());
        Assert.AreEqual(350, fill.GetProperty("target").GetProperty("value").GetDouble());
    }

    [TestMethod]
    public void ScaleTickTitleAndLegendScriptableOptionsSerializeMarkers()
    {
        ChartJsAxisBorder border = new()
        {
            Dash = ChartJsFunction.FromName("axisBorderDash"),
            DashOffset = ChartJsFunction.FromName("axisBorderDashOffset")
        };
        ChartJsGrid grid = new()
        {
            LineWidth = ChartJsFunction.FromName("gridLineWidth"),
            TickBorderDash = new List<double> { 1, 3 },
            TickBorderDashOffset = ChartJsFunction.FromName("gridTickDashOffset")
        };
        ChartJsAxisTick ticks = new()
        {
            Font = ChartJsFunction.FromName("tickFont"),
            ShowLabelBackdrop = ChartJsFunction.FromName("showBackdrop"),
            TextStrokeWidth = ChartJsFunction.FromName("tickStrokeWidth")
        };
        Title title = new()
        {
            Font = ChartJsFunction.FromName("titleFont")
        };
        Labels labels = new()
        {
            Font = new Font { Family = "Inter", Size = 12 }
        };

        using var borderDocument = SerializeToDocument(border);
        using var gridDocument = SerializeToDocument(grid);
        using var ticksDocument = SerializeToDocument(ticks);
        using var titleDocument = SerializeToDocument(title);
        using var labelsDocument = SerializeToDocument(labels);

        Assert.AreEqual("axisBorderDash", GetMarkerName(borderDocument.RootElement.GetProperty("dash")));
        Assert.AreEqual("axisBorderDashOffset", GetMarkerName(borderDocument.RootElement.GetProperty("dashOffset")));
        Assert.AreEqual("gridLineWidth", GetMarkerName(gridDocument.RootElement.GetProperty("lineWidth")));
        Assert.AreEqual(1, gridDocument.RootElement.GetProperty("tickBorderDash")[0].GetDouble());
        Assert.AreEqual(3, gridDocument.RootElement.GetProperty("tickBorderDash")[1].GetDouble());
        Assert.AreEqual("gridTickDashOffset", GetMarkerName(gridDocument.RootElement.GetProperty("tickBorderDashOffset")));
        Assert.AreEqual("tickFont", GetMarkerName(ticksDocument.RootElement.GetProperty("font")));
        Assert.AreEqual("showBackdrop", GetMarkerName(ticksDocument.RootElement.GetProperty("showLabelBackdrop")));
        Assert.AreEqual("tickStrokeWidth", GetMarkerName(ticksDocument.RootElement.GetProperty("textStrokeWidth")));
        Assert.AreEqual("titleFont", GetMarkerName(titleDocument.RootElement.GetProperty("font")));
        Assert.AreEqual("Inter", labelsDocument.RootElement.GetProperty("font").GetProperty("family").GetString());
    }

    [TestMethod]
    public void TooltipScriptableOptionsSerializeMarkers()
    {
        Tooltip tooltip = new()
        {
            Enabled = ChartJsFunction.FromName("tooltipEnabled"),
            Position = ChartJsFunction.FromName("tooltipPosition"),
            TitleFont = ChartJsFunction.FromName("tooltipTitleFont"),
            TitleAlign = ChartJsFunction.FromName("tooltipTitleAlign"),
            TitleSpacing = ChartJsFunction.FromName("tooltipTitleSpacing"),
            TitleMarginBottom = ChartJsFunction.FromName("tooltipTitleMargin"),
            BodyFont = ChartJsFunction.FromName("tooltipBodyFont"),
            BodyAlign = ChartJsFunction.FromName("tooltipBodyAlign"),
            BodySpacing = ChartJsFunction.FromName("tooltipBodySpacing"),
            FooterFont = ChartJsFunction.FromName("tooltipFooterFont"),
            FooterAlign = ChartJsFunction.FromName("tooltipFooterAlign"),
            FooterSpacing = ChartJsFunction.FromName("tooltipFooterSpacing"),
            FooterMarginTop = ChartJsFunction.FromName("tooltipFooterMargin"),
            CaretPadding = ChartJsFunction.FromName("tooltipCaretPadding"),
            CaretSize = ChartJsFunction.FromName("tooltipCaretSize"),
            CornerRadius = ChartJsFunction.FromName("tooltipCornerRadius"),
            DisplayColors = ChartJsFunction.FromName("tooltipDisplayColors"),
            BoxWidth = ChartJsFunction.FromName("tooltipBoxWidth"),
            BoxHeight = ChartJsFunction.FromName("tooltipBoxHeight"),
            UsePointStyle = ChartJsFunction.FromName("tooltipUsePointStyle"),
            BorderWidth = ChartJsFunction.FromName("tooltipBorderWidth"),
            Rtl = ChartJsFunction.FromName("tooltipRtl"),
            TextDirection = ChartJsFunction.FromName("tooltipTextDirection"),
            XAlign = ChartJsFunction.FromName("tooltipXAlign"),
            YAlign = ChartJsFunction.FromName("tooltipYAlign")
        };

        using var document = SerializeToDocument(tooltip);
        var root = document.RootElement;

        Assert.AreEqual("tooltipEnabled", GetMarkerName(root.GetProperty("enabled")));
        Assert.AreEqual("tooltipPosition", GetMarkerName(root.GetProperty("position")));
        Assert.AreEqual("tooltipTitleFont", GetMarkerName(root.GetProperty("titleFont")));
        Assert.AreEqual("tooltipTitleAlign", GetMarkerName(root.GetProperty("titleAlign")));
        Assert.AreEqual("tooltipTitleSpacing", GetMarkerName(root.GetProperty("titleSpacing")));
        Assert.AreEqual("tooltipTitleMargin", GetMarkerName(root.GetProperty("titleMarginBottom")));
        Assert.AreEqual("tooltipBodyFont", GetMarkerName(root.GetProperty("bodyFont")));
        Assert.AreEqual("tooltipBodyAlign", GetMarkerName(root.GetProperty("bodyAlign")));
        Assert.AreEqual("tooltipBodySpacing", GetMarkerName(root.GetProperty("bodySpacing")));
        Assert.AreEqual("tooltipFooterFont", GetMarkerName(root.GetProperty("footerFont")));
        Assert.AreEqual("tooltipFooterAlign", GetMarkerName(root.GetProperty("footerAlign")));
        Assert.AreEqual("tooltipFooterSpacing", GetMarkerName(root.GetProperty("footerSpacing")));
        Assert.AreEqual("tooltipFooterMargin", GetMarkerName(root.GetProperty("footerMarginTop")));
        Assert.AreEqual("tooltipCaretPadding", GetMarkerName(root.GetProperty("caretPadding")));
        Assert.AreEqual("tooltipCaretSize", GetMarkerName(root.GetProperty("caretSize")));
        Assert.AreEqual("tooltipCornerRadius", GetMarkerName(root.GetProperty("cornerRadius")));
        Assert.AreEqual("tooltipDisplayColors", GetMarkerName(root.GetProperty("displayColors")));
        Assert.AreEqual("tooltipBoxWidth", GetMarkerName(root.GetProperty("boxWidth")));
        Assert.AreEqual("tooltipBoxHeight", GetMarkerName(root.GetProperty("boxHeight")));
        Assert.AreEqual("tooltipUsePointStyle", GetMarkerName(root.GetProperty("usePointStyle")));
        Assert.AreEqual("tooltipBorderWidth", GetMarkerName(root.GetProperty("borderWidth")));
        Assert.AreEqual("tooltipRtl", GetMarkerName(root.GetProperty("rtl")));
        Assert.AreEqual("tooltipTextDirection", GetMarkerName(root.GetProperty("textDirection")));
        Assert.AreEqual("tooltipXAlign", GetMarkerName(root.GetProperty("xAlign")));
        Assert.AreEqual("tooltipYAlign", GetMarkerName(root.GetProperty("yAlign")));
    }

    [TestMethod]
    public void GlobalChartOptionsSerializeWithChartJsPropertyNames()
    {
        ChartJsOptions options = new()
        {
            BackgroundColor = ChartJsFunction.FromName("globalBackground"),
            BorderColor = "#112233",
            Clip = false,
            Color = "#445566",
            Datasets = new ChartJsOptionsDatasets
            {
                Bar = new { barPercentage = 0.75 },
                Line = new { tension = 0.4 }
            },
            Font = new Font { Family = "Inter", Size = 13 },
            Hover = new Interactions { Mode = "nearest", Intersect = false },
            HoverBackgroundColor = ["#778899", "#aabbcc"],
            HoverBorderColor = ChartJsFunction.FromName("globalHoverBorder"),
            Normalized = true,
            SpanGaps = 172800000,
            OnClick = ChartJsFunction.FromName("chartClick"),
            OnHover = ChartJsFunction.FromName("chartHover"),
            OnResize = ChartJsFunction.FromName("chartResize")
        };

        using var document = SerializeToDocument(options);
        var root = document.RootElement;

        Assert.AreEqual("globalBackground", GetMarkerName(root.GetProperty("backgroundColor")));
        Assert.AreEqual("#112233", root.GetProperty("borderColor").GetString());
        Assert.IsFalse(root.GetProperty("clip").GetBoolean());
        Assert.AreEqual("#445566", root.GetProperty("color").GetString());
        Assert.AreEqual(0.75, root.GetProperty("datasets").GetProperty("bar").GetProperty("barPercentage").GetDouble());
        Assert.AreEqual(0.4, root.GetProperty("datasets").GetProperty("line").GetProperty("tension").GetDouble());
        Assert.AreEqual("Inter", root.GetProperty("font").GetProperty("family").GetString());
        Assert.AreEqual(13, root.GetProperty("font").GetProperty("size").GetDouble());
        Assert.AreEqual("nearest", root.GetProperty("hover").GetProperty("mode").GetString());
        Assert.IsFalse(root.GetProperty("hover").GetProperty("intersect").GetBoolean());
        Assert.AreEqual("#778899", root.GetProperty("hoverBackgroundColor")[0].GetString());
        Assert.AreEqual("#aabbcc", root.GetProperty("hoverBackgroundColor")[1].GetString());
        Assert.AreEqual("globalHoverBorder", GetMarkerName(root.GetProperty("hoverBorderColor")));
        Assert.IsTrue(root.GetProperty("normalized").GetBoolean());
        Assert.AreEqual(172800000, root.GetProperty("spanGaps").GetInt32());
        Assert.AreEqual("chartClick", GetMarkerName(root.GetProperty("onClick")));
        Assert.AreEqual("chartHover", GetMarkerName(root.GetProperty("onHover")));
        Assert.AreEqual("chartResize", GetMarkerName(root.GetProperty("onResize")));
    }

    [TestMethod]
    public void ChartJsDefaultsOptionsExcludeLibraryEventFlags()
    {
        ChartJsDefaultsOptions defaults = new()
        {
            Color = "#202020",
            OnClick = ChartJsFunction.FromName("defaultClick"),
            Datasets = new ChartJsOptionsDatasets
            {
                Doughnut = new { borderWidth = 0 },
                PolarArea = new { circular = true }
            }
        };

        using var document = SerializeToDocument(defaults);
        var root = document.RootElement;

        Assert.AreEqual("#202020", root.GetProperty("color").GetString());
        Assert.AreEqual("defaultClick", GetMarkerName(root.GetProperty("onClick")));
        Assert.AreEqual(0, root.GetProperty("datasets").GetProperty("doughnut").GetProperty("borderWidth").GetDouble());
        Assert.IsTrue(root.GetProperty("datasets").GetProperty("polarArea").GetProperty("circular").GetBoolean());
        Assert.IsFalse(root.TryGetProperty("onClickEvent", out _));
        Assert.IsFalse(root.TryGetProperty("onHoverEvent", out _));
        Assert.IsFalse(root.TryGetProperty("onResizeEvent", out _));
    }

    [TestMethod]
    public void ElementOptionsCutoutAndKeyedAnimationsSerializeMarkers()
    {
        ChartJsOptions options = new()
        {
            Cutout = "50%",
            Elements = new ChartJsElementsOptions
            {
                Bar = new ChartJsBarElementOptions
                {
                    BackgroundColor = ChartJsFunction.FromName("barBackground"),
                    BorderColor = ChartJsFunction.FromName("barBorder"),
                    BorderWidth = 2
                },
                Point = new ChartJsPointElementOptions
                {
                    Radius = ChartJsFunction.FromName("pointRadius"),
                    PointStyle = ChartJsFunction.FromName("pointStyle")
                },
                Arc = new ChartJsArcElementOptions
                {
                    BackgroundColor = ChartJsFunction.FromName("arcBackground")
                }
            },
            Animation = new Animation
            {
                Delay = ChartJsFunction.FromName("animationDelay"),
                Loop = ChartJsFunction.FromName("animationLoop"),
                OnComplete = ChartJsFunction.FromName("animationComplete")
            },
            Animations = new Animations
            {
                X = new Animations
                {
                    Type = "number",
                    From = ChartJsFunction.FromName("fromNaN"),
                    Delay = ChartJsFunction.FromName("xDelay")
                },
                Y = new Animations
                {
                    Easing = "linear",
                    From = ChartJsFunction.FromName("previousY")
                }
            }
        };

        using var document = SerializeToDocument(options);
        var root = document.RootElement;

        Assert.AreEqual("50%", root.GetProperty("cutout").GetString());
        Assert.AreEqual("barBackground", GetMarkerName(root.GetProperty("elements").GetProperty("bar").GetProperty("backgroundColor")));
        Assert.AreEqual("barBorder", GetMarkerName(root.GetProperty("elements").GetProperty("bar").GetProperty("borderColor")));
        Assert.AreEqual(2, root.GetProperty("elements").GetProperty("bar").GetProperty("borderWidth").GetDouble());
        Assert.AreEqual("pointRadius", GetMarkerName(root.GetProperty("elements").GetProperty("point").GetProperty("radius")));
        Assert.AreEqual("pointStyle", GetMarkerName(root.GetProperty("elements").GetProperty("point").GetProperty("pointStyle")));
        Assert.AreEqual("arcBackground", GetMarkerName(root.GetProperty("elements").GetProperty("arc").GetProperty("backgroundColor")));
        Assert.AreEqual("animationDelay", GetMarkerName(root.GetProperty("animation").GetProperty("delay")));
        Assert.AreEqual("animationLoop", GetMarkerName(root.GetProperty("animation").GetProperty("loop")));
        Assert.AreEqual("animationComplete", GetMarkerName(root.GetProperty("animation").GetProperty("onComplete")));
        Assert.AreEqual("number", root.GetProperty("animations").GetProperty("x").GetProperty("type").GetString());
        Assert.AreEqual("fromNaN", GetMarkerName(root.GetProperty("animations").GetProperty("x").GetProperty("from")));
        Assert.AreEqual("xDelay", GetMarkerName(root.GetProperty("animations").GetProperty("x").GetProperty("delay")));
        Assert.AreEqual("linear", root.GetProperty("animations").GetProperty("y").GetProperty("easing").GetString());
        Assert.AreEqual("previousY", GetMarkerName(root.GetProperty("animations").GetProperty("y").GetProperty("from")));
    }

    private static JsonDocument SerializeToDocument<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonSerializationTestOptions.Default);
        return JsonDocument.Parse(json);
    }

    private static string? GetMarkerName(JsonElement element)
    {
        return element.GetProperty("__chartJsFunction").GetString();
    }
}
