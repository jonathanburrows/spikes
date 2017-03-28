﻿import { RouterModule } from '@angular/router';

import {
    HomeComponent
} from './components';

export const RoRouterModule = RouterModule.forRoot([
    { path: '', component: HomeComponent },
    { path: 'home', component: HomeComponent }
]);
