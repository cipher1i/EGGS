import { Component, OnInit, HostListener } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpEventType, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { saveAs } from 'file-saver';
import { of } from 'rxjs';
import { timeout, catchError } from 'rxjs/operators';

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
  fileEmpty : boolean;
  files : FileList;
  filetype : boolean;
  receivedFiles : boolean;
  uploadProg : number;
  downloaded : boolean;
  decrypted : boolean;
  userKey : any;
  zipID : string;
  copied : boolean;
  goToDecrypt : boolean;
  keyFC : FormControl = new FormControl('', [Validators.required]);

  /* INIT */
  ////////////////////////////////////////////////////////////////////
  constructor(private http : HttpClient, private router : Router) { }

  ngOnInit() : void {
    if(sessionStorage.getItem('user'))
      this.router.navigate(['dash']);

    this.reset();
    this.filetype = false;
  }
  ////////////////////////////////////////////////////////////////////

  /* METHOD DEF */
  ////////////////////////////////////////////////////////////////////
  //use ngOnDestroy instead of doThis
  @HostListener('window:beforeunload', ['$event'])
  onDestroy($event : any) : void
  {
    if(this.labelFor == "download" && this.zipID) 
    {
      this.deleteResources('https://localhost:44331/api/EGGS', this.zipID);
      $event.returnValue = 'Your data will be lost!';
    }

    return;
  }

  deleteResources(url : string, zipID : string)
  {
    this.http.delete(url, { params: { userKey: zipID } })
      .subscribe(() => 
      {
        sessionStorage.removeItem('user');
      });
  }

  reset() : void
  {
    this.primaryText = "EGGS stands for EGG Skrambler and we scramble your source files.  Our service guarantees the security of your codebase with our state of the art encryption software.";
    this.setButton();

    this.receivedFiles = false;
    this.uploadProg = 0;
    this.downloaded = false;
    this.decrypted = false;
    this.goToDecrypt = false;
    this.zipID = "";
    (<HTMLInputElement>document.getElementById('picker')).value = '';
  }

  setButton(type : string = "", item : string = "") : void
  {
    switch(type.toLowerCase())
    {
      case "skramble":
        this.icon = "source";
        this.labelFor = "encrypt";
        this.labelTxt = "Skramble";
        this.btnDescription = item;
        this.btnColor = "accent";
        return;
      case "deskramble":
        this.icon = "source";
        this.labelFor = "encrypt";
        this.labelTxt = "Deskramble";
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
    this.fileEmpty = false;

    this.files = directory.files;
    if(this.files.length < 1)
      return;

    this.receivedFiles = true;

    Array.from(this.files).forEach(f => 
    {
      // use this to catch file types that have failed the support test
      // reduces processing time and memory in resources folder
      switch((<any>f).webkitRelativePath.split('.')[(<any>f).webkitRelativePath.split('.').length - 1].toLowerCase())
      {
        case 'pdf':
        case 'jpg':
        case 'png':
        case 'exe':
        case 'dmg':
        case 'msi':
        case 'map':
        case 'config':
        case '__ivy_ngcc_bak':
          this.filetype = true;
          this.reset();
          return;
        default:
          this.filetype = false;
          break;
      }

      const reader = new FileReader();
      reader.onload = () => 
      {
        if(reader.result == null || reader.result == "")
        {
          //file empty
          this.fileEmpty = true;
          this.reset();
          return;
        }

        var firstline = (<string>reader.result).split('\n').shift();
        if(firstline == "Skrambled EGG")
          this.goToDecrypt = true;
        else
        {
          this.goToDecrypt = false;
          this.setButton('skramble', directory.files[0].webkitRelativePath.split('/')[0]);
          return;
        }
      }

      reader.readAsText(f);
    });

    if(this.labelTxt != 'Skramble' && !this.filetype && (<HTMLInputElement>document.getElementById('picker')).value != '')
      this.setButton('deskramble', directory.files[0].webkitRelativePath.split('/')[0]);

    this.copied = false;
  }

  public upload() : any
  {
    if(this.files.length === 0 || (this.goToDecrypt && this.keyFC.invalid)) 
    {
      this.keyFC.markAsTouched();
      return;
    }

    const formData : FormData = new FormData();
    Array.from(this.files).map((file, index) =>{
      return formData.append('file'+index, file, file.name);
    });

    if(this.goToDecrypt)
      this.postTranslate(formData);
    else
      this.postUpload(formData);
  }

  postTranslate(formData : FormData)
  {
    this.http.post('https://localhost:44331/api/EGGS/translate', formData, {params: { username: sessionStorage.getItem('user'), privateKey: this.keyFC.value }, reportProgress: true, observe: 'events', responseType: 'json'})
      .pipe(
        timeout(20000),
        catchError(e => {
          console.log('upload failure');
          this.filetype = true;
          this.reset();
          return of(e);
        })
      )
      .subscribe(event =>
      {
        if(event.type === HttpEventType.UploadProgress)
        {
          this.uploadProg = Math.round((event.loaded / event.total)*this.files.length);
          if(this.uploadProg == this.files.length)
          {
            this.receivedFiles = false;
            this.goToDecrypt = false;
          }
        }
        if(event.type === HttpEventType.Response)
        {
          this.userKey = event.body;
          const parsedKey = this.userKey.toString().split('+');
          if(parsedKey.length > 1)
          {
            this.userKey = parsedKey[0];
            this.zipID = parsedKey[1];
            const rawKey = this.userKey.split('-');
            if(rawKey.length > 1)
            {
              this.userKey = rawKey[0];
              this.decrypted = true;
            }
          }

          this.filetype = false;
          this.setButton("download");
        }
        //if stat 400, file empty
      });
  }

  postUpload(formData : FormData)
  {
    this.http.post('https://localhost:44331/api/EGGS/upload', formData, {params: { username: sessionStorage.getItem('user') }, reportProgress: true, observe: 'events', responseType: 'json'})
      .pipe(
        timeout(20000),
        catchError(e => {
          console.log('upload failure');
          this.filetype = true;
          this.reset();
          return of(e);
        })
      )
      .subscribe(event =>{
        if(event.type === HttpEventType.UploadProgress)
        {
          this.uploadProg = Math.round((event.loaded / event.total)*this.files.length);
          if(this.uploadProg == this.files.length)
          {
            this.receivedFiles = false;
            this.goToDecrypt = false;
          }
        }
        if(event.type === HttpEventType.Response)
        {
          this.userKey = event.body;
          const parsedKey = this.userKey.toString().split('+');
          if(parsedKey.length > 1)
          {
            this.userKey = parsedKey[0];
            this.zipID = parsedKey[1];
            const rawKey = this.userKey.split('-');
            if(rawKey.length > 1)
            {
              this.userKey = rawKey[0];
              this.decrypted = true;
            }
          }
          this.filetype = false;
          this.setButton("download");
        }
      });
  }

  public download() : void
  {
    this.http.get('https://localhost:44331/api/EGGS/download', { params: { userKey: this.zipID }, responseType: 'blob' })
    .subscribe(success => {
      var blob : Blob = new Blob([success], {type: 'application/zip'});
      this.http.delete('https://localhost:44331/api/EGGS', {params: { userKey: this.zipID } })
      .subscribe(()=>{
        this.downloaded = true;
        if(!this.decrypted)
        {
          this.setButton("copy", this.userKey);
          this.primaryText = "This is your key. Save it to your clipboard and use it to deskramble your code.";
        }
        else
        {
          this.reset();
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

    this.copied = true;
    return this.copied;
  }
}