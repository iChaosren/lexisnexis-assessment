import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ApiService} from '../api.service';
import {CreateOrderRequest} from '../dtos/create-order-request';
import {Currency} from '../dtos/currency';
import {CustomerResponse} from '../dtos/customer-response';
import {oneOf} from '../one-of.validator';

// Assessment excerpt: “Provides a simple way to submit a new order.”
@Component({
  selector: 'order-create',
  templateUrl: './order-create.html',
  imports: [ReactiveFormsModule]
})
export class OrderCreate implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly api = inject(ApiService);

  customers = signal<CustomerResponse[]>([]);
  submitting = signal(false);
  submissionMessage = signal('');

  form = this.formBuilder.nonNullable.group({
    externalReference: ['', Validators.required],
    customerId: ['', Validators.required],
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

  ngOnInit(): void {
    this.api.listCustomers().subscribe({
      next: customers => this.customers.set(customers),
      error: () => this.submissionMessage.set('Could not load customers.')
    });
  }

  submit() {
    if (this.submitting())
      return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.submissionMessage.set('Complete the required fields with valid values.');
      return;
    }

    const request: CreateOrderRequest = this.form.getRawValue();
    this.submitting.set(true);
    this.submissionMessage.set('');
    this.api.submitOrder(request).subscribe({
      next: created => {
        this.submitting.set(false);
        this.submissionMessage.set(created
          ? 'Order submitted.'
          : 'An order with this reference already exists.');
      },
      error: () => {
        this.submitting.set(false);
        this.submissionMessage.set('Could not submit the order.');
      }
    });
  }
}
