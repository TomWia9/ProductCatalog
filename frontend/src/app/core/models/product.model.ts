/** Represents a product returned from the API. */
export interface Product {
  id: string;
  code: string;
  name: string;
  price: number;
  createdAtUtc: string;
}

/** Payload for creating a new product. */
export interface CreateProductRequest {
  code: string;
  name: string;
  price: number;
}
