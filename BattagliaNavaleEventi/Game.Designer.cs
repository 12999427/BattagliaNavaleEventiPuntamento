namespace BattagliaNavaleEventi
{
    partial class Game
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbl_numAttemps = new Label();
            lbl_shipsSunk = new Label();
            tbl_grid = new TableLayoutPanel();
            btn_PosNave4 = new Button();
            btn_PosNave3 = new Button();
            btn_PosNave2 = new Button();
            btn_PosNave1 = new Button();
            tbl_grid2 = new TableLayoutPanel();
            btn_ResetPlacement = new Button();
            txt_Log = new TextBox();
            SuspendLayout();
            // 
            // lbl_numAttemps
            // 
            lbl_numAttemps.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_numAttemps.Location = new Point(561, 11);
            lbl_numAttemps.Name = "lbl_numAttemps";
            lbl_numAttemps.Size = new Size(256, 68);
            lbl_numAttemps.TabIndex = 6;
            lbl_numAttemps.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_shipsSunk
            // 
            lbl_shipsSunk.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_shipsSunk.Location = new Point(564, 79);
            lbl_shipsSunk.Name = "lbl_shipsSunk";
            lbl_shipsSunk.Size = new Size(256, 40);
            lbl_shipsSunk.TabIndex = 5;
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
            tbl_grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid.Location = new Point(14, 11);
            tbl_grid.Name = "tbl_grid";
            tbl_grid.RowCount = 10;
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid.Size = new Size(525, 450);
            tbl_grid.TabIndex = 4;
            // 
            // btn_PosNave4
            // 
            btn_PosNave4.Font = new Font("Sitka Small", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btn_PosNave4.Location = new Point(575, 122);
            btn_PosNave4.Name = "btn_PosNave4";
            btn_PosNave4.Size = new Size(242, 44);
            btn_PosNave4.TabIndex = 8;
            btn_PosNave4.Text = "Posiziona Nave 4";
            btn_PosNave4.UseVisualStyleBackColor = true;
            // 
            // btn_PosNave3
            // 
            btn_PosNave3.Font = new Font("Sitka Small", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btn_PosNave3.Location = new Point(575, 171);
            btn_PosNave3.Name = "btn_PosNave3";
            btn_PosNave3.Size = new Size(242, 44);
            btn_PosNave3.TabIndex = 9;
            btn_PosNave3.Text = "Posiziona Nave 3";
            btn_PosNave3.UseVisualStyleBackColor = true;
            // 
            // btn_PosNave2
            // 
            btn_PosNave2.Font = new Font("Sitka Small", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btn_PosNave2.Location = new Point(575, 220);
            btn_PosNave2.Name = "btn_PosNave2";
            btn_PosNave2.Size = new Size(242, 44);
            btn_PosNave2.TabIndex = 10;
            btn_PosNave2.Text = "Posiziona Nave 2";
            btn_PosNave2.UseVisualStyleBackColor = true;
            // 
            // btn_PosNave1
            // 
            btn_PosNave1.Font = new Font("Sitka Small", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btn_PosNave1.Location = new Point(575, 270);
            btn_PosNave1.Name = "btn_PosNave1";
            btn_PosNave1.Size = new Size(242, 44);
            btn_PosNave1.TabIndex = 11;
            btn_PosNave1.Text = "Posiziona Nave 1";
            btn_PosNave1.UseVisualStyleBackColor = true;
            // 
            // tbl_grid2
            // 
            tbl_grid2.ColumnCount = 10;
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tbl_grid2.Location = new Point(841, 11);
            tbl_grid2.Name = "tbl_grid2";
            tbl_grid2.RowCount = 10;
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tbl_grid2.Size = new Size(525, 450);
            tbl_grid2.TabIndex = 5;
            // 
            // btn_ResetPlacement
            // 
            btn_ResetPlacement.Font = new Font("Sitka Small", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btn_ResetPlacement.Location = new Point(578, 320);
            btn_ResetPlacement.Name = "btn_ResetPlacement";
            btn_ResetPlacement.Size = new Size(242, 44);
            btn_ResetPlacement.TabIndex = 12;
            btn_ResetPlacement.Text = "Reset";
            btn_ResetPlacement.UseVisualStyleBackColor = true;
            // 
            // txt_Log
            // 
            txt_Log.Location = new Point(581, 141);
            txt_Log.Multiline = true;
            txt_Log.Name = "txt_Log";
            txt_Log.ReadOnly = true;
            txt_Log.ScrollBars = ScrollBars.Vertical;
            txt_Log.Size = new Size(231, 320);
            txt_Log.TabIndex = 14;
            txt_Log.Visible = false;
            // 
            // Game
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1382, 476);
            Controls.Add(txt_Log);
            Controls.Add(btn_ResetPlacement);
            Controls.Add(tbl_grid2);
            Controls.Add(btn_PosNave1);
            Controls.Add(btn_PosNave2);
            Controls.Add(btn_PosNave3);
            Controls.Add(btn_PosNave4);
            Controls.Add(lbl_numAttemps);
            Controls.Add(lbl_shipsSunk);
            Controls.Add(tbl_grid);
            Name = "Game";
            Text = "Game";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lbl_numAttemps;
        private Label lbl_shipsSunk;
        private TableLayoutPanel tbl_grid;
        private Button btn_PosNave4;
        private Button btn_PosNave3;
        private Button btn_PosNave2;
        private Button btn_PosNave1;
        private TableLayoutPanel tbl_grid2;
        private Button btn_ResetPlacement;
        private TextBox txt_Log;
    }
}
