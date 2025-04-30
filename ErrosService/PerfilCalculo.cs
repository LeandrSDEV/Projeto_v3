using Microsoft.EntityFrameworkCore;
using Servidor_V3.Data;
using Servidor.Models;


public class PerfilCalculo
{
    private readonly BancoContext _bancoContext;

    public PerfilCalculo(BancoContext bancoContext)
    {
        _bancoContext = bancoContext;
    }

    //============================== PERFIL DE CÁLCULO ==============================//
    public async Task GeradorPerfilCalculo()
    {
        var tabelaTxt = await _bancoContext.Contracheque.AsNoTracking().ToListAsync();
        var tabelaExcel = await _bancoContext.Administrativo.AsNoTracking().ToListAsync();

        // Criar dicionário de comparações baseadas em CPF + Matrícula
        var administrativosMap = tabelaExcel
            .Where(a => !string.IsNullOrWhiteSpace(a.Acoluna1) && !string.IsNullOrWhiteSpace(a.Acoluna2) && !string.IsNullOrWhiteSpace(a.Acoluna6))
            .GroupBy(a => $"{a.Acoluna1.Trim()}{a.Acoluna2.Trim()}")
            .ToDictionary(
                g => g.Key,
                g => ExtrairNumeroInteiro(g.First().Acoluna6)
            );

        var discrepancias = new List<ContrachequeModel>();

        foreach (var linha in tabelaTxt)
        {
            if (string.IsNullOrWhiteSpace(linha.Ccoluna2) || string.IsNullOrWhiteSpace(linha.Ccoluna3) || string.IsNullOrWhiteSpace(linha.Ccoluna18))
                continue;

            var chave = $"{linha.Ccoluna2.Trim()}{linha.Ccoluna3.Trim()}";
            var valorTxt = ExtrairNumeroInteiro(linha.Ccoluna18);

            if (administrativosMap.TryGetValue(chave, out int valorExcel))
            {
                if (valorTxt != valorExcel)
                {
                    discrepancias.Add(linha); // Só adiciona se for diferente
                }
            }
            // Se não encontrar, ignora (não é considerado discrepância nesse cenário)
        }

        var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");
     
        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var filePath = Path.Combine(pasta, "PERFIL DE CALCULO.txt");

        if (discrepancias.Any())
        {
            using var writer = new StreamWriter(filePath);
            var chavesRegistradas = new HashSet<string>();

            foreach (var item in discrepancias)
            {
                var cpf = item.Ccoluna2?.Trim() ?? "";
                var matricula = item.Ccoluna3?.Trim() ?? "";
                var valor = ExtrairNumeroInteiro(item.Ccoluna18).ToString();

                var chaveUnica = $"{cpf}{matricula}";

                if (!chavesRegistradas.Contains(chaveUnica))
                {
                    await writer.WriteLineAsync($"{cpf};{matricula};{valor}");
                    chavesRegistradas.Add(chaveUnica);
                }
            }           
        }      
            Console.WriteLine("✅ Nenhuma discrepância de valor encontrada.");     
    }

    private int ExtrairNumeroInteiro(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;
        var numeros = new string(input.Where(char.IsDigit).ToArray());
        return int.TryParse(numeros, out int result) ? result : 0;
    }

}

