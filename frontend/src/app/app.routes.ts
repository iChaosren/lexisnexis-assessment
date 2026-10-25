import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'orders' },
  // Put the literal 'new' route before ':id' so it is not interpreted as an ID.
  { path: 'orders/new', loadComponent: () => import('./orders/order-create').then(m => m.OrderCreate) },
  { path: 'orders/:id', loadComponent: () => import('./orders/order-detail').then(m => m.OrderDetail) },
  { path: 'orders', loadComponent: () => import('./orders/order-list').then(m => m.OrderList) },
  { path: '**', redirectTo: 'orders' }
];
