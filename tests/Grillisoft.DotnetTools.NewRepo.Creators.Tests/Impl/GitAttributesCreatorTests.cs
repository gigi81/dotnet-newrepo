using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests.Impl;

public class GitAttributesCreatorTests
{
    [Test]
    public async Task Create_AlwaysCreatesFile()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs);
        var creator = new GitAttributesCreator(settings, NullLogger<GitAttributesCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var filePath = fs.Path.Combine(settings.Root.FullName, ".gitattributes");
        await Assert.That(fs.File.Exists(filePath)).IsTrue();
    }

    [Test]
    public async Task Create_FileIsNotEmpty()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs);
        var creator = new GitAttributesCreator(settings, NullLogger<GitAttributesCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var content = fs.File.ReadAllText(fs.Path.Combine(settings.Root.FullName, ".gitattributes"));
        await Assert.That(content).IsNotEmpty();
    }
}
