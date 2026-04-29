
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../core/services/product.service';

/**
 * Reactive form for adding a new product to the catalog.
 * Emits a `productAdded` event on successful creation so the parent
 * can refresh the product list without a full page reload.
 */
@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent {
  /** Emitted after a product is successfully created. */
  @Output() readonly productAdded = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);

  /** Whether a POST request is in flight. */
  readonly isSubmitting = signal(false);

  /** Success message shown after a product is created. */
  readonly successMessage = signal<string | null>(null);

  /** Server-side or network error message. */
  readonly errorMessage = signal<string | null>(null);

  /** The reactive product form group. */
  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    price: [null as number | null, [Validators.required, Validators.min(0.01)]]
  });

  /** Returns the `code` form control for template access. */
  get codeControl() { return this.form.controls.code; }

  /** Returns the `name` form control for template access. */
  get nameControl() { return this.form.controls.name; }

  /** Returns the `price` form control for template access. */
  get priceControl() { return this.form.controls.price; }

  /** Submits the form if valid and calls the product API. */
  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const { code, name, price } = this.form.value;

    this.productService.createProduct({ code: code!, name: name!, price: price! }).subscribe({
      next: (product) => {
        console.log('Product created successfully:', product.id);
        this.successMessage.set(`Product "${product.name}" added successfully!`);
        this.form.reset();
        this.isSubmitting.set(false);
        this.productAdded.emit();

        // Auto-clear success message after 4 seconds
        setTimeout(() => this.successMessage.set(null), 4000);
      },
      error: (err: HttpErrorResponse) => {
        console.error('Failed to create product:', err);
        if (err.status === 409) {
          this.errorMessage.set(`A product with this code already exists.`);
        } else if (err.status === 400 && err.error?.errors) {
          const firstError = Object.values(err.error.errors).flat()[0] as string;
          this.errorMessage.set(firstError);
        } else {
          this.errorMessage.set('Failed to create product. Please try again.');
        }
        this.isSubmitting.set(false);
      }
    });
  }
}
