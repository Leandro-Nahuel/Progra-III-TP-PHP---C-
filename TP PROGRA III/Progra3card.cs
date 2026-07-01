using System;
using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
        ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)

        MostrarDetalleCompleto(numCuenta);    

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n ⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void ObtenerYMostrarTarjetas()
        {
            try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        string Consulta = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";

                        MySqlCommand Comando = new MySqlCommand(Consulta, conexion);

                        MySqlDataReader Leyendo = Comando.ExecuteReader();

                        while (Leyendo.Read())
                        {
                            Console.WriteLine(
                                "{0,-12} {1,-18} {2,-20} {3,-15}",
                                Leyendo ["num_cuenta"],
                                Leyendo ["numero_tarjeta"],
                                Leyendo ["banco_emisor"],
                                Leyendo ["dni_titular"]
                            );
                        }

                        Leyendo.Close();

                    }
                }

                catch (Exception err)
                {
                    Console.WriteLine("Algo Falló: " + err.Message);
                }
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);
        }

        static void MostrarDetalleCompleto(int numCuenta)
        {
             try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        string consulta = @"SELECT
                                            t.num_cuenta,
                                            t.numero_tarjeta,
                                            t.banco_emisor,
                                            t.estado,
                                            t.saldo,
                                            u.documento,
                                            u.nombre,
                                            u.apellido,
                                            u.email
                                            FROM tarjetas t
                                            INNER JOIN usuarios u
                                            ON t.dni_titular = u.documento
                                            WHERE t.num_cuenta = @cuenta";

                        MySqlCommand comando = new MySqlCommand(consulta, conexion);

                        comando.Parameters.AddWithValue("@cuenta", numCuenta);

                        MySqlDataReader Lector = comando.ExecuteReader();

                        if (Lector.Read())
                            {
                                Console.WriteLine("\n===== DATOS DEL CLIENTE =====");
                                Console.WriteLine("Nombre: " + Lector["nombre"]);
                                Console.WriteLine("Apellido: " + Lector["apellido"]);
                                Console.WriteLine("DNI: " + Lector["documento"]);
                                Console.WriteLine("Email: " + Lector["email"]);

                                Console.WriteLine("\n===== DATOS DE LA TARJETA =====");
                                Console.WriteLine("Número de Cuenta: " + Lector["num_cuenta"]);
                                Console.WriteLine("Número de Tarjeta: " + Lector["numero_tarjeta"]);
                                Console.WriteLine("Banco Emisor: " + Lector["banco_emisor"]);
                                Console.WriteLine("Estado: " + Lector["estado"]);
                                Console.WriteLine("Saldo: $" + Lector["saldo"]);
                            }
                        else
                            {
                                Console.WriteLine("NO EXISTE UNA TARJETA CON ESE NUMERO DE CUENTA");
                            }
                        Lector.Close();    
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("OCURRIO UN ERROR: " + ex.Message);
                }
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta
        }

        static bool DarDeBajaTarjeta(int numCuenta)
        {
             try
            {
                using (MySqlConnection conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();

                    string consulta = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";

                    MySqlCommand comando = new MySqlCommand(consulta, conexion);

                    comando.Parameters.AddWithValue("@cuenta", numCuenta);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error: " + ex.Message);
                return false;
            }
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta
        }

            static void MenuEmitirTarjeta()
            {
                Console.Clear();
                Console.WriteLine("--- EMITIR NUEVA TARJETA ---");

                Console.WriteLine("Seleccione tipo de documento:");
                Console.WriteLine("1. DNI");
                Console.WriteLine("2. PASAPORTE");

                string tipoDoc;

                switch (Console.ReadLine())
                {
                    case "1":
                        tipoDoc = "DNI";
                        break;
                    case "2":
                        tipoDoc = "PASAPORTE";
                        break;
                    default:
                        Console.WriteLine("Opción inválida");
                        return;
                }

                Console.Write("Documento: ");
                string documento = Console.ReadLine()?? "";

                Console.Write("Nombre: ");
                string nombre = Console.ReadLine()?? "";

                Console.Write("Apellido: ");
                string apellido = Console.ReadLine()?? "";

                Console.Write("Fecha nacimiento (YYYY-MM-DD): ");
                DateTime fechaNac = Convert.ToDateTime(Console.ReadLine());

                Console.Write("Email: ");
                string email = Console.ReadLine()?? "";

                Console.Write("Usuario (opcional): ");
                string usuario = Console.ReadLine()?? "";

                Console.Write("Password (opcional): ");
                string password = Console.ReadLine()?? "";

                Console.Write("Número de tarjeta (16 dígitos): ");
                string numeroTarjeta = Console.ReadLine()?? "";

                Console.WriteLine("Seleccione banco:");
                Console.WriteLine("1. Banco Nación");
                Console.WriteLine("2. Banco Provincia");
                Console.WriteLine("3. Banco Galicia");
                Console.WriteLine("4. Banco Santander");
                Console.WriteLine("5. Banco BBVA");
                Console.WriteLine("6. Banco Macro");

                string banco ;

                switch (Console.ReadLine())
                {
                    case "1":
                        banco = "Banco Nación";
                        break;
                    case "2":
                        banco = "Banco Provincia";
                        break;
                    case "3":
                        banco = "Banco Galicia";
                        break;
                    case "4":
                        banco = "Banco Santander";
                        break;
                    case "5":
                        banco = "Banco BBVA";
                        break;
                    case "6":
                        banco = "Banco Macro";
                        break;
                    default:
                        Console.WriteLine("Banco inválido");
                        return;
                }

                bool ok = EmitirTarjeta(tipoDoc, documento, nombre, apellido, fechaNac,
                                        email, usuario, password, numeroTarjeta, banco);

                Console.WriteLine(ok ? "\nTarjeta creada correctamente." : "\nError al crear la tarjeta.");

                Console.WriteLine("\nPresione una tecla...");
                Console.ReadKey();
                // A implementar
            }

            static bool EmitirTarjeta(string tipoDoc, string documento, string nombre, string apellido,
                         DateTime fechaNac, string email, string usuario, string password,
                         string numeroTarjeta, string banco)
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        // 1. Insert usuario
                        string sqlUsuario = @"
                            INSERT INTO usuarios 
                            (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
                            VALUES
                            (@doc, @tipo, @nom, @ape, @fecha, @mail, @user, @pass)
                            ON DUPLICATE KEY UPDATE
                            nombre=@nom, apellido=@ape, email=@mail;
                        ";

                        MySqlCommand cmdUser = new MySqlCommand(sqlUsuario, conexion);
                        cmdUser.Parameters.AddWithValue("@doc", documento);
                        cmdUser.Parameters.AddWithValue("@tipo", tipoDoc);
                        cmdUser.Parameters.AddWithValue("@nom", nombre);
                        cmdUser.Parameters.AddWithValue("@ape", apellido);
                        cmdUser.Parameters.AddWithValue("@fecha", fechaNac);
                        cmdUser.Parameters.AddWithValue("@mail", email);
                        cmdUser.Parameters.AddWithValue("@user", usuario);
                        cmdUser.Parameters.AddWithValue("@pass", password);
                        cmdUser.ExecuteNonQuery();

                        // 2. Insert tarjeta
                        string sqlTarjeta = @"
                            INSERT INTO tarjetas
                            (numero_tarjeta, banco_emisor, dni_titular, estado, saldo)
                            VALUES
                            (@tarjeta, @banco, @dni, 'Activa', 0.00);
                        ";

                        MySqlCommand cmdTarjeta = new MySqlCommand(sqlTarjeta, conexion);
                        cmdTarjeta.Parameters.AddWithValue("@tarjeta", numeroTarjeta);
                        cmdTarjeta.Parameters.AddWithValue("@banco", banco);
                        cmdTarjeta.Parameters.AddWithValue("@dni", documento);

                        int filas = cmdTarjeta.ExecuteNonQuery();

                        return filas > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }




            static void MenuEmitirLiquidacion()
            {

                Console.Clear();
                Console.WriteLine("--- EMITIR LIQUIDACIÓN ---");

                Console.Write("Número de cuenta: ");
                int numCuenta = Convert.ToInt32(Console.ReadLine());

                Console.Write("Periodo (YYYY-MM): ");
                string periodo = Console.ReadLine()?? "";

                Console.Write("Fecha vencimiento (YYYY-MM-DD): ");
                DateTime vencimiento = Convert.ToDateTime(Console.ReadLine());

                Console.Write("Total a pagar: ");
                decimal total = Convert.ToDecimal(Console.ReadLine());

                decimal minimo = total * 0.10m; // 10% automático

                bool ok = EmitirLiquidacion(numCuenta, periodo, vencimiento, total, minimo);

                Console.WriteLine(ok ? "\nLiquidación creada correctamente." : "\nError al crear liquidación.");

                Console.WriteLine("\nPresione una tecla...");
                Console.ReadKey();
                // A implementar
            }

            static bool EmitirLiquidacion(int numCuenta, string periodo, DateTime vencimiento,
                             decimal total, decimal minimo)
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection(connectionString))
                    {
                        conexion.Open();

                        string sql = @"
                            INSERT INTO liquidaciones
                            (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo)
                            VALUES
                            (@cuenta, @periodo, @venc, @total, @minimo);
                        ";

                        MySqlCommand cmd = new MySqlCommand(sql, conexion);
                        cmd.Parameters.AddWithValue("@cuenta", numCuenta);
                        cmd.Parameters.AddWithValue("@periodo", periodo);
                        cmd.Parameters.AddWithValue("@venc", vencimiento);
                        cmd.Parameters.AddWithValue("@total", total);
                        cmd.Parameters.AddWithValue("@minimo", minimo);

                        int filas = cmd.ExecuteNonQuery();

                        return filas > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }



    }

}