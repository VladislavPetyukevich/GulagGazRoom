import { Vector3 } from 'three';
import { Entity } from '@/core/Entities/Entity';
import { ENTITY_TYPE, WALL } from '@/constants';
import { SmokeActor } from './SmokeActor';
import { SmokeBehavior } from './SmokeBehavior';
import { Player } from '../Player/Player';

export interface SmokeProps {
  position: Vector3;
  player: Player;
}

export class Smoke extends Entity<SmokeActor, SmokeBehavior> {
  constructor(props: SmokeProps) {
    const actor = new SmokeActor({
      position: props.position,
      player: props.player,
    });
    const behavior = new SmokeBehavior({
      actor
    });
    super(
      ENTITY_TYPE.SMOKE,
      actor,
      behavior
    );
  }

  disableEnableSmoke(isEnable: boolean) {
    if (isEnable) {
      this.behavior.enable();
    } else {
      this.behavior.disable();
    }
  }

  disableImmediately() {
    this.behavior.disableImmediately();
  }
}
