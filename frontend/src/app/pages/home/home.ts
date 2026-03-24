import { Component, inject, OnInit } from '@angular/core';
import { HealthService,HealthResponse } from '../../core/health.service';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  
    private healthService=inject(HealthService);
    health:HealthResponse | null =null;
    error='';
  
  ngOnInit(): void {
    this.healthService.getHealth().subscribe({
      next:(res)=>{
        this.health=res;
        console.log("api health ok: ",res);
      },
      error:(err)=>{
        this.error='Backend nije dostupan';
        console.error('api health error :',err);
      }
    })
  }
}
