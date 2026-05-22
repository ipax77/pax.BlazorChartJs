import type {
    Chart as ChartJsChart,
    ChartConfiguration,
    ChartDataset,
    ChartOptions,
    Plugin
} from "chart.js";

export type ChartJsCallback = (...args: any[]) => any;
export type ChartJsCallbackRegistry = Readonly<Record<string, ChartJsCallback>>;

export type ChartInstance = Omit<ChartJsChart, "data"> & {
    data: {
        labels?: unknown[];
        datasets: InteropChartDataset[];
    };
};

export type InteropChartDataset = {
    id?: string;
    data?: unknown;
    backgroundColor?: unknown;
    borderColor?: unknown;
    label?: string;
    [key: string]: unknown;
};

export type ChartSetupOptionsPayload = {
    chartJsLocation?: string;
    chartJsPluginLabelsLocation?: string;
    chartJsPluginDatalabelsLocation?: string;
    chartJsCallbacksModuleLocation?: string | null;
    defaults?: any;
} & Record<string, any>;

export type ChartJsPlugin = Plugin & Record<string, any>;
export type ChartJsPluginOptionsPayload = Record<string, unknown>;

export type ChartJsOptionsPayload = Record<string, unknown> & {
    onClickEvent?: boolean;
    onHoverEvent?: boolean;
    onResizeEvent?: boolean;
    plugins?: ChartJsPluginOptionsPayload;
};

export type ChartJsDatasetPayload = Record<string, unknown> & {
    id: string;
    data: unknown;
    backgroundColor?: unknown;
    borderColor?: unknown;
    label?: string;
};

export type ChartJsDataPayload = {
    labels?: unknown[];
    datasets: ChartJsDatasetPayload[];
} & Record<string, unknown>;

export type ChartJsConfigPayload = {
    type?: unknown;
    data: ChartJsDataPayload;
    options?: ChartJsOptionsPayload;
    plugins?: ChartJsPlugin[];
} & Record<string, unknown>;

export type ChartJsRuntimeConfiguration = {
    type?: unknown;
    data: ChartJsDataPayload;
    options: ChartJsOptionsPayload;
    plugins: ChartJsPlugin[];
};

export type AddDataPayload = {
    data: unknown;
    backgroundColor?: string | null;
    borderColor?: string | null;
    atPosition?: number | null;
};

export type AddDataPayloadMap = Record<string, AddDataPayload>;

export type SetDataPayload = {
    data: unknown[];
    backgroundColor?: unknown;
    borderColor?: unknown;
};

export type SetDataPayloadMap = Record<string, SetDataPayload>;

export type DatasetDataPayload = {
    datasetId: string;
    data: unknown[];
};

export type BinaryDataFormat =
    | "Float64XY"
    | "Float32XY"
    | "Float64Y"
    | "Float32Y"
    | "Int32Y";

export type BinaryDatasetPayloadMetadata = {
    datasetId: string;
    count: number;
    format: BinaryDataFormat;
    xOffset?: number | null;
    yOffset?: number | null;
    byteStride?: number | null;
};

export type ChartInitResult =
    | {
        success: true;
        height: number;
        width: number;
        windowHeight: number;
        windowWidth: number;
    }
    | {
        success: false;
    };
