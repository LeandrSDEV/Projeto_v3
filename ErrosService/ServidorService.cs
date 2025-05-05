using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using Servidor_V3.Data;

public class ServidorService
{
    private readonly BancoContext _context;

    public ServidorService(BancoContext context)
    {
        _context = context;      
    }

    public async Task GerarEncontradoAsync()
    {
        var contracheques = await _context.Contracheque.AsNoTracking().ToListAsync();
        var administrativos = await _context.Administrativo.AsNoTracking().ToListAsync();

        var discrepancias = new List<ContrachequeModel>();

        // Agrupa por CPF (Ccoluna2)
        var gruposContracheque = contracheques
            .Where(c => !string.IsNullOrWhiteSpace(c.Ccoluna2) && !string.IsNullOrWhiteSpace(c.Ccoluna3))
            .GroupBy(c => c.Ccoluna2.TrimStart('0').Trim());

        foreach (var grupo in gruposContracheque)
        {
            var cpf = grupo.Key;

            var contrachequePorCpf = grupo.ToList();
            var administrativoPorCpf = administrativos
                .Where(a => a.Acoluna1?.TrimStart('0').Trim() == cpf)
                .ToList();

            int diferenca = contrachequePorCpf.Count - administrativoPorCpf.Count;

            if (diferenca > 0)
            {
                var matriculasAdmin = administrativoPorCpf
                    .Select(a => a.Acoluna2?.TrimStart('0').Trim())
                    .ToHashSet();

                var linhasParaAdicionar = contrachequePorCpf
                    .Where(c => !matriculasAdmin.Contains(c.Ccoluna3?.TrimStart('0').Trim()))
                    .Take(diferenca)
                    .ToList();

                discrepancias.AddRange(linhasParaAdicionar);

                foreach (var linha in linhasParaAdicionar)
                {
                    var novaLinha = new AdministrativoModel
                    {
                        Acoluna1 = linha.Ccoluna2,
                        Acoluna2 = linha.Ccoluna3,
                        Acoluna3 = linha.Ccoluna4,
                        Acoluna4 = linha.Ccoluna21,
                        Acoluna5 = linha.Ccoluna16,
                        Acoluna6 = linha.Ccoluna18
                    };

                    _context.Administrativo.Add(novaLinha);
                }
            }
        }

        if (discrepancias.Any())
        {
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Discrepâncias salvas no banco de dados com sucesso.");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro ao salvar no banco: {dbEx.Message}");
                Console.WriteLine($"Detalhes: {dbEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }


            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");
           
            // Garante que a pasta exista
            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var filePath = Path.Combine(pasta, "SERVIDOR.txt");
            await using (var writer = new StreamWriter(filePath))
            {
                foreach (var linha in discrepancias)
                {
                    await writer.WriteLineAsync(FormatarLinha(linha));
                }
            }

            Console.WriteLine("Nenhuma discrepância encontrada. Arquivo não gerado.");
        }
    }

    private string FormatarLinha(ContrachequeModel c)
    {
        return string.Join(';', new[]
        {
            c.Ccoluna1 ?? "",
            c.Ccoluna2 ?? "",
            c.Ccoluna3 ?? "",
            c.Ccoluna4 ?? "",
            c.Ccoluna5 ?? "",
            c.Ccoluna6 ?? "",
            c.Ccoluna7 ?? "",
            c.Ccoluna8 ?? "",
            c.Ccoluna9 ?? "",
            c.Ccoluna10 ?? "",
            c.Ccoluna11 ?? "",
            c.Ccoluna12 ?? "",
            c.Ccoluna13 ?? "",
            c.Ccoluna14 ?? "",
            c.Ccoluna15 ?? "",
            c.Ccoluna16 ?? "",
            c.Ccoluna17 ?? "",
            c.Ccoluna18 ?? "",
            c.Ccoluna19 ?? "",
            c.Ccoluna20 ?? "",
            c.Ccoluna21 ?? "",
            c.Ccoluna22 ?? "",
            c.Ccoluna23 ?? "",
            c.Ccoluna24 ?? "",
            c.Ccoluna25 ?? ""
        });
    }
}
