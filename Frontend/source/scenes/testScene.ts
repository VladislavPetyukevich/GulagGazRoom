import {
  PointLight,
  Vector2,
  Vector3,
  Fog,
  AmbientLight,
  MeshPhongMaterial,
  RepeatWrapping,
  Group,
} from 'three';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { BasicSceneProps, BasicScene } from '@/core/Scene';
import { PLAYER } from '@/constants';
import { Player } from '@/Entities/Player/Player';
import { Gas } from '@/Entities/Gas/Gas';
import { Room, RoomSpawner } from './Spawner/RoomSpawner';
import { CellCoordinates } from './CellCoordinates';
import { randomNumbers } from '@/RandomNumbers';
import HomePakTV from '@/assets/HomePakTV.fbx';
import { TextCanvas } from '@/TextCanvas';
import { TimeoutsManager } from '@/TimeoutsManager';
import { EntitiesPool } from './Spawner/EntitiesPool';
import { playerActions } from '@/PlayerActions';

export interface TestSceneProps extends BasicSceneProps {
  onFinish: Function;
}

let tvQuestionMaterial: MeshPhongMaterial;
let tvChatMaterial: MeshPhongMaterial;
let tvRandomMaterial: MeshPhongMaterial;

let chatText: TextCanvas;

const enum TvText {
  Question,
  Chat,
  Random,
};

type TimeoutNames =
  'lightFlash';

export class TestScene extends BasicScene {
  pointLight: PointLight;
  ambientLight: AmbientLight;
  ambientLightColor: number;
  ambientLightIntensity: number;
  player: Player;
  cellCoordinates: CellCoordinates;
  currentRoom: Room;
  roomSpawner: RoomSpawner;
  timeoutsManager: TimeoutsManager<TimeoutNames>;
  onFinish: Function;
  gasCenter: Vector3;
  gasParticlesPool: EntitiesPool;

  constructor(props: TestSceneProps) {
    super(props);
    const timeoutValues = {
      lightFlash: randomNumbers.getRandomInRange(1, 2),
    };
    this.timeoutsManager = new TimeoutsManager(timeoutValues);

    this.onFinish = props.onFinish;
    this.player = this.entitiesContainer.add(
      new Player({
        camera: this.camera,
        position: new Vector3(0, PLAYER.BODY_HEIGHT, 0),
        container: this.entitiesContainer,
        audioListener: this.audioListener
      })
    ) as Player;
    this.player.setOnHitCallback(() => {
      this.ambientLight.color.setHex(0xFFFFFF);
      this.ambientLight.intensity = 22.3;
      setTimeout(() => {
        this.ambientLight.color.setHex(this.ambientLightColor);
        this.ambientLight.intensity = this.ambientLightIntensity;
      }, 100);
    });
    this.player.setOnDeathCallback(() => {
      this.ambientLight.color.setHex(0xFF0000);
      setTimeout(() => this.finish(), 400);
    });
    this.cellCoordinates = new CellCoordinates({
      size: 3,
    });
    const roomSizeScale = 2;
    this.roomSpawner = new RoomSpawner({
      scene: this,
      player: this.player,
      entitiesContainer: this.entitiesContainer,
      cellCoordinates: this.cellCoordinates,
      roomSize: new Vector2(10 * roomSizeScale, 10 * roomSizeScale),
      doorWidthHalf: 1,
    });
    this.currentRoom = this.roomSpawner.createRoom(new Vector2(0, 0));

    this.camera.rotation.y = 0;

    // lights
    this.ambientLightColor = 0x404040;
    this.ambientLightIntensity = 70;
    this.ambientLight = new AmbientLight(
      this.ambientLightColor,
      this.ambientLightIntensity
    );
    this.scene.add(this.ambientLight);
    const pointLightColor = 0xFFFFFF;
    const pointLightIntensity = 110;
    const pointLightDistance = 130;
    this.pointLight = new PointLight(
      pointLightColor,
      pointLightIntensity,
      pointLightDistance
    );
    this.scene.add(this.pointLight);

    this.scene.fog = new Fog(0x202020, 0.15, 150);

    const loader = new FBXLoader();
    loader.load(HomePakTV, (object) => {
      const tvMain = this.createTV(object, TvText.Question);
      tvMain.position.set(30, 0.8, 44);
      this.scene.add(tvMain);
    });

    loader.load(HomePakTV, (object) => {
      const tvBack = this.createTV(object, TvText.Chat);
      tvBack.position.set(24, 0.8, 45);
      tvBack.rotateY(0.436332);
      this.scene.add(tvBack);
    });

    loader.load(HomePakTV, (object) => {
      const tvBackUp = this.createTV(object.clone(), TvText.Random);
      tvBackUp.position.set(24, 3.07, 45);
      tvBackUp.rotateY(0.436332);
      this.scene.add(tvBackUp);
    });

    this.player.mesh.position.set(31.0, 1.5, 52.0);
    this.camera.rotation.set(0.0, 0.21, 0.0);

    this.roomSpawner.spawnWall(
      new Vector2(30, 45),
      new Vector2(this.roomSpawner.cellCoordinates.size * 15, this.roomSpawner.cellCoordinates.size),
      true
    );

    this.gasCenter = new Vector3(30.55, 1.0, 50.0);
    const gasParticlesCount = 40;
    this.gasParticlesPool = new EntitiesPool(this.createGasParticle, gasParticlesCount);

    playerActions.addActionListener('gasEnable', this.onGasEnable);
    playerActions.addActionListener('gasDisable', this.onGasDisable);
  }

