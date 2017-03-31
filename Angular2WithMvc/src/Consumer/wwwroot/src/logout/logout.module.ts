import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AngularComponentsModule, LogoutComponent } from 'angular-components';

@NgModule({
	bootstrap: [LogoutComponent],
	imports: [
		AngularComponentsModule,
		BrowserModule
	]
})
export class LogoutModule{}
