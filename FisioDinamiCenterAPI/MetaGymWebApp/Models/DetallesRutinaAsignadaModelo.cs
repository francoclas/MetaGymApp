using LogicaNegocio.Clases;

public class DetallesRutinaAsignadaModelo
{
    public RutinaAsignada RutinaAsignada { get; set; }
    public Rutina Rutina { get; set; }
    public List<SesionRutina> Sesiones { get; set; }
}