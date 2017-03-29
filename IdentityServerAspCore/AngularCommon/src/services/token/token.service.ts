import { EventEmitter, Injectable } from '@angular/core';
import { Headers } from '@angular/http';

import { StorageService } from '../storage';
import { DataIdToken } from './data-id-token';

@Injectable()
export class TokenService {
    private _token: DataIdToken;
    private _claims: any;
    private tokenKey = 'security:token';

    public get token() {
        if (!this._token) {
            const serialized = this.storageService.getItem(this.tokenKey);
            this._token = serialized ? JSON.parse(serialized) : null;
            this.resetCountdowns();
        }
        return this._token;
    }

    public set token(value: DataIdToken) {
        const serialized = value ? JSON.stringify(value) : '';
        this.storageService.setItem(this.tokenKey, serialized);
        this._token = value;
        this.resetCountdowns();
    }

    public getClaims() {
        if (!this._claims && this.token) {
            this._claims = this.convertJwtToClaims(this.token.access_token);
        }
        return this._claims;
    }

    public tokenExpired = new EventEmitter<DataIdToken>();
    private tokenExpiryCountdown: any;

    public tokenHalflife = new EventEmitter<DataIdToken>();
    private tokenHalflifeCountdown: any;

    constructor(private storageService: StorageService) { }

    private resetCountdowns() {
        const token = this._token;
        const currentTime = Math.floor(new Date().getTime() / 1000);

        if (this.tokenExpiryCountdown) {
            clearTimeout(this.tokenExpiryCountdown);
            this.tokenExpiryCountdown = null;
        }
        if (token) {
            const claims = this.convertJwtToClaims(token.access_token);
            const secondsToTimeout = claims.exp - currentTime;
            this.tokenExpiryCountdown = setTimeout(() => this.tokenExpired.emit(token), secondsToTimeout);
        }

        if (this.tokenHalflifeCountdown) {
            clearTimeout(this.tokenHalflifeCountdown);
            this.tokenHalflifeCountdown = null;
        }
        if (token) {
            const claims = <any>this.convertJwtToClaims(token.access_token);
            const halflifeDate = claims.nbf + (claims.exp - claims.nbf) / 2;
            const secondsToHalflife = halflifeDate - currentTime;
            this.tokenHalflifeCountdown = setTimeout(() => this.tokenHalflife.emit(token), secondsToHalflife * 1000);
        }
    }

    public isTokenExpired() {
        if (!this.token) {
            return true;
        }
        const claims = this.getClaims();
        return new Date().getTime() > claims.exp;
    }

    public convertJwtToClaims(jwt: string): DataIdToken {
        if (jwt) {
            const subjectIndex = 1;
            const encoded = jwt.split('.')[subjectIndex];
            const decoded = this.urlBase64Decode(encoded);
            return JSON.parse(decoded);
        } else {
            return {};
        }
    }

    private urlBase64Decode(decoding: string) {
        let output = decoding.replace('_', '/').replace('_', '+');

        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += '==';
                break;
            case 3:
                output += '=';
                break;
            default:
                throw new Error('Illegal base64url string.');
        }

        return window.atob(output);
    }
}
