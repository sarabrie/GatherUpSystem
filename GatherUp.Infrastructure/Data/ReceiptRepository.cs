using System;
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
            string originalPath = entity.ReceiptNumber;
            string fileName = Path.GetFileName(originalPath);
            string newPathInProject = Path.Combine(_receiptFilesFolder, $"{entity.Id}_{fileName}");

            if (File.Exists(originalPath))
            {
                File.Copy(originalPath, newPathInProject, overwrite: true);
            }

            XElement newReceiptElement = new XElement("Receipt",
                new XAttribute("Id", entity.Id),
                new XElement("ReceiptNumber", newPathInProject),
                new XElement("Amount", entity.Amount),
                new XElement("Date", entity.Date.ToString("yyyy-MM-ddTHH:mm:ss"))
            );

            XMLDocManager.AddElementToRoot(_filePath, newReceiptElement);
        }

     
        public override ReceiptDetails GetById(int id)
        {
            XDocument doc = XMLDocManager.LoadXmlFile(_filePath);

            XElement element = doc.Root?
                .Elements("Receipt")
                .FirstOrDefault(x => (int?)x.Attribute("Id") == id);

            if (element == null)
            {
                return null;
            }

            return new ReceiptDetails
            {
                Id = (int)element.Attribute("Id"),
                ReceiptNumber = (string)element.Element("ReceiptNumber"),
                Amount = (decimal)element.Element("Amount"),
                Date = (DateTime)element.Element("Date")
            };
        }

       
        public override void Update(ReceiptDetails entity)
        {
            throw new InvalidOperationException("Fatal error: Unable to edit or update a receipt after it was created!");
        }

     
        public override void Delete(int id)
        {
            throw new InvalidOperationException("Fatal error: A receipt cannot be deleted after it has been created!");
        }
    }
}