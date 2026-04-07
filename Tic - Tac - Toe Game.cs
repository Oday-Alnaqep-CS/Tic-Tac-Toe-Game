using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Media;
using Tic_Tac_Toe_Game.Properties;

namespace Tic_Tac_Toe_Game
{
    public partial class Tic___Tac___Toe_Game : Form
    {
        // ≈⁄œ«œ«  «··⁄»… «·√”«”Ì…
        bool isXTurn = true;
        int moves = 0;

        // √·Ê«‰ „ ‰«”ﬁ… „⁄ «·Œ·›Ì… «·Œ‘»Ì… (√·Ê«‰ ‰ÌÊ‰ Œ›Ì›…)
        Color colorX = Color.FromArgb(255, 45, 85);   // Ê—œÌ ‰ÌÊ‰
        Color colorO = Color.FromArgb(0, 210, 255);   // √“—ﬁ ”„«ÊÌ
        Color btnTransparentBg = Color.FromArgb(130, 20, 20, 30); // Œ·›Ì… √“—«— ‘›«›…

        Button[] btns;
        Random rnd = new Random();
        SoundPlayer bgMusic = new SoundPlayer(@"music.wav");
        bool soundOn = true;

        public Tic___Tac___Toe_Game()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // ·„‰⁄ «·Ê„Ì÷ ⁄‰œ «·—”„

            btns = new Button[]
            {
                button1, button2, button3,
                button4, button5, button6,
                button7, button8, button9
            };

            SetupUI();

            try { bgMusic.PlayLooping(); } catch { }
        }

        private void SetupUI()
        {
            // 1. ≈⁄œ«œ«  «·›Ê—„ «·—∆Ì”Ì… · ‰«”» «·Œ·›Ì…
            this.BackgroundImage = Resources.GameImages1;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // 2.  ‰”Ìﬁ «·‰’Ê’ («·⁄‰Ê«‰ Ê«·√œÊ«—)
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.White;
            label1.Font = new Font("Segoe UI", 28, FontStyle.Bold);

            lblTurn.BackColor = Color.Transparent;
            lblTurn.ForeColor = Color.Yellow;
            lblTurn.Font = new Font("Segoe UI", 18, FontStyle.Bold);

            lblWinner.BackColor = Color.Transparent;
            lblWinner.ForeColor = Color.LimeGreen;
            lblWinner.Font = new Font("Segoe UI", 20, FontStyle.Bold);

            // 3.  ‰”Ìﬁ √“—«— «··⁄» («·„—»⁄«  «· ”⁄…)
            foreach (Button btn in btns)
            {
                btn.Text = "";
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(80, 255, 255, 255); // ≈ÿ«— Œ›Ì› Ãœ«
                btn.BackColor = btnTransparentBg; // Œ·›Ì… ‘›«›…  ŸÂ— «·Œ‘» „‰ Œ·›Â«
                btn.Font = new Font("Segoe UI", 40, FontStyle.Bold);
                btn.Cursor = Cursors.Hand;

                // Ã⁄· «·ÕÊ«› œ«∆—Ì… »‘ﬂ· ‰«⁄„
                btn.Region = CreateRoundedRegion(btn.Width, btn.Height, 20);

                //  √ÀÌ—«  «·„«Ê” · Õ”Ì‰  Ã—»… «·„” Œœ„
                btn.MouseEnter += (s, e) => { if (btn.Enabled) btn.BackColor = Color.FromArgb(180, 40, 40, 60); };
                btn.MouseLeave += (s, e) => { if (btn.Text == "" && btn.Enabled) btn.BackColor = btnTransparentBg; };
            }

            // 4.  ‰”Ìﬁ “— ≈⁄«œ… «· ‘€Ì·
            btnRestart.BackColor = Color.FromArgb(0, 255, 150);
            btnRestart.FlatStyle = FlatStyle.Flat;
            btnRestart.FlatAppearance.BorderSize = 0;
            btnRestart.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnRestart.Region = CreateRoundedRegion(btnRestart.Width, btnRestart.Height, 25);

            lblTurn.Text = "Player X";
            lblWinner.Text = "In Progress";
        }

