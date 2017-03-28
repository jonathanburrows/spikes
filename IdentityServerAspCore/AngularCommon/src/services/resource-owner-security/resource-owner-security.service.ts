import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { ConfigurationService } from '../configuration';
import { Credentials, SecurityService } from '../security';
import { StorageService } from '../storage';
import { DataIdToken, TokenService } from '../token';

@Injectable()
export class ResourceOwnerSecurityService extends SecurityService {
    public redirectToLogin() {
        this.postLoginRedirectUrl = this.router.routerState.snapshot.url;
        this.router.navigate(['/account/login']);
    }

    public login(credentials: Credentials) {
        const loginOptions: TokenRequestOptions = {
            grant_type: 'password',
            username: credentials.username,
            password: credentials.password
        };
        this.requestToken(loginOptions).subscribe(dataIdToken => {
            this.tokenService.token = dataIdToken;
            this.router.navigate([this.postLoginRedirectUrl]);
        });
    }

    public logout() {
        throw new Error('Not yet implemented.');
    }

    public logoutLocal() {
        debugger;
        this.tokenService.token = null;
    }

    public refreshToken() {
        const refreshOptions: TokenRequestOptions = {
            refresh_token: this.tokenService.token.refresh_token,
            grant_type: 'refresh_token'
        };
        const request = this.requestToken(refreshOptions);
        request.subscribe(dataIdToken => this.tokenService.token = dataIdToken);
        return request;
    }

    private requestToken(tokenOptions: TokenRequestOptions): Observable<DataIdToken> {
        const url = `${this.configurationService.authorizationServerUrl}/connect/token`;

        const headers = new Headers();
        headers.append('content-type', 'application/x-www-form-urlencoded');

        tokenOptions.client_id = this.configurationService.clientName;
        tokenOptions.client_secret = this.configurationService.clientSecret;
        tokenOptions.scope = 'openid profile offline_access resource-server';

        const urlEncodedBody = this.urlEncodeBody(tokenOptions);

        const request = <any>this.http.post(url, urlEncodedBody, { headers: headers });
        return request.map(tokenResult => tokenResult.json());
    }

    private urlEncodeBody(body: any) {
        const parameters: { key: string, value: string }[] = [];
        for (const i in body) {
            if (body[i]) {
                parameters.push({
                    key: i,
                    value: body[i]
                });
            }
        }
        return parameters.map(p => `${p.key}=${p.value}`).join('&');
    }
}

interface TokenRequestOptions {
    grant_type: string;
    client_id?: string;
    client_secret?: string;
    scope?: string;
    username?: string;
    password?: string;
    refresh_token?: string;
}
