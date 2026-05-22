import { BinaryDatasetPayloadMetadata } from "./types";

type BinaryFloatArray = Float64Array | Float32Array;

const binaryTypedArraysReadLittleEndian = new Uint8Array(new Uint16Array([1]).buffer)[0] === 1;

function validateBinaryLayout(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    compactStride: number,
    valueSize: number,
    usesX: boolean) {
    if (!Number.isInteger(payload.count) || payload.count < 0) {
        throw new Error("Binary dataset payload count must be a non-negative integer.");
    }

    const xOffset = payload.xOffset ?? 0;
    const rawYOffset = payload.yOffset ?? 0;
    const yOffset = usesX && xOffset === 0 && rawYOffset === 0
        ? valueSize
        : rawYOffset;
    const byteStride = payload.byteStride ?? compactStride;
    if (!Number.isInteger(xOffset) || xOffset < 0
        || !Number.isInteger(yOffset) || yOffset < 0
        || !Number.isInteger(byteStride) || byteStride < compactStride) {
        throw new Error("Binary dataset payload has invalid offsets or byte stride.");
    }

    const maxOffset = usesX ? Math.max(xOffset, yOffset) : yOffset;
    if (maxOffset + valueSize > byteStride) {
        throw new Error("Binary dataset payload offsets must fit inside the byte stride.");
    }

    const requiredBytes = payload.count === 0
        ? 0
        : ((payload.count - 1) * byteStride) + maxOffset + valueSize;
    if (bytes.byteLength < requiredBytes) {
        throw new Error(`Binary dataset payload for '${payload.datasetId}' is too small.`);
    }

    return {
        xOffset,
        yOffset,
        byteStride
    };
}

function tryGetBinaryFloatArray(
    bytes: Uint8Array,
    valueSize: number,
    valueKind: "float64" | "float32",
    byteStride: number,
    ...offsets: number[]): BinaryFloatArray | undefined {
    if (!binaryTypedArraysReadLittleEndian
        || bytes.byteOffset % valueSize !== 0
        || byteStride % valueSize !== 0
        || offsets.some(offset => offset % valueSize !== 0)) {
        return undefined;
    }

    const valueCount = Math.floor(bytes.byteLength / valueSize);
    return valueKind === "float64"
        ? new Float64Array(bytes.buffer, bytes.byteOffset, valueCount)
        : new Float32Array(bytes.buffer, bytes.byteOffset, valueCount);
}

function tryGetBinaryInt32Array(bytes: Uint8Array, byteStride: number, yOffset: number): Int32Array | undefined {
    const valueSize = Int32Array.BYTES_PER_ELEMENT;
    if (!binaryTypedArraysReadLittleEndian
        || bytes.byteOffset % valueSize !== 0
        || byteStride % valueSize !== 0
        || yOffset % valueSize !== 0) {
        return undefined;
    }

    const valueCount = Math.floor(bytes.byteLength / valueSize);
    return new Int32Array(bytes.buffer, bytes.byteOffset, valueCount);
}

function readBinaryFloat(view: DataView, byteOffset: number, valueKind: "float64" | "float32"): number {
    return valueKind === "float64"
        ? view.getFloat64(byteOffset, true)
        : view.getFloat32(byteOffset, true);
}


export function decodeBinaryDatasetData(bytes: Uint8Array, payload: BinaryDatasetPayloadMetadata): any[] {
    if (!(bytes instanceof Uint8Array)) {
        throw new Error("Binary dataset payload must be a Uint8Array.");
    }

    switch (payload.format) {
        case "Float64XY":
            return decodeBinaryXY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");

        case "Float32XY":
            return decodeBinaryXY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");

        case "Float64Y":
            return decodeBinaryY(bytes, payload, Float64Array.BYTES_PER_ELEMENT, "float64");

        case "Float32Y":
            return decodeBinaryY(bytes, payload, Float32Array.BYTES_PER_ELEMENT, "float32");

        case "Int32Y":
            return decodeBinaryInt32Y(bytes, payload);

        default:
            throw new Error(`Unsupported binary dataset format '${payload.format}'.`);
    }
}

function decodeBinaryXY(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    valueSize: number,
    valueKind: "float64" | "float32"): Array<{ x: number, y: number }> {
    const layout = validateBinaryLayout(bytes, payload, valueSize * 2, valueSize, true);
    const data = new Array(payload.count);
    const typedValues = tryGetBinaryFloatArray(bytes, valueSize, valueKind, layout.byteStride, layout.xOffset, layout.yOffset);

    if (typedValues) {
        const valueStride = layout.byteStride / valueSize;
        const xOffset = layout.xOffset / valueSize;
        const yOffset = layout.yOffset / valueSize;

        for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
            data[i] = {
                x: typedValues[valueIndex + xOffset],
                y: typedValues[valueIndex + yOffset]
            };
        }

        return data;
    }

    const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);

    for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
        data[i] = {
            x: readBinaryFloat(view, recordOffset + layout.xOffset, valueKind),
            y: readBinaryFloat(view, recordOffset + layout.yOffset, valueKind)
        };
    }

    return data;
}

function decodeBinaryY(
    bytes: Uint8Array,
    payload: BinaryDatasetPayloadMetadata,
    compactStride: number,
    valueKind: "float64" | "float32"): number[] {
    const layout = validateBinaryLayout(bytes, payload, compactStride, compactStride, false);
    const data = new Array(payload.count);
    const typedValues = tryGetBinaryFloatArray(bytes, compactStride, valueKind, layout.byteStride, layout.yOffset);

    if (typedValues) {
        const valueStride = layout.byteStride / compactStride;
        const yOffset = layout.yOffset / compactStride;

        for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
            data[i] = typedValues[valueIndex + yOffset];
        }

        return data;
    }

    const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);

    for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
        data[i] = readBinaryFloat(view, recordOffset + layout.yOffset, valueKind);
    }

    return data;
}

function decodeBinaryInt32Y(bytes: Uint8Array, payload: BinaryDatasetPayloadMetadata): number[] {
    const compactStride = Int32Array.BYTES_PER_ELEMENT;
    const layout = validateBinaryLayout(bytes, payload, compactStride, compactStride, false);
    const data = new Array(payload.count);
    const typedValues = tryGetBinaryInt32Array(bytes, layout.byteStride, layout.yOffset);

    if (typedValues) {
        const valueStride = layout.byteStride / compactStride;
        const yOffset = layout.yOffset / compactStride;

        for (let i = 0, valueIndex = 0; i < payload.count; i++, valueIndex += valueStride) {
            data[i] = typedValues[valueIndex + yOffset];
        }

        return data;
    }

    const view = new DataView(bytes.buffer, bytes.byteOffset, bytes.byteLength);

    for (let i = 0, recordOffset = 0; i < payload.count; i++, recordOffset += layout.byteStride) {
        data[i] = view.getInt32(recordOffset + layout.yOffset, true);
    }

    return data;
}
