using System.Threading;
using System.Threading.Tasks;

namespace Grillisoft.DotnetTools.NewRepo.Creators
{
    public interface ICreator
    {
        Task Create(CancellationToken cancellationToken);
    }
}
