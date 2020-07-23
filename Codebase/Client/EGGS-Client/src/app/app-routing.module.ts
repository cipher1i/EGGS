import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UploadComponent } from './upload/upload.component';
import { SigninComponent } from './signin/signin.component';

const routes: Routes = [
  //{path: '', component: UploadComponent },
  {path: 'signin', component: SigninComponent }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
