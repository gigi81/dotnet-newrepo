using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using Grillisoft.DotnetTools.NewRepo.Creators.Impl;
using Grillisoft.DotnetTools.NewRepo.Creators.Tests.Fakes;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Tests.Impl;

public class ReadmeCreatorTests
{
    [Test]
    public async Task Create_WhenEmpty_CreatesMinimalReadme()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs)
            .With("name", "MyProject")
            .With("emptyReadme", true);
        var creator = new ReadmeCreator(settings, NullLogger<ReadmeCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var readmePath = fs.Path.Combine(settings.Root.FullName, "README.md");
        await Assert.That(fs.File.Exists(readmePath)).IsTrue();
        var content = fs.File.ReadAllText(readmePath);
        await Assert.That(content).IsEqualTo("# MyProject");
    }

    [Test]
    public async Task Create_WhenNotEmpty_SubstitutesProjectName()
    {
        var fs = new MockFileSystem();
        var settings = new FakeSettings(fs)
            .With("name", "MyProject")
            .With("product", "A great product")
            .With("license", "MIT")
            .With("emptyReadme", false);
        var creator = new ReadmeCreator(settings, NullLogger<ReadmeCreator>.Instance);

        await creator.Create(CancellationToken.None);

        var content = fs.File.ReadAllText(fs.Path.Combine(settings.Root.FullName, "README.md"));
        await Assert.That(content).Contains("MyProject");
        await Assert.That(content).Contains("A great product");
        await Assert.That(content).Contains("MIT");
        await Assert.That(content).DoesNotContain("project_name");
        await Assert.That(content).DoesNotContain("project_description");
        await Assert.That(content).DoesNotContain("project_license");
    }
}
