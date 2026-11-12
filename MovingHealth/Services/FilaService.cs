using System.Text.Json;
using MovingHealth.Models;

namespace MovingHealth.Services;

public class FilaService
{
    private readonly string _arquivo;
    private List<Paciente> _pacientes = new();
    private int _proximoId = 1;

    public FilaService(string arquivo)
    {
        _arquivo = arquivo;
        Carregar();
    }

    // Fila de espera: primeiro por prioridade, depois por ordem de chegada
    public List<Paciente> Fila()
    {
        return _pacientes
            .Where(p => p.Status == StatusAtendimento.Aguardando)
            .OrderBy(p => p.Prioridade)
            .ThenBy(p => p.Chegada)
            .ToList();
    }

    public int TotalAtendidos()
    {
        return _pacientes.Count(p => p.Status == StatusAtendimento.Atendido);
    }

    // Indicador de impacto: tempo médio de espera dos atendidos (em minutos)
    public double TempoMedioEspera()
    {
        var atendidos = _pacientes.Where(p => p.Status == StatusAtendimento.Atendido && p.Atendimento != null).ToList();
        if (atendidos.Count == 0)
            return 0;

        return atendidos.Average(p => (p.Atendimento!.Value - p.Chegada).TotalMinutes);
    }

    public Paciente Adicionar(string nome, int idade, bool pcd, bool gestante, bool sinaisDeAlerta)
    {
        var paciente = new Paciente
        {
            Id = _proximoId++,
            Nome = nome,
            Idade = idade,
            Pcd = pcd,
            Gestante = gestante,
            SinaisDeAlerta = sinaisDeAlerta,
            Chegada = DateTime.Now
        };

        paciente.Prioridade = paciente.CalcularPrioridade();

        // Senha: letra da prioridade + número (ex.: U001, A002, N003)
        char letra = paciente.Prioridade == Prioridade.Urgente ? 'U'
                   : paciente.Prioridade == Prioridade.Alta ? 'A' : 'N';
        paciente.Senha = $"{letra}{paciente.Id:000}";

        _pacientes.Add(paciente);
        Salvar();
        return paciente;
    }

    public Paciente? ChamarProximo()
    {
        var proximo = Fila().FirstOrDefault();
        if (proximo == null)
            return null;

        proximo.Status = StatusAtendimento.Atendido;
        proximo.Atendimento = DateTime.Now;
        Salvar();
        return proximo;
    }

    public void Cancelar(int id)
    {
        var paciente = _pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente == null)
            return;

        paciente.Status = StatusAtendimento.Cancelado;
        Salvar();
    }

    private void Salvar()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_arquivo)!);
        string json = JsonSerializer.Serialize(_pacientes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_arquivo, json);
    }

    private void Carregar()
    {
        if (!File.Exists(_arquivo))
            return;

        try
        {
            string json = File.ReadAllText(_arquivo);
            _pacientes = JsonSerializer.Deserialize<List<Paciente>>(json) ?? new List<Paciente>();
            if (_pacientes.Count > 0)
                _proximoId = _pacientes.Max(p => p.Id) + 1;
        }
        catch (Exception)
        {
            // Arquivo corrompido: começa com a fila vazia
            _pacientes = new List<Paciente>();
        }
    }
}
