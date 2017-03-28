import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { environment } from './environments/environment';
import { RoModule } from './ro.module';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(RoModule);
