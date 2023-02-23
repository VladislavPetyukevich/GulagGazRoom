import { Group, Vector3 } from 'three';
import { Entity } from '@/core/Entities/Entity';
import { ENTITY_TYPE, WALL } from '@/constants';
import { TvActor } from './TvActor';
import { TvBehavior } from './TvBehavior';

export interface TvProps {
  position: Vector3;
  rotationY: number;
  model: Group;
  screenSpinSpeed: number;
  screenSpinAxis: 'x' | 'y';
}

export class TV extends Entity<TvActor, TvBehavior> {
  constructor(props: TvProps) {
    const actor = new TvActor({
      position: props.position,
      rotationY: props.rotationY,
      model: props.model,
    });
    const behavior = new TvBehavior({
      actor,
      screenSpinSpeed: props.screenSpinSpeed,
      screenSpinAxis: props.screenSpinAxis,
    });
    super(
      ENTITY_TYPE.TV,
      actor,
      behavior
    );
  }

  printText(text: string) {
    this.actor.printText(text);
  }
}
