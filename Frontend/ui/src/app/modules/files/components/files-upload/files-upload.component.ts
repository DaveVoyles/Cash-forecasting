import { Component, OnInit } from "@angular/core";
import {
	HttpClient,
	HttpRequest,
	HttpEventType,
	HttpResponse
} from "@angular/common/http";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { FileValidator } from "ngx-material-file-input";

@Component({
	selector: "app-files-upload",
	templateUrl: "./files-upload.component.html",
	styleUrls: ["./files-upload.component.css"]
})
export class FilesUploadComponent implements OnInit {
	formDoc: FormGroup;
	message: string;
	progress: number;

	// 100 MB
	readonly maxSize = 104857600;

	constructor(private http: HttpClient, private _fb: FormBuilder) {}

	ngOnInit() {
		this.formDoc = this._fb.group({
			multiplefile: [{ value: undefined, disabled: false }]
		});
	}

	onSubmit(form: FormGroup) {
		const files = form.controls.multiplefile.value;

		if (files._files.length === 0) {
			return;
		}

		const formData = new FormData();

		for (const file of files._files) {
			formData.append(file.name, file);
		}

		// DAVE: Change address based on port exposed by docker
		const uploadReq = new HttpRequest(
			"POST",
			"http://localhost:5001/files/upload",
			formData,
			{
				reportProgress: true
			}
		);

		this.http.request(uploadReq).subscribe(event => {
			if (event.type === HttpEventType.UploadProgress) {
				this.progress = Math.round((100 * event.loaded) / event.total);
			} else if (event.type === HttpEventType.Response) {
				this.message = event.body.toString();
			}
		});
	}
}
