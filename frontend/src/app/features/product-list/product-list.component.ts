import { CommonModule, DatePipe } from "@angular/common";
import { Component, OnInit, inject, signal } from "@angular/core";
import { Product } from "../../core/models/product.model";
import { ProductService } from "../../core/services/product.service";

/**
 * Displays the full product catalog in a sortable table.
 * Refreshes automatically when a product is added via the form.
 */
@Component({
  selector: "app-product-list",
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: "./product-list.component.html",
  styleUrl: "./product-list.component.scss",
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);

  /** All products loaded from the API. */
  readonly products = signal<Product[]>([]);

  /** Whether data is currently being fetched. */
  readonly isLoading = signal(false);

  /** Error message if the fetch fails. */
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadProducts();
  }

  /** Fetches all products from the API and updates the signal. */
  loadProducts(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error("Failed to load products:", err);
        this.error.set("Failed to load products. Please try again.");
        this.isLoading.set(false);
      },
    });
  }
}
