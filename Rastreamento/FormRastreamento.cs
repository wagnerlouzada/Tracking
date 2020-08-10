using Helper;
using HtmlAgilityPack;
using SortableList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomExtensions;

namespace Rastreamento
{

    public partial class FormRastreamento : Form
    {

        // carregar as remessas a serem processadas
        //
        public SortableBindingList<Remessa> Remessas = new SortableBindingList<Remessa>();
        public List<CorreiosDados> Rastreios = new List<CorreiosDados>();
        public SortableBindingList<ResultadoRemessa> Resultados = new SortableBindingList<ResultadoRemessa>();

        public FormRastreamento()
        {
            InitializeComponent();
            CarregarRemessas();
        }

        public void Rastrear()
        {
            Dictionary<String, String> header = new Dictionary<string, string>();
            // read config file

            Resultados.Clear();

            using (Helper.WebHelper client = new Helper.WebHelper("https://www2.correios.com.br/", 120, Encoding.GetEncoding("ISO-8859-1"), true, header)) //, logger: log))
            {

                foreach (var rem in Remessas)
                {
                    Rastreios.Clear();
                    header.Clear();
                    {
                        header.Add("Host", "www2.correios.com.br");
                        header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
                        header.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                        header.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
                        header.Add("Accept-Encoding", "gzip, deflate, br");
                        header.Add("Connection", "keep-alive");
                        header.Add("Referer", "https://www2.correios.com.br/sistemas/rastreamento/resultado.cfm");
                        header.Add("Upgrade-Insecure-Requests", "1");
                    }
                    String url = "/sistemas/rastreamento/";
                    HtmlAgilityPack.HtmlDocument pg1 = null;
                    try
                    {
                        pg1 = client.GetPage(url, header);
                    }
                    catch
                    {
                        MessageBox.Show("Problemas ao tentar conectar com Correios, Verifique seu Firewall ou tente mais tarde","ALERTA", MessageBoxButtons.OK);
                        return;
                    }

                    header.Clear();
                    {
                        header.Add("Host", "www2.correios.com.br");
                        header.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0");
                        header.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                        header.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.5,en;q=0.3");
                        header.Add("Accept-Encoding", "gzip, deflate, br");
                        header.Add("Content-Type", "application/x-www-form-urlencoded");
                        header.Add("Origin", "https://www2.correios.com.br");
                        header.Add("Connection", "keep-alive");
                        header.Add("Referer", "https://www2.correios.com.br/sistemas/rastreamento/");
                        header.Add("Upgrade-Insecure-Requests", "1");
                    }
                    TDictionary frm = new TDictionary();
                    frm["acao"] = "track";
                    frm["objetos"] = rem.CodigoRastreio;
                    frm["btnPesq"] = "Buscar";
                    url = "/sistemas/rastreamento/ctrl/ctrlRastreamento.cfm?";

                    HtmlAgilityPack.HtmlDocument pg2= null;
                    try
                    {
                       pg2 = client.PostPage(url, new FormUrlEncodedContent(frm.ToKeyValuePair()), header);
                    }
                    catch
                    {
                        MessageBox.Show("Problemas ao tentar conectar com Correios, Verifique seu Firewall ou tente mais tarde", "ALERTA", MessageBoxButtons.OK);
                        return;
                    }
                    // aqui ja tem o resultado

                    List<String> dataStr = new List<String>();
                    try
                    {

                        foreach (HtmlNode tbl in pg2.DocumentNode.SelectNodes("//table[@class='listEvent sro']"))
                            foreach (HtmlNode row in tbl.SelectNodes("tr"))
                                foreach (HtmlNode col in row.SelectNodes("td"))
                                    dataStr.Add(col.InnerText);

                        if (dataStr.Count > 0)
                        {
                            int index = 0;
                            while (true)
                            {
                                CorreiosDados data = new CorreiosDados();
                                String s1 = dataStr[index];
                                String s2 = dataStr[index + 1];
                                s1 = s1.Replace("\n", "")
                                       .Replace("\r", "")
                                       .Replace("\t", "")
                                       .Replace("  "," ")
                                       .Replace("&nbsp;", "")
                                       .TrimStart()
                                       .TrimEnd()
                                       ;
                                s2 = s2.Replace("\n", "")
                                       .Replace("\r", "")
                                       .Replace("&nbsp;", "")
                                       .Replace("\t", "")
                                       .Replace("  ", " ")
                                       .TrimStart()
                                       .TrimEnd()
                                       ;

                                data.Date = s1.Substring(0, 16)
                                              .TrimEnd();
                                data.DateExtra = s1.Substring(16, s1.Length - 16)
                                                   .TrimStart()
                                                   .TrimEnd();
                                data.Data = s2.TrimStart()
                                              .TrimEnd();
                                data.Data = data.Data;

                                byte[] utf16String = Encoding.UTF8.GetBytes(data.Data);
                                data.Data = Encoding.UTF8.GetString(utf16String);

                                Rastreios.Add(data);
                                index = index + 2;
                            }
                        }


                    }
                    catch (Exception ex)
                    {

                    }

                    //Boolean First = true;
                    ResultadoRemessa rastreioRemessa = new ResultadoRemessa();
                    rastreioRemessa.CodigoRastreio = rem.CodigoRastreio;
                    rastreioRemessa.NomeDoCliente = rem.NomeDoCliente;
                    rastreioRemessa.NumeroDoPedido = rem.NumeroDoPedido;
                    //rastreioRemessa.Transportadora = rem.Transportadora;
                    //rastreioRemessa.NomeTransportadora = "EBCT";
                    Resultados.Add(rastreioRemessa);
                    foreach (var rastreio in Rastreios)
                    {

                        if (rastreioRemessa.MovimentacaoInicial == null || 
                            rastreioRemessa.MovimentacaoInicial >= Convert.ToDateTime(rastreio.Date))
                        {
                            rastreioRemessa.MovimentacaoInicial = Convert.ToDateTime(rastreio.Date);
                        }

                        if (rastreioRemessa.DataUltimaMovimentacao == null ||
                            rastreioRemessa.DataUltimaMovimentacao <= Convert.ToDateTime(rastreio.Date))
                        {
                            rastreioRemessa.DataUltimaMovimentacao = Convert.ToDateTime(rastreio.Date);
                            rastreioRemessa.Movimentacao = rastreio.Data;
                            rastreioRemessa.Detalhe = rastreio.DateExtra;
                        }

                        if (rastreio.Data.ToUpper().Contains("ENTREG"))
                        {
                            rastreioRemessa.DataEntrega= Convert.ToDateTime(rastreio.Date);
                        }

                        if (rastreio.Data.ToUpper().Contains("SUSPEN"))
                        {
                            rastreioRemessa.DataEntrega = null;
                            rastreioRemessa.DataCancelamento = Convert.ToDateTime(rastreio.Date);
                        }

                    }

      
                }

            }
            foreach(var rastreio in Resultados)
            {

            }

            GridRastreios.DataSource = null;
            GridRastreios.DataSource = Resultados;
            GridRastreios.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRastreios.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRastreios.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRastreios.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Enabled = false;
            Rastrear();
            this.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        private void CarregarRemessas()
        {
            
            String Filename = Application.StartupPath + "\\Remessas.csv";
            if (File.Exists(Filename))
            {

                String[] lines = File.ReadAllLines(Filename);

                int l = 0;
                Boolean first = true;
                foreach (String line in lines)
                {
                    if (!first)
                    {
                        l++;
                        string[] cpos = line.Split(';');
                        if (cpos.Count() != 4)
                        {
                            MessageBox.Show("Erro nos dados de Remessas na linha " + l.ToString(), "ERRO");
                            return;
                        }
                        Remessa remessa = new Remessa();
                        remessa.CodigoRastreio = cpos[0].ToString();
                        //remessa.Transportadora = Convert.ToInt32(cpos[1]);
                         remessa.NomeDoCliente = cpos[1].ToString();
                        remessa.NumeroDoPedido = cpos[2].ToString();
                        try
                        {
                            remessa.DiasParaEntrega = Convert.ToInt32(cpos[3]);
                        }
                        catch
                        {
                            remessa.DiasParaEntrega = 0;
                        }
                        Remessas.Add(remessa);
                    }
                    else first = false;
                }

                // MessageBox.Show("Atualizado com Sucesso.");
            }
            GridRemessas.DataSource = null;
            GridRemessas.DataSource = Remessas;
            GridRemessas.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRemessas.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRemessas.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
            GridRemessas.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        private void SalvarRemessas()
        {

            String Filename = Application.StartupPath + "\\Remessas.csv";

            using (var sw = new StreamWriter(Filename))
            {
                string line = "Codigo;Cliente;Pedido;Dias";

                sw.WriteLine(line);

                foreach (var remessa in Remessas)
                {
                    line = "";
                    line = line + remessa.CodigoRastreio.ToString() + ";";
                    line = line + remessa.NomeDoCliente.ToString() + ";";
                    line = line + remessa.NumeroDoPedido.ToString() + ";";
                    line = line + remessa.DiasParaEntrega.ToString() + "";
                    sw.WriteLine(line);
                }
            }

            MessageBox.Show("Remessas Salvas com Sucesso.");

        }

        private void UpdateRemessaData()
        {

            DataGridViewRow cRow = GridRemessas.SelectedRows[0];

            textBoxCliente.ForeColor = Color.Black;
            textBoxCodigo.ForeColor = Color.Black;
            textBoxPedido.ForeColor = Color.Black;
            textBoxDias.ForeColor = Color.Black;

            textBoxCliente.Text = "";
            textBoxCodigo.Text = "";
            textBoxPedido.Text = "";
            textBoxDias.Text = "0";

            textBoxCodigo.Text = cRow.Cells[0].Value.ToString();
            textBoxCodigo.ReadOnly = true;
            textBoxCliente.Text = cRow.Cells[1].Value.ToString();
            textBoxPedido.Text = cRow.Cells[2].Value.ToString();
            textBoxDias.Text = cRow.Cells[3].Value.ToString();
           
        }

        private void limparEntrada()
        {
            textBoxCodigo.Text = "";
            textBoxCodigo.ReadOnly = false;
            textBoxCliente.Text = "";
            textBoxPedido.Text = "";
        }

        private void novoRastreamentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparEntrada();
        }

        private void NovaRemessa()
        {
            Remessa remessa = new Remessa();
            remessa.CodigoRastreio = textBoxCodigo.Text.ToUpper();
            remessa.NomeDoCliente = textBoxCliente.Text;
            remessa.NumeroDoPedido = textBoxPedido.Text;
            try
            {
                remessa.DiasParaEntrega = Convert.ToInt32(textBoxDias.Text);
            }
            catch
            {
                remessa.DiasParaEntrega = 0;
            }
            if (Remessas.Where(p => p.CodigoRastreio == remessa.CodigoRastreio).FirstOrDefault() != null)
            {

                DialogResult result = MessageBox.Show("Já Existe um Lançamento para este Codigo de Rastreio, Deseja Atualiza-lo", "Alerta", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    exluirRemessa();
                    Remessas.Add(remessa);
                    GridRemessas.DataSource = null;
                    GridRemessas.DataSource = Remessas;
                    GridRemessas.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;

                }
                else if (result == DialogResult.No)
                {
                  
                }
                else
                {
                    
                }


            }
            else
            {
                Remessas.Add(remessa);
                GridRemessas.DataSource = null;
                GridRemessas.DataSource = Remessas;
                GridRemessas.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
                GridRemessas.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
                GridRemessas.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
                GridRemessas.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NovaRemessa();
        }

        private void GridRemessas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateRemessaData();
        }

        private void salvarTudoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarRemessas();
        }

