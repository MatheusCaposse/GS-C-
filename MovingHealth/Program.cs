using MovingHealth.Forms;
using MovingHealth.Services;

namespace MovingHealth;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Os dados ficam em um arquivo JSON ao lado do executável (persistência básica)
        string caminho = Path.Combine(AppContext.BaseDirectory, "dados", "fila.json");

        Application.Run(new MainForm(new FilaService(caminho)));
    }
}
