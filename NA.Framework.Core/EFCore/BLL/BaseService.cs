using Microsoft.EntityFrameworkCore;
using NA.Framework.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using NA.Framework.Core.EFCore.DAL;
using System.Linq;

namespace NA.Framework.Core.EFCore.BLL
{
    public abstract class BaseService<TEntity, TPK, TContext> : IBaseService<TEntity, TPK, TContext> where TEntity : BaseEntity<TPK> where TContext : DbContext
    {
        public abstract IBaseRepository<TEntity, TPK, TContext> GetRepository();

        public TEntity Add(TEntity entity, bool autoSave = true)
        {
            if (entity == null)
                return null;

            return GetRepository().Add(entity, autoSave);
        }

        public TEntity AddOrUpdate(TEntity entity, bool autoSave = true)
        {
            if (entity == null)
                return null;

            return GetRepository().AddOrUpdate(entity, autoSave);
        }

        public void AddRange(IEnumerable<TEntity> entities, bool autoSave = true)
        {
            if (entities == null || entities.Count() <= 0)
                return;

            GetRepository().AddRange(entities, autoSave);
        }

        public bool Any(Expression<Func<TEntity, bool>> where = null)
        {
            return GetRepository().Any(where);
        }

        public void Delete(TEntity entity, bool autoSave = true, bool isSoftDelete = true)
        {
            if (entity == null)
                return;

            GetRepository().Delete(entity, autoSave, isSoftDelete);
        }

        public void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true, bool isSoftDelete = true)
        {
            GetRepository().Delete(where, autoSave, isSoftDelete);
        }

        public void DeleteById(TPK id, bool autoSave = true, bool isSoftDelete = true)
        {
            GetRepository().DeleteById(id, autoSave, isSoftDelete);
        }

        public TEntity GetById(TPK id, Inclusion<TEntity> inclusion = null, bool isTrack = false)
        {
            return GetRepository().GetById(id, inclusion, isTrack);
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> where = null, Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool isTrack = false)
        {
            return GetRepository().GetList(where, inclusion, orderBy, select, isTrack);
        }

        public List<TEntity> GetPageList(int nowPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where = null, Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool IsTrack = false)
        {
            return GetRepository().GetPageList(nowPage, pageSize, out rowCount, where, inclusion, orderBy, select, IsTrack);
        }

        public void Save()
        {
            GetRepository().Save();
        }

        public TEntity Update(TEntity entity, bool autoSave = true)
        {
            if (entity == null)
                return null;

            return GetRepository().Update(entity, autoSave);
        }

        public void UpdateRange(IEnumerable<TEntity> entities, bool autoSave = true)
        {
            if (entities == null || entities.Count() <= 0)
                return;

            GetRepository().UpdateRange(entities, autoSave);
        }

        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> where = null)
        {
            return GetRepository().LastOrDefault(where);
        }

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> source)
        {
            return GetRepository().Max(source);
        }

        public void Update(Expression<Func<TEntity, bool>> where, Action<TEntity> handle, bool autoSave = true)
        {
            GetRepository().Update(where, handle, autoSave);
        }
    }
}
