# Object_Oriented_Programming_Assignment_2_HIA

This project uses **Entity Framework Core (EF Core)** as its Object-Relational Mapper (ORM) to manage database interactions. EF Core enables seamless communication between C# objects and the underlying relational database, allowing for:

- Code-first schema generation using annotated models
- LINQ-based querying for readable and efficient data access
- Automatic migrations to evolve the database structure over time
- Strong typing and validation through model constraints

The database context is defined in `JetDBContext.cs`, and models are scaffolded to reflect domain entities such as `Component`, `Layout`, and `JetInterior`.