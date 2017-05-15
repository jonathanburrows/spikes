import { RouterModule } from '@angular/router';

import { ListComponent } from './components';

export const appRouterModule = RouterModule.forRoot([
    { path: '', pathMatch: 'full', redirectTo: 'list' },
    { path: 'list', component: ListComponent }
]);
