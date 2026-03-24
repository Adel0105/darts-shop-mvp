import {  Injectable,inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";

export interface HealthResponse{
    status:string;
    timestamp:string;
    app:string;
} 

@Injectable({providedIn:'root'})
export class HealthService{
    private http =inject(HttpClient);
    private apiUrl='http://localhost:5231/api';

    getHealth(){
        return this.http.get<HealthResponse>(`${this.apiUrl}/health`);
    
    }
}