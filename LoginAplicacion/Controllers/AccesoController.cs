using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using LoginAplicacion.Models;

namespace LoginAplicacion.Controllers
{
	public class AccesoController : Controller
	{
		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(Usuario oUsuario)
		{
			// Validar si faltan campos
			if (string.IsNullOrEmpty(oUsuario.IdUsuario) || string.IsNullOrEmpty(oUsuario.clave))
			{
				ViewBag.ErrorMessage = "Usuario y contraseña son requeridos.";
				return View();
			}

			try
			{
				string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cn"].ConnectionString;

				using (SqlConnection con = new SqlConnection(connectionString))
				{
					using (SqlCommand cmd = new SqlCommand("sp_LoginUsuario", con))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@nombre_usuario", oUsuario.IdUsuario);

						// Convertir la contraseña a hash
						string hashedPassword = ComputeSha256Hash(oUsuario.clave);
						cmd.Parameters.AddWithValue("@contrasena", hashedPassword);

						// Parámetro de salida para indicar si la autenticación fue exitosa
						SqlParameter outputParam = new SqlParameter("@es_autenticado", SqlDbType.Bit)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(outputParam);

						// Parámetro de salida para el mensaje
						SqlParameter messageParam = new SqlParameter("@mensaje", SqlDbType.NVarChar, 50)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(messageParam);

						con.Open();
						cmd.ExecuteNonQuery();

						bool isAuthenticated = Convert.ToBoolean(cmd.Parameters["@es_autenticado"].Value);
						string message = cmd.Parameters["@mensaje"].Value.ToString();

						if (isAuthenticated)
						{
							return RedirectToAction("Index", "Home");
						}
						else
						{
							ViewBag.ErrorMessage = message; // Mostrar el mensaje de error
							return View();
						}
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = "Se produjo un error al intentar iniciar sesión: " + ex.Message;
				return View();
			}
		}

		private static string ComputeSha256Hash(string rawData)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}

		[HttpGet]
		public ActionResult Registrar()
		{
			return View();
		}


		[HttpPost]
		public ActionResult Registrar(Usuario oUsuario)
		{
			// Verificar si el modelo es válido
			if (!ModelState.IsValid)
			{
				// Mostrar errores de validación en la vista
				ViewData["ErrorMessage"] = "Por favor, corrija los errores en el formulario.";
				return View(oUsuario);
			}

			try
			{
				// Obtener la cadena de conexión desde web.config
				string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cn"].ConnectionString;

				// Convertir la contraseña a hash
				string hashedPassword = ComputeSha256Hash(oUsuario.clave);

				// Crear una conexión SQL
				using (SqlConnection con = new SqlConnection(connectionString))
				{
					// Crear un comando SQL para llamar al procedimiento almacenado
					using (SqlCommand cmd = new SqlCommand("sp_CrearUsuarioNuevo", con))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						// Agregar parámetros al comando
						cmd.Parameters.AddWithValue("@first_name", oUsuario.nombre);
						cmd.Parameters.AddWithValue("@last_name", oUsuario.apellido);
						cmd.Parameters.AddWithValue("@gender", oUsuario.genero);
						cmd.Parameters.AddWithValue("@hire_date", oUsuario.FechaIngreso);
						cmd.Parameters.AddWithValue("@document", oUsuario.IdUsuario);
						cmd.Parameters.AddWithValue("@email", oUsuario.correo);
						cmd.Parameters.AddWithValue("@password", hashedPassword);

						// Parámetro de salida para indicar si la creación fue exitosa
						SqlParameter outputParam = new SqlParameter("@output", SqlDbType.Bit)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(outputParam);

						// Parámetro de salida para el mensaje
						SqlParameter messageParam = new SqlParameter("@message", SqlDbType.NVarChar, 50)
						{
							Direction = ParameterDirection.Output
						};
						cmd.Parameters.Add(messageParam);

						// Abrir la conexión y ejecutar el procedimiento almacenado
						con.Open();
						cmd.ExecuteNonQuery();

						// Obtener los valores de los parámetros de salida
						bool isSuccess = Convert.ToBoolean(cmd.Parameters["@output"].Value);
						string message = cmd.Parameters["@message"].Value.ToString();

						// Verificar si la creación fue exitosa
						if (isSuccess)
						{
							// Redirigir a la página de inicio de sesión si la creación fue exitosa
							return RedirectToAction("Login");
						}
						else
						{
							// Mostrar mensaje de error si la creación falló
							ViewData["ErrorMessage"] = message;
							return View(oUsuario);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Manejar excepciones y mostrar mensaje de error
				ViewData["ErrorMessage"] = "Se produjo un error al intentar registrar: " + ex.Message;
				return View(oUsuario);
			}
		}
	}
}
