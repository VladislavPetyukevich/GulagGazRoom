import { Actor } from './Actor';
import { Behavior } from './Behavior';
import { Vector3 } from 'three';

export class Entity<A extends Actor = Actor, B extends Behavior = Behavior> {
  type: string;
  actor: A;
  behavior: B;
  tag?: string;
  hp?: number;
  velocity?: Vector3;

  constructor(type: string, actor: A, behavior: B) {
    this.type = type;
    this.actor = actor;
    this.behavior = behavior;
  }

  get mesh() {
    return this.actor.mesh;
  }

  onHit(damage: number, entity?: Entity) {
    if (typeof this.hp === 'number') {
      this.hp -= damage;
    }
  }

  onMessage(message: string | number) { }

  onDestroy() { }

  setScaticPositionOptimizations(isEnabled: boolean) {
    this.mesh.updateMatrix();
    this.mesh.matrixAutoUpdate = !isEnabled;
  }

  update(delta: number) {
    this.actor.update(delta);
    this.behavior.update(delta);
  }
};
