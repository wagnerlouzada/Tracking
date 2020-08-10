namespace Rastreamento
{
    partial class FormRastreamento
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRastreamento));
            this.GridRemessas = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRemessas = new System.Windows.Forms.TabPage();
            this.tabRastreios = new System.Windows.Forms.TabPage();
            this.GridRastreios = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxDias = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPedido = new System.Windows.Forms.TextBox();
            this.textBoxCliente = new System.Windows.Forms.TextBox();
            this.textBoxCodigo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recarregarRemessasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.novoRastreamentoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarResultadosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarTudoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ajudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.GridRemessas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabRemessas.SuspendLayout();
            this.tabRastreios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridRastreios)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridRemessas
            // 
            this.GridRemessas.AllowUserToAddRows = false;
            this.GridRemessas.AllowUserToDeleteRows = false;
            this.GridRemessas.AllowUserToOrderColumns = true;
            this.GridRemessas.AllowUserToResizeRows = false;
            this.GridRemessas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridRemessas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridRemessas.DefaultCellStyle = dataGridViewCellStyle1;
            this.GridRemessas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridRemessas.Location = new System.Drawing.Point(3, 3);
            this.GridRemessas.MultiSelect = false;
            this.GridRemessas.Name = "GridRemessas";
            this.GridRemessas.ReadOnly = true;
            this.GridRemessas.RowHeadersVisible = false;
            this.GridRemessas.RowTemplate.Height = 24;
            this.GridRemessas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridRemessas.ShowCellErrors = false;
            this.GridRemessas.ShowCellToolTips = false;
            this.GridRemessas.ShowEditingIcon = false;
            this.GridRemessas.ShowRowErrors = false;
            this.GridRemessas.Size = new System.Drawing.Size(449, 365);
            this.GridRemessas.TabIndex = 0;
            this.GridRemessas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridRemessas_CellClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1033, 400);
            this.splitContainer1.SplitterDistance = 463;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabRemessas);
            this.tabControl1.Controls.Add(this.tabRastreios);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(463, 400);
            this.tabControl1.TabIndex = 1;
            // 
            // tabRemessas
            // 
            this.tabRemessas.Controls.Add(this.GridRemessas);
            this.tabRemessas.Location = new System.Drawing.Point(4, 25);
            this.tabRemessas.Name = "tabRemessas";
            this.tabRemessas.Padding = new System.Windows.Forms.Padding(3);
            this.tabRemessas.Size = new System.Drawing.Size(455, 371);
            this.tabRemessas.TabIndex = 0;
            this.tabRemessas.Text = "Remessas";
            this.tabRemessas.UseVisualStyleBackColor = true;
            // 
            // tabRastreios
            // 
            this.tabRastreios.Controls.Add(this.GridRastreios);
            this.tabRastreios.Location = new System.Drawing.Point(4, 25);
            this.tabRastreios.Name = "tabRastreios";
            this.tabRastreios.Padding = new System.Windows.Forms.Padding(3);
            this.tabRastreios.Size = new System.Drawing.Size(455, 371);
            this.tabRastreios.TabIndex = 1;
            this.tabRastreios.Text = "Rastreios";
            this.tabRastreios.UseVisualStyleBackColor = true;
            // 
            // GridRastreios
            // 
            this.GridRastreios.AllowUserToAddRows = false;
            this.GridRastreios.AllowUserToDeleteRows = false;
            this.GridRastreios.AllowUserToResizeRows = false;
            this.GridRastreios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GridRastreios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.GridRastreios.DefaultCellStyle = dataGridViewCellStyle2;
            this.GridRastreios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridRastreios.Location = new System.Drawing.Point(3, 3);
            this.GridRastreios.MultiSelect = false;
            this.GridRastreios.Name = "GridRastreios";
            this.GridRastreios.ReadOnly = true;
            this.GridRastreios.RowHeadersVisible = false;
            this.GridRastreios.RowTemplate.Height = 24;
            this.GridRastreios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridRastreios.ShowCellErrors = false;
            this.GridRastreios.ShowCellToolTips = false;
            this.GridRastreios.ShowEditingIcon = false;
            this.GridRastreios.ShowRowErrors = false;
            this.GridRastreios.Size = new System.Drawing.Size(449, 365);
            this.GridRastreios.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.textBoxDias);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.textBoxPedido);
            this.panel1.Controls.Add(this.textBoxCliente);
            this.panel1.Controls.Add(this.textBoxCodigo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 400);
            this.panel1.TabIndex = 0;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(96, 222);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 39);
            this.button4.TabIndex = 11;
            this.button4.Text = "Adicionar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(195, 222);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 39);
            this.button3.TabIndex = 10;
            this.button3.Text = "Excluir";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBoxDias
            // 
            this.textBoxDias.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDias.Location = new System.Drawing.Point(79, 181);
            this.textBoxDias.Name = "textBoxDias";
            this.textBoxDias.Size = new System.Drawing.Size(189, 26);
            this.textBoxDias.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 184);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Dias";
            // 
            // textBoxPedido
            // 
            this.textBoxPedido.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPedido.Location = new System.Drawing.Point(79, 140);
            this.textBoxPedido.Name = "textBoxPedido";
            this.textBoxPedido.Size = new System.Drawing.Size(189, 26);
            this.textBoxPedido.TabIndex = 7;
            // 
            // textBoxCliente
            // 
            this.textBoxCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCliente.Location = new System.Drawing.Point(79, 101);
            this.textBoxCliente.Name = "textBoxCliente";
            this.textBoxCliente.Size = new System.Drawing.Size(189, 26);
            this.textBoxCliente.TabIndex = 6;
            // 
            // textBoxCodigo
            // 
            this.textBoxCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCodigo.Location = new System.Drawing.Point(79, 62);
            this.textBoxCodigo.Name = "textBoxCodigo";
            this.textBoxCodigo.Size = new System.Drawing.Size(189, 26);
            this.textBoxCodigo.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Pedido";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Cliente";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Código";
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(15, 222);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 39);
            this.button2.TabIndex = 1;
            this.button2.Text = "Salvar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(293, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 39);
            this.button1.TabIndex = 0;
            this.button1.Text = "Rastrear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem,
            this.ajudaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1033, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recarregarRemessasToolStripMenuItem,
            this.novoRastreamentoToolStripMenuItem,
            this.exportarResultadosToolStripMenuItem,
            this.salvarTudoToolStripMenuItem,
            this.toolStripSeparator1,
            this.sairToolStripMenuItem});
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.arquivoToolStripMenuItem.Text = "Arquivo";
            // 
            // recarregarRemessasToolStripMenuItem
            // 
            this.recarregarRemessasToolStripMenuItem.Name = "recarregarRemessasToolStripMenuItem";
            this.recarregarRemessasToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.recarregarRemessasToolStripMenuItem.Text = "Recarregar Remessas";
            this.recarregarRemessasToolStripMenuItem.Click += new System.EventHandler(this.recarregarRemessasToolStripMenuItem_Click);
            // 
            // novoRastreamentoToolStripMenuItem
            // 
            this.novoRastreamentoToolStripMenuItem.Name = "novoRastreamentoToolStripMenuItem";
            this.novoRastreamentoToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.novoRastreamentoToolStripMenuItem.Text = "Novo Rastreamento";
            this.novoRastreamentoToolStripMenuItem.Click += new System.EventHandler(this.novoRastreamentoToolStripMenuItem_Click);
            // 
            // exportarResultadosToolStripMenuItem
            // 
            this.exportarResultadosToolStripMenuItem.Name = "exportarResultadosToolStripMenuItem";
            this.exportarResultadosToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.exportarResultadosToolStripMenuItem.Text = "Exportar Resultados";
            this.exportarResultadosToolStripMenuItem.Click += new System.EventHandler(this.exportarResultadosToolStripMenuItem_Click);
            // 
            // salvarTudoToolStripMenuItem
            // 
            this.salvarTudoToolStripMenuItem.Name = "salvarTudoToolStripMenuItem";
            this.salvarTudoToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.salvarTudoToolStripMenuItem.Text = "Salvar Remessas";
            this.salvarTudoToolStripMenuItem.Click += new System.EventHandler(this.salvarTudoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.sairToolStripMenuItem.Text = "Sair";
            // 
            // ajudaToolStripMenuItem
            // 
            this.ajudaToolStripMenuItem.Name = "ajudaToolStripMenuItem";
            this.ajudaToolStripMenuItem.Size = new System.Drawing.Size(60, 24);
            this.ajudaToolStripMenuItem.Text = "Ajuda";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1033, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // FormRastreamento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormRastreamento";
            this.Text = "Rastreamento";
            ((System.ComponentModel.ISupportInitialize)(this.GridRemessas)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabRemessas.ResumeLayout(false);
            this.tabRastreios.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridRastreios)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GridRemessas;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxPedido;
        private System.Windows.Forms.TextBox textBoxCliente;
        private System.Windows.Forms.TextBox textBoxCodigo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem novoRastreamentoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarTudoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajudaToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportarResultadosToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRemessas;
        private System.Windows.Forms.TabPage tabRastreios;
        private System.Windows.Forms.DataGridView GridRastreios;
        private System.Windows.Forms.TextBox textBoxDias;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem recarregarRemessasToolStripMenuItem;
        private System.Windows.Forms.Button button4;
    }
}

