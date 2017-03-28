import { Component, HostBinding } from '@angular/core';

import { slideInDownAnimation } from '../../animations';
import { UserLogin } from '../../models';
import { SecurityService } from '../../services';

@Component({
    selector: 'ic-login',
    templateUrl: 'login.component.html',
    styleUrls: ['./login.component.scss'],
    animations: [slideInDownAnimation]
})
export class LoginComponent {
    public model: UserLogin;

    @HostBinding('@routeAnimation') routeAnimation = true;

    constructor(private securityService: SecurityService) {
        this.model = new UserLogin();
    }

    public login() {
        this.securityService.login({
            username: this.model.username,
            password: this.model.password
        });
    }
}
