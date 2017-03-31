import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { RouterModule } from '@angular/router';

import {
    LoginComponent,
	LogoutComponent
} from './components';

@NgModule({
    declarations: [
        LoginComponent,
		LogoutComponent
    ],
    exports: [
        LoginComponent,
		LogoutComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        MaterialModule.forRoot(),
        RouterModule
    ],
	bootstrap: [
		LoginComponent,
		LogoutComponent
	]
})
export class AngularComponentsModule { }