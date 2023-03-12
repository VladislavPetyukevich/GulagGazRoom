import {
  Camera,
  Vector3,
  Audio,
  AudioListener,
} from 'three';
import { PlayerActor } from './PlayerActor';
import { Behavior } from '@/core/Entities/Behavior';
import { EntitiesContainer } from '@/core/Entities/EntitiesContainer';
import { SinTable } from '@/SinTable';
import { audioStore } from '@/core/loaders';

interface ControlledBehaviorProps {
  actor: PlayerActor;
  camera: Camera;
  eyeY: number;
  container: EntitiesContainer;
  velocity: Vector3;
  audioListener: AudioListener;
}

export class ControlledBehavior implements Behavior {
  actor: PlayerActor;
  camera: Camera;
  isCanMove: boolean;
  eyeY: number;
  container: EntitiesContainer;
  velocity: Vector3;
  damageSound: Audio;
  sinTable: SinTable;
  bobTimeout: number;
  maxBobTimeout: number;

  constructor(props: ControlledBehaviorProps) {
    this.sinTable = new SinTable({
      step: 1,
      amplitude: 0.06,
    });
    this.bobTimeout = 0;
    this.maxBobTimeout = 0.001;
    this.actor = props.actor;
    this.eyeY = props.eyeY;
    this.camera = props.camera;
    this.camera.position.y = this.eyeY;
    this.isCanMove = true;
    this.container = props.container;
    this.velocity = props.velocity;
    this.damageSound = new Audio(props.audioListener);
    const damageSoundBuffer = audioStore.getSound('damage');
    this.damageSound.setBuffer(damageSoundBuffer);
    this.damageSound.isPlaying = false;
    this.damageSound.setVolume(0.6);
  }

  onHit() {
    if (this.damageSound.isPlaying) {
      this.damageSound.stop();
    }
    this.damageSound.play();
  }

  onDeath() {
    this.damageSound.stop();
  }

  updateBob(delta: number) {
    this.bobTimeout += delta;
    if (this.bobTimeout >= this.maxBobTimeout) {
      this.bobTimeout = 0;
      const sinValue = this.sinTable.getNextSinValue();
      this.camera.position.y = this.actor.mesh.position.y + sinValue;
    }
  }

  update(delta: number) {
    this.updateCamera();
    this.updatePlayerBob(delta);
  }

  updateCamera() {
    this.camera.position.x = this.actor.mesh.position.x;
    this.camera.position.z = this.actor.mesh.position.z;
  }

  updatePlayerBob(delta: number) {
    this.updateBob(delta);
  }
}
