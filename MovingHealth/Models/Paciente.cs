namespace MovingHealth.Models;

public enum Prioridade
{
    Urgente = 1,
    Alta = 2,
    Normal = 3
}

public enum StatusAtendimento
{
    Aguardando,
    Atendido,
    Cancelado
}

public class Paciente
{
    private string _nome = "";
    private int _idade;

    public int Id { get; set; }
    public string Senha { get; set; } = "";

    // Encapsulamento: o nome e a idade só aceitam valores válidos
    public string Nome
    {
        get => _nome;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome é obrigatório.");
            _nome = value.Trim();
        }
    }

    public int Idade
    {
        get => _idade;
        set
        {
            if (value < 0 || value > 120)
                throw new ArgumentException("Idade inválida.");
            _idade = value;
        }
    }

    public bool Pcd { get; set; }
    public bool Gestante { get; set; }
    public bool SinaisDeAlerta { get; set; }

    public Prioridade Prioridade { get; set; } = Prioridade.Normal;
    public StatusAtendimento Status { get; set; } = StatusAtendimento.Aguardando;

    public DateTime Chegada { get; set; } = DateTime.Now;
    public DateTime? Atendimento { get; set; }

    // Regra de decisão da prioridade
    public Prioridade CalcularPrioridade()
    {
        if (SinaisDeAlerta)
            return Prioridade.Urgente;

        if (Idade >= 60 || Pcd || Gestante)
            return Prioridade.Alta;

        return Prioridade.Normal;
    }
}
