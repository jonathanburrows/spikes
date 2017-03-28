import { IEntity } from './ientity';

export class Planet implements IEntity {
    public id: number;
    public name: string;
    public hexColor: string;
    public radius: number;
}
