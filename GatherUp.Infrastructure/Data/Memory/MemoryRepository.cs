using System;
using System.Collections.Generic;
using System.Linq;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Data.Memory
{
    public class MemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _data = new List<T>();
        private int _nextId = 1;

        public int Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "לא ניתן להוסיף אובייקט ריק");

            if (entity.Id == 0)
            {
                entity.GetType().GetProperty("Id")!.SetValue(entity, _nextId++);
            }
            else
            {
                if (_data.Any(x => x.Id == entity.Id))
                    throw new InvalidOperationException($"שגיאה: אובייקט עם מזהה {entity.Id} כבר קיים במערכת!");
                if (entity.Id >= _nextId)
                    _nextId = entity.Id + 1;
            }

            _data.Add(entity);
            return entity.Id;
        }

        public T GetById(int id) => _data.FirstOrDefault(x => x.Id == id);

        public IEnumerable<T> GetAll() => _data;

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var existing = GetById(entity.Id);
            if (existing == null) throw new KeyNotFoundException($"אובייקט עם מזהה {entity.Id} לא נמצא.");
            _data[_data.IndexOf(existing)] = entity;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null) _data.Remove(entity);
        }
    }
}
