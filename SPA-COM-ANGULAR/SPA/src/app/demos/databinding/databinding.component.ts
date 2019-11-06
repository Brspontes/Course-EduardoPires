import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-databinding',
  templateUrl: './databinding.component.html',
  styles: []
})
export class DatabindingComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  public contadorClique: number = 0;
  public urlImagem: string = "https://angular.io/assets/images/logos/angular/angular.svg";
  public nome: string = "";

  adicionarClique(){
    this.contadorClique += 1;
  }

  zerarContador(){
    this.contadorClique = 0;
  }

 // perdaDeFoco(event: any){
   // this.nome = event.target.value;
  //}
}
