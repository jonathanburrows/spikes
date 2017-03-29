import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Http, HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import {
    ForbiddenComponent,
    LoginComponent,
    LogoutLocalComponent,
    MaliciousWarningComponent,
    SecretComponent,
    SigninOidcComponent,
    UnauthorizedComponent
} from './components';
import { AngularCommonRouterModule } from './angular-common.router.module';
import {
    ApiService,
    ConfigurationService,
    LocalStorageService,
    ExternalProviderService,
    ImplicitSecurityService,
    StorageService,
    TokenService
} from './services';

@NgModule({
    declarations: [
        ForbiddenComponent,
        LoginComponent,
        LogoutLocalComponent,
        MaliciousWarningComponent,
        SecretComponent,
        SigninOidcComponent,
        UnauthorizedComponent
    ],
    exports: [
        ForbiddenComponent,
        LoginComponent,
        LogoutLocalComponent,
        MaliciousWarningComponent,
        SecretComponent,
        SigninOidcComponent,
        UnauthorizedComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        MaterialModule.forRoot(),
        RouterModule,
        AngularCommonRouterModule
    ],
    providers: [
        ExternalProviderService
    ]
})
export class AngularCommonModule { }
