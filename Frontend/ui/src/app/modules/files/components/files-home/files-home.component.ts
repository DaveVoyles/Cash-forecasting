import { Component, OnInit } from "@angular/core";

import { FileGroup } from "../../models/file-group";

@Component({
	selector: "app-files-home",
	templateUrl: "./files-home.component.html",
	styleUrls: ["./files-home.component.css"]
})
export class FilesHomeComponent implements OnInit {
	history: FileGroup[] = [
		{
			date: new Date("2018-10-02"),
			files: [
				{
					name: "Foo",
					type: "csv",
					size: 10,
					md5: "slkgjdfgljdfg"
				},
				{
					name: "Bar",
					type: "txt",
					size: 10230,
					md5: "alksgjdlfgkjdfg"
				},
				{
					name: "Baz",
					type: "xlsx",
					size: 324234,
					md5: "dkfjghsldkgjsld"
				}
			]
		},
		{
			date: new Date("2018-10-01"),
			files: [
				{
					name: "Foo",
					type: "csv",
					size: 10,
					md5: "slkgjdfgljdfg"
				},
				{
					name: "Bar",
					type: "txt",
					size: 10230,
					md5: "alksgjdlfgkjdfg"
				},
				{
					name: "Baz",
					type: "xlsx",
					size: 324234,
					md5: "dkfjghsldkgjsld"
				}
			]
		}
	];

	constructor() {}

	ngOnInit() {
	}
}
