export interface OrderLine {
  sku: string;
  quantity: number;
  unitPrice: number;
  preferredWarehouseId?: string;
}

export interface OrderRequest {
  orderId: string;
  lines: OrderLine[];
}

export interface AllocationDetail {
  sku: string;
  allocatedQty: number;
  warehouseId: string;
  unitCost: number;
}

export interface AllocationResult {
  orderId: string;
  cogs: number;
  revenue: number;
  margin: number;
  details: AllocationDetail[];
}


