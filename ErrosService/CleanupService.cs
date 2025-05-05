using Microsoft.AspNetCore.Mvc;
using Servidor_V3.Data;

public class CleanupService
{
    private readonly BancoContext _context;

    public CleanupService(BancoContext context)
    {
        _context = context;
    }

    public async Task LimparTudoAsync()
    {
        // Remove todos os registros do banco
        _context.Contracheque.RemoveRange(_context.Contracheque);
        _context.Administrativo.RemoveRange(_context.Administrativo);
        await _context.SaveChangesAsync();
        Console.WriteLine("Tabelas Contracheque e Administrativo limpas com sucesso.");

        // Caminho da pasta de discrepâncias
        var pastaDiscrepancias = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");

        if (Directory.Exists(pastaDiscrepancias))
        {
            try
            {
                // Pega todos os arquivos .txt dentro da pasta e subpastas, se houver
                var arquivosTxt = Directory.GetFiles(pastaDiscrepancias, "*.txt", SearchOption.AllDirectories);

                foreach (var arquivo in arquivosTxt)
                {
                    // Remove atributos especiais (caso sejam apenas leitura, etc.)
                    File.SetAttributes(arquivo, FileAttributes.Normal);

                    // Exclui o arquivo
                    File.Delete(arquivo);
                }

                Console.WriteLine("Arquivos .txt da pasta 'discrepancias' excluídos com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir arquivos .txt da pasta 'discrepancias': {ex.Message}");
            }
        }
    }
}
