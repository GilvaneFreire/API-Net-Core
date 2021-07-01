using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class TruckController : ControllerBase
    {
      
        [HttpGet]
        [Route("Veiculos")]
        public IEnumerable<Truck> Get()
        {
            var connection = new MySql.Data.MySqlClient.MySqlConnection("Server=host;Database=dbname;Uid=root;Pwd=password;");

            connection.Open();
            List<Truck> listTruck = new List<Truck>();
            Truck truck = null;

            try
            {
                String sql = @"SELECT va.idVeiculoSascar AS idVeiculo,
                                       v.placa AS placa,
                                       IFNULL(va.dataAtualizacaoSascar, '0000-00-00') AS dataPacote,
                                       IFNULL(c.status_viagem, 'Vazio') AS status,
                                       c.nr_viagem AS viagem,
                                       cli.nome AS cliente,
                                       c.orig_dest,
                                       convert(c.previsao_entrega, char) AS prevEntrega,
                                       coop.nome AS cooperado,
                                       mot.nome AS motorista,
                                       tv.codigo AS cod_TipoVeiculo,
                                       tv.descricao AS nomeTipoVeiculo,
                                       c.placa_cavalinho AS placaCavalo,
                                       va.lat AS cltlat,
                                       va.lng AS cltlng,
                                       va.cidade_atual AS cidadeAtual,
                                       va.uf_atual AS ufAtual,
                                       c.cod_destinatario,
                                       c.cod_cliente,
                                       dest.nome AS destinatario,
                                       DATEDIFF(CURDATE(), va.dataAtualizacaoSascar) AS dias_sem_atualizar
                                FROM veiculo v
                                LEFT JOIN carga c ON v.placa = c.placa_carreta_1
                                AND c.status_viagem in ('Em Andamento',
                                                        'Agendada',
                                                        'Autorizada')
                                LEFT JOIN tipo_veiculo tv ON tv.codigo = v.tipoVeiculo
                                LEFT JOIN motorista mot on mot.cod_motorista = c.cod_motorista
                                LEFT JOIN veic_aux va ON v.placa = va.placa
                                LEFT JOIN cooperado coop ON v.cod_cooperado = coop.cod_cooperado
                                LEFT JOIN cliente dest ON dest.cod_cliente = c.cod_destinatario
                                LEFT JOIN cliente_aux des_aux ON des_aux .cod_cliente = dest.cod_cliente
                                LEFT JOIN cliente cli ON cli.cod_cliente = c.cod_cliente
                                WHERE va.idVeiculoSascar IS NOT NULL
                                  AND va.dataAtualizacaoSascar IS NOT NULL";
                using (var command = new MySql.Data.MySqlClient.MySqlCommand(sql, connection))
                {
                    MySqlDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        truck = new Truck();
                        truck.IdVeiculo = (int)rdr["idVeiculo"];
                        truck.Placa = rdr["placa"].ToString();
                        truck.DataPacote = rdr["dataPacote"].ToString();
                        truck.Status = rdr["status"].ToString();
                        truck.Viagem = rdr["viagem"].ToString();
                        truck.Cliente = rdr["cliente"].ToString();
                        truck.Destino = rdr["orig_dest"].ToString();
                        truck.PrevEntrega = rdr["prevEntrega"].ToString();
                        truck.Cooperado =   rdr["cooperado"].ToString();
                        truck.Motorista = rdr["motorista"].ToString();
                        truck.TipoVeiculo = rdr["cod_TipoVeiculo"].ToString();
                        truck.NomeTipoVeiculo = rdr["nomeTipoVeiculo"].ToString();
                        truck.PlacaCavalo = rdr["placaCavalo"].ToString();
                        truck.Cltlat = (double)rdr["cltlat"];
                        truck.Cltlng = (double)rdr["cltlng"];
                        truck.CidadeAtual = rdr["cidadeAtual"].ToString();
                        truck.UFatual = rdr["ufAtual"].ToString();
                        truck.Destinatario = rdr["destinatario"].ToString();
                        truck.DiasSemAtualizar = rdr["dias_sem_atualizar"].ToString();

                        listTruck.Add(truck);
                    }
                    rdr.Close();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {

            }
            return (IEnumerable<Truck>)listTruck;


        }

        List<TipoVeiculo> listTipos = new List<TipoVeiculo>();
        TipoVeiculo tipo = null;


        [HttpGet]
        [Route("TipoVeiculos")]
        public IEnumerable<TipoVeiculo> GetTipo()
        {
            var connection = new MySql.Data.MySqlClient.MySqlConnection("Server=host;Database=dbname;Uid=root;Pwd=password;");

            connection.Open();

            try
            {
                using (var command = new MySql.Data.MySqlClient.MySqlCommand("select codigo, descricao from tipo_veiculo", connection))
                {
                    MySqlDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        tipo = new TipoVeiculo();
                        tipo.Codigo = (string)rdr["codigo"];
                        tipo.Descricao = (string)rdr["descricao"];

                        listTipos.Add(tipo);
                    }
                    rdr.Close();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {

            }
            return (IEnumerable<TipoVeiculo>)listTipos;
        }

        [HttpGet]
        [Route("DataEntrega")]
        public IEnumerable<Truck> GetEntrega()
        {
            var connection = new MySql.Data.MySqlClient.MySqlConnection("Server=host;Database=dbname;Uid=root;Pwd=password;");

            connection.Open();
            List<Truck> listTruck = new List<Truck>();
            Truck truck = null;

            try
            {
                String sql = @"SELECT va.idVeiculoSascar AS idVeiculo,
                                       c.placa_carreta_1 AS placa,
                                       IFNULL(va.dataAtualizacaoSascar,'0000-00-00' ) AS dataPacote,
                                       c.status_viagem AS status,
                                       c.nr_viagem AS viagem,
                                       cli.nome AS cliente,
                                       c.orig_dest,
                                       convert(c.previsao_entrega, char) AS prevEntrega,
                                       coop.nome AS cooperado,
                                       mot.nome as motorista,
                                       tv.codigo AS cod_TipoVeiculo,
                                       tv.descricao AS nomeTipoVeiculo,
                                       c.placa_cavalinho AS placaCavalo,
                                       des_aux.lat AS cltlat,
                                       des_aux.lng AS cltlng,
                                       c.cod_destinatario,
                                       c.cod_cliente,
                                       dest.nome AS destinatario,
                                       DATEDIFF(CURDATE(), va.dataAtualizacaoSascar) AS dias_sem_atualizar                                       
                                FROM carga c
                                JOIN veiculo v ON v.placa = c.placa_carreta_1
                                LEFT JOIN tipo_veiculo tv ON tv.codigo = v.tipoVeiculo
                                LEFT JOIN motorista mot on mot.cod_motorista = c.cod_motorista
                                LEFT JOIN veic_aux va ON v.placa = va.placa
                                LEFT JOIN cooperado coop ON v.cod_cooperado = coop.cod_cooperado
                                LEFT JOIN cliente dest ON dest.cod_cliente = c.cod_destinatario
                                LEFT JOIN cliente_aux des_aux ON des_aux .cod_cliente = dest.cod_cliente
                                JOIN cliente cli ON cli.cod_cliente = c.cod_cliente
                                WHERE status_viagem in ('Em Andamento',
                                                        'Agendada',
                                                        'Autorizada')
                                  and va.idVeiculoSascar is not null
                                  and va.dataAtualizacaoSascar is not null
                                  and des_aux.lat is not null";
                using (var command = new MySql.Data.MySqlClient.MySqlCommand(sql, connection))
                {
                    MySqlDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        truck = new Truck();
                        truck.IdVeiculo = (int)rdr["idVeiculo"];
                        truck.Placa = (string)rdr["placa"];
                        truck.DataPacote = (string)rdr["dataPacote"];
                        truck.Status = (string)rdr["status"];
                        truck.Viagem = (string)rdr["viagem"];
                        truck.Cliente = (string)rdr["cliente"];
                        truck.Destino = (string)rdr["orig_dest"];
                        truck.PrevEntrega = (string)rdr["prevEntrega"];
                        truck.Cooperado = (string)rdr["cooperado"];
                        truck.Motorista = rdr["motorista"].ToString();
                        truck.TipoVeiculo = (string)rdr["cod_TipoVeiculo"];
                        truck.NomeTipoVeiculo = (string)rdr["nomeTipoVeiculo"];
                        truck.PlacaCavalo = (string)rdr["placaCavalo"];
                        truck.Cltlat = (double)rdr["cltlat"];
                        truck.Cltlng = (double)rdr["cltlng"];
                        truck.Destinatario = (string)rdr["destinatario"];
                        truck.DiasSemAtualizar = rdr["dias_sem_atualizar"].ToString();

                        listTruck.Add(truck);
                    }
                    rdr.Close();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {

            }
            return (IEnumerable<Truck>)listTruck;


        }
    }
}


