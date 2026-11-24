using System.Globalization;
using TelcoNet.Sms;

namespace Huellitas.Citas.Antes;

public static class Program
{
    public static string archivo = "citas.txt";

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("1) Agendar cita  2) Ver citas del día  0) Salir");
            var op = Console.ReadLine();
            if (op == null || op == "0") break;

            if (op == "1")
            {
                Console.Write("Dueño: ");
                var d = Console.ReadLine();
                Console.Write("Teléfono: ");
                var tel = Console.ReadLine();
                Console.Write("Mascota: ");
                var m = Console.ReadLine();
                Console.Write("Tamaño (P/M/G): ");
                var t = Console.ReadLine();
                Console.Write("Servicio (1 = Baño, 2 = Baño y corte): ");
                var s = Console.ReadLine();
                Console.Write("Fecha y hora (yyyy-MM-dd HH:mm): ");
                var f = Console.ReadLine();

                DateTime fecha;
                if (!DateTime.TryParseExact(f, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                {
                    Console.WriteLine("Fecha inválida.");
                    continue;
                }

                int dur = s == "2" ? 90 : 60;
                if (fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    Console.WriteLine("No atendemos los domingos.");
                    continue;
                }
                if (fecha.Hour < 8 || fecha.AddMinutes(dur) > fecha.Date.AddHours(17))
                {
                    Console.WriteLine("Fuera del horario de atención (8:00 a 17:00).");
                    continue;
                }
                if (fecha <= DateTime.Now)
                {
                    Console.WriteLine("La fecha debe ser futura.");
                    continue;
                }

                var lineas = File.Exists(archivo) ? File.ReadAllLines(archivo) : new string[0];
                bool ocupado = false;
                foreach (var l in lineas)
                {
                    var x = l.Split(';');
                    var ini = DateTime.ParseExact(x[5], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    var fin = ini.AddMinutes(int.Parse(x[6]));
                    if (fecha < fin && ini < fecha.AddMinutes(dur)) ocupado = true;
                }
                if (ocupado)
                {
                    Console.WriteLine("Ese horario ya está ocupado.");
                    continue;
                }

                // precios
                double precio;
                if (s == "2")
                {
                    if (t == "P") precio = 250;
                    else if (t == "M") precio = 320;
                    else precio = 420;
                }
                else
                {
                    if (t == "P") precio = 150;
                    else if (t == "M") precio = 200;
                    else precio = 280;
                }
                int previas = 0;
                foreach (var l in lineas)
                    if (l.Split(';')[1] == tel) previas++;
                if (previas >= 5) precio = precio * 0.9;

                File.AppendAllText(archivo, d + ";" + tel + ";" + m + ";" + t + ";" + s + ";" + fecha.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + ";" + dur + ";" + precio.ToString(CultureInfo.InvariantCulture) + "\n");

                TelcoNetClient.SendMessage("504" + tel.Replace("-", ""), "Hola " + d + ", la cita de " + m + " es el " + fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) + ". Total: L " + precio.ToString("N2", CultureInfo.InvariantCulture));

                Console.WriteLine("Cita agendada para " + m + " el " + fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) + ". Total: L " + precio.ToString("N2", CultureInfo.InvariantCulture));
            }
            else if (op == "2")
            {
                Console.Write("Fecha (yyyy-MM-dd): ");
                var f = Console.ReadLine();
                var lineas = File.Exists(archivo) ? File.ReadAllLines(archivo) : new string[0];
                int n = 0;
                foreach (var l in lineas)
                {
                    var x = l.Split(';');
                    if (x[5].StartsWith(f + " "))
                    {
                        Console.WriteLine(x[5].Substring(11) + " - " + x[2] + " (" + (x[4] == "2" ? "Baño y corte" : "Baño") + ") - " + x[0]);
                        n++;
                    }
                }
                if (n == 0) Console.WriteLine("No hay citas para ese día.");
            }
        }
    }
}
