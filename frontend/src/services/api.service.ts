import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { InventoryReceipt, InventoryReceiptRequest } from '../models/inventory-receipt.model';
import { OrderRequest, AllocationResult } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.API_BASE_URL;

  constructor(private http: HttpClient) {}

  getReceipts(): Observable<InventoryReceipt[]> {
    return this.http.get<InventoryReceipt[]>(`${this.apiUrl}/receipts`);
  }

  createReceipt(receipt: InventoryReceiptRequest): Observable<InventoryReceipt> {
    return this.http.post<InventoryReceipt>(`${this.apiUrl}/receipts`, receipt);
  }

  allocateOrder(order: OrderRequest): Observable<AllocationResult> {
    return this.http.post<AllocationResult>(`${this.apiUrl}/orders`, order);
  }

  clearAll(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/clear`, {});
  }
}


