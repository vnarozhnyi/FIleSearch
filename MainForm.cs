using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace FileSearchApp
{
    public partial class MainForm : Form
    {
        private static object locker = new object();

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
            }

            if (textBox1.Text != string.Empty && Directory.Exists(textBox1.Text))
            {
                var threadSearch = new Thread(FileSearch);
                threadSearch.Start(textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && Directory.Exists(textBox1.Text))
            {
                var threadWrite = new Thread(FileSearchView);
                threadWrite.Start(textBox1.Text);

                var threadView = new Thread(FileWrite);
                threadView.Start(textBox1.Text);
            }
        }

        private void FileSearch(object directory)
        {
            FileSearchFunction(directory);
        }

        private FileInfo[] FileSearchFunction(object directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            var acquiredLock = false;
            try
            {
                Monitor.Enter(locker, ref acquiredLock);
                var search = new RecursiveSearch();
                var fileInfo = search.FileSearch(directory);
                return fileInfo;
                
            }
            finally
            {
                Monitor.Pulse(locker);
                if (acquiredLock) Monitor.Exit(locker);
            }
        }

        private void FileSearchView(object directory)
        {
            var fileInfos = FileSearchFunction(directory);
            foreach (var info in fileInfos)
                Invoke(new ThreadStart(delegate
                {
                    var node = new TreeNode();
                    var nodeName = new TreeNode("Name: " + info.FullName);
                    var nodeCreationTime = new TreeNode("Creation Time: " + info.CreationTime.Date);
                    var nodeLastWriteTime = new TreeNode("Last Write Time: " + info.LastWriteTime.Date);
                    var nodeLastAccessTime = new TreeNode("Last Access Time:" + info.LastAccessTime.Date);
                    var nodeAttributes = new TreeNode("Attributes: " + info.Attributes);
                    var nodeLength = new TreeNode("Length: " + info.Length);
                    var nodeOwner =
                        new TreeNode("Owner: " + info.GetAccessControl().GetOwner(typeof(NTAccount)).Value);

                    TreeNode[] nodeAccessRights = { };
                    foreach (FileSystemAccessRule rule in info.GetAccessControl()
                        .GetAccessRules(true, true, typeof(NTAccount)))
                    {
                        var nodeFullControl =
                            new TreeNode(
                                (rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl
                                    ? "Full Control +"
                                    : "Full Control -");
                        var nodeWrite =
                            new TreeNode((rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write
                                ? "Write +"
                                : "Write -");
                        var nodeRead =
                            new TreeNode((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read
                                ? "Read +"
                                : "Read -");
                        var nodeDelete =
                            new TreeNode((rule.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete
                                ? "Delete +"
                                : "Delete -");
                        var nodeModify =
                            new TreeNode((rule.FileSystemRights & FileSystemRights.Modify) == FileSystemRights.Modify
                                ? "Modify +"
                                : "Modify -");
                        nodeAccessRights = new[] {nodeFullControl, nodeWrite, nodeRead, nodeDelete, nodeModify};
                    }

                    TreeNode[] array =
                    {
                        nodeName, nodeCreationTime, nodeLastWriteTime, nodeLastAccessTime, nodeAttributes,
                        nodeLength, nodeOwner
                    };
                    node = new TreeNode("Info", array);
                    node.Nodes.Add(new TreeNode("Access Rights", nodeAccessRights));
                    treeView1.Nodes.Add(node);
                    treeView1.ExpandAll();
                }));
        }

        private void FileWrite(object directory)
        {
            Invoke(new ThreadStart(delegate
            {
                var xml = new EntryXml();
                xml.WritingToXml(directory);
            }));
        }
    }
}
