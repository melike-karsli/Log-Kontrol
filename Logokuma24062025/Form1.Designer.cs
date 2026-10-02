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
            this.datagridhataraporu = new System.Windows.Forms.DataGridView();
            this.dataGridToplamHatalarıGoster = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.datagridsiparisadet = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguarddetay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridhataraporu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridToplamHatalarıGoster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridsiparisadet)).BeginInit();
            this.SuspendLayout();
            // 
            // tarihsec
            // 
            this.tarihsec.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tarihsec.Location = new System.Drawing.Point(558, 12);
            this.tarihsec.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tarihsec.Name = "tarihsec";
            this.tarihsec.Size = new System.Drawing.Size(350, 32);
            this.tarihsec.TabIndex = 0;
            // 
            // kaynak_combo
            // 
            this.kaynak_combo.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.kaynak_combo.FormattingEnabled = true;
            this.kaynak_combo.Location = new System.Drawing.Point(923, 12);
            this.kaynak_combo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.kaynak_combo.Name = "kaynak_combo";
            this.kaynak_combo.Size = new System.Drawing.Size(350, 32);
            this.kaynak_combo.TabIndex = 1;
            this.kaynak_combo.SelectedIndexChanged += new System.EventHandler(this.kaynak_combo_SelectedIndexChanged);
            // 
            // datagridrcguarddetay
            // 
            this.datagridrcguarddetay.AllowUserToAddRows = false;
            this.datagridrcguarddetay.AllowUserToOrderColumns = true;
            this.datagridrcguarddetay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagridrcguarddetay.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridrcguarddetay.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridrcguarddetay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridrcguarddetay.Location = new System.Drawing.Point(39, 579);
            this.datagridrcguarddetay.Name = "datagridrcguarddetay";
            this.datagridrcguarddetay.RowHeadersWidth = 51;
            this.datagridrcguarddetay.RowTemplate.Height = 24;
            this.datagridrcguarddetay.Size = new System.Drawing.Size(1457, 200);
            this.datagridrcguarddetay.TabIndex = 5;
            this.datagridrcguarddetay.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.datagridrcguarddetay_CellContentClick);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(1288, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(350, 32);
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
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(34, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "SİPARİŞ ÖZET";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(34, 554);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "SİPARİŞ DETAY";
            // 
            // datagridrcguard
            // 
            this.datagridrcguard.AllowUserToAddRows = false;
            this.datagridrcguard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagridrcguard.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridrcguard.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridrcguard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridrcguard.Location = new System.Drawing.Point(39, 75);
            this.datagridrcguard.Name = "datagridrcguard";
            this.datagridrcguard.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.datagridrcguard.RowTemplate.Height = 24;
            this.datagridrcguard.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datagridrcguard.Size = new System.Drawing.Size(1752, 469);
            this.datagridrcguard.TabIndex = 8;
            this.datagridrcguard.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.datagridrcguard_CellContentClick);
            this.datagridrcguard.SelectionChanged += new System.EventHandler(this.datagridrcguard_SelectionChanged);
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
            this.datagridhataraporu.Location = new System.Drawing.Point(39, 819);
            this.datagridhataraporu.Name = "datagridhataraporu";
            this.datagridhataraporu.RowHeadersWidth = 51;
            this.datagridhataraporu.RowTemplate.Height = 24;
            this.datagridhataraporu.Size = new System.Drawing.Size(1100, 250);
            this.datagridhataraporu.TabIndex = 13;
            // 
            // dataGridToplamHatalarıGoster
            // 
            this.dataGridToplamHatalarıGoster.AllowUserToAddRows = false;
            this.dataGridToplamHatalarıGoster.AllowUserToOrderColumns = true;
            this.dataGridToplamHatalarıGoster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridToplamHatalarıGoster.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridToplamHatalarıGoster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridToplamHatalarıGoster.Location = new System.Drawing.Point(1153, 819);
            this.dataGridToplamHatalarıGoster.Name = "dataGridToplamHatalarıGoster";
            this.dataGridToplamHatalarıGoster.RowHeadersWidth = 51;
            this.dataGridToplamHatalarıGoster.RowTemplate.Height = 24;
            this.dataGridToplamHatalarıGoster.Size = new System.Drawing.Size(638, 250);
            this.dataGridToplamHatalarıGoster.TabIndex = 14;
            this.dataGridToplamHatalarıGoster.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridToplamHatalarıGoster_CellContentClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(1149, 794);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "HATA SAYISI";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(34, 794);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "HATA RAPORU";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(1506, 554);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(135, 20);
            this.label6.TabIndex = 19;
            this.label6.Text = "SİPARİŞ ADET";
            // 
            // datagridsiparisadet
            // 
            this.datagridsiparisadet.AllowUserToAddRows = false;
            this.datagridsiparisadet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridsiparisadet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridsiparisadet.Location = new System.Drawing.Point(1511, 579);
            this.datagridsiparisadet.Name = "datagridsiparisadet";
            this.datagridsiparisadet.ReadOnly = true;
            this.datagridsiparisadet.RowHeadersVisible = false;
            this.datagridsiparisadet.RowHeadersWidth = 51;
            this.datagridsiparisadet.RowTemplate.Height = 24;
            this.datagridsiparisadet.Size = new System.Drawing.Size(280, 200);
            this.datagridsiparisadet.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1857, 1100);
            this.Controls.Add(this.datagridsiparisadet);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridToplamHatalarıGoster);
            this.Controls.Add(this.datagridhataraporu);
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
            this.Text = "RestoPOS Sipariş Takip";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguarddetay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridrcguard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridhataraporu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridToplamHatalarıGoster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datagridsiparisadet)).EndInit();
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
        private System.Windows.Forms.DataGridView datagridhataraporu;
        private System.Windows.Forms.DataGridView dataGridToplamHatalarıGoster;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView datagridsiparisadet;
    }
}

