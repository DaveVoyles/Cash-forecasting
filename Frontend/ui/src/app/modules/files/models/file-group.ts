import { FileMetadata } from "./file-metadata";

export interface FileGroup {
	date: Date;
	files: FileMetadata[];
}
