namespace Logokuma24062025
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tarihsec = new System.Windows.Forms.DateTimePicker();
            this.kaynak_combo = new System.Windows.Forms.ComboBox();
            this.datagridrcguarddetay = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.datagridrcguard = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.datagridhataraporu = new System.Windows.Forms.DataGridView();
            this.dataGridToplamHatalarıGoster = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguarddetay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridhataraporu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridToplamHatalarıGoster)).BeginInit();
            this.SuspendLayout();
            // 
            // tarihsec
            // 
            this.tarihsec.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tarihsec.Location = new System.Drawing.Point(625, 62);
            this.tarihsec.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tarihsec.Name = "tarihsec";
            this.tarihsec.Size = new System.Drawing.Size(295, 30);
            this.tarihsec.TabIndex = 0;
            // 
            // kaynak_combo
            // 
            this.kaynak_combo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.kaynak_combo.FormattingEnabled = true;
            this.kaynak_combo.Location = new System.Drawing.Point(972, 59);
            this.kaynak_combo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.kaynak_combo.Name = "kaynak_combo";
            this.kaynak_combo.Size = new System.Drawing.Size(121, 33);
            this.kaynak_combo.TabIndex = 1;
            this.kaynak_combo.SelectedIndexChanged += new System.EventHandler(this.kaynak_combo_SelectedIndexChanged);
            // 
            // datagridrcguarddetay
            // 
            this.datagridrcguarddetay.AllowUserToAddRows = false;
            this.datagridrcguarddetay.AllowUserToOrderColumns = true;
            this.datagridrcguarddetay.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridrcguarddetay.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridrcguarddetay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridrcguarddetay.Location = new System.Drawing.Point(36, 268);
            this.datagridrcguarddetay.Name = "datagridrcguarddetay";
            this.datagridrcguarddetay.RowHeadersWidth = 51;
            this.datagridrcguarddetay.RowTemplate.Height = 24;
            this.datagridrcguarddetay.Size = new System.Drawing.Size(1970, 146);
            this.datagridrcguarddetay.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(846, 149);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(182, 30);
            this.button1.TabIndex = 7;
            this.button1.Text = "LOG YAZDIR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(34, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "RCGUARD SİPARİŞ ÖZET";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(34, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(286, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "RCGUARD SİPARİŞ DETAY";
            // 
            // datagridrcguard
            // 
            this.datagridrcguard.AllowUserToAddRows = false;
            this.datagridrcguard.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridrcguard.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridrcguard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridrcguard.Location = new System.Drawing.Point(36, 62);
            this.datagridrcguard.Name = "datagridrcguard";
            this.datagridrcguard.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.datagridrcguard.RowTemplate.Height = 24;
            this.datagridrcguard.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridrcguard.Size = new System.Drawing.Size(495, 146);
            this.datagridrcguard.TabIndex = 8;
            this.datagridrcguard.SelectionChanged += new System.EventHandler(this.datagridrcguard_SelectionChanged);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button2.Location = new System.Drawing.Point(39, 435);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(405, 36);
            this.button2.TabIndex = 11;
            this.button2.Text = "HATA RAPORU GÖRÜNTÜLE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button3_Click);
            // 
            // datagridhataraporu
            // 
            this.datagridhataraporu.AllowUserToAddRows = false;
            this.datagridhataraporu.AllowUserToOrderColumns = true;
            this.datagridhataraporu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridhataraporu.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.datagridhataraporu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.datagridhataraporu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridhataraporu.Location = new System.Drawing.Point(36, 488);
            this.datagridhataraporu.Name = "datagridhataraporu";
            this.datagridhataraporu.RowHeadersWidth = 51;
            this.datagridhataraporu.RowTemplate.Height = 24;
            this.datagridhataraporu.Size = new System.Drawing.Size(961, 469);
            this.datagridhataraporu.TabIndex = 13;
            // 
            // dataGridToplamHatalarıGoster
            // 
            this.dataGridToplamHatalarıGoster.AllowUserToAddRows = false;
            this.dataGridToplamHatalarıGoster.AllowUserToOrderColumns = true;
            this.dataGridToplamHatalarıGoster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridToplamHatalarıGoster.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridToplamHatalarıGoster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridToplamHatalarıGoster.Location = new System.Drawing.Point(1034, 488);
            this.dataGridToplamHatalarıGoster.Name = "dataGridToplamHatalarıGoster";
            this.dataGridToplamHatalarıGoster.RowHeadersWidth = 51;
            this.dataGridToplamHatalarıGoster.RowTemplate.Height = 24;
            this.dataGridToplamHatalarıGoster.Size = new System.Drawing.Size(603, 469);
            this.dataGridToplamHatalarıGoster.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(1019, 446);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(244, 25);
            this.label3.TabIndex = 15;
            this.label3.Text = "TOPLAM HATA SAYISI";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1857, 1010);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridToplamHatalarıGoster);
            this.Controls.Add(this.datagridhataraporu);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.datagridrcguarddetay);
            this.Controls.Add(this.datagridrcguard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.kaynak_combo);
            this.Controls.Add(this.tarihsec);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguarddetay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridhataraporu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridToplamHatalarıGoster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker tarihsec;
        private System.Windows.Forms.ComboBox kaynak_combo;
        private System.Windows.Forms.DataGridView datagridrcguarddetay;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView datagridrcguard;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView datagridhataraporu;
        private System.Windows.Forms.DataGridView dataGridToplamHatalarıGoster;
        private System.Windows.Forms.Label label3;
    }
}

