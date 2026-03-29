using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests;

public class AzurePipelinesCreatorTests
{
    [Test]
    public async Task Create_WhenEnabled_CreatesFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs).With("adobuild", true);
        var creator = new AzurePipelinesCreator(settings, NullLogger<AzurePipelinesCreator>.Instance);

        await creator.Create(CancellationToken.None);

        await Assert.That(fs.File.Exists(fs.Path.Combine(settings.Root.FullName, "azure-pipelines.yml"))).IsTrue();
    }

    [Test]
    public async Task Create_WhenDisabled_SkipsFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs); // adobuild defaults to false
        var creator = new AzurePipelinesCreator(settings, NullLogger<AzurePipelinesCreator>.Instance);

        await creator.Create(CancellationToken.None);

        await Assert.That(fs.File.Exists(fs.Path.Combine(settings.Root.FullName, "azure-pipelines.yml"))).IsFalse();
    }
}
