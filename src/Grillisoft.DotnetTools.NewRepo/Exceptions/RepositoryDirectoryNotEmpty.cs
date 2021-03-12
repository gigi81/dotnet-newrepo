using System;
using System.Runtime.Serialization;

namespace Grillisoft.DotnetTools.NewRepo
{
    [Serializable]
    internal class RepositoryDirectoryNotEmpty : Exception
    {
        public RepositoryDirectoryNotEmpty(string folder)
            : base($"{folder} is not empty. Cannot create repository")
        {
        }

        protected RepositoryDirectoryNotEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}