<?php
session_start();

$conexion = new mysqli("localhost", "root", "", "banco");

if ($conexion->connect_error) {
    die("Error de conexión");
}

if (!isset($_SESSION['usuario'])) {
    die("Debes iniciar sesión");
}

$documento = $_SESSION['documento'];

$sql = "
SELECT 
    u.usuario,
    u.nombre,
    u.apellido,
    t.numero_tarjeta,
    t.banco_emisor,
    t.saldo,
    t.estado,
    l.periodo,
    l.fecha_vencimiento,
    l.total_a_pagar,
    l.pago_minimo
FROM usuarios u
JOIN tarjetas t ON u.documento = t.dni_titular
LEFT JOIN liquidaciones l ON t.dni_titular = l.dni_titular
WHERE u.documento = ?
";

$stmt = $conexion->prepare($sql);
$stmt->bind_param("s", $documento);
$stmt->execute();
$result = $stmt->get_result();

echo "<h2>Resumen de cuenta</h2>";

while ($row = $result->fetch_assoc()) {

    echo "<hr>";
    echo "Usuario: " . $row['usuario'] . "<br>";
    echo "Nombre: " . $row['nombre'] . " " . $row['apellido'] . "<br>";
    echo "Tarjeta: " . $row['numero_tarjeta'] . "<br>";
    echo "Banco: " . $row['banco_emisor'] . "<br>";
    echo "Saldo: $" . $row['saldo'] . "<br>";
    echo "Estado: " . $row['estado'] . "<br>";
    echo "Periodo: " . $row['periodo'] . "<br>";
    echo "Vencimiento: " . $row['fecha_vencimiento'] . "<br>";
    echo "Total: $" . $row['total_a_pagar'] . "<br>";
    echo "Pago mínimo: $" . $row['pago_minimo'] . "<br>";
}

?>