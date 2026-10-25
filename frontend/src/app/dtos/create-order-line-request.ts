export interface CreateOrderLineRequest {
  sku: string;
  name: string;
  quantity: number;
  unitPrice: number;
}
