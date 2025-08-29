export interface RomFile {
  type: string;
  fileName: string;
  romByteArray: Uint8Array;
}

export interface PreviewFile {
  type: string;
  fileName: string;
  previewByteArray: Uint8Array;
}
