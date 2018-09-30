using Microsoft.EntityFrameworkCore;
using NA.Framework.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using NA.Framework.Core.Extensions;

namespace NA.Framework.Core.EFCore.DAL
{
    public class BaseRepository<TEntity, TPK, TContext> : IBaseRepository<TEntity, TPK, TContext> where TEntity : BaseEntity<TPK> where TContext : DbContext
    {
        protected string TableName
        {
            get
            {
                if (_tableName == null)
                {
                    _tableName = GetDbContext().Model.GetEntityTypes().SingleOrDefault(e => e.ClrType == typeof(TEntity))?.Relational().TableName;
                }
                return _tableName;
            }
        }
        private string _tableName = null;

        private readonly TContext _context;

        public BaseRepository(TContext context)
        {
            _context = context;
        }

        public virtual TContext GetDbContext()
        {
            return _context;
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        #region 查询

        public virtual TEntity GetById(TPK id, Inclusion<TEntity> inclusion = null, bool isTrack = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();
            if (inclusion != null)
            {
                var queryableResultWithIncludes = inclusion.Includes
                    .Aggregate(query,
                        (current, include) => current.Include(include));
                var secondaryResult = inclusion.IncludeStrings
                    .Aggregate(queryableResultWithIncludes,
                        (current, include) => current.Include(include));

                query = secondaryResult;
            }

            var temp = query.Where(e => e.Id.Equals(id));
            var result = (isTrack ? temp : temp.AsNoTracking()).FirstOrDefault();
            return result;
        }

        protected virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> where = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool isTrack = false)
        {
            if (where == null)
            {
                Expression<Func<TEntity, bool>> expression = e => true;
                where = expression;
            }

            if (select == null)
            {
                Expression<Func<TEntity, TEntity>> expression = e => e;
                select = expression;
            }

            var temp = _context.Set<TEntity>().Where(where).Select(select).OrderBy(orderBy);
            var result = isTrack ? temp : temp.AsNoTracking();
            return result;
        }

        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> where = null, Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool isTrack = false)
        {
            if (where == null)
            {
                Expression<Func<TEntity, bool>> expression = e => true;
                where = expression;
            }

            if (select == null)
            {
                Expression<Func<TEntity, TEntity>> expression = e => e;
                select = expression;
            }

            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();
            if (inclusion != null)
            {
                var queryableResultWithIncludes = inclusion.Includes
                    .Aggregate(query,
                        (current, include) => current.Include(include));
                var secondaryResult = inclusion.IncludeStrings
                    .Aggregate(queryableResultWithIncludes,
                        (current, include) => current.Include(include));

                query = secondaryResult;
            }

            var temp = query.Where(where).Select(select).OrderBy(orderBy);
            var result = (isTrack ? temp : temp.AsNoTracking()).ToList();
            return result;
        }

        public virtual List<TEntity> GetPageList(int nowPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where = null,
            Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool IsTrack = false)
        {
            nowPage = nowPage <= 0 ? 1 : nowPage;
            pageSize = pageSize < 0 ? 0 : pageSize;
            if (where == null)
            {
                Expression<Func<TEntity, bool>> expression = e => true;
                where = expression;
            }

            if (select == null)
            {
                Expression<Func<TEntity, TEntity>> expression = e => e;
                select = expression;
            }

            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();
            if (inclusion != null)
            {
                var queryableResultWithIncludes = inclusion.Includes
                    .Aggregate(query,
                        (current, include) => current.Include(include));
                var secondaryResult = inclusion.IncludeStrings
                    .Aggregate(queryableResultWithIncludes,
                        (current, include) => current.Include(include));

                query = secondaryResult;
            }

            var temp = query.Where(where);

            rowCount = temp.Count();

            var tempResult = temp.Select(select)
                .OrderBy(orderBy)
                .Skip((nowPage - 1) * pageSize)
                .Take(pageSize);

            var result = (IsTrack ? tempResult : tempResult.AsNoTracking()).ToList();
            return result;
        }

        public virtual TEntity LastOrDefault(Expression<Func<TEntity, bool>> where = null)
        {
            TEntity lastItem;
            if (where == null)
            {
                lastItem = _context.Set<TEntity>().LastOrDefault();
            }
            else
            {
                lastItem = _context.Set<TEntity>().LastOrDefault(where);
            }
            return lastItem;
        }

        public virtual TResult Max<TResult>(Expression<Func<TEntity, TResult>> source)
        {
            var result = _context.Set<TEntity>().Max(source);
            return result;
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> where = null)
        {
            bool any;
            if (where == null)
            {
                any = _context.Set<TEntity>().Any();
            }
            else
            {
                any = _context.Set<TEntity>().Any(where);
            }
            return any;
        }

        #endregion

        #region 添加

        public virtual TEntity Add(TEntity entity, bool autoSave = true)
        {
            _context.Set<TEntity>().Add(entity);
            if (autoSave)
            {
                Save();
            }
            return entity;
        }

        public virtual TEntity AddOrUpdate(TEntity entity, bool autoSave = true)
        {
            TEntity result;
            if (GetById(entity.Id) != null)
            {
                result = Update(entity, autoSave);
            }
            else
            {
                result = Add(entity, autoSave);
            }

            return result;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool autoSave = true)
        {
            _context.Set<TEntity>().AddRange(entities);
            if (autoSave)
            {
                Save();
            }
        }

        #endregion

        #region 删除

        public virtual void Delete(TEntity entity, bool autoSave = true, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                entity.IsDeleted = 1;
                _context.Set<TEntity>().Update(entity);
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }

            if (autoSave)
            {
                Save();
            }
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true, bool isSoftDelete = true)
        {
            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            var entities = _context.Set<TEntity>().Where(where).AsNoTracking().ToList();
            if (isSoftDelete)
            {
                entities.ForEach(e => e.IsDeleted = 1);
                UpdateRange(entities);
            }
            else
            {
                _context.Set<TEntity>().RemoveRange(entities);
            }

            if (autoSave)
            {
                Save();
            }
        }

        public virtual void DeleteById(TPK id, bool autoSave = true, bool isSoftDelete = true)
        {
            var entity = GetById(id);
            if (isSoftDelete)
            {
                entity.IsDeleted = 1;
                _context.Set<TEntity>().Update(entity);
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }

            if (autoSave)
            {
                Save();
            }
        }

        #endregion
        
        #region 修改

        public virtual void Update(Expression<Func<TEntity, bool>> where, Action<TEntity> handle, bool autoSave = true)
        {
            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            var entities = _context.Set<TEntity>().Where(where).AsNoTracking().ToList();
            entities.ForEach(handle);
            _context.Set<TEntity>().UpdateRange(entities);
            if (autoSave)
            {
                Save();
            }
        }

        public virtual TEntity Update(TEntity entity, bool autoSave = true)
        {
            entity.UpdateTime = DateTime.Now.ToCNZone();

            _context.Set<TEntity>().Update(entity);
            if (autoSave)
            {
                Save();
            }
            return entity;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool autoSave = true)
        {
            entities = entities.Where(e => e != null);
            foreach (var entity in entities)
            {
                entity.UpdateTime = DateTime.Now.ToCNZone();
            }

            _context.Set<TEntity>().UpdateRange(entities);
            if (autoSave)
            {
                Save();
            }
        }

        #endregion
    }
}