        private Region CreateRoundedRegion(int w, int h, int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90);
            path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseAllFigures();
            return new Region(path);
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Text != "" || lblWinner.Text.Contains("Wins") || lblWinner.Text.Contains("Draw"))
                return;

            if (isXTurn)
            {
                btn.Text = "X";
                btn.ForeColor = colorX;
            }
            else
            {
                btn.Text = "O";
                btn.ForeColor = colorO;
            }

            moves++;
            isXTurn = !isXTurn;

            UpdateTurnLabel();
            CheckForWinner();

            // „‰ÿﬁ «·–ﬂ«¡ «·«’ÿ‰«⁄Ì (AI)
            if (chkAI.Checked && !isXTurn && !lblWinner.Text.Contains("Wins"))
                ComputerMove();
        }

        private void UpdateTurnLabel()
        {
            lblTurn.Text = chkAI.Checked ? (isXTurn ? "Your Turn" : "AI Thinking...") : (isXTurn ? "Player X" : "Player O");
        }

        private void ComputerMove()
        {
            Timer t = new Timer { Interval = 500 };
            t.Tick += (s, e) =>
            {
                t.Stop();
                if (TryMove("O")) return; // „Õ«Ê·… «·›Ê“
                if (TryMove("X")) return; // „Õ«Ê·… ’œ «··«⁄»

                // «Œ Ì«— «·„—ﬂ“ ≈–« ﬂ«‰ ›«—€«
                if (button5.Text == "") { button5.PerformClick(); return; }

                // «Œ Ì«— ⁄‘Ê«∆Ì ··„—»⁄«  «·›«—€…
                var empty = btns.Where(b => b.Text == "").ToList();
                if (empty.Count > 0)
                    empty[rnd.Next(empty.Count)].PerformClick();
            };
            t.Start();
        }

        private bool TryMove(string player)
        {
            foreach (Button btn in btns)
            {
                if (btn.Text == "")
                {
                    btn.Text = player;
                    if (CheckWinOnly()) { btn.Text = ""; btn.PerformClick(); return true; }
                    btn.Text = "";
                }
            }
            return false;
        }

        private bool CheckWinOnly()
        {
            return CheckTriple(button1, button2, button3) || CheckTriple(button4, button5, button6) ||
                   CheckTriple(button7, button8, button9) || CheckTriple(button1, button4, button7) ||
                   CheckTriple(button2, button5, button8) || CheckTriple(button3, button6, button9) ||
                   CheckTriple(button1, button5, button9) || CheckTriple(button3, button5, button7);
        }

        private void CheckForWinner()
        {
            if (CheckTriple(button1, button2, button3)) EndGame(button1, button2, button3);
            else if (CheckTriple(button4, button5, button6)) EndGame(button4, button5, button6);
            else if (CheckTriple(button7, button8, button9)) EndGame(button7, button8, button9);
            else if (CheckTriple(button1, button4, button7)) EndGame(button1, button4, button7);
            else if (CheckTriple(button2, button5, button8)) EndGame(button2, button5, button8);
            else if (CheckTriple(button3, button6, button9)) EndGame(button3, button6, button9);
            else if (CheckTriple(button1, button5, button9)) EndGame(button1, button5, button9);
            else if (CheckTriple(button3, button5, button7)) EndGame(button3, button5, button7);
            else if (moves == 9)
            {
                lblWinner.Text = "Draw! ??";
                MessageBox.Show("It's a tie!", "Game Over");
            }
        }

        private bool CheckTriple(Button b1, Button b2, Button b3)
        {
            return b1.Text != "" && b1.Text == b2.Text && b2.Text == b3.Text;
        }

        private void EndGame(Button b1, Button b2, Button b3)
        {
            Color winColor = Color.FromArgb(200, 0, 255, 150);
            b1.BackColor = winColor;
            b2.BackColor = winColor;
            b3.BackColor = winColor;

            string winner = b1.Text == "X" ? "Player X" : "Player O";
            lblWinner.Text = winner + " Wins! ??";

            foreach (var btn in btns) btn.Enabled = false;
            MessageBox.Show(winner + " is the winner!", "Victory");
        }

        private void ResetGame()
        {
            foreach (Button btn in btns)
            {
                btn.Text = "";
                btn.BackColor = btnTransparentBg;
                btn.Enabled = true;
            }
            isXTurn = true;
            moves = 0;
            lblTurn.Text = "Player X";
            lblWinner.Text = "In Progress";
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void Tic___Tac___Toe_Game_Load(object sender, EventArgs e)
        {
            //  „ ‰ﬁ· «·≈⁄œ«œ«  ·‹ SetupUI ·÷„«‰ «· — Ì»
        }

        private void Tic___Tac___Toe_Game_Paint(object sender, PaintEventArgs e)
        {
            //  „ ≈›—«€Â« ··”„«Õ ·’Ê—… «·Œ·›Ì… »«·ŸÂÊ— œÊ‰ ÿ»ﬁ… ·Ê‰Ì… „⁄ „…
        }
    }
}