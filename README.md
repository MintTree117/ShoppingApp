# Shopping App

This shopping app interacts with two APIs:

- [CatalogApi](https://github.com/MintTree117/CatalogApi.git) Provides read-only product and related information.
- [OrderingApi](https://github.com/MintTree117/OrderingApi.git) Manages orders, carts, user-accounts, and related functionalities.

See the fully deployed application [here](https://happy-bush-0b0f3e80f.5.azurestaticapps.net/).

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
