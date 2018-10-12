import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule, Routes } from "@angular/router";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MaterialFileInputModule } from "ngx-material-file-input";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { MaterialModule } from "../material/material.module";

import { FilesComponent } from "./components/files/files.component";
import { FilesHomeComponent } from "./components/files-home/files-home.component";
import { FilesUploadComponent } from "./components/files-upload/files-upload.component";
import { FileListItemComponent } from "./components/file-list-item/file-list-item.component";

const routes: Routes = [
	{
		path: "files",
		component: FilesComponent,
		children: [
			{ path: "", component: FilesHomeComponent },
			{ path: "upload", component: FilesUploadComponent }
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		BrowserModule,
		FormsModule,
		ReactiveFormsModule,
		MatFormFieldModule,
		MaterialFileInputModule,
		MaterialModule
	],
	declarations: [
		FilesComponent,
		FilesHomeComponent,
		FilesUploadComponent,
		FileListItemComponent
	],
	exports: [RouterModule]
})
export class FilesModule {}
