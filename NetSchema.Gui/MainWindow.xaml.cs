using System.IO;
using System.Windows;
using NetSchema.Gui.Schema;

namespace NetSchema.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly DirectoryInfo rootDirectory = new (@"C:\Users\mikec\RiderProjects\NetSchema-final\Data");
        private static FileInfo GetFile(params string[] parts) => new (Path.Combine(rootDirectory.FullName, Path.Combine(parts)));
        private static DirectoryInfo GetDirectory(params string[] parts) => new (Path.Combine(rootDirectory.FullName, Path.Combine(parts)));
        
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly AppViewModel viewModel;
  
        public MainWindow()
        {
            InitializeComponent();
            this.viewModel = new ();
            this.DataContext = this.viewModel;
        }
        
    }
}