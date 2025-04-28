using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using Servidor.Models;
using Servidor_V3.Data;

namespace Servidor_V3.Controllers
{
    public class ArquivosController : Controller
    {
        private readonly BancoContext _bancoContext;

        public ArquivosController(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }

        public IActionResult Index()
        {
            var options = _bancoContext.SelectOptions
                .Select(x => new SelectOptionModel
                {
                    Id = x.Id,
                    Nome = x.Nome
                })
                .ToList();

            var statuses = options.Select(option => new SelectListItem
            {
                Value = option.Id.ToString(),
                Text = option.Nome
            }).ToList();

            ViewBag.Statuses = statuses;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessarArquivo(IFormFile arquivoTxt, IFormFile arquivoExcel, int SelectOptionId)
        {
            if (arquivoTxt == null || arquivoTxt.Length == 0 || arquivoExcel == null || arquivoExcel.Length == 0)
            {
                return Json(new { success = false, message = "Erro nos arquivos enviados. Por favor, envie arquivos válidos." });
            }

            if (SelectOptionId == 0)
            {
                return Json(new { success = false, message = "Selecione um município válido." });
            }

            var selectOptionFromDb = await _bancoContext.SelectOptions
                .FirstOrDefaultAsync(x => x.Id == SelectOptionId);

            if (selectOptionFromDb == null)
            {
                return Json(new { success = false, message = "Município não encontrado no banco de dados." });
            }

            // Processar os arquivos
            var contracheque = await ProcessarArquivoTxt(arquivoTxt, selectOptionFromDb);
            var administrativo = await ProcessarArquivoExcel(arquivoExcel);

            // Salvar os dados no banco
            if (contracheque.Any()) _bancoContext.Contracheque.AddRange(contracheque);
            if (administrativo.Any()) _bancoContext.Administrativo.AddRange(administrativo);
            await _bancoContext.SaveChangesAsync();

            return Ok();
        }


        private async Task<List<ContrachequeModel>> ProcessarArquivoTxt(IFormFile arquivoTxt, SelectOptionModel selectOptionFromDb)
        {
            var registros = new List<ContrachequeModel>();

            using (var stream = arquivoTxt.OpenReadStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string linha;

                while ((linha = reader.ReadLine()) != null)
                {
                    if (linha.StartsWith("F"))
                    {
                        var colunas = linha.Split(';');
                        linha = linha.Trim();

                        if (colunas.Length >= 20)
                        {
                            var item = new ContrachequeModel
                            {
                                Ccoluna1 = colunas[7],
                                Ccoluna2 = colunas[3],
                                Ccoluna3 = colunas[4],
                                Ccoluna4 = colunas[5],
                                Ccoluna5 = "Rua A",
                                Ccoluna6 = "S/N",
                                Ccoluna7 = "CASA",
                                Ccoluna8 = "CENTRO",
                                Ccoluna9 = selectOptionFromDb.Municipio,
                                Ccoluna10 = selectOptionFromDb.Uf,
                                Ccoluna11 = "99999999",
                                Ccoluna12 = "99999999999",
                                Ccoluna13 = "99999999999",
                                Ccoluna14 = "99999999999",
                                Ccoluna15 = colunas[9],
                                Ccoluna16 = string.IsNullOrEmpty(colunas[16]) ? "14" : colunas[16],
                                Ccoluna17 = "0",
                                Ccoluna18 = colunas[18],
                                Ccoluna19 = "0",
                                Ccoluna20 = "Teste@gmail.com",
                                Ccoluna21 = colunas[19],
                                Ccoluna22 = "0",
                                Ccoluna23 = colunas[10],
                                Ccoluna24 = "0",
                                Ccoluna25 = "0",
                                Cargo = colunas[8]
                            };

                            registros.Add(item);
                        }
                    }
                }
            }

            return registros;
        }

        private async Task<List<AdministrativoModel>> ProcessarArquivoExcel(IFormFile arquivoExcel)
        {
            var registros = new List<AdministrativoModel>();

            var categoriaMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"PENSIONISTA", 1},
        {"EFETIVO", 2},
        {"MILITAR", 3},
        {"APOSENTADO", 4},
        {"CONTRATADO", 5},
        {"PRESTADOR DE SERVIÇO", 6},
        {"COMISSIONADO", 7},
        {"ESTAGIARIO", 8},
        {"CELETISTA", 9},
        {"ESTATUTARIO", 10},
        {"Estatutário", 10},
        {"TEMPORARIO", 11},
        {"BENEFICÍARIO", 12},
        {"Beneficiário", 12},
        {"AGENTE POLITICO", 13},
        {"AGUARDANDO ESPECIFICAR", 14},
        {"EFETIVO/COMISSÃO", 15},
        {"ESTÁVEL", 16},
        {"CONSELHEIRO TUTELAR", 17},
        {"REGIME ADMINISTRATIVO", 18},
        {"TRABALHADOR AVULSO", 19},
        {"PENSÃO POR MORTE", 20},
        {"INTERESSE PÚBLICO", 21},
        {"EMPREGO PÚBLICO", 22},
        {"REINTEGRAÇÃO", 23},
        {"REGIME JURÍDICO", 24},
        {"CONTRATADO/COMISSIONADO", 25},
        {"SEM CATEGORIA", 26},
        {"PENSÃO ALIMENTÍCIA", 27},
        {"INATIVO", 28},
        {"FUNÇÃO PÚBLICA RELEVANTE", 29},
        {"PENSÃO ESPECIAL", 30},
        {"Efetivo/Cedido", 31},
        {"Avulsos", 32},
        {"CEDIDO", 33},
        {"Autônomo", 34},
        {"Comissionado/Estatutário", 35},
        {"Temporário/Estatutário", 36},
        {"Concursado", 37},
        {"Contribuinte Individual", 38},
        {"Eletivo", 39},
        {"Estatutário/Agente Político", 41},
        {"Auxílio", 42},
        {"Bolsa Auxílio", 48},
        {"Temporário - CLT", 49},
        {"Prefeito", 51},
        {"TUTELAR", 52},
        {"Temporário",11},
    };

            using (var stream = arquivoExcel.OpenReadStream())
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                {
                    var row = sheet.GetRow(rowIdx);
                    if (row == null) continue;

                    string valorSecretaria = row.GetCell(12)?.ToString().Trim() ?? "";
                    string valorCategoria = row.GetCell(13)?.ToString().Trim() ?? "";

                    var administrativo = new AdministrativoModel
                    {
                        Acoluna1 = row.GetCell(2)?.ToString() ?? "",
                        Acoluna2 = row.GetCell(3)?.ToString().Length >= 10
                                    ? row.GetCell(3).ToString().Substring(row.GetCell(3).ToString().Length - 10)
                                    : row.GetCell(3)?.ToString().PadLeft(10, '0') ?? "0000000000",
                        Acoluna3 = row.GetCell(4)?.ToString() ?? "",
                        Acoluna4 = row.GetCell(11)?.ToString() ?? "",
                        Acoluna5 = categoriaMap.TryGetValue(valorCategoria, out var idCategoria) ? idCategoria.ToString() : row.GetCell(13).ToString(),
                        Acoluna6 = row.GetCell(14)?.ToString() ?? "",
                    };
                    registros.Add(administrativo);
                }
            }

            return registros;
        }
    }
}
