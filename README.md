ECommerceAPI

Description
This is a RESTful API for managing an eCommerce backend, including products, users, carts, and orders. Built with ASP.NET Core, it provides authentication, product management, and order processing to support a complete eCommerce system.

Features

* Full CRUD operations for products, categories, and inventory
* User registration, authentication, and JWT-based authorization
* Shopping cart and order management
* Search, filter, and pagination support for products
* Role-based access control for Admin and User
* Swagger integration for API testing

Tech Stack

* Backend: ASP.NET Core and C#
* Database: SQL Server, MySQL, or PostgreSQL
* Authentication: JWT Tokens
* Documentation: Swagger or OpenAPI
* Tools: Postman for testing endpoints

Project Structure

* Controllers: API endpoints
* Models: Database models
* Data: DbContext and migrations
* Services: Business logic
* ECommerceAPI.sln: Solution file
* README.md: Project documentation
* .gitignore: Files to ignore in Git

Getting Started

Clone the Repository

* Run `git clone https://github.com/Preet-savliya/ECommerceAPI.git`
* Navigate to the project folder with `cd ECommerceAPI`

Install Dependencies

* Run `dotnet restore` to install required packages

Configure the Database

* Update the connection string in `appsettings.json`
* Example:
  ConnectionStrings
  DefaultConnection : Server=YOUR_SERVER;Database=ECommerceDB;Trusted_Connection=True;

Run Migrations

* Run `dotnet ef database update` to create the database schema

Launch the API

* Run `dotnet run` to start the server
* By default, the API will run at `http://localhost:5000`

API Endpoints

Products

* GET /api/products : List all products
* GET /api/products/{id} : Get product details by ID
* POST /api/products : Add a new product
* PUT /api/products/{id} : Update product
* DELETE /api/products/{id} : Delete product

Users

* POST /api/users/register : Register new user
* POST /api/users/login : Login user and get JWT token

Orders

* POST /api/orders : Place a new order
* GET /api/orders/{id} : Get order details

Testing

* Use Swagger UI at `http://localhost:5001/swagger`
* Use Postman to test all API endpoints

Contribution

* Fork the repository
* Create a feature branch
* Commit your changes
* Open a pull request

License
This project is licensed under the MIT License.
