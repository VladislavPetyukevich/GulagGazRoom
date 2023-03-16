import { Behavior } from '@/core/Entities/Behavior';
import { GasActor } from './GasActor';

interface GasBehaviorProps {
  actor: GasActor;
  maxY: number;
}

export class GasBehavior implements Behavior {
  actor: GasActor;
  originalY: number;
  maxY: number;
  enabled: boolean;
  moveSpeed: number;

  constructor(props: GasBehaviorProps) {
    this.actor = props.actor;
    this.maxY = props.maxY;
    this.originalY = this.actor.mesh.position.y;
    this.enabled = false;
    this.moveSpeed = 0.2;
    // this.moveSpeed = 0.5;
  }

  enable() {
    this.enabled = true;
  }

  disable() {
    this.enabled = false;
  }

  disableImmediately() {
    this.disable();
    this.actor.mesh.position.y = this.originalY;
  }

  clamp(value: number, min: number, max: number) {
    return Math.min(Math.max(value, min), max);
  }

  update(delta: number) {
    this.actor.mesh.rotateZ(delta * 0.12);
    if (
      this.enabled ||
      this.actor.mesh.position.y !== this.originalY
    ) {
      this.actor.mesh.position.y += delta * this.moveSpeed;
      this.actor.mesh.position.y = this.clamp(
        this.actor.mesh.position.y,
        this.originalY,
        this.maxY
      );
      if (this.actor.mesh.position.y === this.maxY) {
        this.actor.mesh.position.y = this.originalY;
      }
    }
  }
}
