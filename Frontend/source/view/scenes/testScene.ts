import {
  PointLight,
  Vector2,
  Vector3,
  Fog,
  AmbientLight,
  Audio,
} from 'three';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader';
import { BasicSceneProps, BasicScene } from '@/core/Scene';
import { PLAYER } from '@/constants';
import { Player } from '@/Entities/Player/Player';
import { Gas } from '@/Entities/Gas/Gas';
import { TV } from '@/Entities/TV/TV';
import { Room, RoomSpawner } from './Spawner/RoomSpawner';
import { CellCoordinates } from './CellCoordinates';
import { randomNumbers } from '@/RandomNumbers';
import HomePakTV from '@/assets/HomePakTV.fbx';
import { TimeoutsManager } from '@/TimeoutsManager';
import { EntitiesPool } from './Spawner/EntitiesPool';
import { PlayerAction, PlayerActionListener, PlayerActionName, playerActions } from '@/PlayerActions';
import { Stats } from './Stats';
import { audioStore } from '@/core/loaders';
import { AudioSlices, SliceName } from './AudioSlices';

interface LightEffect {
  colorHex: number;
  intensity: number;
  duration: number;
}

type LightEffectName = 'flick';

export interface TestSceneProps extends BasicSceneProps {
  preload: {
    question?: string;
    likes: number;
    dislikes: number;
  },
  onFinish: Function;
}

