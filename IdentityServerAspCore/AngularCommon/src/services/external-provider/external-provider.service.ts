import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { ConfigurationService } from '../configuration';
import { ExternalProvider } from '../../models';
import { StorageService } from '../storage';

@Injectable()
export class ExternalProviderService {
    constructor(
        private http: Http,
        private configuration: ConfigurationService,
        private router: Router,
        private storageService: StorageService) { }

    public getProviders(): Observable<ExternalProvider[]> {
        const url = `${this.configuration.authorizationServerUrl}/account/GetExternalProviders?returnUrl=${this.configuration.postLoginRedirectUrl}`;
        const request: any = this.http.get(url);
        return request.map(response => response.json());
    }

    public login(authenticationScheme: string) {
        const returnUrl = this.router.routerState.snapshot.url;
        this.storageService.setItem('postLoginRedirectUrl', returnUrl);

        const url = `${this.configuration.authorizationServerUrl}/account/ExternalLogin?provider=${authenticationScheme}&returnUrl=${this.configuration.postLoginRedirectUrl}`;
        window.location.href = url;
    }
}
