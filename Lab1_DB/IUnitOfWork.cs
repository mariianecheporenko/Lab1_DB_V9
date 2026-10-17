using Npgsql;
using System.Data;

namespace MultiStoreLab;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    void Commit();
    void Rollback();
}