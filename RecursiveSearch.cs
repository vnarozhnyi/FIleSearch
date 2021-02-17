using System;
using System.IO;
using System.Windows.Forms;

namespace FileSearchApp
{
    internal class RecursiveSearch
    {
        public FileInfo[] FileSearch(object dir)
        {
            var dirInfo = new DirectoryInfo((string) dir);
            DirectoryInfo[] subDir = null;

            try
            {
                subDir = dirInfo.GetDirectories();
            }

            catch (UnauthorizedAccessException)
            {
                subDir = new DirectoryInfo[0];
            }

            if (subDir != null)
                for (var i = 0; i < subDir.Length; ++i)
                    FileSearch(subDir[i].FullName);
            try
            {
                var fi = dirInfo.GetFiles();
                return fi;
            }
            catch (UnauthorizedAccessException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }

            return null;
        }
    }
}
