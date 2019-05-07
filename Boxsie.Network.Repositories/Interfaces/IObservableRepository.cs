using System;
using System.Collections.Generic;

namespace Boxsie.Network.Repositories.Interfaces
{
    public interface IObservableRepository<T, in TY>
    {
        void Register();
        void Subscribe(TY subscriber);
        void Unsubscribe(string id);

        T Get(Guid id);
        List<T> GetAll();
        void Update(T obj);
        void Update(Guid itemId, byte[] bytes);
        void Delete(Guid id);
    }
}