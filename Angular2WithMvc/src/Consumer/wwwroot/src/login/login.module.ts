import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AngularComponentsModule, LoginComponent } from 'angular-components';

@NgModule({
	bootstrap: [LoginComponent],
	imports: [
		AngularComponentsModule,
		BrowserModule
	]
})
export class LoginModule{}
