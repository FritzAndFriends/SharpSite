# Contributing to SharpSite

Thank you for your interest in contributing to SharpSite! Your contributions are valuable and help improve the project for everyone.

SharpSite's source code is organized with a release branch, release train system to group new features and fixes to the application.  You will see branches in the main repository with a name like **v0.5** that indicate these are the release branches the team is actively working on.  Developers that are working on features as part of this **v0.5** release will send pull-requests to the **v0.5** release and those changes are queued up in a **v0.5** pull request that merges the **v0.5** branch into **main**.  When work is completed for a release, this pull-request is completed and merged into the **main** branch.  A matching tag is applied to the **main** branch for the former branch number.

When released versions need patches / fixes applied, there will be a similar fix-release branch with a name like **v0.5.1** that is based on the **v0.5** tag.  A similar working branch and pull-request is created to organize and apply the fixes

Here's how you can get started working with the source code, submit features and code fixes:

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)


## Getting Started

1. Fork the repository to your GitHub account.
2. Clone the forked repository to your local machine using:

```
git clone https://github.com/YOUR-USERNAME/SharpSite.git
```

3. Add an upstream reference to the main repository:

```
git remote add upstream git@github.com:FritzAndFriends/SharpSite.git
```

4. Checkout a copy of the release branch that you would like to contribute to.  If the case of milestone 0.5, the release branch is called **v0.5** you would execute this command:

```
git checkout --track upstream/v0.5
```

5. Create a new branch for your feature or bugfix.  By running this command from the **v0.5** branch that was checked out in step 4, it will be based on the **v0.5** branch the team is actively collaborating on:

```
git checkout -b feature_your-feature-name
```

## Development Setup

1. Ensure you have .NET 9 SDK installed on your machine.
2. Configure Docker or Podman on your machine.  We will use this with .NET Aspire.  More details about configuring and using .NET Aspire can be found at: https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling
3. Install the necessary dependencies.  From the root folder, run this command:

```
dotnet restore
```

3. Build the project from the **SharpSite.AppHost** folder:

```
dotnet build
```

4. Install development certificates for local development.

Generate a new HTTPS development certificate:
```bash
dotnet dev-certs https --trust
```

Export the HTTPS development certificate:
```bash
dotnet dev-certs https -ep path/to/certificate.pfx -p yourpassword
```

Verify the HTTPS development certificate:
```bash
dotnet dev-certs https --check --trust
```

5. Run the project locally with .NET Aspire from the **SharpSite.AppHost** folder:

```
dotnet run
```

## Keeping your local code up to date

You should run a few commands periodically to ensure that your local code is updated with the current changes in the main repsitory.

### Update your local code from the shared repository

The sample command updates the **main** branch.  You can run this for other feature branches that you are tracking and working with

```
git checkout main
git pull upstream
```

### Merge changes from another branch into your local working branch

You should bring the latest commits from the shared branch into your feature branch to minimize the conflicts when you are ready to share your code with the rest of the project team.  This sample checks out your work in the **feature_MY-WORKING-BRANCH** branch and merges the latest updates from the **main** branch 

```
git checkout feature_MY-WORKING-BRANCH
git merge main
```

## Default User

The default user that is built and initialized in SharpSite is an Administrator and has these credentials:
- username: `admin@localhost`
- password: `Admin123!`

## Coding Guidelines

- Follow the C# coding conventions as outlined in the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Keep code changes concise and focused.
- Write clear, descriptive commit messages.

## Adding Translations

To add translations for a new locale, follow these steps:

1. **Add a .resx File**:
    - Navigate to the `SharpSite.Web/Locales` folder in the project.
    - Add a new `.resx` file with the appropriate naming convention, e.g., `SharedResource.<locale>.resx` (e.g., `SharedResource.fr.resx` for French).
    - Add the necessary key-value pairs for the translations in the `.resx` file.

2. **Update Locales Configuration**:
    - Open `Locales/Configuration.cs`.
    - Add an entry for the new locale in the appropriate configuration section. For example:

      ```csharp
      public static class Configuration
      {
        public readonly static string[] SupportedCultures = {
           "en-US",
           "nl-NL"
            // Add your new locale here
           "<locale>"
        };
        ...
      }

      ```

3. **Verify Translations**:
    - Ensure that the translations are correctly loaded and displayed in the application.
    - Test the application with the new locale to verify that all translations are working as expected.

## Submitting Changes

1. Ensure all tests pass before submitting a pull request.
2. Update documentation as needed.
3. Push your branch to your forked repository:

```
git push origin feature_your-feature-name
```

4. Open a pull request against the release branch (in the sample above, it was v0.5) of the [main repository](https://github.com/FritzAndFriends/SharpSite).  GitHub will typically prompt you with a banner at the top of the repository that offers to assist you in creating this pull request.  Make sure that it is merging into a **base** that is the release branch you are working from, in the sample case that branch is v0.5

## Reporting Issues

If you find a bug or have a feature request, please [open an issue](https://github.com/FritzAndFriends/SharpSite/issues) on GitHub. Provide as much detail as possible to help us address the issue quickly.

## Lead Project Maintainer

[Jeff Fritz](https://github.com/csharpfritz) is the lead project maintainer. Feel free to reach out to him for any significant inquiries or guidance regarding SharpSite.

## Code of Conduct

We adhere to the Contributor Covenant [Code of Conduct](https://www.contributor-covenant.org/version/2/0/code_of_conduct/). By participating, you are expected to uphold this code.

## License

By contributing to SharpSite, you agree that your contributions will be licensed under the [MIT License](LICENSE).
