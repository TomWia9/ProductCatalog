import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateProductRequest, Product } from '../models/product.model';

/**
 * Service responsible for communicating with the Product Catalog REST API.
 * Provides methods for fetching and creating products.
 */
@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/products`;

  /**
   * Retrieves all products from the catalog.
   * @returns Observable emitting an array of {@link Product}.
   */
  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  /**
   * Adds a new product to the catalog.
   * @param request The product creation payload.
   * @returns Observable emitting the newly created {@link Product}.
   */
  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request);
  }
}
