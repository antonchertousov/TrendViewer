using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace TrendViewer.Helpers
{
    /// <summary>
    /// Helper class for open the file dialog and get the selected file name
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OpenFileDialogHelper : IOpenFileDialogHelper
    {
        public string OpenDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}