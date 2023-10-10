using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Folha_de_Pagamentos.Data;
using Folha_de_Pagamentos.Models;

namespace Folha_de_Pagamentos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolhaDePagamentoesController : ControllerBase
    {
        private readonly Folha_de_PagamentosContext _context;

        public FolhaDePagamentoesController(Folha_de_PagamentosContext context)
        {
            _context = context;
        }

        // GET: api/FolhaDePagamentoes
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<object>>> GetFolhaDePagamento()
        {
            var folhasDePagamento = await _context.FolhaDePagamento.ToListAsync();

            var folhasComFuncionarios = new List<object>();

            foreach (var folha in folhasDePagamento)
            {
                var funcionario = await _context.Funcionario.FindAsync(folha.FuncionarioId);

                double salarioBruto = folha.Valor * folha.Quantidade;

                // Cálculo do IR
                double ir = 0;
                if (salarioBruto <= 1903.98) ir = 0;
                else if (salarioBruto <= 2826.65) ir = salarioBruto * 0.075 - 142.80;
                else if (salarioBruto <= 3751.05) ir = salarioBruto * 0.15 - 354.80;
                else if (salarioBruto <= 4664.68) ir = salarioBruto * 0.225 - 636.13;
                else ir = salarioBruto * 0.275 - 869.36;

                // Cálculo do INSS
                double inss;
                if (salarioBruto <= 1693.72) inss = salarioBruto * 0.08;
                else if (salarioBruto <= 2822.90) inss = salarioBruto * 0.09;
                else if (salarioBruto <= 5645.80) inss = salarioBruto * 0.11;
                else inss = 621.03;

                // Cálculo do FGTS
                double fgts = salarioBruto * 0.08;

                // Cálculo do Salário Líquido
                double salarioLiquido = salarioBruto - ir - inss;

                folhasComFuncionarios.Add(new
                {
                    folha.Id,
                    folha.Valor,
                    folha.Quantidade,
                    folha.Mes,
                    folha.Ano,
                    folha.FuncionarioId,
                    Funcionario = funcionario,
                    Calculos = new
                    {
                        SalarioBruto = salarioBruto,
                        IR = ir,
                        INSS = inss,
                        FGTS = fgts,
                        SalarioLiquido = salarioLiquido
                    }
                });
            }

            return folhasComFuncionarios;
        }

        // GET: api/FolhaDePagamentoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FolhaDePagamento>> GetFolhaDePagamento(int id)
        {
          if (_context.FolhaDePagamento == null)
          {
              return NotFound();
          }
            var folhaDePagamento = await _context.FolhaDePagamento.FindAsync(id);

            if (folhaDePagamento == null)
            {
                return NotFound();
            }

            return folhaDePagamento;
        }

        // POST: api/FolhaDePagamentoes
        [HttpPost("cadastrar")]
        public async Task<ActionResult<FolhaDePagamento>> PostFolhaDePagamento(FolhaDePagamento folhaDePagamento)
        {
            if (_context.FolhaDePagamento == null)
            {
                return Problem("Entity set 'Folha_de_PagamentosContext.FolhaDePagamento' is null.");
            }

            // Buscar o Funcionario baseado no FuncionarioId
            var funcionario = await _context.Funcionario.FindAsync(folhaDePagamento.FuncionarioId);
            if (funcionario == null)
            {
                return NotFound("Funcionario não encontrado.");
            }

            // Adicionar FolhaDePagamento ao contexto e salvar
            _context.FolhaDePagamento.Add(folhaDePagamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolhaDePagamento", new { id = folhaDePagamento.Id }, folhaDePagamento);
        }

        /* 
         * O método puxa a Folha de Pagamento com o Funcionario usando o CPF do mesmo, mês e ano.
         * Também trás o cálculo de impostos na Folha de Pagamento.
         **/
        // GET: api/Folha/buscar/{cpf}/{mes}/{ano}
        [HttpGet("buscar/{cpf}/{mes}/{ano}")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarFolhasPorFuncionario(string cpf, int mes, int ano)
        {
            // Encontra o Funcionário pelo CPF
            var funcionario = await _context.Funcionario.FirstOrDefaultAsync(f => f.CPF == cpf);

            if (funcionario == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            // Busca as Folhas de Pagamento deste Funcionário
            var folhas = await _context.FolhaDePagamento
                .Where(f => f.FuncionarioId == funcionario.Id && f.Mes == mes && f.Ano == ano)
                .ToListAsync();

            if (!folhas.Any())
            {
                return NotFound("Folhas de pagamento não encontradas para este funcionário.");
            }

            var folhasComCalculos = new List<object>();

            foreach (var folha in folhas)
            {
                double salarioBruto = folha.Valor * folha.Quantidade;

                // Cálculo do IR
                double ir = 0;
                if (salarioBruto <= 1903.98) ir = 0;
                else if (salarioBruto <= 2826.65) ir = salarioBruto * 0.075 - 142.80;
                else if (salarioBruto <= 3751.05) ir = salarioBruto * 0.15 - 354.80;
                else if (salarioBruto <= 4664.68) ir = salarioBruto * 0.225 - 636.13;
                else ir = salarioBruto * 0.275 - 869.36;

                // Cálculo do INSS
                double inss;
                if (salarioBruto <= 1693.72) inss = salarioBruto * 0.08;
                else if (salarioBruto <= 2822.90) inss = salarioBruto * 0.09;
                else if (salarioBruto <= 5645.80) inss = salarioBruto * 0.11;
                else inss = 621.03;

                // Cálculo do FGTS
                double fgts = salarioBruto * 0.08;

                // Cálculo do Salário Líquido
                double salarioLiquido = salarioBruto - ir - inss;

                // Cálculo do IR, INSS, FGTS, e Salário Líquido... 
               
                folhasComCalculos.Add(new
                {
                    folha.Id,
                    folha.Valor,
                    folha.Quantidade,
                    folha.Mes,
                    folha.Ano,
                    folha.FuncionarioId,
                    Funcionario = funcionario,
                    Calculos = new
                    {
                        SalarioBruto = salarioBruto,
                        IR = ir,
                        INSS = inss,
                        FGTS = fgts,
                        SalarioLiquido = salarioLiquido
                    }
                });
            }

            return folhasComCalculos;
        }
    }
}
