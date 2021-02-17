using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;

namespace FileSearchApp
{
    internal class EntryXml
    {
        public void WritingToXml(object dir)
        {
            var search = new RecursiveSearch();
            var fi = search.FileSearch(dir);

            var xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("info");
            xmlDoc.AppendChild(rootNode);
            foreach (var node in fi)
            {
                XmlNode nameNode = xmlDoc.CreateElement("name");
                nameNode.InnerText = node.Name;
                rootNode.AppendChild(nameNode);

                XmlNode creationTimeNode = xmlDoc.CreateElement("creationtime");
                creationTimeNode.InnerText = node.CreationTime.Date.ToString();
                rootNode.AppendChild(creationTimeNode);

                XmlNode lastWriteTimeNode = xmlDoc.CreateElement("lastwritetime");
                lastWriteTimeNode.InnerText = node.LastWriteTime.Date.ToString();
                rootNode.AppendChild(lastWriteTimeNode);

                XmlNode lastAccessTimeNode = xmlDoc.CreateElement("lastaccesstime");
                lastAccessTimeNode.InnerText = node.LastAccessTime.Date.ToString();
                rootNode.AppendChild(lastAccessTimeNode);

                XmlNode attributesNode = xmlDoc.CreateElement("attributes");
                attributesNode.InnerText = node.Attributes.ToString();
                rootNode.AppendChild(attributesNode);

                XmlNode lengthNode = xmlDoc.CreateElement("length");
                lengthNode.InnerText = node.Length.ToString();
                rootNode.AppendChild(lengthNode);

                XmlNode ownerNode = xmlDoc.CreateElement("owner");
                ownerNode.InnerText = node.GetAccessControl().GetOwner(typeof(NTAccount)).Value;
                rootNode.AppendChild(ownerNode);

                foreach (FileSystemAccessRule rule in node.GetAccessControl()
                    .GetAccessRules(true, true, typeof(NTAccount)))
                {
                    XmlNode fullControlNode = xmlDoc.CreateElement("fullcontrol");
                    fullControlNode.InnerText =
                        (rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl
                            ? "Full Control +"
                            : "Full Control -";
                    rootNode.AppendChild(fullControlNode);

                    XmlNode writeNode = xmlDoc.CreateElement("write");
                    writeNode.InnerText = (rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write
                        ? "Write +"
                        : "Write -";
                    rootNode.AppendChild(writeNode);

                    XmlNode readNode = xmlDoc.CreateElement("read");
                    readNode.InnerText = (rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read
                        ? "Read +"
                        : "Read -";
                    rootNode.AppendChild(readNode);

                    XmlNode deleteNode = xmlDoc.CreateElement("delete");
                    deleteNode.InnerText = (rule.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete
                        ? "Delete +"
                        : "Delete -";
                    rootNode.AppendChild(deleteNode);

                    XmlNode modifyNode = xmlDoc.CreateElement("modify");
                    modifyNode.InnerText = (rule.FileSystemRights & FileSystemRights.Modify) == FileSystemRights.Modify
                        ? "Modify +"
                        : "Modify -";
                    rootNode.AppendChild(modifyNode);

                    break;
                }
            }

            xmlDoc.Save("File Info.xml");
        }
    }
}
