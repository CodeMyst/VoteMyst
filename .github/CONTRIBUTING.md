# Contributing

When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change. We will only merge changes that we approve of. If we request a change in your pull request, we kindly ask that you are open to feedback.

## Table of Contents

- [Writing code](#writing-code)
  - [Extending the database](#extending-the-database)
  - [Modifying existing code](#modifying-existing-code)
- [Adding HTML/CSS content](#adding-html/css-content)

---

## Writing Code

VoteMyst runs on ASP.NET Core, which means that functionality is written in C#. We try to maintain the standard C# naming conventions throughout the projects and request that you try the same to ensure a consistent convention. Please refer to existing code for that matter and do not shy away from asking for feedback in your pull requests.

Note that the project makes heavy use of database queries and dependency injection.

### Extending the database

If you modify the database models during a change note that you need to create a new migration for the change with `dotnet ef migrations add <name>`. Note that we try to change the database as little as possible to allow maximum compatibility.

Please turn to the EntityFramework documentation for questions on how to configure the columns.

### Modifying existing code

If possible, try to keep existing code intact while making a change. We will approach changes to existing functionality in a more critical manner, to ensure breaking changes do not occur.

## Adding HTML/CSS content

If you add a new view to the project please try to stick to the styling conventions established by the other pages inside the project. For color choices, stick to the pre-defined CSS variables in `:root`. If you want to propose a new color variable to be added, do not hesitate to include it.
