import { Injectable, Type } from '@angular/core';
import { Headers, Http, RequestOptionsArgs } from '@angular/http';
import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from '../configuration';
import { IEntity } from '../../models';
import { SecurityService } from '../security';

@Injectable()
export class ApiService {
    constructor(private http: Http, private configurationService: ConfigurationService, private securityService: SecurityService) { }

    public getSingle<TEntity extends Object, IEntity>(entityType: Type<TEntity>, id: number): Observable<TEntity> {
        const name = this.getTypeName(entityType);
        const resourceServerUrl = this.configurationService.resourceServerUrl;
        const url = `${resourceServerUrl}/api/${name}/${id}`;
        const headers = this.securityService.getHeaders();

        const getRequest = <any>this.http.get(url, { headers: headers });
        return getRequest.map(entity => entity.json());
    }

    public get<TEntity extends Object, IEntity>(entityType: Type<TEntity>): Observable<TEntity[]> {
        const name = this.getTypeName(entityType);
        const resourceServerUrl = this.configurationService.resourceServerUrl;
        const url = `${resourceServerUrl}/api/${name}`;
        const headers = this.securityService.getHeaders();

        const getRequest = <any>this.http.get(url, { headers: headers });
        return getRequest.map(entities => entities.json());
    }

    private getTypeName<TEntity>(entityType: Type<TEntity>): string {
        const entityFunction = <any>entityType;
        return entityFunction.name;
    }
}
