using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CalculaterWithin.Annotations;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CalculaterWithin
{
    /// <summary>
    /// Логика взаимодействия для UserControl.xaml
    /// </summary>
    public partial class UserControl : System.Windows.Controls.UserControl
    {

        public UserControl()
        {
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            tb?.ScrollToHorizontalOffset(tb.MaxLines);
        }
    }


    public class UserControlLogic : DependencyObject
    {
        private Random _rnd = new Random();
        public static readonly string[] MathodsId = {"1", "2", "3", "+", "c", "<-", "7", "4", "5", "/", "-", "*", "6", "8", "9", "sin", "cos", "^", "+/-", "0", ".", "(", ")", "ln" };

        public static readonly DependencyProperty ControlWidthProperty = DependencyProperty.Register(nameof(ControlWidth), typeof(double), typeof(UserControlLogic),
            new PropertyMetadata(300.0));

        public double ControlWidth
        {
            get => (double)(GetValue(ControlWidthProperty) ?? default(double));
            set
            {
                CalcsButton.WithChangedOn(value);
                SetValue(ControlWidthProperty, value);
            }
        }

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            nameof(TextBoxText), typeof(string), typeof(UserControlLogic), new PropertyMetadata("0"));

        public string TextBoxText
        {
            get { return (string) GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }

        public static readonly DependencyProperty ControlHightProperty = DependencyProperty.Register(nameof(ControlHight), typeof(double), typeof(UserControlLogic),
            new PropertyMetadata(300.0));

        public double ControlHight
        {
            get => (double)(GetValue(ControlHightProperty) ?? default(double));
            set
            {
                CalcsButton.HighChangedOn(value);
                SetValue(ControlHightProperty, value);
            }
        }

        public ObservableCollection<CalcsButton> CalcsButtons { get; set; }

        public UserControlLogic()
        {
            //new CalcsButton(ControlWidth, ControlHight, "*", () => ControlWidth = _rnd.Next(100, 300)) //TODO I NEED TO SOLVE IT
            CalcsButtons = new ObservableCollection<CalcsButton>();
            foreach (var id in MathodsId)
            {
                CalcsButtons.Add(new CalcsButton(ControlWidth, ControlHight, id, (buttonText) =>
                {
                    TextBoxText = TextBoxText == null ? buttonText : TextBoxText + buttonText;
                    ControlWidth = _rnd.Next(100, 300);
                }));
            }
        }
    }

    public class CalcsButton : DependencyObject
    {
        static readonly int ColumnCount = 6;
        static readonly int CountCount = UserControlLogic.MathodsId.Length;
        static readonly int RowCount = (int)Math.Ceiling((double)CountCount / ColumnCount);

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CalcsButton), new PropertyMetadata(default(string)));
        public string ButtonText
        {
            get => (string) GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(CalcsButton), new PropertyMetadata(default(double)));
        public double ButtonWidth
        {
            get => (double)(GetValue(ButtonWidthProperty) ?? default(double));
            set => SetValue(ButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ButtonHightProperty = DependencyProperty.Register(nameof(ButtonHight), typeof(double), typeof(CalcsButton), new PropertyMetadata(default(double)));
        public double ButtonHight
        {
            get => (double)(GetValue(ButtonHightProperty) ?? default(double));
            set => SetValue(ButtonHightProperty, value);
        }

        public CalcCommand LogiCommand { get; set; }

        private static event Action<double> _withChanged;
        private static event Action<double> _highChanged;

        public static event Action<double> WithChanged
        {
            add => _withChanged += value;
            remove => _withChanged -= value;
        }
        public static event Action<double> HighChanged
        {
            add => _highChanged += value;
            remove => _highChanged -= value;
        }
        public static void WithChangedOn(double value) => _withChanged?.Invoke(value);
        public static void HighChangedOn(double value) => _highChanged?.Invoke(value);

        public CalcsButton(double controlWidth, double controlHight, string buttonText, Action<string> command)
        {
            WithChanged += (width) => ButtonWidth = width / ColumnCount - 3;
            HighChanged += (hight) => ButtonHight = hight / RowCount - 20;
            WithChangedOn(controlWidth);
            HighChangedOn(controlHight);
            ButtonText = buttonText;
            LogiCommand = new CalcCommand{ActionCommand = () => command(ButtonText)};
        }
    }

    public class CalcCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public Action ActionCommand;

        public void Execute(object parameter)
        {
            ActionCommand?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
