using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor_V3.Data;
using Servidor_V3.Models;

namespace Servidor_V3.Controllers
{
    public class FluxoController : Controller
    {
        private readonly BancoContext _bancoContext;
        private readonly ServidorService _servidorService;
        private readonly CategoriaService _categoriaService;
        private readonly MatriculaService _matriculaService;
        private readonly SecretariaService _secretariaService;
        private readonly PerfilCalculo _perfilCalculo;
        private readonly CleanupService _cleanupService;

        public FluxoController(BancoContext bancoContext, ServidorService servidorService, CategoriaService categoriaService,
                                  MatriculaService matriculaService, SecretariaService secretariaService, PerfilCalculo perfilCalculo,
                                  CleanupService cleanupService)
        {
            _bancoContext = bancoContext;
            _servidorService = servidorService;
            _categoriaService = categoriaService;
            _matriculaService = matriculaService;
            _secretariaService = secretariaService;
            _perfilCalculo = perfilCalculo;
            _cleanupService = cleanupService;
        }

        //====================   FLUXO PREFEITURA ====================

        // Exibe a view da Prefeitura com os valores distintos de Ccoluna1
        [HttpGet]
        public async Task<IActionResult> Prefeitura()
        {
            var valores = await _bancoContext.Contracheque
                .Select(x => x.Ccoluna1)
                .Distinct()
                .ToListAsync();

            ViewBag.Valores = valores;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarPrefeitura(List<string> ValoresAntigos, List<string> NovosValores)
        {
            if (ValoresAntigos == null || NovosValores == null || ValoresAntigos.Count != NovosValores.Count)
                return BadRequest("Dados inválidos.");

            for (int i = 0; i < ValoresAntigos.Count; i++)
            {
                var valorAntigo = ValoresAntigos[i];
                var novoValor = NovosValores[i];

                if (!string.IsNullOrEmpty(novoValor))
                {
                    var registros = await _bancoContext.Contracheque
                        .Where(x => x.Ccoluna1 == valorAntigo)
                        .ToListAsync();

                    foreach (var registro in registros)
                    {
                        registro.Ccoluna21 = novoValor;
                    }
                }
            }

            await _bancoContext.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Valores atualizados com sucesso!";
            return RedirectToAction("Categoria");
        }

        //====================   FLUXO CARGO ====================

        [HttpGet]
        public async Task<IActionResult> Cargo()
        {
            var valores = await _bancoContext.Contracheque
                .Select(x => x.Cargo)
                .Distinct()
                .ToListAsync();

            ViewBag.Valores = valores;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarCargo(List<string> ValoresAntigos, List<string> NovosValores)
        {
            if (ValoresAntigos == null || NovosValores == null || ValoresAntigos.Count != NovosValores.Count)
                return BadRequest("Dados inválidos.");

            for (int i = 0; i < ValoresAntigos.Count; i++)
            {
                var valorAntigo = ValoresAntigos[i];
                var novoValor = NovosValores[i];

                if (!string.IsNullOrEmpty(novoValor))
                {
                    var registros = await _bancoContext.Contracheque
                        .Where(x => x.Cargo == valorAntigo)
                        .ToListAsync();

                    foreach (var registro in registros)
                    {
                        registro.Ccoluna21 = novoValor;
                    }
                }
            }

            await _bancoContext.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Valores atualizados com sucesso!";
            return RedirectToAction("Categoria");
        }

        //====================   FLUXOS   ====================

        //====================   CATEGORIA   ====================

        [HttpGet]
        public async Task<IActionResult> Categoria()
        {
            var valoresDistintos = await _bancoContext.Contracheque
                .Select(x => x.Ccoluna16)
                .Distinct()
                .ToListAsync();

            ViewBag.ValoresDistintos = valoresDistintos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarCategoria([FromForm] Dictionary<string, string> NovosValores)
        {
            if (NovosValores == null || !NovosValores.Any())
            {
                return Json(new { success = false, message = "Nenhum valor foi enviado para atualização." });
            }

            foreach (var item in NovosValores)
            {
                var valorAntigo = item.Key;
                var novoValor = item.Value;

                if (!string.IsNullOrEmpty(novoValor))
                {
                    var registros = await _bancoContext.Contracheque
                        .Where(x => x.Ccoluna16 == valorAntigo)
                        .ToListAsync();

                    foreach (var registro in registros)
                    {
                        registro.Ccoluna16 = novoValor; // Atualiza o valor
                    }
                }
            }

            await _bancoContext.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Valores atualizados com sucesso!";
            return RedirectToAction("Calculo");
        }

        //====================   CALCULO   ====================

        [HttpGet]
        public async Task<IActionResult> Calculo()
        {
            var categorias = new Dictionary<string, string>
{
    { "1", "Pensionista" }, { "2", "Efetivo" }, { "3", "Militar" }, { "4", "Aposentado" },
    { "5", "Contratado" }, { "6", "Prestador de Serviço" }, { "7", "Comissionado" },
    { "8", "Estagiario" }, { "9", "Celetista" }, { "10", "Estatutario" }, { "11", "Temporario" },
    { "12", "Beneficiario" }, { "13", "Agente Politico" }, { "14", "Aguardando Especificar" },
    { "15", "Efetivo/Comissão" }, { "16", "Estável" }, { "17", "CConselheiro Tutelar" },
    { "18", "Regime Administrativo" }, { "19", "Trabalhador Avulso" }, { "20", "Pensão por Morte" },
    { "21", "Interesse Público" }, { "22", "Emprego Público" }, { "23", "Reintegração" },
    { "24", "Regime Jurídico" }, { "25", "Contratado/Comissionado" }, { "26", "Sem Categoria" },
    { "27", "Pensão Alimenticia" }, { "28", "Inativo" }, { "29", "Função Pública Relevante" },
    { "30", "Pensão Especial" }, { "31", "Efetivo/Cedido" }, { "32", "Avulsos" }, { "33", "Cedido" },
    { "34", "Autônomo" }, { "35", "Comissionado/Estatutário" }, { "36", "Temporário/Estatutário" },
    { "37", "Concursado" }, { "38", "Contribuinte Individual" }, { "39", "Eletivo" },
    { "41", "Estatutário/Agente Político" }, { "42", "Auxílio" }, { "48", "Bolsa Auxílio" },
    { "49", "Temporário - CLT" }, { "51", "Prefeito" }, { "52", "Tutelar" }
};

            var valoresCcoluna16 = await _bancoContext.Contracheque
                .Select(x => x.Ccoluna16)
                .Distinct()
                .ToListAsync();

            var valoresFormatados = valoresCcoluna16
                .Select(valor => $"{valor} - {categorias.GetValueOrDefault(valor, "DESCONHECIDO")}")
                .ToList();

            ViewBag.ValoresDistintos = valoresFormatados;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarCalculo(Dictionary<string, string> NovosValores)
        {
            if (NovosValores == null || NovosValores.Count == 0)
            {
                return BadRequest(new { message = "Nenhum valor foi enviado para atualização." });
            }

            int totalAlterados = 0;
            List<string> valoresAlterados = new List<string>(); // Lista para armazenar os valores alterados

            // Inicia a transação
            using (var transaction = await _bancoContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Percorre cada item enviado
                    foreach (var item in NovosValores)
                    {
                        var valorOriginal = item.Key.Trim();
                        var novoValor = item.Value.Trim();

                        // Verifica se o valor original não está vazio e o novo valor é válido
                        if (string.IsNullOrEmpty(valorOriginal) || string.IsNullOrEmpty(novoValor))
                        {
                            continue; // Ignora entradas com valores inválidos
                        }

                        // Realiza a atualização
                        var alterados = await _bancoContext.Contracheque
                            .Where(x => x.Ccoluna16 == valorOriginal)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Ccoluna18, novoValor));

                        // Verifica se algum valor foi alterado
                        if (alterados > 0)
                        {
                            valoresAlterados.Add($"Valor {valorOriginal} alterado para {novoValor}");
                        }

                        totalAlterados += alterados;
                    }

                    // Se algum valor foi alterado, realiza o commit
                    if (totalAlterados > 0)
                    {
                        await transaction.CommitAsync();

                        // Chama os serviços após a atualização
                        await _servidorService.GerarEncontradoAsync();
                        await _matriculaService.GerarMatriculasAsync();
                        await _categoriaService.GerarVinculoAsync();
                        await _secretariaService.GerarSecretariasAsync();
                        await _perfilCalculo.GeradorPerfilCalculo();
                        await _cleanupService.LimparTabelasAsync();

                        // Redireciona para a view Discrepancia após sucesso
                        return RedirectToAction("Discrepancia");
                    }
                    else
                    {
                        // Caso nenhum valor tenha sido alterado, ainda redireciona para a view Discrepancia
                        return RedirectToAction("Discrepancia");
                    }
                }
                catch (Exception ex)
                {
                    // Caso ocorra erro, realiza o rollback e retorna erro com detalhes
                    await transaction.RollbackAsync();
                    return RedirectToAction("Discrepancia");
                }
            }
        }

        //====================   DISCREPANCIA   ====================

        [HttpGet]
        public IActionResult Discrepancia()
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                var servidorPath = Path.Combine(desktopPath, "SERVIDOR.txt");
                var matriculaPath = Path.Combine(desktopPath, "MATRICULA.txt");
                var categoriaPath = Path.Combine(desktopPath, "CATEGORIA.txt");
                var secretariaPath = Path.Combine(desktopPath, "SECRETARIAS.txt");
                var perfilCalculoPath = Path.Combine(desktopPath, "PERFIL DE CALCULO.txt");

                int servidor = System.IO.File.Exists(servidorPath) ? System.IO.File.ReadAllLines(servidorPath).Length : 0;
                int matricula = System.IO.File.Exists(matriculaPath) ? System.IO.File.ReadAllLines(matriculaPath).Length : 0;
                int categoria = System.IO.File.Exists(categoriaPath) ? System.IO.File.ReadAllLines(categoriaPath).Length : 0;
                int secretaria = System.IO.File.Exists(secretariaPath) ? System.IO.File.ReadAllLines(secretariaPath).Length : 0;
                int perfilCalculo = System.IO.File.Exists(perfilCalculoPath) ? System.IO.File.ReadAllLines(perfilCalculoPath).Length : 0;

                var model = new DiscrepanciaModel
                {
                    Servidor = servidor,
                    Matricula = matricula,
                    Categoria = categoria,
                    Secretaria = secretaria,
                    PerfilCalculo = perfilCalculo
                };

                // Retorna a view com o modelo preenchido
                return View(model);
            }
            catch (Exception ex)
            {
                // Em caso de erro, exibe a mensagem de erro na View
                ViewBag.ErrorMessage = "Erro ao carregar as discrepâncias: " + ex.Message;
                return View();
            }
        }

    }
}
