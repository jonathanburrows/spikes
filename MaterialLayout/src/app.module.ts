import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
    LayoutComponent,
    ListComponent,
    RootComponent
} from './components';

import { appRouterModule } from './app.router.module';

@NgModule({
    declarations: [
        LayoutComponent,
        ListComponent,
        RootComponent
    ],
    imports: [
        appRouterModule,
        BrowserAnimationsModule,
        BrowserModule,
        FormsModule,
        HttpModule,
        MaterialModule
    ],
    providers: [],
    bootstrap: [RootComponent]
})
export class AppModule { }