  createTV = (object: Group, text: TvText) => {
    const question = new TextCanvas({
      size: { width: 256 * 2, height: 32 * 5 },
      prefix: '💀',
      textAlign: 'center',
    });
    const scale = 30;
    object.scale.set(scale, scale, scale);

    const screenMaterial: MeshPhongMaterial = (object.children[0].children as any)[0].children[0].material;

    screenMaterial.map = question.texture;

    screenMaterial.map.wrapS = screenMaterial.map.wrapT = RepeatWrapping;
    screenMaterial.map.repeat.x = 9;
    screenMaterial.map.repeat.y = 10;
    screenMaterial.map.offset.set(0.8, 0.5);
    screenMaterial.map.center.set(0.1, 0);
    question.clear();
    switch (text) {
      case TvText.Question:
        const questionText = '💀\nЧем контекст выполнения\nотличается от\nлексического окружения?';
        question.printAll(questionText);
        tvQuestionMaterial = screenMaterial;
        break;
      case TvText.Chat:
        const chatMessage = 'izede:\nЗа этим стоит лаборатория';
        question.printAll(chatMessage);
        tvChatMaterial = screenMaterial;
        chatText = question;
        break;
      case TvText.Random:
        const scoresText = '666\n😍\n777\n😈';
        question.printAll(scoresText);
        tvRandomMaterial = screenMaterial;
        break;
      default:
        break;
    }

    return object;
  };

  getInitialPlayerPositon() {
    const roomCenterCell = this.getCenterPosition(this.currentRoom.cellPosition, this.roomSpawner.roomSize);
    const positionShift = 8;
    return new Vector2(
      roomCenterCell.x * this.cellCoordinates.size,
      (roomCenterCell.y + positionShift) * this.cellCoordinates.size
    );
  }

  getCenterPosition(position: Vector2, size: Vector2) {
    return new Vector2(
      position.x + size.x / 2,
      position.y + size.y / 2
    );
  }

  update(delta: number) {
    super.update(delta);
    this.pointLight.position.copy(this.player.mesh.position);
    if (tvQuestionMaterial && tvQuestionMaterial.map) {
      tvQuestionMaterial.map.offset.y -= delta * 0.2;
    }
    if (tvChatMaterial && tvChatMaterial.map) {
      tvChatMaterial.map.offset.y += delta * 8;
    }
    if (tvRandomMaterial && tvRandomMaterial.map) {
      tvRandomMaterial.map.offset.x += delta * 0.1;
    }
    this.timeoutsManager.updateTimeOut('lightFlash', delta);
    if (this.timeoutsManager.checkIsTimeOutExpired('lightFlash')) {
      this.player.onHit(0);
      if (randomNumbers.getRandom() > 0.5) {
        this.timeoutsManager.initialTimeOuts.lightFlash = randomNumbers.getRandomInRange(1, 30);
      } else {
        this.timeoutsManager.initialTimeOuts.lightFlash = randomNumbers.getRandomInRange(1, 2);
      }
      this.timeoutsManager.updateExpiredTimeOut('lightFlash');
    }
  }

  onGasEnable = () => {
    this.disableEnableGas(true);
  }

  onGasDisable = () => {
    this.disableEnableGas(false);
  }

  disableEnableGas(isEnable: boolean) {
    this.gasParticlesPool.entities.forEach(
      gasParticle => (gasParticle as Gas).disableEnableGas(isEnable)
    );
  }

  createGasParticle = () => {
    const gasPosition = this.gasCenter.clone();
    gasPosition.add(new Vector3(
      randomNumbers.getRandomFloatInRange(-2, 1),
      randomNumbers.getRandomFloatInRange(-0.5, 0.5),
      randomNumbers.getRandomFloatInRange(-1, 0.5),
    ));
    const gas = new Gas({
      position: gasPosition,
      player: this.player,
    });
    gas.disableImmediately();
    return this.entitiesContainer.add(gas);
  };

  finish() {
    playerActions.removeActionListener('gasEnable', this.onGasEnable);
    playerActions.removeActionListener('gasDisable', this.onGasDisable);
    this.onFinish();
  }
}
