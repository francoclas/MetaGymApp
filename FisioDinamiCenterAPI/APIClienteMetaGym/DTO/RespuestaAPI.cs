namespace APIClienteMetaGym.DTO
{
    //Se usa para devolver mas facil los erroes
    public class RespuestaApi<T>
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static RespuestaApi<T> Ok(T data, string message = "Éxito") =>
            new() { Success = true,Status = 200, Message = message, Data = data };

        public static RespuestaApi<T> Created(T data, string message = "Recurso creado correctamente") =>
            new() { Success = true,Status = 201, Message = message, Data = data };

        public static RespuestaApi<T> NoContent(string message = "Sin contenido") =>
            new() { Success = true, Status = 204, Message = message, Data = default };

        public static RespuestaApi<T> BadRequest(string message = "Solicitud incorrecta") =>
            new() { Success = false, Status = 400, Message = message, Data = default };

        public static RespuestaApi<T> Unauthorized(string message = "No autorizado") =>
            new() { Success = false, Status = 401, Message = message, Data = default };
        public static RespuestaApi<T> Error(string message)
        {
            return new RespuestaApi<T> { Success = false, Message = message };
        }
        public static RespuestaApi<T> Forbidden(string message = "Acceso denegado") =>
            new() { Success = false,Status = 403, Message = message, Data = default };

        public static RespuestaApi<T> NotFound(string message = "Recurso no encontrado") =>
            new() { Success = false, Status = 404, Message = message, Data = default };
    }
}
