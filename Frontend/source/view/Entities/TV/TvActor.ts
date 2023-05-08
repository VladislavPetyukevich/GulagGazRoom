import { Mesh, Vector3, Group, MeshPhongMaterial, RepeatWrapping } from 'three';
import { Actor } from '@/core/Entities/Actor';
import { TextCanvas } from '@/TextCanvas';

interface TvActorProps {
  position: Vector3;
  rotationY: number;
  model: Group;
}

export class TvActor implements Actor {
  mesh: Mesh;
  model: Group;
  textCanvas: TextCanvas;
  screenMaterial: MeshPhongMaterial;

  constructor(props: TvActorProps) {
    this.mesh = new Mesh();
    this.model = props.model;
    this.model.position.copy(props.position);
    this.model.rotation.y = props.rotationY;
    this.textCanvas = new TextCanvas({
      size: { width: 256 * 2, height: 32 * 5 },
      textAlign: 'center',
    });
    const scale = 30;
    this.model.scale.set(scale, scale, scale);
    this.screenMaterial = (this.model.children[0].children as any)[0].children[0].material;

    this.screenMaterial.map = this.textCanvas.texture;

    this.screenMaterial.map.wrapS = this.screenMaterial.map.wrapT = RepeatWrapping;
    this.screenMaterial.map.repeat.x = 9;
    this.screenMaterial.map.repeat.y = 10;
    this.screenMaterial.map.offset.set(0.8, 0.3);
    this.screenMaterial.map.center.set(0.1, 0);
  }

  printText(text: string) {
    this.textCanvas.clear();
    this.textCanvas.printAll(text);
  }

  update() {
  }
}
