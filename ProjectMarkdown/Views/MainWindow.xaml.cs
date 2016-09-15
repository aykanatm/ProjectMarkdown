﻿using System;
using System.Windows;
using WpfCodeTextbox;

namespace ProjectMarkdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var manager = new HighlightManager(AppDomain.CurrentDomain.BaseDirectory + "SyntaxConfig");
            codeTextBox.CurrentHighlighter = manager.Highlighters["MarkdownSyntax"];
        }
    }
}
