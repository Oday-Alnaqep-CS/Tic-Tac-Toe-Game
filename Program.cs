using System;
using System.Windows.Forms;
using Tic_Tac_Toe_Game;

namespace Tic_Tac_Toe_Game
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Tic___Tac___Toe_Game());
        }
    }
}