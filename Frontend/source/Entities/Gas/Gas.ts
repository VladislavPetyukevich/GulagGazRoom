import { Vector3 } from 'three';
import { Entity } from '@/core/Entities/Entity';
import { ENTITY_TYPE, WALL } from '@/constants';
import { GasActor } from './GasActor';
import { GasBehavior } from './GasBehavior';
import { Player } from '../Player/Player';

export interface GasProps {
  position: Vector3;
  maxY: number;
  player: Player;
}

export class Gas extends Entity<GasActor, GasBehavior> {
  constructor(props: GasProps) {
    const actor = new GasActor({
      position: props.position,
      player: props.player,
    });
    const behavior = new GasBehavior({
      actor,
      maxY: props.maxY,
    });
    super(
      ENTITY_TYPE.GAS,
      actor,
      behavior
    );
  }

  disableEnableGas(isEnable: boolean) {
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
