import { Scene, Mesh, Vector3 } from 'three';
import { Entity } from './Entity';

export class EntitiesContainer {
  scene: Scene;
  entities: Entity[];
  entitiesMeshes: Mesh[];
  meshIdToEntity: Map<number, Entity>;

  constructor(scene: Scene) {
    this.scene = scene;
    this.entities = [];
    this.entitiesMeshes = [];
    this.meshIdToEntity = new Map();
  }

  add(entity: Entity) {
    this.entities.push(entity);
    this.entitiesMeshes.push(entity.mesh);
    this.meshIdToEntity.set(entity.mesh.id, entity);
    this.scene.add(entity.mesh);
    return entity;
  }

  remove(mesh: Mesh) {
    const meshId = mesh.id;
    for (let i = this.entities.length; i--;) {
      if (this.entities[i].mesh.id === meshId) {
        this.entities.splice(i, 1);
        this.entitiesMeshes.splice(i, 1);
        break;
      }
    }
    this.scene.remove(mesh);
  }

  getEntityByMeshId(id: number) {
    return this.meshIdToEntity.get(id);
  }

  onDestroy() {
    this.entities.forEach(entity => {
      entity.onDestroy();
    });
  }

  update(delta: number) {
    this.entities.forEach(entity => {
      if (!entity.velocity) {
        entity.update(delta);
        return;
      }

      entity.mesh.position.set(
        entity.mesh.position.x,
        entity.mesh.position.y + entity.velocity.y * delta,
        entity.mesh.position.z
      );
      const newPositionX = entity.mesh.position.x + entity.velocity.x * delta;
      const newPositionZ = entity.mesh.position.z + entity.velocity.z * delta;
      this.updateEntityPosition(
        entity,
        new Vector3(
          newPositionX,
          entity.mesh.position.y,
          newPositionZ
        )
      );
      entity.update(delta);
    });
  }

  updateEntityPosition(entity: Entity, newPosition: Vector3) {
    entity.mesh.position.set(
      newPosition.x,
      newPosition.y,
      newPosition.z
    );
  }
}
