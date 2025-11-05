export interface InventoryReceipt {
  sku: string;
  quantityReceived: number;
  quantityUsed: number;
  unitCost: number;
  receivedAtUtc: string;
  warehouseId: string;
  quantityAvailable?: number;
}

export interface InventoryReceiptRequest {
  sku: string;
  quantityReceived: number;
  unitCost: number;
  receivedAtUtc: string;
  warehouseId: string;
}


