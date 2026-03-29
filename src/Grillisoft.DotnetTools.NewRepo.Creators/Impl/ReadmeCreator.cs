using Grillisoft.DotnetTools.NewRepo.Abstractions;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Impl;

public class ReadmeCreator : CreatorBase
{
    public const string Name = "README.md";

    public ReadmeCreator(
        INewRepoSettings settings,
        ILogger<ReadmeCreator> logger)
        : base(settings, logger)
    {
    }

    public override async Task Create(CancellationToken cancellationToken)
    {
        if (_settings.EmptyReadme)
        {
            await this.CreateTextFile(this.Root.File(Name), $"# {_settings.Name}");
            return;
        }

        var content = (await GetTemplateContent(Name))
            .Replace("project_name", _settings.Name)
            .Replace("project_description", _settings.Product)
            .Replace("project_license", _settings.License);

        await this.CreateTextFile(this.Root.File(Name), content);
    }
}