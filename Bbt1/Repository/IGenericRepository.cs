using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt1.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> SelectAll();
        T SelectById(int id);
        int Insert(T t);
        int Update(T t);
        int Delete(T t);
    }
}
