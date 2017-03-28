import { IEntity } from './ientity';

export class UserLogin implements IEntity {
    public id: number;
    public username: string;
    public password: string;
}
