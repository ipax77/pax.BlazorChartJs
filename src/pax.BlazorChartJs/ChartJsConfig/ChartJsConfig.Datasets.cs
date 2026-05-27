
namespace pax.BlazorChartJs;

public partial class ChartJsConfig
{
    /// <summary>
    /// Adds the dataset to the config and updates the Chart
    /// </summary>
    public void AddDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        AddDatasets([dataset]);
    }

    /// <summary>
    /// Adds the datasets to the config and updates the Chart
    /// </summary>

    public void AddDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        if (datasets.Count == 0)
        {
            return;
        }

        foreach (var dataset in datasets)
        {
            Data.Datasets.Add(dataset);
        }
        OnDatasetsAdd(new DatasetsAddEventArgs(datasets));
    }

    /// <summary>
    /// Adds the dataset after the dataset with the given id and updates the Chart.
    /// </summary>
    public void AddDatasetAfter(string afterDatasetId, ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        AddDatasetsAfter(afterDatasetId, [dataset]);
    }

    /// <summary>
    /// Adds the datasets after the dataset with the given id and updates the Chart.
    /// If the id is not found, the datasets are appended.
    /// </summary>
    public void AddDatasetsAfter(string afterDatasetId, IList<ChartJsDataset> datasets)
    {
        ArgumentException.ThrowIfNullOrEmpty(afterDatasetId);
        ArgumentNullException.ThrowIfNull(datasets);

        if (datasets.Count == 0)
        {
            return;
        }

        AddDatasetsAtAnchor(afterDatasetId, datasets, insertBefore: false);
    }

    /// <summary>
    /// Adds the dataset before the dataset with the given id and updates the Chart.
    /// If the id is not found, the dataset is appended.
    /// </summary>
    public void AddDatasetBefore(string beforeDatasetId, ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        AddDatasetsBefore(beforeDatasetId, [dataset]);
    }

    /// <summary>
    /// Adds the datasets before the dataset with the given id and updates the Chart.
    /// If the id is not found, the datasets are appended.
    /// </summary>
    public void AddDatasetsBefore(string beforeDatasetId, IList<ChartJsDataset> datasets)
    {
        ArgumentException.ThrowIfNullOrEmpty(beforeDatasetId);
        ArgumentNullException.ThrowIfNull(datasets);

        if (datasets.Count == 0)
        {
            return;
        }

        AddDatasetsAtAnchor(beforeDatasetId, datasets, insertBefore: true);
    }

    /// <summary>
    /// Removes the dataset from the config and updates the Chart
    /// </summary>
    public void RemoveDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        RemoveDatasets([dataset]);
    }

    /// <summary>
    /// Removes the datasets from the config and updates the Chart
    /// </summary>
    public void RemoveDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        List<string> removeDatasetIds = new(datasets.Count);

        foreach (var dataset in datasets.ToArray())
        {
            int index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                removeDatasetIds.Add(dataset.Id);
                Data.Datasets.RemoveAt(index);
            }
        }
        OnDatasetsRemove(new DatasetsRemoveEventArgs(removeDatasetIds));
    }

    /// <summary>
    /// Updates the Chart dataset.
    /// </summary>
    /// <param name="dataset">The dataset containing the updated values.</param>
    public void UpdateDataset(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        UpdateDatasets([dataset]);
    }

    /// <summary>
    /// Updates the Chart datasets.
    /// </summary>
    /// <param name="datasets">The datasets containing the updated values.</param>
    public void UpdateDatasets(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        List<ChartJsDataset> updateDatasets = new(datasets.Count);
        foreach (var dataset in datasets)
        {
            var index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                updateDatasets.Add(dataset);
            }
        }
        OnDatasetsUpdate(new DatasetsUpdateEventArgs(updateDatasets));
    }

    /// <summary>
    /// Updates the Chart dataset smoothly, if possible.
    /// Note: Updating dataset properties to NULL (default values) or single values is currently not working if they were an array. <see href="https://github.com/chartjs/Chart.js/issues/11679"/>
    /// </summary>
    /// <param name="dataset">The dataset containing the updated values.</param>
    public void UpdateDatasetSmooth(ChartJsDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);
        UpdateDatasetsSmooth([dataset]);
    }

    /// <summary>
    /// Updates the Chart datasets smoothly, if possible.
    /// Note: Updating dataset properties to NULL (default values) or single values is currently not working if they were an array. <see href="https://github.com/chartjs/Chart.js/issues/11679"/>
    /// </summary>
    /// <param name="datasets">The datasets containing the updated values.</param>
    public void UpdateDatasetsSmooth(IList<ChartJsDataset> datasets)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        List<ChartJsDataset> updateDatasets = new(datasets.Count);
        foreach (var dataset in datasets)
        {
            var index = Data.Datasets.IndexOf(dataset);
            if (index >= 0)
            {
                updateDatasets.Add(dataset);
            }
        }
        OnDatasetsUpdate(new DatasetsUpdateEventArgs(updateDatasets, smooth: true));
    }

    /// <summary>
    /// Updates a dataset's data smoothly by dataset id.
    /// </summary>
    public void UpdateDatasetDataSmooth(string datasetId, IList<object> data)
    {
        ArgumentException.ThrowIfNullOrEmpty(datasetId);
        ArgumentNullException.ThrowIfNull(data);

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var dataset = Data.Datasets[i];
            if (dataset.Id == datasetId)
            {
                dataset.Data = data;
                ApplyDatasetChangesSmooth(new DatasetsSmoothChangeSet(CreateCurrentDatasetIds())
                {
                    DatasetsToUpdateSmooth = [dataset]
                });
                return;
            }
        }
    }

    /// <summary>
    /// Updates multiple datasets' data smoothly by dataset id.
    /// </summary>
    public void UpdateDatasetsDataSmooth(IReadOnlyDictionary<string, IList<object>> dataByDatasetId)
    {
        ArgumentNullException.ThrowIfNull(dataByDatasetId);

        if (dataByDatasetId.Count == 0)
        {
            return;
        }

        List<string> desiredDatasetIds = new(Data.Datasets.Count);
        List<ChartJsDataset> datasetsToUpdateSmooth = new(Math.Min(Data.Datasets.Count, dataByDatasetId.Count));

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var dataset = Data.Datasets[i];
            if (string.IsNullOrEmpty(dataset.Id))
            {
                throw new InvalidOperationException("Existing dataset contains a null or empty Id.");
            }

            desiredDatasetIds.Add(dataset.Id);

            if (dataByDatasetId.TryGetValue(dataset.Id, out var data))
            {
                ArgumentNullException.ThrowIfNull(data);
                dataset.Data = data;
                datasetsToUpdateSmooth.Add(dataset);
            }
        }

        if (datasetsToUpdateSmooth.Count == 0)
        {
            return;
        }

        ApplyDatasetChangesSmooth(new DatasetsSmoothChangeSet(desiredDatasetIds)
        {
            DatasetsToUpdateSmooth = datasetsToUpdateSmooth
        });
    }

    /// <summary>
    /// Updates the Chart with the current Datasets
    /// </summary>
    public void SetDatasets()
    {
        OnDatasetsSet(new DatasetsSetEventArgs(Data.Datasets));
    }

    /// <summary>
    /// Smoothly replaces the current dataset collection, adding, updating, removing, and reordering by dataset id in a single chart update.
    /// </summary>
    public void SetDatasetsSmooth(
        IList<ChartJsDataset> datasets,
        IList<string>? labels = null,
        bool updateOptions = false)
    {
        ArgumentNullException.ThrowIfNull(datasets);

        HashSet<string> existingDatasetIds = new(Data.Datasets.Count, StringComparer.Ordinal);
        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var dataset = Data.Datasets[i];
            if (string.IsNullOrWhiteSpace(dataset.Id))
            {
                throw new InvalidOperationException("Existing dataset contains a null or empty Id.");
            }

            if (!existingDatasetIds.Add(dataset.Id))
            {
                throw new InvalidOperationException($"Duplicate existing dataset id '{dataset.Id}'.");
            }
        }

        List<string> desiredDatasetIds = new(datasets.Count);
        HashSet<string> desiredDatasetIdSet = new(datasets.Count, StringComparer.Ordinal);
        List<ChartJsDataset> datasetsToAdd = new(datasets.Count);
        List<ChartJsDataset> datasetsToUpdateSmooth = new(datasets.Count);

        for (int i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            ArgumentNullException.ThrowIfNull(dataset);
            if (string.IsNullOrEmpty(dataset.Id))
            {
                throw new ArgumentException("Dataset id cannot be null or empty.", nameof(datasets));
            }

            desiredDatasetIds.Add(dataset.Id);
            _ = desiredDatasetIdSet.Add(dataset.Id);

            if (existingDatasetIds.Contains(dataset.Id))
            {
                datasetsToUpdateSmooth.Add(dataset);
            }
            else
            {
                datasetsToAdd.Add(dataset);
            }
        }

        List<string> datasetIdsToRemove = new(Math.Max(0, existingDatasetIds.Count - desiredDatasetIdSet.Count));
        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var datasetId = Data.Datasets[i].Id;
            if (!desiredDatasetIdSet.Contains(datasetId))
            {
                datasetIdsToRemove.Add(datasetId);
            }
        }

        ApplyDatasetChangesSmooth(new DatasetsSmoothChangeSet(desiredDatasetIds)
        {
            DatasetsToAdd = datasetsToAdd,
            DatasetsToUpdateSmooth = datasetsToUpdateSmooth,
            DatasetIdsToRemove = datasetIdsToRemove,
            Labels = labels,
            UpdateOptions = updateOptions
        });
    }

    /// <summary>
    /// Applies dataset additions, smooth updates, removals, final ordering,
    /// and optional labels/options as one consolidated chart update.
    /// </summary>
    public void ApplyDatasetChangesSmooth(DatasetsSmoothChangeSet changeSet)
    {
        ArgumentNullException.ThrowIfNull(changeSet);

        var desiredDatasetIds = changeSet.DesiredDatasetIds;
        ArgumentNullException.ThrowIfNull(desiredDatasetIds);

        _ = CreateValidatedIdSet(desiredDatasetIds, nameof(desiredDatasetIds), rejectDuplicates: true);

        HashSet<string> removeIdSet = changeSet.DatasetIdsToRemove == null
            ? new(StringComparer.Ordinal)
            : CreateValidatedIdSet(changeSet.DatasetIdsToRemove, nameof(changeSet.DatasetIdsToRemove), rejectDuplicates: false);

        Dictionary<string, ChartJsDataset> addDatasetsById = CreateDatasetMap(changeSet.DatasetsToAdd);
        Dictionary<string, ChartJsDataset> updateDatasetsById = CreateDatasetMap(changeSet.DatasetsToUpdateSmooth);

        foreach (var id in addDatasetsById.Keys)
        {
            if (updateDatasetsById.ContainsKey(id))
            {
                throw new ArgumentException(
                    $"Dataset id '{id}' cannot appear in both datasetsToAdd and datasetsToUpdateSmooth.");
            }
        }

        int candidateCapacity = Data.Datasets.Count + addDatasetsById.Count;

        Dictionary<string, ChartJsDataset> candidateDatasets =
            new(candidateCapacity, StringComparer.Ordinal);

        HashSet<string> existingIds = new(StringComparer.Ordinal);

        foreach (var dataset in Data.Datasets)
        {
            if (string.IsNullOrWhiteSpace(dataset.Id))
            {
                throw new InvalidOperationException("Existing dataset contains a null or empty Id.");
            }

            if (!candidateDatasets.TryAdd(dataset.Id, dataset))
            {
                throw new InvalidOperationException($"Duplicate existing dataset id '{dataset.Id}'.");
            }

            _ = existingIds.Add(dataset.Id);
        }

        List<string> effectiveDatasetIdsToRemove = [.. removeIdSet.Where(existingIds.Contains)];

        foreach (var dataset in addDatasetsById.Values)
        {
            if (!removeIdSet.Contains(dataset.Id))
            {
                candidateDatasets[dataset.Id] = dataset;
            }
        }

        foreach (var dataset in updateDatasetsById.Values)
        {
            if (!removeIdSet.Contains(dataset.Id) && candidateDatasets.ContainsKey(dataset.Id))
            {
                candidateDatasets[dataset.Id] = dataset;
            }
        }

        List<ChartJsDataset> finalDatasets = new(desiredDatasetIds.Count);
        List<string> finalDatasetIds = new(desiredDatasetIds.Count);
        List<ChartJsDataset> effectiveDatasetsToAdd = new(addDatasetsById.Count);
        List<ChartJsDataset> effectiveDatasetsToUpdateSmooth = new(updateDatasetsById.Count);

        foreach (var desiredDatasetId in desiredDatasetIds)
        {
            if (removeIdSet.Contains(desiredDatasetId))
            {
                continue;
            }

            if (!candidateDatasets.TryGetValue(desiredDatasetId, out var dataset))
            {
                continue; // Or throw if desiredDatasetIds should be strict.
            }

            finalDatasets.Add(dataset);
            finalDatasetIds.Add(desiredDatasetId);

            if (addDatasetsById.TryGetValue(desiredDatasetId, out var datasetToAdd))
            {
                effectiveDatasetsToAdd.Add(datasetToAdd);
            }

            if (updateDatasetsById.TryGetValue(desiredDatasetId, out var datasetToUpdate))
            {
                effectiveDatasetsToUpdateSmooth.Add(datasetToUpdate);
            }
        }

        Data.Datasets.Clear();

        foreach (var dataset in finalDatasets)
        {
            Data.Datasets.Add(dataset);
        }

        if (changeSet.Labels != null)
        {
            Data.Labels = [.. changeSet.Labels];
        }

        OnDatasetChangesSmooth(new DatasetsSmoothChangeSet(finalDatasetIds)
        {
            DatasetsToAdd = effectiveDatasetsToAdd,
            DatasetsToUpdateSmooth = effectiveDatasetsToUpdateSmooth,
            DatasetIdsToRemove = effectiveDatasetIdsToRemove,
            Labels = changeSet.Labels,
            UpdateOptions = changeSet.UpdateOptions
        });
    }

    private static HashSet<string> CreateValidatedIdSet(IList<string> datasetIds, string parameterName, bool rejectDuplicates)
    {
        HashSet<string> datasetIdSet = new(datasetIds.Count, StringComparer.Ordinal);
        for (int i = 0; i < datasetIds.Count; i++)
        {
            var datasetId = datasetIds[i];
            if (string.IsNullOrEmpty(datasetId))
            {
                throw new ArgumentException("Dataset id cannot be null or empty.", parameterName);
            }

            if (!datasetIdSet.Add(datasetId) && rejectDuplicates)
            {
                throw new ArgumentException($"Duplicate dataset id '{datasetId}'.", parameterName);
            }
        }

        return datasetIdSet;
    }

    private static Dictionary<string, ChartJsDataset> CreateDatasetMap(IList<ChartJsDataset>? datasets)
    {
        if (datasets == null || datasets.Count == 0)
        {
            return new(StringComparer.Ordinal);
        }

        Dictionary<string, ChartJsDataset> datasetsById = new(datasets.Count, StringComparer.Ordinal);
        for (int i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            ArgumentNullException.ThrowIfNull(dataset);
            if (string.IsNullOrEmpty(dataset.Id))
            {
                throw new ArgumentException("Dataset id cannot be null or empty.", nameof(datasets));
            }

            datasetsById[dataset.Id] = dataset;
        }

        return datasetsById;
    }

    private void AddDatasetsAtAnchor(string anchorDatasetId, IList<ChartJsDataset> datasets, bool insertBefore)
    {
        ValidateDatasets(datasets);

        List<string> desiredDatasetIds = new(Data.Datasets.Count + datasets.Count);
        bool inserted = false;

        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var existingDatasetId = Data.Datasets[i].Id;
            if (string.IsNullOrEmpty(existingDatasetId))
            {
                throw new InvalidOperationException("Existing dataset contains a null or empty Id.");
            }

            if (insertBefore && existingDatasetId == anchorDatasetId)
            {
                AddDatasetIds(desiredDatasetIds, datasets);
                inserted = true;
            }

            desiredDatasetIds.Add(existingDatasetId);

            if (!insertBefore && existingDatasetId == anchorDatasetId)
            {
                AddDatasetIds(desiredDatasetIds, datasets);
                inserted = true;
            }
        }

        if (!inserted)
        {
            AddDatasetIds(desiredDatasetIds, datasets);
        }

        ApplyDatasetChangesSmooth(new DatasetsSmoothChangeSet(desiredDatasetIds)
        {
            DatasetsToAdd = datasets
        });
    }

    private static void ValidateDatasets(IList<ChartJsDataset> datasets)
    {
        for (int i = 0; i < datasets.Count; i++)
        {
            var dataset = datasets[i];
            ArgumentNullException.ThrowIfNull(dataset);
            if (string.IsNullOrEmpty(dataset.Id))
            {
                throw new ArgumentException("Dataset id cannot be null or empty.", nameof(datasets));
            }
        }
    }

    private static void AddDatasetIds(List<string> datasetIds, IList<ChartJsDataset> datasets)
    {
        for (int i = 0; i < datasets.Count; i++)
        {
            datasetIds.Add(datasets[i].Id);
        }
    }

    private List<string> CreateCurrentDatasetIds()
    {
        List<string> datasetIds = new(Data.Datasets.Count);
        for (int i = 0; i < Data.Datasets.Count; i++)
        {
            var datasetId = Data.Datasets[i].Id;
            if (string.IsNullOrEmpty(datasetId))
            {
                throw new InvalidOperationException("Existing dataset contains a null or empty Id.");
            }

            datasetIds.Add(datasetId);
        }

        return datasetIds;
    }
}
