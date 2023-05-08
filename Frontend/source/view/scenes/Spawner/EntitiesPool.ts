import { Entity } from '@/core/Entities/Entity';

export class EntitiesPool {
  entities: Entity[];

  constructor(createCallback: () => Entity, count: number) {
    this.entities = Array.from({ length: count }, createCallback);
  }
};
