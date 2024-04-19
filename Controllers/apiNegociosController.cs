using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using apiNegocios_13.Models;
namespace apiNegocios_13.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class apiNegociosController : ControllerBase
    {
        //cadena de conexion:
        string cadena = @"server = DESKTOP-2RMOSKH\SQLEXPRESS; database = Negocios2022; Trusted_Connection = True;" +
            "MultipleActiveResultSets = True; TrustServerCertificate = False;Encrypt = False";

        IEnumerable<Pais> getpaises() //RETORNA LA LISTA DE LOS REGIUSTROS DE tb_paises
        {
            List<Pais> temporal = new List<Pais>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from tb_paises", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Pais()
                    {
                        idpais = dr.GetString(0),
                        nombrepais = dr.GetString(1)
                    });
                }
                cn.Close();
            }
            return temporal;
        }
        IEnumerable<Cliente> getclientes()
        {
            List<Cliente> clientes = new List<Cliente>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from tb_clientes", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    clientes.Add(new Cliente()
                    {
                        idcliente = dr.GetString(0),
                        nombrecia = dr.GetString(1),
                        direccion = dr.GetString(2),
                        idpais = dr.GetString(3),
                        telefono = dr.GetString(4)
                    });
                }
                cn.Close();
            }
            return clientes;
        }
        Cliente buscar(string idcliente)
        {
            /*retornar el registro de tb_clientes por su incliente*/
            return getclientes().FirstOrDefault(c => c.idcliente == idcliente);
        }
        /*AGREGAR*/
        string agregar(Cliente reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(
                                   "insert into tb_clientes values(@idcliente, @nombrecia, @direccion, @idpais ,@telefono)", cn);
                    cmd.Parameters.AddWithValue("@idcliente", reg.idcliente);
                    cmd.Parameters.AddWithValue("@nombrecia", reg.nombrecia);
                    cmd.Parameters.AddWithValue("@direccion", reg.direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idpais);
                    cmd.Parameters.AddWithValue("@telefono", reg.telefono);
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha insertado el registro del codigo {reg.idcliente}";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        /*ACTUALIZAR*/
        string actualizar(string idcliente, Cliente reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(
                        "Update tb_clientes Set nombrecia=@nombrecia ,direccion=@direccion, idpais =@idpais ," +
                        "telefono= @telefono Where idcliente=@idcliente", cn);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente); // Utilizamos el idcliente recibido como parámetro
                    cmd.Parameters.AddWithValue("@nombrecia", reg.nombrecia);
                    cmd.Parameters.AddWithValue("@direccion", reg.direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idpais);
                    cmd.Parameters.AddWithValue("@telefono", reg.telefono);
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado el registro del codigo {idcliente}"; // Utilizamos idcliente aquí también
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }


        /*ELIMINAR*/
        string eliminar(string idcliente)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM tb_clientes WHERE idcliente = @idcliente", cn);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        mensaje = $"Se ha eliminado el registro del cliente con el ID {idcliente}.";
                    }
                    else
                    {
                        mensaje = $"No se encontró ningún registro con el ID {idcliente}.";
                    }
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally
                {
                    cn.Close();
                }
            }
            return mensaje;
        }

        [HttpGet("paises")]public async Task<ActionResult<IEnumerable<Pais>>> paises()
        {
            return Ok(await Task.Run(() => getpaises()));
        }
        [HttpGet("clientes")] public async Task<ActionResult<IEnumerable<Cliente>>> cliente()
        {
            return Ok(await Task.Run(() => getclientes()));
        }
        [HttpGet("buscar/{idcliente}")] public async Task<ActionResult<Cliente>> buscaqueda(string idcliente)
        {
            return Ok(await Task.Run(() => buscar(idcliente)));
        }
        [HttpPost("agregar")] public async Task<ActionResult<string>> agregarcliente(Cliente reg)
        {
            return Ok(await Task.Run(() => agregar(reg)));
        }
        
        [HttpPut("actualizar/{idcliente}")]
        public async Task<ActionResult<string>> actualizarcliente(string idcliente, Cliente reg)
        {
            return Ok(await Task.Run(() => actualizar(idcliente, reg)));
        }

        [HttpDelete("eliminar/{idcliente}")]
        public async Task<ActionResult<string>> eliminarcliente(string idcliente)
        {
            return Ok(await Task.Run(() => eliminar(idcliente)));
        }
    }
}