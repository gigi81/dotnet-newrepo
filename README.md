[![Build Status](https://luigigrilli.visualstudio.com/dotnet-newrepo/_apis/build/status/gigi81.dotnet-newrepo?branchName=master)](https://luigigrilli.visualstudio.com/dotnet-newrepo/_build/latest?definitionId=15&branchName=master)
[![Nuget][nuget-shield]][nuget-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<br />
<p align="center">
  <h3 align="center">dotnet newrepo</h3>

  <p align="center">
    This is a dotnet tool to create a git repo for a dotnet project including folders, solution, test project, readme, license, azure devops build template and more...
    <br />
    <br />
    <a href="https://github.com/gigi81/dotnet-newrepo/issues">Report Bug</a>
    ·
    <a href="https://github.com/gigi81/dotnet-newrepo/issues">Request Feature</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary><h2 style="display: inline-block">Table of Contents</h2></summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#prerequisites">Prerequisites</a></li>
    <li><a href="#installation">Installation</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project
There is some sort of an [unofficial standard structure](https://gist.github.com/davidfowl/ed7564297c61fe9ab814) when it comes to dotnet repositories.
For example having a 'src' and 'tests' folder, a solution on the root, etc.
As I was finding myself recreating this sctructure multiple times, I decided to create this tool which helps to create
as much as possible of an entire repository structure, so that you can focus on addding your value and code instead of
worrying about things like how to build the project or how to share settings between projects with DirectoryBuildProps.

## Prerequisites

You will need to have the "dotnet" and "git" commands in the path

## Installation

To first install the tool

```PowerShell
dotnet tool install dotnet-newrepo --global
```

To install tool's updates

```PowerShell
dotnet tool update dotnet-newrepo --global
```

## Usage

```PowerShell
#first create a directory host your repo
mkdir Organization.Project
cd Organization.Project
#then create the init.yml file (this file is used to specify a few settings like your github username)
newrepo init
#customize the init.yml file with your custom settings
notepad init.yml
#then finally create the repo
newrepo
```

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

Distributed under the MIT License. See <a href="LICENSE.md">`LICENSE.md`</a> for more information.

## Contact

Luigi Grilli - [@gigifun](https://twitter.com/gigifun)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/gigi81/dotnet-newrepo
[contributors-url]: https://github.com/gigi81/repo/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/gigi81/dotnet-newrepo
[forks-url]: https://github.com/gigi81/repo/network/members
[stars-shield]: https://img.shields.io/github/stars/gigi81/dotnet-newrepo
[stars-url]: https://github.com/gigi81/repo/stargazers
[issues-shield]: https://img.shields.io/github/issues/gigi81/dotnet-newrepo
[issues-url]: https://github.com/gigi81/repo/issues
[license-shield]: https://img.shields.io/github/license/gigi81/dotnet-newrepo
[license-url]: https://github.com/gigi81/repo/blob/master/LICENSE.md
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-blue
[linkedin-url]: https://linkedin.com/in/luigigrilli
[nuget-shield]:https://img.shields.io/nuget/v/dotnet-newrepo
[nuget-url]:https://www.nuget.org/packages/dotnet-newrepo/