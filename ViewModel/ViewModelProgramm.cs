using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class ViewModelProgramm : DependencyObject
    {
        private Model.Calculate _calculator;

        public static readonly DependencyProperty TextBoxTextProperty = DependencyProperty.Register(nameof(TextBoxText), typeof(string), typeof(ViewModelProgramm), new PropertyMetadata("0"));
        public string TextBoxText
        {
            get => (string)GetValue(TextBoxTextProperty);
            set => SetValue(TextBoxTextProperty, value);
        }

        public static readonly DependencyProperty GetResaultProperty = DependencyProperty.Register(nameof(GetResault), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand GetResault
        {
            get => (CalcCommand) GetValue(GetResaultProperty);
            set => SetValue(GetResaultProperty, value);
        }

        public static readonly DependencyProperty DelProperty = DependencyProperty.Register(nameof(Del), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand Del
        {
            get => (CalcCommand)GetValue(DelProperty);
            set => SetValue(DelProperty, value);
        }

        public static readonly DependencyProperty CalctProperty = DependencyProperty.Register(nameof(Calc), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand Calc
        {
            get => (CalcCommand)GetValue(CalctProperty);
            set => SetValue(CalctProperty, value);
        }


        public static readonly DependencyProperty UnoMinProperty = DependencyProperty.Register(nameof(UnoMin), typeof(CalcCommand), typeof(ViewModelProgramm), new PropertyMetadata(default(CalcCommand)));

        public CalcCommand UnoMin
        {
            get => (CalcCommand) GetValue(UnoMinProperty);
            set => SetValue(UnoMinProperty, value);
        }
        public ViewModelProgramm()
        {
            _calculator = new Calculate();
            Calc = new CalcCommand((text) => TextBoxText = TextBoxText == "0" ? text : TextBoxText += text);
            Del = new CalcCommand((text) => TextBoxText = String.IsNullOrEmpty(TextBoxText) || TextBoxText.Length == 1 ? "0" : TextBoxText.Substring(0, TextBoxText.Length - 1));
            UnoMin = new CalcCommand((text) =>
            {
                if (!String.IsNullOrEmpty(TextBoxText))
                {
                    TextBoxText = TextBoxText.First() == '-' ? TextBoxText.Substring(1, TextBoxText.Length - 1) : "-" + TextBoxText;
                }
            });
            GetResault = new CalcCommand((text) => TextBoxText = _calculator.Start(TextBoxText));
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
            if(parameter is string text) ActionCommand?.Invoke(text);
        }

        public CalcCommand(Action<string> actionCommand)
        {
            ActionCommand = actionCommand;
        }
        public event EventHandler CanExecuteChanged;
    }
}

