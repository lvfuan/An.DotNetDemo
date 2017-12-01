using BLL.IService;
using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DLL;
using System.Data.Entity;

namespace BLL.Service
{
    public class BaseCommon<T> : IBaseCommon<T> where T : class
    {
        private MyDbContext _db = new MyDbContext();

        private DbSet<T> _dbSet { get { return _db.Set<T>(); } }
        ~BaseCommon() { _db.Dispose(); }
        public void Add(T t)
        {
            this._dbSet.Add(t);
            this.Save();
        }

        public void Delete(T t)
        {
            this._dbSet.Remove(t);
            this.Save();
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            this._dbSet.Where(where).Reverse();
            this.Save();
        }

        public void Edit(T t)
        {
            var entity= this._db.Entry(t);
            entity.State= EntityState.Modified;
            this.Save();
        }

        public bool isExists(Expression<Func<T, bool>> where)
        {
           return this._dbSet.Where(where).Count() > 0;
        }
        private void Save()
        {
            this._db.SaveChanges();
        }
    }
}
