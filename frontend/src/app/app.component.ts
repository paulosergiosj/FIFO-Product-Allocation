import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api.service';
import { InventoryReceipt, InventoryReceiptRequest } from '../models/inventory-receipt.model';
import { OrderRequest, OrderLine, AllocationResult } from '../models/order.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  receipts: InventoryReceipt[] = [];
  groupedReceipts: { [sku: string]: InventoryReceipt[] } = {};
  
  receiptForm: InventoryReceiptRequest = {
    sku: '',
    quantityReceived: 0,
    unitCost: 0,
    receivedAtUtc: '',
    warehouseId: ''
  };

  orderForm = {
    orderId: '',
    lines: [{ sku: '', quantity: 0, unitPrice: 0, preferredWarehouseId: '' }] as OrderLine[]
  };

  orderResult: AllocationResult | null = null;
  showModal = false;
  errorMessage = '';

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadInventory();
  }

  loadInventory(): void {
    this.apiService.getReceipts().subscribe({
      next: (receipts) => {
        this.receipts = receipts;
        this.groupReceiptsBySku();
      },
      error: (error) => {
        console.error('Error loading inventory:', error);
      }
    });
  }

  groupReceiptsBySku(): void {
    this.groupedReceipts = {};
    this.receipts.forEach(receipt => {
      if (!this.groupedReceipts[receipt.sku]) {
        this.groupedReceipts[receipt.sku] = [];
      }
      this.groupedReceipts[receipt.sku].push(receipt);
    });
  }

  getSortedSkus(): string[] {
    return Object.keys(this.groupedReceipts).sort();
  }

  onReceiptSubmit(form: any): void {
    if (form.invalid) {
      Object.keys(form.controls).forEach(key => {
        form.controls[key].markAsTouched();
      });
      return;
    }

    this.errorMessage = '';
    const receipt: InventoryReceiptRequest = {
      sku: this.receiptForm.sku,
      quantityReceived: this.receiptForm.quantityReceived,
      unitCost: this.receiptForm.unitCost,
      receivedAtUtc: this.convertToUtc(this.receiptForm.receivedAtUtc),
      warehouseId: this.receiptForm.warehouseId
    };

    this.apiService.createReceipt(receipt).subscribe({
      next: () => {
        this.loadInventory();
        this.receiptForm = {
          sku: '',
          quantityReceived: 0,
          unitCost: 0,
          receivedAtUtc: '',
          warehouseId: ''
        };
        form.resetForm();
      },
      error: (error) => {
        this.errorMessage = this.formatError(error);
      }
    });
  }

  onOrderSubmit(form: any): void {
    if (form.invalid) {
      Object.keys(form.controls).forEach(key => {
        form.controls[key].markAsTouched();
      });
      return;
    }

    this.errorMessage = '';
    const order: OrderRequest = {
      orderId: this.orderForm.orderId,
      lines: this.orderForm.lines.map(line => ({
        sku: line.sku,
        quantity: line.quantity,
        unitPrice: line.unitPrice,
        ...(line.preferredWarehouseId && { preferredWarehouseId: line.preferredWarehouseId })
      }))
    };

    this.apiService.allocateOrder(order).subscribe({
      next: (result) => {
        this.orderResult = result;
        this.showModal = true;
        this.loadInventory();
        this.orderForm = {
          orderId: '',
          lines: [{ sku: '', quantity: 0, unitPrice: 0, preferredWarehouseId: '' }]
        };
        form.resetForm();
      },
      error: (error) => {
        this.errorMessage = this.formatError(error);
      }
    });
  }

  addOrderLine(): void {
    this.orderForm.lines.push({ sku: '', quantity: 0, unitPrice: 0, preferredWarehouseId: '' });
  }

  removeOrderLine(index: number): void {
    if (this.orderForm.lines.length > 1) {
      this.orderForm.lines.splice(index, 1);
    }
  }

  clearAll(): void {
    if (confirm('Are you sure you want to clear all data?')) {
      this.apiService.clearAll().subscribe({
        next: () => {
          this.receipts = [];
          this.groupedReceipts = {};
          this.errorMessage = '';
          alert('All data cleared successfully');
        },
        error: (error) => {
          this.errorMessage = 'Failed to clear data';
        }
      });
    }
  }

  closeModal(): void {
    this.showModal = false;
    this.orderResult = null;
  }

  formatDateInput(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    
    if (value.length > 8) {
      value = value.substring(0, 8);
    }
    
    let formatted = value;
    if (value.length >= 3) {
      formatted = value.substring(0, 2) + '/' + value.substring(2);
    }
    if (value.length >= 5) {
      formatted = value.substring(0, 2) + '/' + value.substring(2, 4) + '/' + value.substring(4);
    }
    
    this.receiptForm.receivedAtUtc = formatted;
    event.target.value = formatted;
  }

  validateDate(dateString: string): void {
    if (!dateString) return;
    
    const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    const match = dateString.match(dateRegex);
    
    if (!match) {
      if (dateString.length === 10) {
        this.receiptForm.receivedAtUtc = '';
      }
      return;
    }
    
    const month = parseInt(match[1], 10);
    const day = parseInt(match[2], 10);
    const year = parseInt(match[3], 10);
    
    if (month < 1 || month > 12) {
      this.receiptForm.receivedAtUtc = '';
      return;
    }
    
    const daysInMonth = new Date(year, month, 0).getDate();
    if (day < 1 || day > daysInMonth) {
      this.receiptForm.receivedAtUtc = '';
      return;
    }
    
    if (year < 1900 || year > 2100) {
      this.receiptForm.receivedAtUtc = '';
      return;
    }
  }
  convertToUtc(localDate: string): string {
    const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    const match = localDate.match(dateRegex);
    
    if (match) {
      const month = parseInt(match[1], 10) - 1;
      const day = parseInt(match[2], 10);
      const year = parseInt(match[3], 10);
      const date = new Date(Date.UTC(year, month, day, 0, 0, 0));
      return date.toISOString();
    }
    
    const date = new Date(localDate + 'T00:00:00');
    return date.toISOString();
  }

  formatDateUS(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return '';
    const month = String(date.getUTCMonth() + 1).padStart(2, '0');
    const day = String(date.getUTCDate()).padStart(2, '0');
    const year = date.getUTCFullYear();
    return `${month}/${day}/${year}`;
  }

  formatCurrency(value: number | undefined | null): string {
    if (value === undefined || value === null || isNaN(value)) {
      return '0.00';
    }
    return value.toFixed(2);
  }

  formatError(error: any): string {
    if (error?.error?.errors) {
      const errorMessages = Object.values(error.error.errors).flat() as string[];
      return errorMessages.join(', ');
    }
    return error?.error?.title || error?.message || 'An error occurred';
  }

  limitNumberLength(event: any, maxLength: number): void {
    const input = event.target;
    let value = input.value;
    
    if (value === '' || value === null || value === undefined) {
      return;
    }
    
    let valueStr = value.toString().replace(/[^\d.-]/g, '');
    
    if (valueStr.length > maxLength) {
      valueStr = valueStr.substring(0, maxLength);
      input.value = valueStr;
      
      const numValue = parseFloat(valueStr) || 0;
      
      if (input.id === 'quantityReceived') {
        this.receiptForm.quantityReceived = numValue;
      } else if (input.id === 'unitCost') {
        this.receiptForm.unitCost = numValue;
      } else if (input.name && input.name.startsWith('quantity')) {
        const match = input.name.match(/quantity(\d+)/);
        if (match) {
          const index = parseInt(match[1]);
          if (this.orderForm.lines[index]) {
            this.orderForm.lines[index].quantity = numValue;
          }
        }
      } else if (input.name && input.name.startsWith('unitPrice')) {
        const match = input.name.match(/unitPrice(\d+)/);
        if (match) {
          const index = parseInt(match[1]);
          if (this.orderForm.lines[index]) {
            this.orderForm.lines[index].unitPrice = numValue;
          }
        }
      }
    }
  }

  isControlInvalid(form: any, controlName: string): boolean {
    const control = form.controls[controlName];
    return control && control.invalid && (control.dirty || control.touched);
  }
}

