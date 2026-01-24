namespace Likano.Application.Interfaces
{
    public interface IStatisticRepository
    {
        Task AddViewCount(int productId);
    }
}
