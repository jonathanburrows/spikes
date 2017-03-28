import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Http, HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import {
    AppComponent,
    HomeComponent
} from './components';
import { IcRouterModule } from './ic.router.module';
import {
    AngularCommonModule,
    ApiService,
    ConfigurationService,
    ImplicitSecurityService,
    LocalStorageService,
    SecurityService,
    StorageService,
    TokenService
} from 'angular-common';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    exports: [
        AppComponent,
        HomeComponent
    ],
    providers: [
        ApiService,
        ConfigurationService,
        { provide: SecurityService, useClass: ImplicitSecurityService },
        { provide: StorageService, useClass: LocalStorageService },
        TokenService
    ],
    imports: [
        AngularCommonModule,
        BrowserModule,
        CommonModule,
        FormsModule,
        HttpModule,
        MaterialModule.forRoot(),
        RouterModule,
        IcRouterModule
    ],
    bootstrap: [AppComponent]
})
export class IcModule { }
