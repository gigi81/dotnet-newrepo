﻿using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl
{
    public class DotnetCreator : CreatorBase
    {
        private const string FolderGuid = "2150E333-8FDC-42A3-9474-1A3956D46DE8";

        public DotnetCreator(
            INewRepoSettings settings,
            ILogger<DotnetCreator> logger)
            : base(settings, logger)
        {
        }

        public override bool IsParallel => false;

        public override async Task Create(CancellationToken cancellationToken)
        {
            var src = this.Src.CreateSubdirectory(_settings.Name);
            var tests = this.Tests.CreateSubdirectory(_settings.Name + ".Tests");

            await Run("dotnet", "new sln", cancellationToken);
            await Run("dotnet", "new classlib", src, cancellationToken);
            await Run("dotnet", "new classlib", tests, cancellationToken);

            var projects = this.Root.GetFiles("*.csproj", System.IO.SearchOption.AllDirectories)
                               .Select(prj => "\"" + prj.FullName + "\"");

            foreach (var project in projects)
                await Run("dotnet", "sln add " + project, cancellationToken);

            var solution = this.Root.GetFiles("*.sln").First();
            var content = PatchContent(await solution.ReadAllLinesAsync(cancellationToken));
            await solution.WriteAllLinesAsync(content, cancellationToken);

            if (_settings.EnableSlnx)
            {
                await Run("dotnet", "sln migrate", cancellationToken);
                solution.Delete();
            }
        }

        private List<string> PatchContent(IEnumerable<string> lines)
        {
            var ret = new List<string>();

            foreach(var line in lines)
            {
                if (line.Equals("Global"))
                {
                    ret.AddRange(new[] {
                        "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{" + Guid.NewGuid() + "}\"",
                        "\tProjectSection(SolutionItems) = preProject",
                        "\t\tDirectory.Build.props = Directory.Build.props",
                        "\t\tDirectory.Packages.props = Directory.Packages.props",
                        "\t\tLICENSE.md = LICENSE.md",
                        "\t\tREADME.md = README.md",
                        "\tEndProjectSection",
                        "EndProject"
                    });
                }

                ret.Add(line);

                if(line.Contains(FolderGuid))
                {
                    if(line.Contains("src"))
                    {
                        ret.AddRange(new[] {
                            "\tProjectSection(SolutionItems) = preProject",
                            "\t\tsrc\\Directory.Build.props = src\\Directory.Build.props",
                            "\tEndProjectSection"
                        });
                    }
                    else if (line.Contains("tests"))
                    {
                        ret.AddRange(new[] {
                            "\tProjectSection(SolutionItems) = preProject",
                            "\t\ttests\\Directory.Build.props = tests\\Directory.Build.props",
                            "\tEndProjectSection"
                        });
                    }
                }
            }

            return ret;
        }
    }
}
