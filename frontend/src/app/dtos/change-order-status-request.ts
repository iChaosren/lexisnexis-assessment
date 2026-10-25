import {OrderStatus} from './order-status';

export interface ChangeOrderStatusRequest {
  orderId: string;
  newStatus: OrderStatus;
}
