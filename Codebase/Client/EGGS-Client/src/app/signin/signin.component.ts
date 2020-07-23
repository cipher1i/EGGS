import { Component, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective, NgForm, FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpEventType, HttpHeaders } from '@angular/common/http';
//import { ErrorStateMatcher } from '@angular/material/core';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})

export class SigninComponent implements OnInit {

  hide: boolean = true;
  signinFG = new FormGroup({
    emailFC: new FormControl('', [Validators.required, Validators.email]),
    passwordFC: new FormControl('', [Validators.required])
  });

  constructor(private http: HttpClient) { }

  ngOnInit(): void { }

  onSubmit()
  {
    const url :string = 'https://localhost:44331/api/Auth';
    const formData = new FormData();
    formData.append("username", this.signinFG.controls['emailFC'].value);
    formData.append("password", this.signinFG.controls['passwordFC'].value);
    this.http.post(url, formData)
    .subscribe(
      result => { console.log('POST successful value returned in body', result); },
      error => { console.log('POST in error', error); },
      () => { console.log('POST completed'); }
    );
  }
}