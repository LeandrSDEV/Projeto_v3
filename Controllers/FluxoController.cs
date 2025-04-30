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
            var dicionarioCcoluna16 = new Dictionary<string, string>
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

            var dados = await _bancoContext.Contracheque
            .Select(x => new
            {
                Ccoluna1 = x.Ccoluna1,
                Ccoluna16 = x.Ccoluna16,
                Ccoluna18 = x.Ccoluna18
            })
            .Distinct()
            .ToListAsync();

            // Agrupar os dados para que Ccoluna1 e Ccoluna16 sejam únicos, e a Ccoluna18 seja definida conforme a combinação dessas duas
            var dadosAgrupados = dados.GroupBy(x => new { x.Ccoluna1, x.Ccoluna16 })
                .Select(group => new CalculoViewModel
                {
                    Ccoluna1Original = group.Key.Ccoluna1, // mantém o valor original de Ccoluna1
                    Ccoluna1Exibicao = group.Key.Ccoluna1.ToString(), // exibe Ccoluna1 como está (sem dicionário)
                    Ccoluna16 = group.Key.Ccoluna16, // mantém o valor original de Ccoluna16 (sem alteração no banco)
                    Ccoluna16Exibicao = dicionarioCcoluna16.GetValueOrDefault(group.Key.Ccoluna16.ToString(), group.Key.Ccoluna16.ToString()), // valor exibido no dicionário ou o original
                    Ccoluna18Atual = group.Select(g => g.Ccoluna18).FirstOrDefault() // Define Ccoluna18 como o valor único relacionado à combinação Ccoluna1 + Ccoluna16
                })
                .ToList();

            return View(dadosAgrupados);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarCalculo(Dictionary<string, string> NovosValores)
        {
            if (NovosValores == null || NovosValores.Count == 0)
            {
                return BadRequest(new { message = "Nenhum valor foi enviado para atualização." });
            }

            int totalAlterados = 0;
            List<string> valoresAlterados = new List<string>();

            using (var transaction = await _bancoContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in NovosValores)
                    {
                        var chaveComposta = item.Key.Trim();
                        var novoValor = item.Value.Trim();

                        if (string.IsNullOrEmpty(chaveComposta) || string.IsNullOrEmpty(novoValor))
                        {
                            continue;
                        }

                        var partes = chaveComposta.Split('|');
                        if (partes.Length != 2)
                        {
                            continue;
                        }

                        var ccoluna1 = partes[0].Trim(); // mantemos como string
                        var ccoluna16 = partes[1].Trim(); // mantemos como string

                        var alterados = await _bancoContext.Contracheque
                            .Where(x => x.Ccoluna1 == ccoluna1 && x.Ccoluna16 == ccoluna16)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Ccoluna18, novoValor));

                        if (alterados > 0)
                        {
                            valoresAlterados.Add($"Valor {ccoluna1} | {ccoluna16} alterado para {novoValor}");
                        }

                        totalAlterados += alterados;
                    }

                    if (totalAlterados > 0)
                    {
                        await transaction.CommitAsync();

                        // Chama os serviços necessários após a alteração
                        await _servidorService.GerarEncontradoAsync();
                        await _matriculaService.GerarMatriculasAsync();
                        await _categoriaService.GerarVinculoAsync();
                        await _secretariaService.GerarSecretariasAsync();
                        await _perfilCalculo.GeradorPerfilCalculo();
                        await _cleanupService.LimparTabelasAsync();

                        return RedirectToAction("Discrepancia");
                    }
                    else
                    {
                        return RedirectToAction("Discrepancia");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return RedirectToAction("Discrepancia");
                }
            }
        }
        public class CalculoViewModel
        {
            public string Ccoluna1Original { get; set; }
            public string Ccoluna1Exibicao { get; set; } // Exibe Ccoluna1 diretamente
            public string Ccoluna16 { get; set; } // Valor original de Ccoluna16 (não alterado)
            public string Ccoluna16Exibicao { get; set; } // Exibe o valor descrito do dicionário para Ccoluna16
            public string Ccoluna18Atual { get; set; }
        }

        //====================   DISCREPANCIA   ====================

        [HttpGet]
        public IActionResult Discrepancia()
        {
            try
            {
                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");

                var servidorPath = Path.Combine(pasta, "SERVIDOR.txt");
                var matriculaPath = Path.Combine(pasta, "MATRICULA.txt");
                var categoriaPath = Path.Combine(pasta, "CATEGORIA.txt");
                var secretariaPath = Path.Combine(pasta, "SECRETARIA.txt");
                var perfilCalculoPath = Path.Combine(pasta, "PERFIL DE CALCULO.txt");

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

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Erro ao carregar as discrepâncias: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult BaixarArquivo(string nome)
        {
            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");
            var caminho = Path.Combine(pasta, nome);

            if (!System.IO.File.Exists(caminho))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(caminho);
            return File(bytes, "text/plain", nome);
        }

        [HttpPost]
        public IActionResult Finalizar()
        {
            var pastaDiscrepancias = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "discrepancias");

            if (Directory.Exists(pastaDiscrepancias))
            {
                // Exclui todas as subpastas e seus conteúdos
                var pastas = Directory.GetDirectories(pastaDiscrepancias);
                foreach (var pasta in pastas)
                {
                    try
                    {
                        Directory.Delete(pasta, true); // true para deletar com conteúdo
                    }
                    catch (Exception ex)
                    {
                        // Log de erro, caso a exclusão falhe
                        Console.WriteLine($"Erro ao deletar pasta: {ex.Message}");
                    }
                }
            }

            // Redireciona para a tela inicial ou outra view após excluir as pastas
            return RedirectToAction("Index", "Home");
        }

    }
}
