# SharpSite

[![Built With .NET](https://img.shields.io/badge/Built_With_.NET-512BD4?style=plastic&logo=DotNet&logoColor=white)](https://dot.net)<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-13-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END --> 
[![.NET Build + Test](https://github.com/FritzAndFriends/SharpSite/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/FritzAndFriends/SharpSite/actions/workflows/dotnet-build.yml)
[![Test Results](https://fritzblog.blob.core.windows.net/githubartifacts/unittest-badge.svg?0.6)](https://fritzblog.blob.core.windows.net/githubartifacts/unittest-badge.svg)
[![End-to-End Test Results](https://fritzblog.blob.core.windows.net/githubartifacts/playwright-badge.svg?0.6.1)](https://fritzblog.blob.core.windows.net/githubartifacts/playwright-badge.svg)

A modern, accessible CMS built with .NET 9 and Blazor that combines the simplicity of traditional content management with the power of modern web development.

## Purpose

SharpSite aims to be a highly customizable content management system that adapts to your website needs. Whether you're a non-technical user looking to create a simple blog, or a developer wanting to build a complex web application, SharpSite provides the flexibility to customize as little or as much as you need using HTML, Markdown, C#, or Blazor code.

## System Requirements

- .NET 9 SDK
- PostgreSQL 16 or later
- Visual Studio 2022 or VS Code (recommended)
- Docker or Podman container runtime

## Getting Started

1. Clone the repository

2. Configure your PostgreSQL connection string in `appsettings.json`

3. Ensure your container runtime (Docker or Podman) is running

4. Run the application using your preferred method:
   - Using Visual Studio: Open `SharpSite.sln` and run the `AppHost` project
   - Using command line: `dotnet run --project src/AppHost`

5. Navigate to `https://localhost:5001` in your browser

### Default Administrator Account
- Username: `admin@localhost`
- Password: `Admin123!`

## Current Features

### Core Features

* **Authentication & Authorization**
  * Built-in user management with roles (Admin, Editor, User)
  * Social login support with external authentication providers
  * Two-factor authentication (2FA) with authenticator apps
  * Email confirmation and account recovery

* **Content Management**
  * Blog posts and custom pages creation
  * Markdown and HTML content support
  * RSS feed generation
  * Automatic sitemap generation
  * Robots.txt customization

* **System Features**
  * Flexible theming system
  * Plugin architecture for extensibility
  * Localization support for admin interfaces
  * User-friendly admin dashboard
  * PostgreSQL database support

### Administration

* Complete user management interface
* Plugin configuration and management
* Site settings customization
* Content moderation tools

## Planned Features

Our roadmap includes exciting features to enhance the platform's capabilities:

### Core Enhancements
- Content versioning and history
- Advanced output caching
- Docker container support
- Email notification system
- Full-text search capabilities
- Form builder with customizable CRUD operations

### Content Management
- Content tagging and categorization
- Content scheduling and publishing
- Social media integration
- Content export and backup tools
- Static site generation
- Multi-tenant support

### Advanced Features
- Multiple database support (beyond PostgreSQL)
- Email mailing list management
- Payment processing integration
- WordPress import wizard
- Mobile app for content management

### Developer Features
- Enhanced plugin development tools
- API documentation and examples
- Custom theme development kit
- Performance optimization tools

## User Personas

SharpSite is designed to serve three key user types:

### Content Creator
- Non-technical users who want to create websites without coding
- Focus on content creation through user-friendly interfaces
- Uses built-in templates and visual editors

### Web Developer
- Familiar with HTML, CSS, and basic web technologies
- Can customize themes and layouts
- Creates custom templates and styling

### System Integrator
- Experienced with Blazor, .NET, and web development
- Develops custom plugins and extensions
- Implements complex integrations and features

## Contributing

We welcome contributions from all skill levels! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://mas.to/@csharpfritz"><img src="https://avatars.githubusercontent.com/u/78577?v=4?s=100" width="100px;" alt="Jeffrey T. Fritz"/><br /><sub><b>Jeffrey T. Fritz</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=csharpfritz" title="Code">💻</a> <a href="#projectManagement-csharpfritz" title="Project Management">📆</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://deepx.de"><img src="https://avatars.githubusercontent.com/u/3179474?v=4?s=100" width="100px;" alt="Mario 'DeepX' Staats"/><br /><sub><b>Mario 'DeepX' Staats</b></sub></a><br /><a href="#design-deepx" title="Design">🎨</a> <a href="#translation-deepx" title="Translation">🌍</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/MdeBruin93"><img src="https://avatars.githubusercontent.com/u/16732519?v=4?s=100" width="100px;" alt="MdeBruin"/><br /><sub><b>MdeBruin</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=MdeBruin93" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/DjeeBay"><img src="https://avatars.githubusercontent.com/u/22008152?v=4?s=100" width="100px;" alt="DjeeBay"/><br /><sub><b>DjeeBay</b></sub></a><br /><a href="#translation-DjeeBay" title="Translation">🌍</a> <a href="https://github.com/FritzAndFriends/SharpSite/commits?author=DjeeBay" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/DimitarPramatarov"><img src="https://avatars.githubusercontent.com/u/51478619?v=4?s=100" width="100px;" alt="Dimitar Pramatarov"/><br /><sub><b>Dimitar Pramatarov</b></sub></a><br /><a href="#translation-DimitarPramatarov" title="Translation">🌍</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/genga898"><img src="https://avatars.githubusercontent.com/u/84174227?v=4?s=100" width="100px;" alt="Emmanuel Genga"/><br /><sub><b>Emmanuel Genga</b></sub></a><br /><a href="#translation-genga898" title="Translation">🌍</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/mcNets"><img src="https://avatars.githubusercontent.com/u/24267381?v=4?s=100" width="100px;" alt="Joan Magnet"/><br /><sub><b>Joan Magnet</b></sub></a><br /><a href="#translation-mcnets" title="Translation">🌍</a></td>
    </tr>
    <tr>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/default-writer/c"><img src="https://avatars.githubusercontent.com/u/383256?v=4?s=100" width="100px;" alt="default-writer"/><br /><sub><b>default-writer</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=default-writer" title="Documentation">📖</a> <a href="https://github.com/FritzAndFriends/SharpSite/commits?author=default-writer" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/degenone"><img src="https://avatars.githubusercontent.com/u/48437506?v=4?s=100" width="100px;" alt="Tero Kilpeläinen"/><br /><sub><b>Tero Kilpeläinen</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=degenone" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/TheLankyScottishNerd"><img src="https://avatars.githubusercontent.com/u/8051530?v=4?s=100" width="100px;" alt="Declan McIlhatton"/><br /><sub><b>Declan McIlhatton</b></sub></a><br /><a href="#design-TheLankyScottishNerd" title="Design">🎨</a> <a href="https://github.com/FritzAndFriends/SharpSite/commits?author=TheLankyScottishNerd" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://www.simstools.com"><img src="https://avatars.githubusercontent.com/u/301535?v=4?s=100" width="100px;" alt="Occular Malice"/><br /><sub><b>Occular Malice</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=bsimser" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://github.com/JustCallMeAD"><img src="https://avatars.githubusercontent.com/u/4316208?v=4?s=100" width="100px;" alt="JustCallMeAD"/><br /><sub><b>JustCallMeAD</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=JustCallMeAD" title="Code">💻</a></td>
      <td align="center" valign="top" width="14.28%"><a href="https://lucyllewy.com/"><img src="https://avatars.githubusercontent.com/u/147548?v=4?s=100" width="100px;" alt="Lucy Llewellyn"/><br /><sub><b>Lucy Llewellyn</b></sub></a><br /><a href="https://github.com/FritzAndFriends/SharpSite/commits?author=lucyllewy" title="Code">💻</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