        private void exluirRemessa()
        {
            if (textBoxCodigo.Text != "")
            {
                try
                {
                    var remessa = Remessas.Where(p => p.CodigoRastreio == textBoxCodigo.Text.ToString().ToUpper()).FirstOrDefault();

                    Remessas.Remove(remessa);
                    GridRemessas.DataSource = null;
                    GridRemessas.DataSource = Remessas;
                    GridRemessas.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
                    GridRemessas.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
                    UpdateRemessaData();
                }
                catch { }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            exluirRemessa();
        }

        private void recarregarRemessasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remessas.Clear();
            CarregarRemessas();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            textBoxCodigo.Width = panel1.Width - textBoxCodigo.Left - 10;
            textBoxCliente.Width = panel1.Width - textBoxCliente.Left - 10;
            textBoxPedido.Width = panel1.Width - textBoxPedido.Left - 10;
            textBoxDias.Width = panel1.Width - textBoxDias.Left - 10;
        }

        private void ExportarResultados()
        {
            String Filename = Application.StartupPath + "\\Resultados_"+DateTime.Now.Date.ToString("yyyyMMdd")+".csv";

            using (var sw = new StreamWriter(Filename))
            {
                string line = "Codigo;Cliente;Pedido;Dias;Inicial;UltMovimento;Entrega;Cancelamento;Movimento;Detalhe;Parado";

                sw.WriteLine(line);

                foreach (var resultado in Resultados)
                {
                    line = "";
                    line = line + resultado.CodigoRastreio.ToString().ForceValidString() + ";";
                    line = line + resultado.NomeDoCliente.ToString().ForceValidString() + ";";
                    line = line + resultado.NumeroDoPedido.ToString().ForceValidString() + ";";
                    line = line + resultado.DiasParaEntrega.ToString().ForceValidString() + ";";

                    line = line + resultado.MovimentacaoInicial.ToString().ForceValidString() + ";";
                    line = line + resultado.DataUltimaMovimentacao.ToString().ForceValidString() + ";";
                    line = line + resultado.DataEntrega.ToString().ForceValidString() + ";";
                    line = line + resultado.DataCancelamento.ToString().ForceValidString() + ";";
                    line = line + resultado.Movimentacao.ForceValidString() + ";";
                    line = line + resultado.Detalhe.ForceValidString() + ";";
                    line = line + resultado.DiasParado.ToString().ForceValidString() + "";
          
                    sw.WriteLine(line);

                }
            }

            MessageBox.Show("Resultado Salvo com Sucesso.");
        }

