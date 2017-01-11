using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWriting
{
    public interface IRepository<T> where T: class
    {
        void Insert(T entity);
        void Update(T entity);
        List<T> ListOf(int count = 0);
        void Delete(T entity);


    }
}
