import { NgModule } from "@angular/core";
import {
	MatButtonModule,
	MatCheckboxModule,
	MatToolbarModule,
	MatFormFieldModule,
	MatIconModule,
	MatSlideToggleModule,
	MatInputModule,
	MatRippleModule,
	MatDatepickerModule,
	MatNativeDateModule,
	MatMenuModule,
	MatListModule
} from "@angular/material";

@NgModule({
	exports: [
		MatButtonModule,
		MatCheckboxModule,
		MatToolbarModule,
		MatFormFieldModule,
		MatIconModule,
		MatSlideToggleModule,
		MatInputModule,
		MatRippleModule,
		MatDatepickerModule,
		MatNativeDateModule,
		MatMenuModule,
		MatListModule
	]
})
export class MaterialModule {}
