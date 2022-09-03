using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace JSONParser
{
    public partial class MainWindow : Window
    {
        private int _counter;

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
            RichTextBar.SelectAll();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextBar.Copy();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextBar.Paste();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextBar.Document.Blocks.Clear();
        }

        private void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnteredText(sender);
        }

        private void RwsButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnteredText(sender);
        }

        private void HighLightButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnteredText(sender);
        }

        private void TreeViewButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEnteredText(sender);
        }

        private void ProcessEnteredText(object sender)
        {
            string text = (sender == HighLightButton) ? RichTextBar.GetText() : RichTextBar.GetText().Replace(Environment.NewLine, " ");
            Lexer lexer = new(text);
            List<Lexem> lexems = lexer.MoveNext();
            Formatter formatter = new(lexems);
            if (formatter.CheckIfTokenListValid())
            {
                if (sender == TreeViewButton)
                {
                    lexems = formatter.RemoveWhiteSpaces();
                    AddItemsToTreeView(lexems);
                }
                else
                {
                    RichTextBar.Document.Blocks.Clear();
                    lexems = (sender == HighLightButton) ? lexems : ChangeJsonFormat(sender, formatter, lexems);
                    foreach (Lexem lexem in lexems)
                    {
                        PaintJson(lexem);
                    }
                }
            }
            else if (sender == FormatButton || sender == RwsButton)
            {
                MessageBox.Show("Error");
            }
        }

        private void AddItemsToTreeView(List<Lexem> lexems)
        {
            _counter = 0;
            TreeViewBar.Items.Clear();
            while (_counter < lexems.Count)
            {
                TreeViewItem objectBrace = new();
                objectBrace.Header = lexems[_counter].Value;
                TreeViewBar.Items.Add(objectBrace);
                _counter++;
                if (_counter < lexems.Count - 1)
                {                    
                    AddObjectToTreeView(lexems, objectBrace);
                }
            }
        }

        private void AddObjectToTreeView(List<Lexem> lexems, TreeViewItem item)
        {
            bool objectAddedToThreeView = false;
            while (_counter < lexems.Count)
            {
                if (lexems[_counter].Key == (int)Tokens.Key)
                {
                    TreeViewItem objectItem = new();
                    objectItem.Header = lexems[_counter].Value + lexems[_counter + 1].Value + lexems[_counter + 2].Value;
                    _counter += 3;
                    item.Items.Add(objectItem);
                    if (lexems[_counter - 1].Key == (int)Tokens.OpenObjectBrace)
                    {
                        AddObjectToTreeView(lexems, objectItem);
                        objectAddedToThreeView = true;
                    }
                    else if (lexems[_counter - 1].Key == (int)Tokens.OpenArrayBrace)
                    {
                        AddArrayToTreeView(lexems, objectItem);
                    }
                }
                else if (lexems[_counter].Key == (int)Tokens.Comma)
                {
                    _counter++;
                }
                else if (lexems[_counter].Key == (int)Tokens.CloseArrayBrace)
                {
                    TreeViewItem objectItem = new();
                    objectItem.Header = lexems[_counter].Value;
                    _counter++;
                    item.Items.Add(objectItem);
                }
                else if (lexems[_counter].Key == (int)Tokens.CloseObjectBrace)
                {
                    if (objectAddedToThreeView)
                    {
                        TreeViewItem objectItem = new();
                        objectItem.Header = lexems[_counter].Value;
                        _counter++;
                        item.Items.Add(objectItem);
                        objectAddedToThreeView = false;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void AddArrayToTreeView(List<Lexem> lexems, TreeViewItem item)
        {
            while (_counter < lexems.Count)
            {
                if (lexems[_counter].Key == (int)Tokens.Comma)
                {
                    _counter++;
                }
                else if (lexems[_counter].Key == (int)Tokens.CloseArrayBrace)
                {
                    return;
                }
                else
                {
                    TreeViewItem ArrayItem = new();
                    ArrayItem.Header = lexems[_counter].Value;
                    _counter++;
                    item.Items.Add(ArrayItem);
                    if (lexems[_counter - 1].Key == (int)Tokens.OpenObjectBrace)
                    {
                        AddObjectToTreeView(lexems, ArrayItem);
                    }
                }
            }
        }

        private List<Lexem> ChangeJsonFormat(object sender, Formatter formatter, List<Lexem> lexems)
        {
            if (sender == FormatButton)
            {
                lexems = formatter.GetFormattedTokenList();
            }
            else if (sender == RwsButton)
            {
                lexems = formatter.RemoveWhiteSpaces();
            }

            return lexems;
        }

        private void PaintJson(Lexem lexem)
        {
            TextRange rangeOfTokenList = new(RichTextBar.Document.ContentEnd, RichTextBar.Document.ContentEnd);
            rangeOfTokenList.Text = lexem.Value;
            if (lexem.Key == (int)Tokens.Key)
            {
                rangeOfTokenList.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightBlue);
            }
            else if (lexem.Key == (int)Tokens.String)
            {
                rangeOfTokenList.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.SandyBrown);
            }
            else if (lexem.Key == (int)Tokens.IntOrDouble)
            {
                rangeOfTokenList.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightGreen);
            }
            else if (lexem.Key == (int)Tokens.BoolOrNull)
            {
                rangeOfTokenList.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
            }
            else
            {
                rangeOfTokenList.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.LightGray);
            }
        }
    }
}
