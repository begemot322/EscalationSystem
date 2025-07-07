using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Infrastructure.Data.Repositories;

namespace EscalationService.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    
    private ICommentRepository _commentRepository;
    private ICriteriaRepository _criteriaRepository;
    private IEscalationRepository _escalationRepository;
    private IEscalationUserRepository _escalationUserRepository;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public ICommentRepository Comments => 
        _commentRepository ??= new CommentRepository(_db);

    public ICriteriaRepository Criterias =>
        _criteriaRepository ??= new CriteriaRepository(_db);

    public IEscalationRepository Escalations =>
        _escalationRepository ??= new EscalationRepository(_db);

    public IEscalationUserRepository EscalationUsers =>
        _escalationUserRepository ??= new EscalationUserRepository(_db);
    
    public void Dispose()
    {
        _db.Dispose();
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
}