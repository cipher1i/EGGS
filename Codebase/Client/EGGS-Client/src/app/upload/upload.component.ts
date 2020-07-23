import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { Router } from '@angular/router';
import { UploadService } from '../SERVICES/upload/upload.service';
import { FileDetector } from 'protractor';
import { ExecFileOptions } from 'child_process';
import { GeneratedFile, StylesCompileDependency } from '@angular/compiler';
import { type } from 'os';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})

export class UploadComponent implements OnInit {

  btnColor : string = "primary";
  icon: string = "folder";
  labelTxt : string = "Upload My Code";
  labelFor : string = "picker";
  files : FileList;
  gotFiles : boolean = false;
  public uploadProg: number;
  public message: string;
  
  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {}

  checkUserCredentials()
  {
    if(sessionStorage.getItem("User") == null)
    {
      this.router.navigate(['signin']);
      return false;
    }
    return true;
  }

  getFolder(directory : any)
  {
    this.files = directory.files;
    this.labelTxt = this.files.length > 0 ? directory.files[0].webkitRelativePath.split('/')[0] : "Upload My Code";
    
    if(this.files.length > 0)
    {
      this.sleep(2000).then(()=>{
        this.gotFiles = true;
        this.labelTxt = "Skramble";
        this.btnColor = "accent";
        this.labelFor = "upload";
      });
    }
  }

  sleep(ms : number) 
  {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  @Output() public onUploadFinished = new EventEmitter();
  public upload(){

    if(this.files.length === 0) 
      return;

    const formData = new FormData();
    
    Array.from(this.files).map((file, index) =>{
      return formData.append('file'+index, file, file.name);
    });
    
    this.http.post('https://localhost:44336/api/EGGS', formData, {reportProgress: true, observe: 'events'}).subscribe(event =>{
      if(event.type === HttpEventType.UploadProgress)
        this.uploadProg = Math.round((event.loaded / event.total)*this.files.length);
      else if(this.uploadProg == this.files.length)
      {
        this.gotFiles = false;
        this.btnColor = "";
        this.icon = "cloud_download";
        this.labelFor = "download";
        this.labelTxt = "Download";
        this.onUploadFinished.emit();
      }
    });
  }

  public download()
  {
    alert('downloading');
  }
  
}