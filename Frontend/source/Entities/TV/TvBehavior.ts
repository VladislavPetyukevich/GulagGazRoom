import { Behavior } from '@/core/Entities/Behavior';
import { TvActor } from './TvActor';

interface TvBehaviorProps {
  actor: TvActor;
  screenSpinSpeed: number;
  screenSpinAxis: 'x' | 'y';
}

export class TvBehavior implements Behavior {
  actor: TvActor;
  screenSpinSpeed: number;
  screenSpinAxis: 'x' | 'y';

  constructor(props: TvBehaviorProps) {
    this.actor = props.actor;
    this.screenSpinSpeed = props.screenSpinSpeed;
    this.screenSpinAxis = props.screenSpinAxis;
  }

  update(delta: number) {
    if (this.actor.screenMaterial.map) {
      this.actor.screenMaterial.map.offset[this.screenSpinAxis] -= delta * this.screenSpinSpeed;
    }
  }
}
