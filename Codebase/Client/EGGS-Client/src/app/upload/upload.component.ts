import { Component, OnInit, HostListener } from '@angular/core';
import { FormGroup, FormGroupDirective, NgForm, FormControl, Validators } from '@angular/forms';
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
  keyFC: FormControl = new FormControl('', [Validators.required, Validators.minLength(10)]);

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
  //use ngOnDestroy instead of doThis
  @HostListener('window:beforeunload', ['$event'])
  doThis($event)
  {
    if(this.labelFor == "download" && this.zipID) 
    {
      this.http.delete('https://localhost:44331/api/EGGS', {params: { userKey: this.zipID } })
      .subscribe(()=>{
        sessionStorage.removeItem('user');
      });

      $event.returnValue='Your data will be lost!';
    }
  }

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
        this.labelFor = "encrypt";
        this.labelTxt = "Skramble";
        this.btnDescription = item;
        this.btnColor = "accent";
        return;
      case "deskarmble":
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

  goToDecrypt : boolean = false;
  getFolder(directory : any) : void
  {
    this.files = directory.files;
    if(this.files.length < 1)
      return;

    this.receivedFiles = true;
    
    Array.from(this.files).forEach(f => {
      const reader = new FileReader();

      reader.onload = () => {
        var firstline = (<string>reader.result).split('\n').shift();
        if(firstline == "Skrambled EGG")
        {
          this.goToDecrypt = true;
          //ask for key part1 before decrypt
          //post full file with key and user to decrypt
          //get user from db
          //validate part 1 of user key against posted key
          //validate part 2 of user key against key in file after 'Skrambled EGG'
          //validate for all files
          //decrypt and return files if valid
          //otherwise return failure response
        }
        else
        {
          this.goToDecrypt = false;
          return;
        }
      }
      reader.readAsText(f);
    });
    
    if(this.goToDecrypt)
      this.setButton('decrypt', directory.files[0].webkitRelativePath.split('/')[0]);
    else
      this.setButton('skramble', directory.files[0].webkitRelativePath.split('/')[0]);

    this.copied = false;
  }

  zipID : string;
  public upload() : any
  {
    if(this.files.length === 0 || (this.goToDecrypt && this.keyFC.invalid)) 
      return;

    const formData : FormData = new FormData();
    Array.from(this.files).map((file, index) =>{
      return formData.append('file'+index, file, file.name);
    });
    
    if(this.goToDecrypt)
    {
      this.http.post('https://localhost:44331/api/EGGS/translate', formData, {params: { username: sessionStorage.getItem('user'), privateKey: this.keyFC.value }, reportProgress: true, observe: 'events', responseType: 'json'})
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
        }
      });
    }
    else
    {
      this.http.post('https://localhost:44331/api/EGGS/upload', formData, {params: { username: sessionStorage.getItem('user') }, reportProgress: true, observe: 'events', responseType: 'json'})
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
        }
      });
    }
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