import { Mesh, BoxGeometry, MeshLambertMaterial, Vector3 } from 'three';
import { Actor } from '@/core/Entities/Actor';
import { Player } from '@/Entities/Player/Player';
import { texturesStore } from '@/core/loaders/TextureLoader';
import { randomNumbers } from '@/RandomNumbers';

interface GasActorProps {
  position: Vector3;
  player: Player;
}

export class GasActor implements Actor {
  mesh: Mesh;
  player: Player;

  constructor(props: GasActorProps) {
    const smokeFile = texturesStore.getTexture('gasTextureFile');
    const size = 1.5;
    const geometry = new BoxGeometry(size, size, 0.1);
    const material = new MeshLambertMaterial({
      map: smokeFile,
      transparent: true,
      opacity: 0.25,
    });
    material.transparent = true;
    const materials: MeshLambertMaterial[] = [];
    materials[4] = material;
    this.mesh = new Mesh(geometry, materials);
    this.mesh.position.copy(props.position);
    this.mesh.rotateZ(randomNumbers.getRandomFloatInRange(0, Math.PI * 2));
    this.player = props.player;
  }

  update() {
    const playerMesh = this.player.mesh;
    this.mesh.rotation.y = Math.atan2(
      (playerMesh.position.x - this.mesh.position.x), (playerMesh.position.z - this.mesh.position.z)
    );
  }
}
