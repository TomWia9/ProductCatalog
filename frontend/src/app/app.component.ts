import { Component, ViewChild } from '@angular/core';
import { ProductFormComponent } from './features/product-form/product-form.component';
import { ProductListComponent } from './features/product-list/product-list.component';

/**
 * Root application component.
 * Orchestrates the product form and product list, triggering list refreshes
 * on successful product creation.
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ProductFormComponent, ProductListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  @ViewChild(ProductListComponent) private readonly productList!: ProductListComponent;

  /** Called when the form emits a productAdded event. Refreshes the list. */
  onProductAdded(): void {
    this.productList.loadProducts();
  }
}
