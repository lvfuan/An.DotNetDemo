using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IBaseCommon<T> where T:class
    {
        void Add(T t);
        void Delete(T t);
        void Delete(Expression<Func<T,bool>> where);
        void Edit(T t);
        bool isExists(Expression<Func<T,bool>> where);
    }
}
