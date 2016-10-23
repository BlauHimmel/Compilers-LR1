using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR_1_
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Rule> list = new List<Rule>();
            Rule rule1 = new Rule("S", "E");
            rule1.ahead.Add('#');
            Rule rule2 = new Rule("E", "E+(E)");
            Rule rule3 = new Rule("E", "a");
            list.Add(rule1);
            list.Add(rule2);
            list.Add(rule3);
            LR lr = new LR(list);
            lr.GetTable();
            Console.WriteLine("==================SENTENCES==================");
            Console.WriteLine(rule1);
            Console.WriteLine(rule2);
            Console.WriteLine(rule3);
            Console.WriteLine("====================TABLE====================");
            lr.PrintTable();
            Console.WriteLine("====================RESULT===================");
            Console.WriteLine("Check string:a+(a)+(a) \t" + lr.Check("a+(a)+(a)"));
            Console.WriteLine("Check string:a+(a) \t" + lr.Check("a+(a)"));
            Console.WriteLine("Check string:a \t\t" + lr.Check("a"));
            Console.WriteLine("Check string:a+(a \t" + lr.Check("a+(a"));
            Console.WriteLine("Check string:a+(a)+a \t" + lr.Check("a+(a)+a"));
        }
    }
}
