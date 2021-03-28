using System;
using System.Runtime.Serialization;

namespace Grillisoft.DotnetTools.NewRepo
{
    [Serializable]
    internal class RepositoryDirectoryNotEmpty : Exception
    {
        public RepositoryDirectoryNotEmpty(string directory)
            : base($"Cannot create repository. {directory} is not empty. There are other files in the directory other than " + NewRepoSettings.InitFilename)
        {
        }

        protected RepositoryDirectoryNotEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}