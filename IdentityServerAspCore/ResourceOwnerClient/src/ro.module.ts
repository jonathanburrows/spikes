import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Http, HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AngularCommonModule, ResourceOwnerSecurityService } from 'angular-common';

import {
    AppComponent,
    HomeComponent
} from './components';
import { RoRouterModule } from './ro.router.module';
import {
    ApiService,
    ConfigurationService,
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
        { provide: SecurityService, useClass: ResourceOwnerSecurityService },
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
        RoRouterModule
    ],
    bootstrap: [AppComponent]
})
export class RoModule { }
