import { Component, HostBinding } from '@angular/core';

import { slideInDownAnimation } from '../../animations';
import { ExternalProvider, UserLogin } from '../../models';
import { ExternalProviderService, SecurityService } from '../../services';

@Component({
    selector: 'ic-login',
    templateUrl: 'login.component.html',
    styleUrls: ['./login.component.scss'],
    animations: [slideInDownAnimation]
})
export class LoginComponent {
    public model: UserLogin;
    public externalProviders: ExternalProvider[] = [];

    @HostBinding('@routeAnimation') routeAnimation = true;

    constructor(private externalProviderService: ExternalProviderService, private securityService: SecurityService) {
        this.model = new UserLogin();
        this.externalProviderService.getProviders().subscribe(externalProviders => this.externalProviders = externalProviders);
    }

    public login() {
        this.securityService.login({
            username: this.model.username,
            password: this.model.password
        });
    }
}
