export type ChartJsCallback = (...args: any[]) => any;
export type ChartJsCallbackRegistry = Readonly<Record<string, ChartJsCallback>>;

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