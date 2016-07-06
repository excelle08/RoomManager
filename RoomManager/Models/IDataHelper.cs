using System.Collections.Generic;

namespace RoomManager.Model
{
    public interface IDataHelper<T>
    {
        T Insert(T item);
        IEnumerable<T> SelectAll(string condition);
        T SelectOne(string condition);
        IEnumerable<T> Select(string condition, int offset = 0, int limit = 0);
        int Count(string condition);
        int Update(T item);
        int Delete(T item);
    }
}