        private void exportarResultadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportarResultados();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            limparEntrada();
        }

    }

    // fururamente para viabilizar outras transportadoras, tipo azul cargo, gol etc
    public class Transportadoras
    {
        public int Codigo { get; set; }
        public String Nome { get; set; }
    }

    // lista dos rastreios a serem acompanhados diariamente/periodicamente
    public class Remessa
    {
        public String CodigoRastreio { get; set; }
        //public int Transportadora { get; set; }
        public String NomeDoCliente { get; set; }
        public String NumeroDoPedido { get; set; }
        public int? DiasParaEntrega { get; set; }
    }

    public class ResultadoRemessa
    {
        public String CodigoRastreio { get; set; }

        //public int Transportadora { get; set; }
        //public String NomeTransportadora { get; set; }

        public String NomeDoCliente { get; set; }
        public String NumeroDoPedido { get; set; }

        public DateTime? MovimentacaoInicial { get; set; }
        //public DateTime? UltimoRastreamento { get; set; }
        
        //public List<Historico> Historico { get; set; }
        public DateTime? DataUltimaMovimentacao { get; set; }
        public DateTime? DataEntrega { get; set; }
        public DateTime? DataCancelamento { get; set; }
        //public String UltimaMovimentacao { get; set; }
        public String Movimentacao { get; set; }
        public String Detalhe { get; set; }
        public int? DiasParado { get; set; }
        public int? DiasParaEntrega { get; set; }

        public ResultadoRemessa()
        {
            //List<Historico> historico = new List<Historico>();
        }
    }

    public class Historico
    {
        public DateTime Data { get; set; }
        public String Nota { get; set; }
    }

    // especifico para leitura de dados do correio
    public class CorreiosDados
    {
        public String Date { get; set; }
        public String DateExtra { get; set; }
        public String Data { get; set; }
  
    }

}
