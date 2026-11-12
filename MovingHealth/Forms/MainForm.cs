using MovingHealth.Services;

namespace MovingHealth.Forms;

public class MainForm : Form
{
    // Paleta: fundo escuro + ciano
    private static readonly Color Fundo = Color.FromArgb(11, 20, 24);
    private static readonly Color Painel = Color.FromArgb(18, 32, 38);
    private static readonly Color Ciano = Color.FromArgb(0, 229, 255);
    private static readonly Color Texto = Color.FromArgb(230, 250, 252);
    private static readonly Color Erro = Color.FromArgb(255, 138, 128);

    private readonly FilaService _fila;

    private readonly TextBox _txtNome = new();
    private readonly NumericUpDown _numIdade = new();
    private readonly CheckBox _chkPcd = new();
    private readonly CheckBox _chkGestante = new();
    private readonly CheckBox _chkAlerta = new();
    private readonly Label _lblMensagem = new();
    private readonly DataGridView _grid = new();
    private readonly Label _lblChamado = new();
    private readonly Label _lblResumo = new();

    public MainForm(FilaService fila)
    {
        _fila = fila;
        MontarTela();
        Atualizar();
    }

    private void MontarTela()
    {
        Text = "Moving Health";
        BackColor = Fundo;
        ForeColor = Texto;
        Font = new Font("Segoe UI", 12F);
        Size = new Size(1100, 680);
        MinimumSize = new Size(1000, 620);
        StartPosition = FormStartPosition.CenterScreen;

        // ----- Topo -----
        var topo = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Fundo };
        topo.Controls.Add(new Label
        {
            Text = "Moving Health",
            Font = new Font("Segoe UI", 26F, FontStyle.Bold),
            ForeColor = Ciano,
            AutoSize = true,
            Location = new Point(16, 6)
        });
        topo.Controls.Add(new Label
        {
            Text = "Fila de atendimento com prioridade para idosos e pessoas com deficiência",
            ForeColor = Texto,
            AutoSize = true,
            Location = new Point(20, 52)
        });

