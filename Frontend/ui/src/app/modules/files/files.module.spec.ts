import { FilesModule } from "./files.module";

describe("FilesModule", () => {
	let filesModule: FilesModule;

	beforeEach(() => {
		filesModule = new FilesModule();
	});

	it("should create an instance", () => {
		expect(filesModule).toBeTruthy();
	});
});
