using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure.XML; 

namespace GatherUp.Infrastructure.Data
{
    public class XmlRepository<T> : IRepository<T> where T : class, IEntity, new()
    {
        protected readonly string _filePath;

        
        public XmlRepository()
        {
            string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "GatherUp.Infrastructure"));
            string folderPath = Path.Combine(projectPath, "XmlDatabase");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{typeof(T).Name}.xml";
            _filePath = Path.Combine(folderPath, fileName);
        }

        public virtual IEnumerable<T> GetAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            return XMLSerializer.ReadFromFile<List<T>>(_filePath) ?? new List<T>();
        }

        public virtual T GetById(int id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public virtual void Add(T entity)
        {
            List<T> currentData = GetAll().ToList();

            if (entity.Id == 0)
            {
                int nextId = currentData.Count > 0 ? currentData.Max(x => x.Id) + 1 : 1;
                ((dynamic)entity).Id = nextId;
            }

            currentData.Add(entity);
            XMLSerializer.WriteToFile(_filePath, currentData);
        }

        public virtual void Update(T entity)
        {
            List<T> currentData = GetAll().ToList();

            int index = currentData.FindIndex(x => x.Id == entity.Id);

            if (index != -1)
            {
                currentData[index] = entity;

                XMLSerializer.WriteToFile(_filePath, currentData);
            }
            else
            {
                throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");
            }
             
        }

       
        public virtual void Delete(int id)
        {
            List<T> currentData = GetAll().ToList();

            T entityToDelete = currentData.FirstOrDefault(x => x.Id == id);

            if (entityToDelete != null)
            {
                currentData.Remove(entityToDelete);

                XMLSerializer.WriteToFile(_filePath, currentData);
            }
        }
    }
}