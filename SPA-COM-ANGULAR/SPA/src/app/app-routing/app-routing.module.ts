import { DatabindingComponent } from './../demos/databinding/databinding.component';
import { SobreComponent } from './../institucional/sobre/sobre.component';
import { ContatoComponent } from './../institucional/contato/contato.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './../navegacao/home/home.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' }, //CASO SEJA VAZIO
  { path: 'home', component: HomeComponent },
  { path: 'contato', component: ContatoComponent },
  { path: 'sobre', component: SobreComponent},
  { path: 'data-binding', component: DatabindingComponent },
  { path: '**', component: HomeComponent }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})

export class AppRoutingModule { }
