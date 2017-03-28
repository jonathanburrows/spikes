import { EventEmitter, Injectable } from '@angular/core';
import {
    Headers,
    Http,
    Response
} from '@angular/http';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { ConfigurationService } from '../configuration';
import { DataIdToken } from '../token/data-id-token';
import { SecurityService } from '../security';
import { StorageService } from '../storage';
import { TokenService } from '../token';

@Injectable()
export class ImplicitSecurityService extends SecurityService {
    private nonceKey = 'security:nonce';
    private stateKey = 'security:state';

    public redirectToLogin() {
        this.postLoginRedirectUrl = this.router.routerState.snapshot.url;
        window.location.href = this.getRedirectUrl();
    }

    private getRedirectUrl(idTokenHint?: string) {
        const nonce = 'N' + Math.random() + '' + Date.now();
        const state = Date.now() + '' + Math.random();

        this.storageService.setItem(this.nonceKey, nonce);
        this.storageService.setItem(this.stateKey, state);

        const options: AuthorizationOptions = {
            client_id: 'implicit-client',
            redirect_uri: `${this.configurationService.url}/account/signin-oidc`,
            response_type: 'id_token token',
            scope: 'resource-server openid profile',
            id_token_hint: idTokenHint,
            prompt: idTokenHint ? 'none' : undefined,
            nonce: nonce,
            state: state
        };

        const queryParameters = this.convertLiteralToQueryParameters(options);
        return `${this.configurationService.authorizationServerUrl}/connect/authorize?${queryParameters}`;
    }

    public login() {
        const hash = window.location.hash.substr(1);

        const result = <AuthorizationResult>hash.split('&').reduce((r: Object, item: string) => {
            const keyValue = item.split('=');
            r[keyValue[0]] = keyValue[1];
            return r;
        }, {});

        const originalNonce = this.storageService.getItem(this.nonceKey);
        const originalState = this.storageService.getItem(this.stateKey);
        const existingToken = this.tokenService.token;
        const resultingToken = this.tokenService.convertJwtToClaims(result.id_token);

        if (result.error) {
            this.tokenService.token = null;
            this.router.navigate(['/account/unauthorized']);
        } else if (originalState !== result.state || originalNonce !== resultingToken.nonce) {
            this.tokenService.token = null;
            this.router.navigate(['/account/malicious-warning']);
        } else {
            this.tokenService.token = result;
            this.router.navigate([this.postLoginRedirectUrl]);
        }
    }

    public logout() {
        const options: LogoffOptions = {
            id_token_hint: this.tokenService.token.id_token,
            post_logout_redirect_uri: this.configurationService.url
        };

        const queryParameters = this.convertLiteralToQueryParameters(options);
        const url = `${this.configurationService.authorizationServerUrl}/connect/endsession?${queryParameters}`;

        this.tokenService.token = null;
        window.location.href = url;
    }

    public logoutLocal() {
        this.tokenService.token = null;
    }

    public refreshToken() {
        const refreshAction = new Subject<DataIdToken>();

        const renewIframe = window.document.createElement('iframe');
        renewIframe.style.display = 'none';
        window.document.body.appendChild(renewIframe);

        renewIframe.onload = () => {
            const serialized = this.storageService.getItem('security:token');
            this.tokenService.token = JSON.parse(serialized);
            refreshAction.next(this.tokenService.token);
            renewIframe.remove();
        };

        const hint = this.tokenService.token ? this.tokenService.token.id_token : null;
        renewIframe.src = this.getRedirectUrl(hint);

        return refreshAction;
    }

    private convertLiteralToQueryParameters(literal: Object) {
        const parameters = [];
        for (const i in literal) {
            if (literal[i]) {
                const encodedValue = encodeURI(literal[i]);
                parameters.push(`${i}=${encodedValue}`);
            }
        }

        return parameters.join('&');
    }
}

interface AuthorizationOptions {
    response_type: string;
    client_id: string;
    redirect_uri: string;
    scope: string;
    nonce: string;
    state: string;
    id_token_hint?: string;
    prompt?: string;
}

interface AuthorizationResult {
    id_token: string;
    access_token: string;
    state: string;
    error: string;
}

interface LogoffOptions {
    id_token_hint: string;
    post_logout_redirect_uri: string;
}
