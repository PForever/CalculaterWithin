using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public class Calculate
    {
        public string Start(string result)
        {
            //double dbX = -0.395;

            try
            {
                var expressionCalc = new Calc(result);
                result = expressionCalc.CalcResult().ToString();
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            //Console.WriteLine(expressionCalc.OperandResult.Uncover(NameOfOferand.Add, null).Print);

            //var str2 = expressionCalc?.PrintExpression();
            //Console.WriteLine(new Calc(str2.Replace(',', '.')).CalcResult(dbX));
            return result;
        }
    }

    class Calc
    {
        private static readonly char UnoMinus = '-';
        private static readonly char OpenBracket = '(';
        private static readonly char CloseBracket = ')';
        private static readonly char CharVariable = 'x';
        private enum FuncEnum { Nun, Sin, Cos, Log, Min };

        private int _braketsNumber;

        public IOperand OperandResult;

        public Calc(string startValue)
        {
            _braketsNumber = 0;
            var node = new LinkedList<char>(startValue.Where(ch => ch != ' ')).First;
            OperandResult = StrResult(default(char), ref node, out char value, out int preor);
            //Result = StrResult(default(char), ref node, out char value, out int preor).SetIntValue(xValue);
            if (_braketsNumber != 0) throw new Exception("Не закрыто " + _braketsNumber + " cкобок");
        }

        public double CalcResult(double xValue = 0) => OperandResult.SetIntValue(xValue);
        public string PrintExpression() => OperandResult.Print;


        private IOperand StrResult(char preOperand, ref LinkedListNode<char> sValue, out char lastOperand, out int lastOperandPreor)
        {
            if (sValue == null)
            {
                lastOperand = default(char);
                lastOperandPreor = -1;
                return null;
            }
            var a = FindValue(ref sValue);
            var operand = FindOperand(ref sValue, out int operandPreor);

            if (operand == CloseBracket || preOperand != default(char) && operandPreor < 2)
            {
                lastOperand = operand;
                lastOperandPreor = operandPreor;
                return a;
            }

            do
            {
                if (operandPreor == 0)
                {
                    var m = StrResult(operand, ref sValue, out lastOperand, out lastOperandPreor);
                    if (m == null && operand == default(char)) return a;
                    var p = DuoOperand(a, m, operand);
                    return p;
                }

                var b = FindValue(ref sValue);
                var nextOperand = FindOperand(ref sValue, out int nextOperandPreor);


                if (operandPreor >= nextOperandPreor)
                {
                    if (b == null && operand == default(char)) break;
                    a = DuoOperand(a, b, operand);
                    operand = nextOperand;
                    operandPreor = nextOperandPreor;
                }
                else
                {
                    a = DuoOperand(a,
                        DuoOperand(b, StrResult(operand, ref sValue, out char tempOperand, out operandPreor),
                            nextOperand), operand);
                    operand = tempOperand;
                }
            } while (operand != CloseBracket && sValue != null);
            lastOperand = default(char);
            lastOperandPreor = -1;
            return a;
        }

        private char FindOperand(ref LinkedListNode<char> charter, out int intPreor)
        {
            if (charter == null)
            {
                intPreor = 0;
                return default(char);
            }
            switch (charter.Value)
            {
                case '+':
                case '-':
                    charter = charter.Next ?? throw new Exception("Отсутствует слагаемое в конце");
                    intPreor = 0;
                    return charter.Previous.Value;
                case '*':
                case '/':

                    charter = charter.Next ?? throw new Exception("Отсутствует множитель в конце");
                    intPreor = 1;
                    return charter.Previous.Value;
                case '^':
                    charter = charter.Next ?? throw new Exception("Отсутствует показатель в конце");
                    intPreor = 2;
                    return charter.Previous.Value;
                case ')':
                    if (_braketsNumber == 0) throw new Exception("Лишня скобка");
                    _braketsNumber--;
                    var temp = charter.Value;
                    charter = charter.Next;
                    intPreor = -1;
                    return temp;
                default:
                    intPreor = 1;
                    return '*';
            }
        }
        private IOperand FindValue(ref LinkedListNode<char> charter)
        {
            if (charter == null)
            {
                return null;
            }
            //if(charter == null) throw new Exception("Закрывайте скобки");
            var blUno1 = false;
            var blUno2 = false;
            IOperand aValue;
            var strNumbs = "";
            if (charter.Value == UnoMinus)
            {
                blUno1 = true;
                charter = charter.Next;
            }
            FindFunc(ref charter, out FuncEnum funcValue);
            if (charter.Value == OpenBracket)
            {
                charter = charter.Next;
                _braketsNumber++;
                aValue = StrResult(default(char), ref charter, out char tempOper, out int tempPreor);
            }
            else
            {

                if (charter.Value == UnoMinus)
                {
                    blUno2 = true;
                    charter = charter.Next;
                }

                while (charter != null && NumbersPredicate(charter.Value))
                {
                    strNumbs += charter.Value;
                    charter = charter.Next;
                }
                if (charter?.Value == CharVariable)
                {
                    aValue = strNumbs == "" ? (IOperand)new Variable() : new Umn(new Const(Double.Parse(strNumbs.Replace('.', ','))), new Variable());
                    charter = charter.Next;
                }
                else
                {
                    if (strNumbs == "") throw new Exception("не указан аргумент функции");
                    aValue = new Const(strNumbs == "" ? 1 : Double.Parse(strNumbs.Replace('.', ',')));
                }
                if (blUno2) aValue = UnoOperand(aValue, FuncEnum.Min);
            }
            aValue = UnoOperand(aValue, funcValue);
            if (blUno1) aValue = UnoOperand(aValue, FuncEnum.Min);
            return aValue;
        }

        private void FindFunc(ref LinkedListNode<char> nodeValue, out FuncEnum funcValue)
        {
            funcValue = FuncEnum.Nun;
            var strFunc = "";
            while (nodeValue != null && !NumbersPredicate(nodeValue.Value) && nodeValue.Value != UnoMinus && nodeValue.Value != OpenBracket && nodeValue.Value != CharVariable)
            {
                strFunc += nodeValue.Value;
                nodeValue = nodeValue.Next;
            }

            switch (strFunc)
            {
                case "Sin":
                case "sin":
                    {
                        funcValue = FuncEnum.Sin;
                        break;
                    }
                case "Cos":
                case "cos":
                    {
                        funcValue = FuncEnum.Cos;
                        break;
                    }
                case "Ln":
                case "ln":
                    {
                        funcValue = FuncEnum.Log;
                        break;
                    }
                default:
                    {
                        if (strFunc.Length > 0) throw new Exception("Введён неверный оператор");
                        break;
                    }
            }
        }


        IOperand DuoOperand(IOperand a, IOperand b, char operand)
        {
            IOperand value;
            switch (operand)
            {
                case '+':
                    value = new Add(a, b);
                    break;
                case '-':
                    value = new Min(a, b);
                    break;
                case '*':
                    value = new Umn(a, b);
                    break;
                case '/':
                    value = new Del(a, b);
                    break;
                case '^':
                    value = new Up(a, b);
                    break;
                default:
                    throw new Exception("Некорректная запись");
            }
            return value;
        }

        IOperand UnoOperand(IOperand a, FuncEnum operand)
        {
            IOperand value;
            switch (operand)
            {
                case FuncEnum.Nun:
                    value = a;
                    break;
                case FuncEnum.Sin:
                    value = new Sin(a);
                    break;
                case FuncEnum.Cos:
                    value = new Cos(a);
                    break;
                case FuncEnum.Log:
                    value = new Ln(a);
                    break;
                case FuncEnum.Min:
                    value = new UnoMin(a);
                    break;
                default:
                    throw new Exception("Некорректная запись");
            }
            return value;
        }

        bool NumbersPredicate(char ch) => (ch >= '0') && (ch <= '9') || (ch == '.');
    }

    enum NameOfOferand
    {
        Const, Variable, Min, Add, Umn, Del, Up, Func
    }
    interface IOperand
    {
        NameOfOferand Name { get; }
        double SetIntValue(double value);
        string Print { get; }
        IOperand Uncover(NameOfOferand name, IOperand deel);

    }

    class Const : IOperand
    {
        private readonly double _doubValue;

        public Const(double value)
        {
            Name = NameOfOferand.Const;
            _doubValue = value;
        }

        public double SetIntValue(double value) => _doubValue;

        public string Print => _doubValue.ToString();
        public NameOfOferand Name { get; }
        public IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var deelCov = deel?.Uncover(NameOfOferand.Add, null);
            switch (name)
            {
                case NameOfOferand.Min:
                    return new UnoMin(this);
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Del(this, deelCov);
                case NameOfOferand.Umn:
                    return new Umn(this, deelCov);
                case NameOfOferand.Up:
                    return new Up(this, deelCov);//TODO binom if const
                default:
                    return new Add(this, deelCov);
            }
        }
    }

    class Variable : IOperand
    {
        private double _doubValue;

        public Variable()
        {
            Name = NameOfOferand.Variable;
        }
        public double SetIntValue(double value)
        {
            _doubValue = value;
            return _doubValue;
        }

        public string Print => "x";
        public NameOfOferand Name { get; }
        public IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var deelCov = deel?.Uncover(NameOfOferand.Add, null);
            switch (name)
            {
                case NameOfOferand.Min:
                    return new UnoMin(this);
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Del(this, deelCov);
                case NameOfOferand.Umn:
                    return new Umn(this, deelCov);
                case NameOfOferand.Up:
                    return new Up(this, deelCov);//TODO binom if const
                default:
                    return new Add(this, deelCov);
            }
        }
    }

    abstract class UnoMethod : IOperand
    {
        protected readonly IOperand _value1;
        private double _resultValue;
        //public IOperand Uncover => this; //TODO ??????

        protected UnoMethod(IOperand a, NameOfOferand name)
        {
            _value1 = a;
            Name = name;
        }

        //public abstract IOperand Uncover(NameOfOferand name, IOperand deel);
        protected abstract double Operation(double a);
        protected abstract string PrintOperation(string a);
        public double SetIntValue(double value)
        {
            _resultValue = Operation(_value1.SetIntValue(value));
            return _resultValue;
        }


        public string Print => PrintOperation(_value1.Print);
        public NameOfOferand Name { get; }
        public IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var temp = _value1.Uncover(NameOfOferand.Add, null);
            var deelCov = deel?.Uncover(NameOfOferand.Add, null);
            switch (name)
            {
                case NameOfOferand.Min:
                    return new UnoMin(temp);
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Del(temp, deelCov);
                case NameOfOferand.Umn:
                    return new Umn(temp, deelCov);
                case NameOfOferand.Up:
                    return new Up(temp, deelCov);//TODO binom if const
                default:
                    return new Add(temp, deelCov);
            }
        }
    }

    class Sin : UnoMethod
    {
        public Sin(IOperand a) : base(a, NameOfOferand.Func)
        {
        }

        protected override double Operation(double a) => Math.Sin(a);
        protected override string PrintOperation(string a) => "Sin(" + a + ")";

    }

    class Cos : UnoMethod
    {
        public Cos(IOperand a) : base(a, NameOfOferand.Func)
        {
        }

        protected override double Operation(double a) => Math.Cos(a);
        protected override string PrintOperation(string a) => "cos(" + a + ")";
    }

    class Ln : UnoMethod
    {
        public Ln(IOperand a) : base(a, NameOfOferand.Func)
        {
        }

        protected override double Operation(double a) => Math.Log(a);
        protected override string PrintOperation(string a) => "log(" + a + ")";

    }

    class UnoMin : UnoMethod
    {
        public UnoMin(IOperand a) : base(a, NameOfOferand.Func)
        {
        }

        protected override double Operation(double a) => -a;
        protected override string PrintOperation(string a) => "-" + a;
    }

    abstract class Method : IOperand
    {
        protected readonly IOperand _value1;
        protected readonly IOperand _value2;
        private double _resultValue;

        protected Method(IOperand a, IOperand b, NameOfOferand name)
        {
            Name = name;
            _value1 = a;
            _value2 = b;
        }

        public abstract double Operation(double a, double b);
        public abstract string PrintOperation(string a, string b);
        public double SetIntValue(double value)
        {
            _resultValue = Operation(_value1.SetIntValue(value), _value2.SetIntValue(value));
            return _resultValue;
        }



        public string Print => PrintOperation(_value1.Print, _value2.Print);
        public NameOfOferand Name { get; }
        public abstract IOperand Uncover(NameOfOferand name, IOperand deel);
    }

    class Min : Method
    {
        public Min(IOperand a, IOperand b) : base(a, b, NameOfOferand.Min)
        {
        }

        public override double Operation(double a, double b) => (a - b);
        public override string PrintOperation(string a, string b) => "(" + a + ") - (" + b + ")";

        public override IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var temp1 = _value1.Uncover(NameOfOferand.Add, null);
            var temp2 = _value2.Uncover(NameOfOferand.Min, null); //TODO whithout temp (-)
            switch (name)
            {
                case NameOfOferand.Min:
                    return new Add(temp1.Uncover(NameOfOferand.Min, null), temp2.Uncover(NameOfOferand.Min, null));
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Add(temp1.Uncover(NameOfOferand.Del, deel), temp2.Uncover(NameOfOferand.Del, deel));
                case NameOfOferand.Umn:
                    return new Add(new Add(temp1.Uncover(NameOfOferand.Umn, deel), deel), new Add(temp2.Uncover(NameOfOferand.Umn, deel), deel));
                case NameOfOferand.Up:
                    return new Up(new Add(temp1, temp2), deel);//TODO binom if const
                default:
                    return new Add(temp1, temp2);
            }
        }
    }

    class Add : Method
    {
        public Add(IOperand a, IOperand b) : base(a, b, NameOfOferand.Add)
        {
        }

        public override double Operation(double a, double b) => a + b;
        public override string PrintOperation(string a, string b) => "(" + a + ") + (" + b + ")";
        public override IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var temp1 = _value1.Uncover(NameOfOferand.Add, null);
            var temp2 = _value2.Uncover(NameOfOferand.Add, null); //TODO whithout temp (-)
            switch (name)
            {
                case NameOfOferand.Min:
                    return new Add(temp1.Uncover(NameOfOferand.Min, null), temp2.Uncover(NameOfOferand.Min, null));
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Add(temp1.Uncover(NameOfOferand.Del, deel), temp2.Uncover(NameOfOferand.Del, deel));
                case NameOfOferand.Umn:
                    return new Add(new Add(temp1.Uncover(NameOfOferand.Umn, deel), deel), new Add(temp2.Uncover(NameOfOferand.Umn, deel), deel));
                case NameOfOferand.Up:
                    return new Up(new Add(temp1, temp2), deel);//TODO binom if const
                default:
                    return new Add(temp1, temp2);
            }
        }
    }

    class Umn : Method
    {
        public Umn(IOperand a, IOperand b) : base(a, b, NameOfOferand.Umn)
        {
        }

        public override double Operation(double a, double b) => a * b;
        public override string PrintOperation(string a, string b) => "(" + a + ") * (" + b + ")";
        public override IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            //var result = new Add(_value1.Uncover(NameOfOferand.Add), _value2.Uncover(NameOfOferand.Min);
            var temp1 = _value1.Uncover(NameOfOferand.Add, null);
            var temp2 = _value2.Uncover(NameOfOferand.Add, null); //TODO whithout temp (-)
            //var temp = _value1.Uncover(NameOfOferand.Umn, _value2);
            switch (name)
            {
                case NameOfOferand.Min:
                    return temp2.Uncover(NameOfOferand.Min, null).Uncover(NameOfOferand.Umn, temp1); //Add(new UnoMin(temp1), new UnoMin(temp2));
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return temp2.Uncover(NameOfOferand.Del, deel).Uncover(NameOfOferand.Umn, temp1);
                case NameOfOferand.Umn:
                    return temp2.Uncover(NameOfOferand.Umn, deel).Uncover(NameOfOferand.Umn, deel);
                case NameOfOferand.Up:
                    return new Umn(temp1.Uncover(NameOfOferand.Up, deel), temp2.Uncover(NameOfOferand.Up, deel)); //new Up(temp1, temp2);//TODO binom if const
                default:
                    return temp1.Uncover(NameOfOferand.Umn, temp2);
            }
        }
    }

    class Del : Method
    {
        public Del(IOperand a, IOperand b) : base(a, b, NameOfOferand.Del)
        {
        }

        public override double Operation(double a, double b)
        {
            if (Math.Abs(b) < 0.000000000000000000000001) throw new Exception("не делите на ноль");
            return a / b;
        }
        public override string PrintOperation(string a, string b) => "(" + a + ") / (" + b + ")";
        public override IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            var temp1 = _value1.Uncover(NameOfOferand.Add, null);
            var temp2 = _value2.Uncover(NameOfOferand.Add, null); //TODO whithout temp (-)
            //var temp = _value1.Uncover(NameOfOferand.Umn, _value2);
            switch (name)
            {
                case NameOfOferand.Min:
                    return temp1.Uncover(NameOfOferand.Min, null).Uncover(NameOfOferand.Del, temp2); //Add(new UnoMin(temp1), new UnoMin(temp2));
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return temp1.Uncover(NameOfOferand.Del, temp2.Uncover(NameOfOferand.Umn, deel));
                case NameOfOferand.Umn:
                    return temp1.Uncover(NameOfOferand.Umn, deel).Uncover(NameOfOferand.Del, temp2);
                case NameOfOferand.Up:
                    return temp1.Uncover(NameOfOferand.Up, deel)
                        .Uncover(NameOfOferand.Del, temp2.Uncover(NameOfOferand.Up, deel));//new Up(temp, deel.Uncover(NameOfOferand.Add, null)); //new Up(temp1, temp2);//TODO binom if const
                default:
                    return temp1.Uncover(NameOfOferand.Del, temp2);
            }
        }
    }

    class Up : Method
    {
        public Up(IOperand a, IOperand b) : base(a, b, NameOfOferand.Up)
        {
        }

        public override double Operation(double a, double b)
        {
            return Math.Pow(a, b);
        }
        public override string PrintOperation(string a, string b) => "(" + a + ") ^ (" + b + ")";
        public override IOperand Uncover(NameOfOferand name, IOperand deel)
        {
            var temp1 = _value1.Uncover(NameOfOferand.Add, null);
            var temp2 = _value2.Uncover(NameOfOferand.Add, null); //TODO whithout temp (-)
                                                                  //var temp = _value1.Uncover(NameOfOferand.Umn, _value2);
            switch (name)
            {
                case NameOfOferand.Min:
                    return new UnoMin(new Up(temp1, temp2)); //Add(new UnoMin(temp1), new UnoMin(temp2));
                case NameOfOferand.Del:
                    //return new Add(new Del(temp1, deel), new Del(temp2, deel));
                    return new Del(new Up(temp1, temp2), deel.Uncover(NameOfOferand.Add, null));
                case NameOfOferand.Umn:
                    return new Umn(new Up(temp1, temp2), deel.Uncover(NameOfOferand.Add, null));
                case NameOfOferand.Up:
                    return new Up(temp1, new Add(temp2, deel.Uncover(NameOfOferand.Add, null)));
                default:
                    return new Up(temp1, temp2);
            }
        }
    }
}
