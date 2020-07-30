import { Component, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective, NgForm, FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
//import { ErrorStateMatcher } from '@angular/material/core';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})

export class SigninComponent implements OnInit {

  hide : boolean = true;
  session : boolean = false;
  header : string = "Psst! My developers told me not to speak with strangers";
  message : string = "Introduce yourself";
  error : string = "";
  signinFG : FormGroup = new FormGroup({
    emailFC: new FormControl('', [Validators.required, Validators.email]),
    passwordFC: new FormControl('', [Validators.required])
  });

  constructor(private http : HttpClient, private router : Router) { }

  ngOnInit() : void { }

  onSubmit() : void
  {
    const url : string = 'https://localhost:44331/api/Auth';
    const formData : FormData = new FormData();
    formData.append("username", this.signinFG.controls['emailFC'].value);
    formData.append("password", this.signinFG.controls['passwordFC'].value);
    this.http.post(url, formData)
    .subscribe(
      result => { 
        this.error = "";
        sessionStorage.setItem('user', JSON.stringify(result)); 
        console.log('POST successful value returned in body', result); 
        this.session = true; 
        setTimeout(()=>{this.router.navigate(['dash']);}, 3000);
      },
      error => { console.log('POST in error', error); this.error = "OOP something's wrong!" },
      () => { console.log('POST completed'); }
    );
  }
}