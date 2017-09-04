using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public class ViewModelProgramm : DependencyObject
    {
        public static readonly DependencyProperty TextBoxTextProperty = DependencyProperty.Register(
            nameof(TextBoxText), typeof(string), typeof(ViewModelProgramm), new PropertyMetadata("0"));

        public string TextBoxText
        {
            get => (string)GetValue(TextBoxTextProperty);
            set => SetValue(TextBoxTextProperty, value);
        }

        public static readonly DependencyProperty PrintProperty = DependencyProperty.Register(
            nameof(Print), typeof(RoutedCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(RoutedCommand)));

        public RoutedCommand Print
        {
            get => (RoutedCommand) GetValue(PrintProperty);
            set => SetValue(PrintProperty, value);
        }

        public static readonly DependencyProperty PrintBildProperty = DependencyProperty.Register(
            nameof(PrintBild), typeof(CommandBinding), typeof(ViewModelProgramm), new PropertyMetadata(default(CommandBinding)));

        public CommandBinding PrintBild
        {
            get => (CommandBinding) GetValue(PrintBildProperty);
            set => SetValue(PrintBildProperty, value);
        }
        public ViewModelProgramm()
        {
            Print = new RoutedCommand();
            ExecutedPrintCommand += ExecutedPrintCommandHandler;
            PrintBild = new CommandBinding(Print);
        }

        public static readonly DependencyProperty ExecutedPrintCommandProperty = DependencyProperty.Register(
            nameof(ExecutedPrintCommand), typeof(Action<object, ExecutedRoutedEventArgs>), typeof(ViewModelProgramm), new PropertyMetadata(default(Action<object, ExecutedRoutedEventArgs>)));

        public Action<object, ExecutedRoutedEventArgs> ExecutedPrintCommand
        {
            get => (Action<object, ExecutedRoutedEventArgs>) GetValue(ExecutedPrintCommandProperty);
            set => SetValue(ExecutedPrintCommandProperty, value);
        }

        private void ExecutedPrintCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            TextBoxText = e.RoutedEvent.Name;
        }
    }

    public class CalcCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        //public Action ActionCommand;

        public void Execute(object parameter)
        {
            //ActionCommand?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
