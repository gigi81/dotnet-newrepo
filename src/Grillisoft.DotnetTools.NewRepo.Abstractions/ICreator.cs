using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Abstractions
{
    public interface ICreator
    {
        Task Create(CancellationToken cancellationToken);

        bool IsParallel { get; }
    }
}
