import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { LogoutModule } from './logout.module';

enableProdMode();

platformBrowserDynamic().bootstrapModule(LogoutModule);