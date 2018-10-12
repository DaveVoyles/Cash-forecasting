import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FlexLayoutModule } from "@angular/flex-layout";
import { RouterModule } from "@angular/router";

import { MaterialModule } from "../modules/material/material.module";
import { HeaderComponent } from "./components/header/header.component";
import { LoginComponent } from "./components/login/login.component";
import { UnauthorizedComponent } from "./components/unauthorized/unauthorized.component";
import { HomeComponent } from "./components/home/home.component";

@NgModule({
	imports: [CommonModule, MaterialModule, FlexLayoutModule, RouterModule],
	exports: [HeaderComponent],
	declarations: [
		HeaderComponent,
		LoginComponent,
		UnauthorizedComponent,
		HomeComponent
	]
})
export class SharedModule {}
