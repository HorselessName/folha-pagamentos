using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Folha_de_Pagamentos.Models;
using Folha_de_Pagamentos.Data;

namespace Folha_de_Pagamentos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionariosController : ControllerBase
    {
        private readonly Folha_de_PagamentosContext _context;

        public FuncionariosController(Folha_de_PagamentosContext context)
        {
            _context = context;
        }

        // GET: api/Funcionarios
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Funcionario>>> GetFuncionario()
        {
            if (_context.Funcionario == null)
            {
                return NotFound();
            }
            return await _context.Funcionario.ToListAsync();
        }

        // Exemplo: GET - api/Funcionarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Funcionario>> GetFuncionario(int id)
        {
            if (_context.Funcionario == null)
            {
                return NotFound();
            }
            var funcionario = await _context.Funcionario.FindAsync(id);

            if (funcionario == null)
            {
                return NotFound();
            }

            return funcionario;
        }

        /**
         * O POST a seguir, do modelo "Funcionário", faz o cadastro do "Funcionário".
         * Ele espera receber: Nome, CPF.
         * O ID é Auto Incrementado pelo Entity Framework.
         * 
         * URL: POST {{base_url}}/funcionario/cadastrar
         * O método é feito usando o HttpPost, e a URL para cadastrar é o /cadastrar
         **/
        [HttpPost("cadastrar")]
        public async Task<ActionResult<Funcionario>> PostFuncionario(Funcionario funcionario)
        {
            try
            {
                _context.Funcionario.Add(funcionario);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetFuncionario", new { id = funcionario.Id }, funcionario);
            }
            catch (Exception ex)
            {
                // Aqui você pode logar a exceção "ex" se quiser.
                return Problem("Um erro ocorreu ao tentar adicionar o funcionário.");
            }
        }
    }
}
