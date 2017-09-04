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

        public static readonly DependencyProperty PrintProperty = DependencyProperty.Register(nameof(Print), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand Print
        {
            get => (CalcCommand) GetValue(PrintProperty);
            set => SetValue(PrintProperty, value);
        }

        public static readonly DependencyProperty DelProperty = DependencyProperty.Register(nameof(Del), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand Del
        {
            get => (CalcCommand)GetValue(DelProperty);
            set => SetValue(DelProperty, value);
        }

        public ViewModelProgramm()
        {
            Print = new CalcCommand((text) => TextBoxText = TextBoxText == "0" ? text : TextBoxText += text);
            Del = new CalcCommand((text) => TextBoxText = String.IsNullOrEmpty(TextBoxText) || TextBoxText.Length == 1 ? "0" : TextBoxText.Substring(0, TextBoxText.Length - 1));
        }

        public static readonly DependencyProperty ExecutedPrintCommandProperty = DependencyProperty.Register(
            nameof(ExecutedPrintCommand), typeof(Action<object, ExecutedRoutedEventArgs>), typeof(ViewModelProgramm), new PropertyMetadata(default(Action<object, ExecutedRoutedEventArgs>)));

        public Action<object, ExecutedRoutedEventArgs> ExecutedPrintCommand
        {
            get => (Action<object, ExecutedRoutedEventArgs>) GetValue(ExecutedPrintCommandProperty);
            set => SetValue(ExecutedPrintCommandProperty, value);
        }
    }

    public class CalcCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public Action<string> ActionCommand;

        public void Execute(object parameter)
        {
            var text = parameter as string;
            if(text != null) ActionCommand?.Invoke(text);
        }

        public CalcCommand(Action<string> actionCommand)
        {
            ActionCommand = actionCommand;
        }
        public event EventHandler CanExecuteChanged;
    }
}

