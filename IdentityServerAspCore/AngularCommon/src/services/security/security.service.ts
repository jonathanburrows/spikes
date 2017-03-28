import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { ConfigurationService } from '../configuration';
import { Credentials } from './credentials';
import { StorageService } from '../storage';
import { DataIdToken, TokenService } from '../token';

@Injectable()
export abstract class SecurityService {
    public get isAuthorized() {
        return this.tokenService.token && this.tokenService.isTokenExpired;
    }

    public get postLoginRedirectUrl() {
        return this._postLoginRedirectUrl || this.storageService.getItem('postLoginRedirectUrl') || '/';
    }
    public set postLoginRedirectUrl(value: string) {
        this._postLoginRedirectUrl = value;
        this.storageService.setItem('postLoginRedirectUrl', value || '');
    }
    private _postLoginRedirectUrl: string;

    constructor(
        protected http: Http,
        protected router: Router,
        protected tokenService: TokenService,
        protected configurationService: ConfigurationService,
        protected storageService: StorageService) {

        this.tokenService.tokenHalflife.subscribe(_ => this.refreshToken());
    }

    public abstract redirectToLogin();

    public abstract login(credentials?: Credentials);

    public abstract logout();

    public abstract logoutLocal();

    public abstract refreshToken(): Observable<DataIdToken>;

    protected getUserData(): Observable<string[]> {
        const headers = this.getHeaders();
        const userInfoUrl = `${this.configurationService.authorizationServerUrl}/connect/userinfo`;
        const userDataRequest = <any>this.http.get(userInfoUrl, { headers: headers, body: '' });
        return userDataRequest.map(res => res.json());
    }

    protected handleError(error: any) {
        if (error.status === 403) {
            this.router.navigate(['/Forbidden']);
        } else if (error.status === 401) {
            this.tokenService.token = null;
            this.router.navigate(['/Unauthorized']);
        }
    }

    public getHeaders() {
        const headers = new Headers({
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        });

        if (this.tokenService.token) {
            headers.append('Authorization', `Bearer ${this.tokenService.token.access_token}`);
        }

        return headers;
    }
}
