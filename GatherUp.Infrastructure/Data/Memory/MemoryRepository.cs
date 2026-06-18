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

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "לא ניתן להוסיף אובייקט ריק");

            if (entity.Id == 0)
            {
                ((dynamic)entity).Id = _nextId++;
            }
            else
            {
                if (_data.Any(x => x.Id == entity.Id))
                {
                    throw new InvalidOperationException($"שגיאה: אובייקט עם מזהה {entity.Id} כבר קיים במערכת!");
                }

                if (entity.Id >= _nextId)
                {
                    _nextId = entity.Id + 1;
                }
            }

            _data.Add(entity);
        }

        public T GetById(int id)
        {
            return _data.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _data;
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingEntity = GetById(entity.Id);

            if (existingEntity != null)
            {
                int index = _data.IndexOf(existingEntity);
                _data[index] = entity;
            }
            else
            {
                throw new KeyNotFoundException($"אובייקט עם מזהה {entity.Id} לא נמצא במערכת ולא ניתן לעדכון.");
            }
        }

        public void Delete(int id)
        {
            var entityToDelete = GetById(id);

            if (entityToDelete != null)
            {
                _data.Remove(entityToDelete);
            }
        }
    }
}