# Contributing to SharpSite

Thank you for your interest in contributing to SharpSite! Your contributions are valuable and help improve the project for everyone. Here's how you can get started:

## Getting Started

1. Fork the repository to your GitHub account.
2. Clone the forked repository to your local machine using:

```
git clone https://github.com/YOUR-USERNAME/SharpSite.git
```

3. Create a new branch for your feature or bugfix:

```
git checkout -b feature_your-feature-name
```

## Development Setup

1. Ensure you have .NET 9 SDK installed on your machine.
2. Install the necessary dependencies.  From the root folder, run this command:

```
dotnet restore
```

3. Build the project from the **SharpSite.AppHost** folder:

```
dotnet build
```

4. Run the project locally with .NET Aspire from the **SharpSite.AppHost** folder:

```
dotnet run
```

## Coding Guidelines

- Follow the C# coding conventions as outlined in the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Keep code changes concise and focused.
- Write clear, descriptive commit messages.

## Submitting Changes

1. Ensure all tests pass before submitting a pull request.
2. Update documentation as needed.
3. Push your branch to your forked repository:

```
git push origin feature_your-feature-name
```

4. Open a pull request against the `main` branch of the original repository.

## Reporting Issues

If you find a bug or have a feature request, please [open an issue](https://github.com/FritzAndFriends/SharpSite/issues) on GitHub. Provide as much detail as possible to help us address the issue quickly.

## Lead Project Maintainer 

[Jeff Fritz](https://github.com/csharpfritz) is the lead project maintainer. Feel free to reach out to him for any significant inquiries or guidance regarding SharpSite. 

## Code of Conduct

We adhere to the Contributor Covenant [Code of Conduct](https://www.contributor-covenant.org/version/2/0/code_of_conduct/). By participating, you are expected to uphold this code.

## License

By contributing to SharpSite, you agree that your contributions will be licensed under the [MIT License](LICENSE).
