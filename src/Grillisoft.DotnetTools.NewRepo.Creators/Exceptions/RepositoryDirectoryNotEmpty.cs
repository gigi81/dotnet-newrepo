using System;

namespace Grillisoft.DotnetTools.NewRepo.Creators.Exceptions
{
    [Serializable]
    internal class RepositoryDirectoryNotEmpty(string directory, string initFile) : Exception(
        $"Cannot create repository. {directory} is not empty. There are other files in the directory other than {initFile}");
}