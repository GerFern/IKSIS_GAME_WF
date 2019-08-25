using System;
using System.Reflection;

namespace Reactive
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Test<int,int> test = new Test<int, int>();
            test.PropertyChanged += Test_PropertyChanged;
            Type type = test.GetType();
            var atr=type.GetCustomAttribute<MyAttribute>();
            var atr2 = type.GetCustomAttribute<MyAttribute>();
            atr.Counter = 3;
            atr2.Counter = 5;
            //test[3] = 0;
            //test.A = "a";
        }

        private static void Test_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
            //throw new NotImplementedException();
        }
    }
}
