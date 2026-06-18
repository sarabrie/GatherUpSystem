using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GatherUp.Core.DO.Finance;
using GatherUp.Infrastructure.XML;

namespace GatherUp.Infrastructure.Data
{
    public class ReceiptRepository : XmlRepository<ReceiptDetails>
    {
        private readonly string _receiptFilesFolder;

        public ReceiptRepository() : base()
        {
            _receiptFilesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReceiptFiles");
            if (!Directory.Exists(_receiptFilesFolder))
            {
                Directory.CreateDirectory(_receiptFilesFolder);
            }

            XMLDocManager.CreateEmptyXmlFile(_filePath, "Receipts");
        }

        public override void Add(ReceiptDetails entity)
        {
            if (entity == null) return;

            string originalPath = entity.ReceiptFilePath;
            string fileName = Path.GetFileName(originalPath);
            string newPathInProject = Path.Combine(_receiptFilesFolder, $"{entity.Id}_{fileName}");

            if (File.Exists(originalPath))
            {
                File.Copy(originalPath, newPathInProject, overwrite: true);
            }

            entity.ReceiptFilePath = newPathInProject;

            XElement newReceiptElement = XMLDocManager.CreateReceiptElement(
                entity.Id,
                entity.ReceiptNumber,
                entity.ReceiptFilePath,
                entity.Amount,
                entity.Date
            );

            XMLDocManager.AddElementToRoot(_filePath, newReceiptElement);
        }

        public override ReceiptDetails GetById(int id)
        {
            XElement? element = XMLDocManager.GetElementById(_filePath, "Receipt", id);

            if (element == null)
            {
                return null;
            }

            return new ReceiptDetails
            {
                Id = (int)element.Attribute("Id"),
                ReceiptNumber = (string)element.Element("ReceiptNumber"),
                ReceiptFilePath = (string)element.Element("ReceiptFilePath"), // קריאת השדה החדש
                Amount = (decimal)element.Element("Amount"),
                Date = (DateTime)element.Element("Date")
            };
        }

        public override IEnumerable<ReceiptDetails> GetAll()
        {
            var elements = XMLDocManager.GetAllElements(_filePath, "Receipt");

            return elements.Select(element => new ReceiptDetails
            {
                Id = (int)element.Attribute("Id"),
                ReceiptNumber = (string)element.Element("ReceiptNumber"),
                ReceiptFilePath = (string)element.Element("ReceiptFilePath"), // קריאת השדה החדש
                Amount = (decimal)element.Element("Amount"),
                Date = (DateTime)element.Element("Date")
            }).ToList();
        }
    }
}