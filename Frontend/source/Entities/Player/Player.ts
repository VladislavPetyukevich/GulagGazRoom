import { Vector3, Camera, AudioListener } from 'three';
import { Entity } from '@/core/Entities/Entity';
import { EntitiesContainer } from '@/core/Entities/EntitiesContainer';
import { PlayerActor } from './PlayerActor';
import { ControlledBehavior } from './ControlledBehavior';
import { PLAYER, ENTITY_TYPE } from '@/constants';

export interface PlayerProps {
  position: Vector3;
  camera: Camera;
  container: EntitiesContainer;
  audioListener: AudioListener;
}

export class Player extends Entity<PlayerActor, ControlledBehavior> {
  camera: Camera;
  container: EntitiesContainer;
  hp: number;
  isDead: boolean;
  onHitCallback?: Function;
  onDeathCallback?: Function;

  constructor(props: PlayerProps) {
    const actor = new PlayerActor({
      position: new Vector3(props.position.x, props.position.y, props.position.z),
      size: { width: PLAYER.BODY_WIDTH, height: PLAYER.BODY_HEIGHT, depth: PLAYER.BODY_DEPTH }
    });
    props.camera.position.set(props.position.x, props.position.y, props.position.z);
    const velocity = new Vector3();
    super(
      ENTITY_TYPE.PLAYER,
      actor,
      new ControlledBehavior({
        actor: actor,
        camera: props.camera,
        eyeY: PLAYER.BODY_HEIGHT,
        container: props.container,
        velocity: velocity,
        audioListener: props.audioListener
      }),
    );
    this.camera = props.camera;
    this.container = props.container;
    this.velocity = velocity;
    this.hp = PLAYER.HP;
    this.isDead = false;
  }

  onHit(damage: number) {
    super.onHit(damage);
    if (this.isDead) {
      return;
    }
    if (this.hp <= 0) {
      this.isDead = true;
      this.cantMove();
      this.behavior.onDeath();
      if (this.onDeathCallback) {
        this.onDeathCallback();
      }
      return;
    }
    this.behavior.onHit();
    if (this.onHitCallback) {
      this.onHitCallback();
    }
  }

  setOnHitCallback(callback: Function) {
    this.onHitCallback = callback;
  }

  setOnDeathCallback(callback: Function) {
    this.onDeathCallback = callback;
  }

  setHp(hp: number) {
    this.hp = hp;
  }

  canMove() {
    this.behavior.isCanMove = true;
  }

  cantMove() {
    this.behavior.isCanMove = false;
  }
}
