import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { UploadService } from '../services/upload-service/upload.service';
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

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  dir : FileList;
  labeltxt : string = "Upload My Code";
  matcolor : string = "primary";
  labelfor : string = "picker";
  status : boolean = false;
  GetData(directory : any)
  {
    this.dir = directory.files;
    this.labeltxt = this.dir.length > 0 ? directory.files[0].webkitRelativePath.split('/')[0] : "Upload My Code";
    if(this.dir.length > 0)
    {
      this.sleep(2000).then(()=>{
        this.status = true;
        this.labeltxt = "Skramble";
        this.matcolor = "accent";
        this.labelfor = "submit";
      });
    }
  }

  sleep(ms : number) 
  {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  public progress: number;
  public message: string;
  //@Output() public onUploadFinished = new EventEmitter();
  public Submit(){

    if(this.dir.length === 0) {
      return;
    }

    const formData = new FormData();
    
    Array.from(this.dir).map((file, index) =>{
      return formData.append('file'+index, file, file.name);
    });
    
    this.http.post('https://localhost:44336/api/EGGS', formData, {reportProgress: true, observe: 'events'}).subscribe(event =>{
      if(event.type === HttpEventType.UploadProgress)
      {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if(event.type === HttpEventType.Response)
      {
        this.message = 'Upload success.';
        //this.onUploadFinished.emit(event.body);
      }
    });
  }
}
