using System.ComponentModel.DataAnnotations.Schema;

namespace Folha_de_Pagamentos.Models
{
    public class FolhaDePagamento
    {
        public int Id { get; set; }
        public double Valor { get; set; }
        public int Quantidade { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }

        // Chave estrangeira para o Funcionário
        public int FuncionarioId { get; set; }
    }
}
