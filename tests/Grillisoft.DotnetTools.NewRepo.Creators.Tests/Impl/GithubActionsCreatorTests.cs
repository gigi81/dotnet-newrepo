using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests.Impl;

public class GithubActionsCreatorTests
{
    [Test]
    public async Task Create_WhenEnabled_CreatesWorkflowFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs).With("githubactions", true);
        var creator = new GithubActionsCreator(settings, NullLogger<GithubActionsCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var expectedPath = fs.Path.Combine(settings.Root.FullName, ".github", "workflows", "ci.yml");
        await Assert.That(fs.File.Exists(expectedPath)).IsTrue();
    }

    [Test]
    public async Task Create_WhenDisabled_SkipsFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs); // githubactions defaults to false
        var creator = new GithubActionsCreator(settings, NullLogger<GithubActionsCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var expectedPath = fs.Path.Combine(settings.Root.FullName, ".github", "workflows", "ci.yml");
        await Assert.That(fs.File.Exists(expectedPath)).IsFalse();
    }
}