        // ----- Lado esquerdo: cadastro -----
        var esquerda = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 340,
            BackColor = Painel,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16)
        };

        _txtNome.Width = 300;
        _txtNome.MaxLength = 60;
        EstilizarCampo(_txtNome);

        _numIdade.Width = 300;
        _numIdade.Minimum = 0;
        _numIdade.Maximum = 120;
        EstilizarCampo(_numIdade);

        ConfigurarCheck(_chkPcd, "Pessoa com deficiência");
        ConfigurarCheck(_chkGestante, "Gestante");
        ConfigurarCheck(_chkAlerta, "Sinais de alerta (falta de ar, dor forte, febre alta)");

        _lblMensagem.AutoSize = true;
        _lblMensagem.MaximumSize = new Size(300, 0);
        _lblMensagem.Margin = new Padding(3, 12, 3, 3);

        esquerda.Controls.Add(Rotulo("Nome do paciente"));
        esquerda.Controls.Add(_txtNome);
        esquerda.Controls.Add(Rotulo("Idade"));
        esquerda.Controls.Add(_numIdade);
        esquerda.Controls.Add(_chkPcd);
        esquerda.Controls.Add(_chkGestante);
        esquerda.Controls.Add(_chkAlerta);
        esquerda.Controls.Add(CriarBotao("Gerar senha", 300, Adicionar));
        esquerda.Controls.Add(_lblMensagem);

        // ----- Lado direito: fila -----
        ConfigurarGrid();

        _lblChamado.Text = "Nenhum paciente chamado";
        _lblChamado.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        _lblChamado.ForeColor = Ciano;
        _lblChamado.AutoSize = true;
        _lblChamado.Location = new Point(0, 8);

        _lblResumo.AutoSize = true;
        _lblResumo.Location = new Point(2, 55);

        var chamar = CriarBotao("Chamar próximo", 220, ChamarProximo);
        chamar.Location = new Point(0, 95);
        var cancelar = CriarBotao("Cancelar selecionado", 240, Cancelar);
        cancelar.Location = new Point(235, 95);

        var rodape = new Panel { Dock = DockStyle.Bottom, Height = 150 };
        rodape.Controls.Add(_lblChamado);
        rodape.Controls.Add(_lblResumo);
        rodape.Controls.Add(chamar);
        rodape.Controls.Add(cancelar);

        var direita = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
        direita.Controls.Add(_grid);   // Fill primeiro
        direita.Controls.Add(rodape);

        Controls.Add(direita);
        Controls.Add(esquerda);
        Controls.Add(topo);
    }

    // ----- Eventos -----

    private void Adicionar(object? sender, EventArgs e)
    {
        int idade = (int)_numIdade.Value;
        string? erro = Validador.Validar(_txtNome.Text, idade);
        if (erro != null)
        {
            Mensagem(erro, true);
            return;
        }

        try
        {
            var paciente = _fila.Adicionar(_txtNome.Text, idade, _chkPcd.Checked, _chkGestante.Checked, _chkAlerta.Checked);
            Mensagem($"Senha {paciente.Senha} gerada para {paciente.Nome} (prioridade {paciente.Prioridade}).", false);
        }
        catch (Exception ex)
        {
            Mensagem("Erro ao salvar: " + ex.Message, true);
            return;
        }

        _txtNome.Clear();
        _numIdade.Value = 0;
        _chkPcd.Checked = false;
        _chkGestante.Checked = false;
        _chkAlerta.Checked = false;
        _txtNome.Focus();
        Atualizar();
    }

    private void ChamarProximo(object? sender, EventArgs e)
    {
        try
        {
            var paciente = _fila.ChamarProximo();
            if (paciente == null)
            {
                MessageBox.Show("A fila está vazia.", "Moving Health");
                return;
            }

            _lblChamado.Text = $"Chamando: {paciente.Senha} - {paciente.Nome}";
            Atualizar();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao salvar: " + ex.Message, "Moving Health");
        }
    }

    private void Cancelar(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow == null)
        {
            MessageBox.Show("Selecione um paciente na fila.", "Moving Health");
            return;
        }

        int id = (int)_grid.CurrentRow.Cells["Id"].Value;
        var resposta = MessageBox.Show("Cancelar este paciente da fila?", "Moving Health", MessageBoxButtons.YesNo);
        if (resposta != DialogResult.Yes)
            return;

        try
        {
            _fila.Cancelar(id);
            Atualizar();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao salvar: " + ex.Message, "Moving Health");
        }
    }

    // ----- Auxiliares -----

    private void Atualizar()
    {
        var lista = _fila.Fila();

        _grid.DataSource = lista.Select(p => new
        {
            p.Id,
            p.Senha,
            p.Nome,
            Prioridade = p.Prioridade.ToString(),
            Chegada = p.Chegada.ToString("HH:mm")
        }).ToList();

        if (_grid.Columns["Id"] != null)
            _grid.Columns["Id"]!.Visible = false;

        _lblResumo.Text = $"Na fila: {lista.Count}   |   Atendidos: {_fila.TotalAtendidos()}   |   Espera média: {_fila.TempoMedioEspera():0} min";
    }

    private void Mensagem(string texto, bool ehErro)
    {
        _lblMensagem.Text = texto;
        _lblMensagem.ForeColor = ehErro ? Erro : Ciano;
    }

    private Label Rotulo(string texto)
    {
        return new Label { Text = texto, AutoSize = true, ForeColor = Ciano, Margin = new Padding(3, 10, 3, 3) };
    }

    private void EstilizarCampo(Control campo)
    {
        campo.BackColor = Fundo;
        campo.ForeColor = Texto;
        if (campo is TextBox caixa)
            caixa.BorderStyle = BorderStyle.FixedSingle;
    }

    private void ConfigurarCheck(CheckBox check, string texto)
    {
        check.Text = texto;
        check.AutoSize = true;
        check.MaximumSize = new Size(300, 0);
        check.ForeColor = Texto;
        check.Margin = new Padding(3, 10, 3, 3);
    }

    private Button CriarBotao(string texto, int largura, EventHandler acao)
    {
        var botao = new Button
        {
            Text = texto,
            Width = largura,
            Height = 44,
            FlatStyle = FlatStyle.Flat,
            BackColor = Ciano,
            ForeColor = Fundo,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(3, 16, 3, 3)
        };
        botao.FlatAppearance.BorderSize = 0;
        botao.Click += acao;
        return botao;
    }

    private void ConfigurarGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.BackgroundColor = Fundo;
        _grid.BorderStyle = BorderStyle.None;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.EnableHeadersVisualStyles = false;
        _grid.GridColor = Painel;
        _grid.RowTemplate.Height = 36;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        _grid.ColumnHeadersHeight = 40;

        _grid.ColumnHeadersDefaultCellStyle.BackColor = Painel;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Ciano;
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Painel;
        _grid.DefaultCellStyle.BackColor = Fundo;
        _grid.DefaultCellStyle.ForeColor = Texto;
        _grid.DefaultCellStyle.SelectionBackColor = Ciano;
        _grid.DefaultCellStyle.SelectionForeColor = Fundo;
    }
}
