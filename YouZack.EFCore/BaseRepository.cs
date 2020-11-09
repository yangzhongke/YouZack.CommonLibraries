using Infrastructures.DI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructures.EFCore
{
    public abstract class BaseRepository : IRepository
    {
        private readonly DbContext dbCtx;
        public BaseRepository(DbContext dbCtx)
        {
            this.dbCtx = dbCtx;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.dbCtx.SaveChangesAsync(cancellationToken);
        }

        protected Task DeleteById<TEntity>(Guid id) where TEntity : BaseEntity
        {
            var dbSet = this.dbCtx.Set<TEntity>();
            var entity = dbSet.Find(id);
            entity.IsDeleted = true;
            return Task.CompletedTask;
        }

        protected Task DeleteRange<TEntity>(Func<TEntity, bool> predicate) where TEntity : BaseEntity
        {
            var dbSet = this.dbCtx.Set<TEntity>();
            var entities = dbSet.Where(predicate);
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }
            return Task.CompletedTask;
        }

        protected Task<bool> Exists<TEntity>(Guid id) where TEntity : BaseEntity
        {
            var dbSet = this.dbCtx.Set<TEntity>();
            return dbSet.AnyAsync(e => e.Id == id);
        }

        //必须要明确指定排序规则，否则在有的情况下排序方式可能和你想象的不一致，曾经在mysql中发现过
        protected Task<IQueryable<TEntity>> FindPaged<TEntity>(IOrderedQueryable<TEntity> orderedQueryable, int pageSize, int pageIndex) where TEntity : BaseEntity
        {
            var items = orderedQueryable.Skip(pageIndex * pageSize).Take(pageSize);
            return Task.FromResult(items);
        }
    }
}