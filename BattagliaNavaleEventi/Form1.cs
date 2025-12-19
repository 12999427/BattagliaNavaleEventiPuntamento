namespace BattagliaNavaleEventi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_GiocatoreSingolo_Click(object sender, EventArgs e)
        {
            LanciaGioco(false);
        }

        private void btn_DueGiocatori_Click(object sender, EventArgs e)
        {
            LanciaGioco(true);
        }

        private void LanciaGioco(bool multiplayer, bool bot=false)
        {
            Hide();
            try
            {
                using (Game form = new Game(multiplayer, bot))
                {
                    DialogResult dr = form.ShowDialog();
                    if (dr != DialogResult.Abort)
                        Show();
                }
            }
            catch
            {
                MessageBox.Show("Errore sconosciuto", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Bot_Click(object sender, EventArgs e)
        {
            LanciaGioco(true, true);
        }
    }
}