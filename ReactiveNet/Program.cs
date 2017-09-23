using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Observable.Create<string>((IObserver<string> observer) =>
            {
                return () =>
                {

                };
            });

            Console.ReadLine();
        }
    }
}