type TimeoutNames =
  'lightFlick';

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
  actions: Record<PlayerActionName, PlayerActionListener['listener']>;
  gasEnabled: boolean;
  lightEffects: Record<LightEffectName, LightEffect>;
  buzzSound: Audio;
  gasAudios: Audio[];
  gasAudioIndex: number;
  audioSlices: AudioSlices;
  currentQuestion: string;
  stats: Stats;
  tvMain?: TV;
  tvStatsAnimationInProgress: boolean;
  flickAduioNames: ['lightFlick1', 'lightFlick2', 'lightFlick3', 'lightFlick4',];
  likeAduioNames: ['like1'];
  dislikeAduioNames: ['dislike1', 'dislike2', 'dislike3', 'dislike4', 'dislike5'];

  constructor(props: TestSceneProps) {
    super(props);
    const timeoutValues = {
      lightFlick: randomNumbers.getRandomInRange(1, 2),
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

    this.currentQuestion = props.preload.question || 'Здесь будет вопрос';
    this.stats = new Stats();
    this.stats.setCount('like', props.preload.likes);
    this.stats.setCount('dislike', props.preload.dislikes);

    const loader = new FBXLoader();
    loader.load(HomePakTV, (object) => {
      this.tvMain = this.entitiesContainer.add(new TV({
        model: object,
        position: new Vector3(30, 0.8, 45),
        rotationY: 0,
        screenSpinSpeed: -0.2,
        screenSpinAxis: 'y',
      })) as TV;
      this.scene.add(object);
      this.updateMainTv();
    });

    this.tvStatsAnimationInProgress = false;

    this.player.mesh.position.set(31.05, 0.85, 52.9);
    this.camera.rotation.set(0.21, 0.0, 0.0);

    const wallPos = new Vector2(31, 46);
    const wallSize = new Vector2(this.roomSpawner.cellCoordinates.size * 5, this.roomSpawner.cellCoordinates.size);

    this.roomSpawner.spawnWall(wallPos, wallSize, true);

    const wallUpper = this.roomSpawner.spawnWall(wallPos, wallSize, true);
    wallUpper.actor.mesh.position.setY(4.5);
    wallUpper.actor.mesh.updateMatrix();

    const wallUpper2 = this.roomSpawner.spawnWall(wallPos, wallSize, true);
    wallUpper2.actor.mesh.position.setY(7.5);
    wallUpper2.actor.mesh.updateMatrix();

    this.gasCenter = new Vector3(31.0, -2.0, 51.0);
    const gasParticlesCount = 80;
    this.gasParticlesPool = new EntitiesPool(this.createGasParticle, gasParticlesCount);

    this.buzzSound = new Audio(this.audioListener);
    const buzzSoundBuffer = audioStore.getSound('buzz');
    this.buzzSound.setBuffer(buzzSoundBuffer);
    this.buzzSound.setLoop(true);
    this.buzzSound.setVolume(0.15);

    this.lightEffects = {
      flick: {
        colorHex: 0xFFFFFF,
        intensity: 70.0,
        duration: 100,
      },
    };

    this.gasAudioIndex = 0;
    this.gasAudios = [];
    for (let i = 1; i <= 2; i++) {
      const gasAudio = this.createAudio(`gazBeat${i}`);
      gasAudio.setLoop(true);
      this.gasAudios.push(gasAudio);
    }

    this.audioSlices = new AudioSlices({
      audioListener: this.audioListener,
    });

    this.flickAduioNames = [
      'lightFlick1',
      'lightFlick2',
      'lightFlick3',
      'lightFlick4',
    ];
    this.likeAduioNames = [
      'like1',
    ];
    this.dislikeAduioNames = [
      'dislike1',
      'dislike2',
      'dislike3',
      'dislike4',
      'dislike5',
    ];

    this.gasEnabled = false;
    this.actions = {
      gas: this.onGasSwitch,
      newQuestion: this.onQuestion,
      like: this.onLike,
      dislike: this.onDislike,
      chatMessage: () => {},
    };
    this.addActionListeners();
  }

  getInitialPlayerPositon() {
    const roomCenterCell = this.getCenterPosition(this.currentRoom.cellPosition, this.roomSpawner.roomSize);
    const positionShift = 8;
    return new Vector2(
      roomCenterCell.x * this.cellCoordinates.size,
      (roomCenterCell.y + positionShift) * this.cellCoordinates.size
    );
  }

  resumeSounds() {
    this.buzzSound.play();
    if (
      this.gasEnabled &&
      !this.gasAudios[this.gasAudioIndex].isPlaying
    ) {
      this.gasAudios[this.gasAudioIndex].play();
    }
  }

  stopSounds() {
    this.buzzSound.stop();
    if (
      this.gasEnabled &&
      this.gasAudios[this.gasAudioIndex].isPlaying
    ) {
      this.gasAudios[this.gasAudioIndex].pause();
    }
  }

  setNextGasAudioIndex() {
    this.gasAudioIndex++;
    if (this.gasAudioIndex === this.gasAudios.length) {
      this.gasAudioIndex = 0;
    }
  }

  createAudio(soundName: string) {
    const audio = new Audio(this.audioListener);
    const audioBuffer = audioStore.getSound(soundName);
    audio.setBuffer(audioBuffer);
    audio.setVolume(0.5);
    return audio;
  }

  getCenterPosition(position: Vector2, size: Vector2) {
    return new Vector2(
      position.x + size.x / 2,
      position.y + size.y / 2
    );
  }

  playAudio(audio: Audio) {
    if (audio.isPlaying) {
      audio.stop();
    }
    audio.play();
  }

  createLightEffect(effect: LightEffect) {
    this.ambientLight.color.setHex(effect.colorHex);
    this.ambientLight.intensity = effect.intensity;
    setTimeout(() => {
      this.ambientLight.color.setHex(this.ambientLightColor);
      this.ambientLight.intensity = this.ambientLightIntensity;
    }, effect.duration);
  }

  playRandomAudioSlice(names: SliceName[]) {
    const name = names[randomNumbers.getRandomInRange(0, names.length - 1)];
    this.playAudio(this.audioSlices.getAudio(name));
  }

  lightFlick() {
    this.createLightEffect(this.lightEffects.flick);
    this.playRandomAudioSlice(this.flickAduioNames);
  }

  update(delta: number) {
    super.update(delta);
    this.pointLight.position.copy(this.player.mesh.position);
    this.timeoutsManager.updateTimeOut('lightFlick', delta);
    if (this.timeoutsManager.checkIsTimeOutExpired('lightFlick')) {
      this.lightFlick();
      if (randomNumbers.getRandom() > 0.5) {
        this.timeoutsManager.initialTimeOuts.lightFlick = randomNumbers.getRandomInRange(5, 30);
      } else {
        this.timeoutsManager.initialTimeOuts.lightFlick = randomNumbers.getRandomInRange(5, 10);
      }
      this.timeoutsManager.updateExpiredTimeOut('lightFlick');
    }
  }

  onGasSwitch = () => {
    if (this.gasEnabled) {
      this.gasDisable();
    } else {
      this.gasEnable();
    }
  }

  gasEnable = () => {
    if (this.gasEnabled) {
      return;
    }
    this.setNextGasAudioIndex();
    this.playAudio(this.gasAudios[this.gasAudioIndex]);
    this.disableEnableGas(true);
  }

  gasDisable = () => {
    if (!this.gasEnabled) {
      return;
    }
    this.gasAudios[this.gasAudioIndex].stop();
    this.disableEnableGas(false);
  }

  disableEnableGas(isEnable: boolean) {
    this.gasEnabled = isEnable;
    this.gasParticlesPool.entities.forEach(
      gasParticle => (gasParticle as Gas).disableEnableGas(isEnable)
    );
  }

  onQuestion = (action: PlayerAction) => {
    this.currentQuestion = action.payload;
    this.updateMainTv();
  }

  startTvStatsAnimation(actionSymbol: string) {
    if (this.tvStatsAnimationInProgress) {
      return;
    }

    this.tvStatsAnimationInProgress = true;
    const variant1 = `${actionSymbol} ${actionSymbol}\n`;
    const variant2 = ` ${actionSymbol} \n`;
    let line1 = '';
    let line2 = '';
    for (let i = 5; i--;) {
      line1 += (i % 2 === 0) ? variant1 : variant2;
      line2 += (i % 2 === 0) ? variant2 : variant1;
    }
    this.tvMain?.printText(line1);
    setTimeout(() => {
      this.tvMain?.printText(line2);
    }, 300);
    setTimeout(() => {
      this.updateMainTv();
      this.tvStatsAnimationInProgress = false;
    }, 600);
  }

  checkAdminAction(action: PlayerAction) {
    return action.payload === 'admin';
  }

  onLike = (action: PlayerAction) => {
    if (
      action.payload === 'like1' ||
      action.payload === 'like2'
    ) {
      this.playAudio(this.audioSlices.getAudio(action.payload));
    }
    this.stats.increaseCount('like');
    this.startTvStatsAnimation('👍');
  }

  onDislike = (action: PlayerAction) => {
    if (
      action.payload === 'dislike1' ||
      action.payload === 'dislike2' ||
      action.payload === 'dislike3' ||
      action.payload === 'dislike4' ||
      action.payload === 'dislike5' ||
      action.payload === 'dislike6' ||
      action.payload === 'dislike7' ||
      action.payload === 'dislike8' ||
      action.payload === 'dislike9' ||
      action.payload === 'dislike10' ||
      action.payload === 'dislike11'
    ) {
      this.playAudio(this.audioSlices.getAudio(action.payload));
    }
    this.stats.increaseCount('dislike');
    this.startTvStatsAnimation('👎');
  }

  updateMainTv() {
    this.tvMain?.printText(`${this.stats.toString()}\n${this.currentQuestion}`)
  }

  createGasParticle = () => {
    const gasPosition = this.gasCenter.clone();
    gasPosition.add(new Vector3(
      randomNumbers.getRandomFloatInRange(-1.5, 1),
      randomNumbers.getRandomFloatInRange(-8, 0),
      randomNumbers.getRandomFloatInRange(-1, 0.5),
    ));
    const gas = new Gas({
      position: gasPosition,
      player: this.player,
      maxY: 8,
    });
    return this.entitiesContainer.add(gas);
  };

  finish() {
    this.removeActionListeners();
    this.onFinish();
  }

  addActionListeners() {
    Object.keys(this.actions).forEach(actionName =>
      playerActions.addActionListener(
        actionName as PlayerActionName,
        this.actions[actionName as PlayerActionName]
      )
    );
  }

  removeActionListeners() {
    Object.keys(this.actions).forEach(actionName =>
      playerActions.removeActionListener(
        actionName as PlayerActionName,
        this.actions[actionName as PlayerActionName]
      )
    );
  }
}
