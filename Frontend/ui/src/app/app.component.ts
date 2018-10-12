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
	selector: "app-root",
	templateUrl: "./app.component.html",
	styleUrls: ["./app.component.css"]
})
export class AppComponent implements OnInit {
	constructor() {}

	ngOnInit() {}
}
