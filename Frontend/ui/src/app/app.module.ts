import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HttpModule } from "@angular/http";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { RouterModule, Routes } from "@angular/router";
import { FlexLayoutModule } from "@angular/flex-layout";
import { AdalService, AdalGuard, AdalInterceptor } from "adal-angular4";

import { MaterialModule } from "./modules/material/material.module";
import { SharedModule } from "./shared/shared.module";
import { FilesModule } from "./modules/files/files.module";

import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MaterialFileInputModule } from "ngx-material-file-input";
import { AppComponent } from "./app.component";

import { UnauthorizedComponent } from "./shared/components/unauthorized/unauthorized.component";
import { HomeComponent } from "./shared/components/home/home.component";

const routes: Routes = [
	{ path: "", redirectTo: "/files", pathMatch: "full" },
	// { path: "unauthorized", component: UnauthorizedComponent },
	{
		path: "home",
		component: HomeComponent,
		// canActivate: [AdalGuard]
	}
];

@NgModule({
	declarations: [AppComponent],
	imports: [
		RouterModule.forRoot(routes),
		BrowserModule,
		BrowserAnimationsModule,
		HttpModule,
		HttpClientModule,
		RouterModule,
		FlexLayoutModule,
		FormsModule,
		ReactiveFormsModule,
		MatFormFieldModule,
		SharedModule,
		MaterialModule,
		FilesModule,
		MaterialFileInputModule
	],
	exports: [RouterModule],
	providers: [
		AdalService,
		AdalGuard,
		{ provide: HTTP_INTERCEPTORS, useClass: AdalInterceptor, multi: true }
	],
	bootstrap: [AppComponent]
})
export class AppModule {}
