using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace intern.Views
{
    /// <summary>
    /// Логика взаимодействия для MaterialsEditWindow.xaml
    /// </summary>
    public partial class MaterialsEditWindow : Window
    {
        public MaterialsEditWindow()
        {
            InitializeComponent();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            var text = tb.Text.Insert(tb.CaretIndex, e.Text);

            if (!decimal.TryParse(text.Replace('.', ','), out _))
                e.Handled = true;
        }
    }
}
