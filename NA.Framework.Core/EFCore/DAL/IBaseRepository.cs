using Microsoft.EntityFrameworkCore;
using NA.Framework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NA.Framework.Core.EFCore.DAL
{
    public interface IBaseRepository<TEntity, TPK, TContext> where TEntity : BaseEntity<TPK> where TContext : DbContext
    {
        #region 查询

        List<TEntity> GetList(Expression<Func<TEntity, bool>> where = null, Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool isTrack = false);

        TEntity GetById(TPK id, Inclusion<TEntity> inclusion = null, bool isTrack = false);

        bool Any(Expression<Func<TEntity, bool>> where = null);

        List<TEntity> GetPageList(int nowPage, int pageSize, out int rowCount, Expression<Func<TEntity, bool>> where = null, Inclusion<TEntity> inclusion = null, string orderBy = "id asc", Expression<Func<TEntity, TEntity>> select = null, bool IsTrack = false);

        TEntity LastOrDefault(Expression<Func<TEntity, bool>> where = null);

        TResult Max<TResult>(Expression<Func<TEntity, TResult>> source);

        #endregion

        #region 添加

        TEntity Add(TEntity entity, bool autoSave = true);

        void AddRange(IEnumerable<TEntity> entities, bool autoSave = true);

        #endregion

        #region 修改

        void Update(Expression<Func<TEntity, bool>> where, Action<TEntity> handle, bool autoSave = true);

        TEntity Update(TEntity entity, bool autoSave = true);

        void UpdateRange(IEnumerable<TEntity> entities, bool autoSave = true);

        TEntity AddOrUpdate(TEntity entity, bool autoSave = true);

        #endregion

        #region 删除

        void Delete(TEntity entity, bool autoSave = true, bool isSoftDelete = true);

        void DeleteById(TPK id, bool autoSave = true, bool isSoftDelete = true);

        void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true, bool isSoftDelete = true);

        #endregion

        #region 其他

        void Save();

        TContext GetDbContext();

        #endregion
    }
}
