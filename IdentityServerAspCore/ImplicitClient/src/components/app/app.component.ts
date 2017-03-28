import { Component } from '@angular/core';

import { SecurityService } from 'angular-common';

@Component({
    selector: 'ic-app',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.scss']
})
export class AppComponent {
    constructor(public securityService: SecurityService) { }

    public login() {
        this.securityService.redirectToLogin();
    }

    public logoff() {
        this.securityService.logout();
    }
}
