import { Behavior } from '@/core/Entities/Behavior';
import { GasActor } from './GasActor';

interface GasBehaviorProps {
  actor: GasActor;
}

export class GasBehavior implements Behavior {
  actor: GasActor;
  originalY: number;
  disabledY: number;
  targetY: number;
  moveSpeed: number;

  constructor(props: GasBehaviorProps) {
    this.actor = props.actor;
    this.originalY = this.actor.mesh.position.y;
    this.disabledY = -1;
    this.targetY = this.originalY;
    this.moveSpeed = 0.2;
  }

  enable() {
    this.targetY = this.originalY;
  }

  disable() {
    this.targetY = this.disabledY;
  }

  disableImmediately() {
    this.disable();
    this.actor.mesh.position.y = this.targetY;
  }

  clamp(value: number, min: number, max: number) {
    return Math.min(Math.max(value, min), max);
  }

  update(delta: number) {
    this.actor.mesh.rotateZ(delta * 0.12);
    if (this.actor.mesh.position.y !== this.targetY) {
      const direction = this.targetY - this.actor.mesh.position.y < 0 ? -1 : 1;
      this.actor.mesh.position.y += direction * delta * this.moveSpeed;
      this.actor.mesh.position.y = this.clamp(
        this.actor.mesh.position.y,
        this.disabledY,
        this.originalY
      );
    }
  }
}
