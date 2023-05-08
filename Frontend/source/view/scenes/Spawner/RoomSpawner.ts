import {
  PlaneGeometry,
  Mesh,
  Matrix4,
  MeshLambertMaterial,
  Vector2,
  Vector3,
  RepeatWrapping,
  Color,
} from 'three';
import { Entity } from '@/core/Entities/Entity';
import { texturesStore } from '@/core/loaders/TextureLoader';
import { WALL, PI_2 } from '@/constants';
import { Player } from '@/Entities/Player/Player';
import { WallProps } from '@/Entities/Wall/Wall';
import { WallNeutral } from '@/Entities/Wall/Inheritor/WallNeutral';
import { CellCoordinates } from '@/scenes/CellCoordinates';
import { EntitiesContainer } from '@/core/Entities/EntitiesContainer';
import { TestScene } from '../testScene';

export interface Room {
  cellPosition: Vector2;
  floor: Mesh;
  entities: Entity[];
}

export interface RoomSpawnerProps {
  scene: TestScene;
  player: Player;
  entitiesContainer: EntitiesContainer;
  cellCoordinates: CellCoordinates;
  roomSize: Vector2;
  doorWidthHalf: number;
}

export class RoomSpawner {
  scene: RoomSpawnerProps['scene'];
  player: RoomSpawnerProps['player'];
  entitiesContainer: RoomSpawnerProps['entitiesContainer'];
  cellCoordinates: RoomSpawnerProps['cellCoordinates'];
  roomSize: RoomSpawnerProps['roomSize'];
  doorWidthHalf: RoomSpawnerProps['doorWidthHalf'];

  constructor(props: RoomSpawnerProps) {
    this.scene = props.scene;
    this.player = props.player;
    this.entitiesContainer = props.entitiesContainer;
    this.cellCoordinates = props.cellCoordinates;
    this.roomSize = props.roomSize;
    this.doorWidthHalf = props.doorWidthHalf;
  };

  createRoom(cellPosition: Vector2): Room {
    const worldCoordinates = this.cellCoordinates.toWorldCoordinates(cellPosition);
    const worldSize = this.cellCoordinates.toWorldCoordinates(this.roomSize);
    const room: Room = {
      cellPosition: cellPosition,
      floor: this.spawnRoomFloor(worldCoordinates, worldSize),
      entities: [],
    };
    return room;
  }

  deleteRoom(room: Room | null) {
    if (!room) {
      return;
    }
    this.scene.scene.remove(room.floor);
    room.entities.forEach(entity =>
      this.entitiesContainer.remove(entity.mesh)
    );
  }

  removeEntity(entity: Entity | null) {
    if (!entity) {
      return;
    }
    this.entitiesContainer.remove(entity.mesh);
  }

  spawnRoomFloor(worldCoordinates: Vector2, worldSize: Vector2) {
    const floorGeometry = new PlaneGeometry(worldSize.x, worldSize.y);
    floorGeometry.applyMatrix(new Matrix4().makeRotationX(-PI_2));
    const floorTexture = texturesStore.getTexture('floorTextureFile');
    floorTexture.wrapS = floorTexture.wrapT = RepeatWrapping;
    floorTexture.repeat.x = floorTexture.repeat.y = 32;
    floorTexture.needsUpdate = true;
    const floorMaterial = new MeshLambertMaterial({ map: texturesStore.getTexture('floorTextureFile') });
    const floorMesh = new Mesh(floorGeometry, floorMaterial);
    const floorPosition = this.getCenterPosition(
      worldCoordinates, worldSize
    );
    floorMesh.position.set(
      floorPosition.x,
      0,
      floorPosition.y
    );
    floorMesh.receiveShadow = true;
    this.scene.scene.add(floorMesh);
    return floorMesh;
  }

  spawnWall(coordinates: Vector2, size: Vector2, withDecals: boolean) {
    const isHorizontalWall = size.x > size.y;
    const props: WallProps = {
      position: new Vector3(coordinates.x, 1.5, coordinates.y),
      size: { width: size.x, height: WALL.SIZE, depth: size.y },
      isHorizontalWall: isHorizontalWall,
      withDecals: withDecals,
    };
    const wallNeutral = this.entitiesContainer.add(
      new WallNeutral(props)
    );
    wallNeutral.setScaticPositionOptimizations(true);
    return wallNeutral;
  }

  getCenterPosition(position: Vector2, size: Vector2) {
    return new Vector2(
      position.x + size.x / 2,
      position.y + size.y / 2
    );
  }
}
