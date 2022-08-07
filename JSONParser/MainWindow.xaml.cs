using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JSONParser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        #region Window Title Bar

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                this.Top = 0;
            }

            this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            TextBar.SelectAll();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            TextBar.Copy();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            TextBar.Paste();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TextBar.Clear();
        }

        private void RwsButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = TextBar.Text;
            TextBar.Text = Lexer.RemoveWhiteSpaces(inputText);
        }
    }
}
