import { Component, OnInit } from '@angular/core';

import { SecurityService } from '../../services';

@Component({
    selector: 'ic-signin-oidc',
    templateUrl: 'signin-oidc.component.html'
})
export class SigninOidcComponent implements OnInit {
    constructor(private securityService: SecurityService) { }

    ngOnInit() {
        this.securityService.login();
    }
}
