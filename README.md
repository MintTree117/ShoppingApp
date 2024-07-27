# Shopping App

This shopping app interacts with two APIs:

- **Catalog API:** Provides read-only product and related information.
- **Ordering API:** Manages orders, carts, user-accounts, and related functionalities.

## Features

### Technologies Used

- .NET 8
- Blazor WebAssembly

### Authentication & Security

- JWT and Cookie Authentication
- Cascading Authentication with Custom Implementation
- User Accounts
  - Manage Addresses
  - Edit Profile
  - 2FA Login
  - Recovery Codes
  - Password Reset
  - Account Validation
- Session Management

### Shopping Cart

- Guest (Offline/Local) Shopping Cart
- User (Server) Shopping Cart

### Product Search

- Full Product Search Capabilities
  - Search by Price, Trends, Categories, Brands
  - Sorting and Pagination
  - Text Search

### Product Details

- Detailed Product Page
  - Similar Product Suggestions

### User Experience

- Calculates Shipping Time Based on Location
- Orders View

### Frontend

- Fully Responsive Design
- Extensive Use of Custom Razor Components
- Custom Blazor Layouts
- Custom Styling with CSS
- JavaScript Interop
