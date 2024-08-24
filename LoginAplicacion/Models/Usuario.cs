using System;
using System.ComponentModel.DataAnnotations;

namespace LoginAplicacion.Models
{
	public class Usuario
	{
		[Required(ErrorMessage = "Requerido.")]
		[StringLength(14, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 14 caracteres.")]
		public string nombre { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[StringLength(14, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 14 caracteres.")]
		public string apellido { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[RegularExpression("^[MF]$", ErrorMessage = "El género debe ser 'M' o 'F'.")]
		public string genero { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[DataType(DataType.Date)]
		public DateTime FechaIngreso { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[StringLength(10, ErrorMessage = "Máximo 10 caracteres.")]
		public string IdUsuario { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[EmailAddress(ErrorMessage = "No es válido.")]
		[StringLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres.")]
		public string correo { get; set; }

		[Required(ErrorMessage = "Requerido.")]
		[StringLength(150, MinimumLength = 6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres.")]
		public string clave { get; set; }

		public int id { get; set; }
		public int codigoEmpleado { get; set; }
	}
}
