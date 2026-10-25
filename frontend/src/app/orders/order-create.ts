import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ApiService} from '../api.service';
import {CreateOrderRequest} from '../dtos/create-order-request';
import {CreateOrderLineRequest} from '../dtos/create-order-line-request';
import {Currency} from '../dtos/currency';
import { ValidatorFn } from '@angular/forms';

function oneOf(allowed: readonly unknown[]): ValidatorFn {
  return control =>
    allowed.includes(control.value) ? null : { oneOf: true };
}

// Assessment excerpt: “Provides a simple way to submit a new order.”
@Component({
  selector: 'order-create',
  templateUrl: './order-create.html',
  imports: [ReactiveFormsModule]
})
export class OrderCreate {
  private readonly formBuilder = inject(FormBuilder);
  private readonly api = inject(ApiService);

  form = this.formBuilder.nonNullable.group({
    externalReference: ['', Validators.required],
    currency: [Currency.ZAR, [Validators.required, oneOf(Object.values(Currency))]],
    lines: this.formBuilder.nonNullable.array([
      this.formBuilder.nonNullable.group({
        sku: ['', Validators.required],
        name: ['', Validators.required],
        quantity: [1, [Validators.required, Validators.min(1)]],
        unitPrice: [0, [Validators.required, Validators.min(0)]]
      })
    ])
  });

  submit() {
    if (this.form.valid)
      this.api.submitOrder(this.form.value as CreateOrderRequest);
  }
}
