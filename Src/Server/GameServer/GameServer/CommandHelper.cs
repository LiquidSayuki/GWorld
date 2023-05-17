using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine().ToLower().Trim();
                try
                {
                    char[] saparator = { ' ' };
                    string[] cmd = line.Split(saparator, StringSplitOptions.RemoveEmptyEntries);
                    switch (cmd[0])
                    {
                        case "addexp":
                            AddExp(int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "exit":
                            run = false;
                            break;
                        default:
                            Help();
                            break;
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                }

            }
        }
        public static void AddExp(int characterId, int exp)
        {
            var character = Managers.CharacterManager.Instance.GetCharacter(characterId);
            if(character == null)
            {
                Console.WriteLine("character {0} not found", characterId);
                return;
            }
            character.AddExp(exp);
        }
        public static void Help()
        {
            Console.Write(@"
Help:
    exit    Exit Game Server
    help    Show Help
");
        }
    }
}
