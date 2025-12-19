namespace BattagliaNavaleEventi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbl_bestAttemps = new Label();
            lbl_numAttemps = new Label();
            lbl_shipsSunk = new Label();
            tbl_grid = new TableLayoutPanel();
            lbl_Titolo = new Label();
            btn_GiocatoreSingolo = new Button();
            btn_DueGiocatori = new Button();
            btn_Bot = new Button();
            SuspendLayout();
            // 
            // lbl_bestAttemps
            // 
            lbl_bestAttemps.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_bestAttemps.Location = new Point(500, 106);
            lbl_bestAttemps.Name = "lbl_bestAttemps";
            lbl_bestAttemps.Size = new Size(324, 63);
            lbl_bestAttemps.TabIndex = 7;
            lbl_bestAttemps.Text = "Tentativi min salvati: 0";
            lbl_bestAttemps.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_numAttemps
            // 
            lbl_numAttemps.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_numAttemps.Location = new Point(500, 43);
            lbl_numAttemps.Name = "lbl_numAttemps";
            lbl_numAttemps.Size = new Size(324, 63);
            lbl_numAttemps.TabIndex = 6;
            lbl_numAttemps.Text = "Numero tentativi: 0";
            lbl_numAttemps.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_shipsSunk
            // 
            lbl_shipsSunk.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_shipsSunk.Location = new Point(500, -20);
            lbl_shipsSunk.Name = "lbl_shipsSunk";
            lbl_shipsSunk.Size = new Size(324, 63);
            lbl_shipsSunk.TabIndex = 5;
            lbl_shipsSunk.Text = "Navi affondate: 0";
            lbl_shipsSunk.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbl_grid
            // 
            tbl_grid.ColumnCount = 10;
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tbl_grid.Location = new Point(0, 0);
            tbl_grid.Name = "tbl_grid";
            tbl_grid.RowCount = 10;
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tbl_grid.Size = new Size(200, 100);
            tbl_grid.TabIndex = 0;
            // 
            // lbl_Titolo
            // 
            lbl_Titolo.Anchor = AnchorStyles.Top;
            lbl_Titolo.Font = new Font("Trebuchet MS", 26.25F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_Titolo.Location = new Point(124, 26);
            lbl_Titolo.Name = "lbl_Titolo";
            lbl_Titolo.Size = new Size(500, 63);
            lbl_Titolo.TabIndex = 5;
            lbl_Titolo.Text = "🛳🚢 Battaglia Navale 🚢🛳";
            lbl_Titolo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_GiocatoreSingolo
            // 
            btn_GiocatoreSingolo.Anchor = AnchorStyles.Top;
            btn_GiocatoreSingolo.Font = new Font("Sitka Small", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            btn_GiocatoreSingolo.Location = new Point(206, 155);
            btn_GiocatoreSingolo.Name = "btn_GiocatoreSingolo";
            btn_GiocatoreSingolo.Size = new Size(290, 48);
            btn_GiocatoreSingolo.TabIndex = 6;
            btn_GiocatoreSingolo.Text = "  👤   | Giocatore Singolo ";
            btn_GiocatoreSingolo.TextAlign = ContentAlignment.MiddleLeft;
            btn_GiocatoreSingolo.UseVisualStyleBackColor = true;
            btn_GiocatoreSingolo.Click += btn_GiocatoreSingolo_Click;
            // 
            // btn_DueGiocatori
            // 
            btn_DueGiocatori.Anchor = AnchorStyles.Top;
            btn_DueGiocatori.Font = new Font("Sitka Small", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            btn_DueGiocatori.Location = new Point(206, 234);
            btn_DueGiocatori.Name = "btn_DueGiocatori";
            btn_DueGiocatori.Size = new Size(290, 48);
            btn_DueGiocatori.TabIndex = 7;
            btn_DueGiocatori.Text = "👤👤 | Due Giocatori ";
            btn_DueGiocatori.TextAlign = ContentAlignment.MiddleLeft;
            btn_DueGiocatori.UseVisualStyleBackColor = true;
            btn_DueGiocatori.Click += btn_DueGiocatori_Click;
            // 
            // btn_Bot
            // 
            btn_Bot.Anchor = AnchorStyles.Top;
            btn_Bot.Font = new Font("Sitka Small", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            btn_Bot.Location = new Point(206, 312);
            btn_Bot.Name = "btn_Bot";
            btn_Bot.Size = new Size(290, 48);
            btn_Bot.TabIndex = 8;
            btn_Bot.Text = "👤💻 | Contro Bot";
            btn_Bot.TextAlign = ContentAlignment.MiddleLeft;
            btn_Bot.UseVisualStyleBackColor = true;
            btn_Bot.Click += btn_Bot_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(708, 468);
            Controls.Add(btn_Bot);
            Controls.Add(btn_DueGiocatori);
            Controls.Add(btn_GiocatoreSingolo);
            Controls.Add(lbl_Titolo);
            MinimumSize = new Size(602, 372);
            Name = "Form1";
            Text = "Seleziona Modalita";
            ResumeLayout(false);
        }

        #endregion

        private Label lbl_bestAttemps;
        private Label lbl_numAttemps;
        private Label lbl_shipsSunk;
        private TableLayoutPanel tbl_grid;
        private Label lbl_Titolo;
        private Button btn_GiocatoreSingolo;
        private Button btn_DueGiocatori;
        private Button btn_Bot;
    }
}