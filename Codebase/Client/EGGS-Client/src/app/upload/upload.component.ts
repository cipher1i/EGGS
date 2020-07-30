import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { Router } from '@angular/router';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})

export class UploadComponent implements OnInit {

  /* DECLARATIONS */
  primaryText: string;
  icon : string;
  btnColor : string;
  btnDescription : string;
  labelTxt : string;
  labelFor : string;
  files : FileList;
  receivedFiles : boolean;
  uploadProg : number;
  downloaded : boolean;
  decrypted : boolean;
  userKey : any;
  copied : boolean;

  /* INIT */
  ////////////////////////////////////////////////////////////////////
  constructor(private http : HttpClient, private router : Router) { }

  ngOnInit() : void {
    if(sessionStorage.getItem('user'))
      this.router.navigate(['dash']);

    this.reset();
  }
  ////////////////////////////////////////////////////////////////////

  /* METHOD DEF */
  reset() : void
  {
    this.primaryText = "EGGS stands for EGG Skrambler and we scramble your source files.  Our service guarantees the security of your codebase with our state of the art encryption software.";
    this.setButton();

    this.receivedFiles = false;
    this.uploadProg = 0;
    this.downloaded = false;
    this.decrypted = false;
  }

  setButton(type : string = "", item : string = "") : void
  {
    switch(type.toLowerCase())
    {
      case "skramble":
        this.icon = "source";
        this.labelFor = "upload";
        this.labelTxt = "Skramble";
        this.btnDescription = item;
        this.btnColor = "accent";
        return;
      case "download":
        this.icon = "cloud_download";
        this.labelFor = "download";
        this.labelTxt = "Download";
        this.btnDescription = "";
        this.btnColor = "";
        return;
      case "copy":
        this.icon = "vpn_key";
        this.labelFor = "clipboard";
        this.labelTxt = item;
        this.btnDescription = "";
        this.btnColor = "";
        return;
      default:
        this.icon = "folder";
        this.labelFor = "picker";
        this.labelTxt = "Upload My Code";
        this.btnDescription = "";
        this.btnColor = "primary";
        return;
    }
  }

  checkUserCredentials() : boolean
  {
    if(!sessionStorage.getItem('user'))
    {
      this.router.navigate(['signin']);
      return false;
    }
    return true;
  }

  getFolder(directory : any) : void
  {
    this.files = directory.files;
    if(this.files.length > 0)
    {
      this.receivedFiles = true;
      this.setButton('skramble', directory.files[0].webkitRelativePath.split('/')[0]);
    }

    this.copied = false;
  }

  public upload() : any
  {
    if(this.files.length === 0) 
      return;

    const formData : FormData = new FormData();
    Array.from(this.files).map((file, index) =>{
      return formData.append('file'+index, file, file.name);
    });
    
    this.http.post('https://localhost:44331/api/EGGS/upload', formData, {reportProgress: true, observe: 'events', responseType: 'json'})
    .subscribe(event =>{
      if(event.type === HttpEventType.UploadProgress)
      {
        this.uploadProg = Math.round((event.loaded / event.total)*this.files.length);
        if(this.uploadProg == this.files.length)
        {
          this.receivedFiles = false;
          this.setButton("download");
        }
      }
      if(event.type === HttpEventType.Response)
      {
        this.userKey = event.body;
        const parsedKey = this.userKey.toString().split('-');
        if(parsedKey.length > 1)
        {
          this.userKey = parsedKey[0];
          this.decrypted = true;
        }
      }
    });
  }

  public download() : void
  {
    this.http.get('https://localhost:44331/api/EGGS/download', { params: { userKey: this.userKey }, responseType: 'blob' })
    .subscribe(success => {
      var blob : Blob = new Blob([success], {type: 'application/zip'});
      this.http.delete('https://localhost:44331/api/EGGS', {params: { userKey: this.userKey } })
      .subscribe(()=>{
        this.downloaded = true;
        if(!this.decrypted)
        {
          this.setButton("copy", this.userKey);
          this.primaryText = "This is your key. Save this to your clipboard and use it to decrypt your code.";
        }
        else
        {
          this.reset();
          (<HTMLInputElement>document.getElementById('picker')).value = '';
        }
      });
      saveAs(blob,'EGGS');
    });
  }

  public copy() : string
  {
    return this.labelTxt;
  }

  public showTooltip()
  {
    this.reset();
    (<HTMLInputElement>document.getElementById('picker')).value = '';

    this.copied = true;
    return this.copied;
  }
}