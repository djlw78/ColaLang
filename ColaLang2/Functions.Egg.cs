using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SplitAndMerge
{
    class ColaCredits : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            Console.WriteLine("Cola Credits");
            Console.WriteLine("ColaLang developer is ProfessorDJ");
            Console.WriteLine("ColaIDE/ColaLang (co) developer is RedCube");
            return Variable.EmptyInstance;
        }
    }
    class EggBeepFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var who = Utils.GetSafeString(args, 0);

            if (who == "mario")
            {
                Console.Beep(659, 125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(523, 125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(784, 125);
                Thread.Sleep(375);
                Console.Beep(392, 125);
                Thread.Sleep(375);
                Console.Beep(523, 125);
                Thread.Sleep(250);
                Console.Beep(392, 125);
                Thread.Sleep(250);
                Console.Beep(330, 125);
                Thread.Sleep(250);
                Console.Beep(440, 125);
                Thread.Sleep(125);
                Console.Beep(494, 125);
                Thread.Sleep(125);
                Console.Beep(466, 125);
                Thread.Sleep(42);
                Console.Beep(440, 125);
                Thread.Sleep(125);
                Console.Beep(392, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(784, 125);
                Thread.Sleep(125);
                Console.Beep(880, 125);
                Thread.Sleep(125);
                Console.Beep(698, 125);
                Console.Beep(784, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(587, 125);
                Console.Beep(494, 125);
                Thread.Sleep(125);
                Console.Beep(523, 125);
                Thread.Sleep(250);
                Console.Beep(392, 125);
                Thread.Sleep(250);
                Console.Beep(330, 125);
                Thread.Sleep(250);
                Console.Beep(440, 125);
                Thread.Sleep(125);
                Console.Beep(494, 125);
                Thread.Sleep(125);
                Console.Beep(466, 125);
                Thread.Sleep(42);
                Console.Beep(440, 125);
                Thread.Sleep(125);
                Console.Beep(392, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(784, 125);
                Thread.Sleep(125);
                Console.Beep(880, 125);
                Thread.Sleep(125);
                Console.Beep(698, 125);
                Console.Beep(784, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(587, 125);
                Console.Beep(494, 125);
                Thread.Sleep(375);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(415, 125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(698, 125);
                Thread.Sleep(125);
                Console.Beep(698, 125);
                Console.Beep(698, 125);
                Thread.Sleep(625);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(415, 125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(622, 125);
                Thread.Sleep(250);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(523, 125);
                Thread.Sleep(1125);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(415, 125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(698, 125);
                Thread.Sleep(125);
                Console.Beep(698, 125);
                Console.Beep(698, 125);
                Thread.Sleep(625);
                Console.Beep(784, 125);
                Console.Beep(740, 125);
                Console.Beep(698, 125);
                Thread.Sleep(42);
                Console.Beep(622, 125);
                Thread.Sleep(125);
                Console.Beep(659, 125);
                Thread.Sleep(167);
                Console.Beep(415, 125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Thread.Sleep(125);
                Console.Beep(440, 125);
                Console.Beep(523, 125);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(622, 125);
                Thread.Sleep(250);
                Console.Beep(587, 125);
                Thread.Sleep(250);
                Console.Beep(523, 125);
            }
            else if (who == "starwars")
            {
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
                Console.Beep(250, 500);
                Thread.Sleep(50);
                Console.Beep(350, 250);
                Console.Beep(300, 500);
                Thread.Sleep(50);
            }
            else
            {
                Console.WriteLine("EasterEgg not found!");
            }

            return Variable.EmptyInstance;

        }
    }
}

