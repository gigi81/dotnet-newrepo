using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests;

public class IssueTrackerCreatorTests
{
    [Test]
    public async Task Create_WhenGithubConfigured_CreatesFileWithUrl()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs).With("github", "myorg/myrepo");
        var creator = new IssueTrackerCreator(settings, NullLogger<IssueTrackerCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var filePath = fs.Path.Combine(settings.Root.FullName, ".issuetracker");
        await Assert.That(fs.File.Exists(filePath)).IsTrue();
        var content = fs.File.ReadAllText(filePath);
        await Assert.That(content).Contains("https://github.com/myorg/myrepo.git");
        await Assert.That(content).DoesNotContain("github_url");
    }

    [Test]
    public async Task Create_WhenGithubNotConfigured_SkipsFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs); // github defaults to empty
        var creator = new IssueTrackerCreator(settings, NullLogger<IssueTrackerCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var filePath = fs.Path.Combine(settings.Root.FullName, ".issuetracker");
        await Assert.That(fs.File.Exists(filePath)).IsFalse();
    }
}
