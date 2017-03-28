import { RouterModule } from '@angular/router';

import {
    ForbiddenComponent,
    LoginComponent,
    LogoutLocalComponent,
    SecretComponent,
    SigninOidcComponent,
    UnauthorizedComponent
} from './components';

export const AngularCommonRouterModule = RouterModule.forChild([
    { path: 'account/forbidden', component: ForbiddenComponent },
    { path: 'account/secret', component: SecretComponent },
    { path: 'account/login', component: LoginComponent },
    { path: 'account/logout-local', component: LogoutLocalComponent },
    { path: 'account/signin-oidc', component: SigninOidcComponent },
    { path: 'account/unauthorized', component: UnauthorizedComponent }
]);
