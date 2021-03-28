using System;
using System.Runtime.Serialization;

namespace Grillisoft.DotnetTools.NewRepo.Creators
{
    [Serializable]
    internal class RepositoryDirectoryNotEmpty : Exception
    {
        public RepositoryDirectoryNotEmpty(string directory, string initFile)
            : base($"Cannot create repository. {directory} is not empty. There are other files in the directory other than {initFile}")
        {
        }

        protected RepositoryDirectoryNotEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}