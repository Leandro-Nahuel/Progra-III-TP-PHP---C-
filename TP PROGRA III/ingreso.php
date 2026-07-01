<?php
session_start();

$conexion = new mysqli("localhost", "root", "", "banco");

if ($conexion->connect_error) {
    die("Error de conexión");
}

$usuario = $_POST['usuario'];
$password = $_POST['password'];

$sql = "SELECT * FROM usuarios WHERE usuario = ? AND password = ?";
$stmt = $conexion->prepare($sql);
$stmt->bind_param("ss", $usuario, $password);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows == 1) {

    $fila = $result->fetch_assoc();

    $_SESSION['usuario'] = $fila['usuario'];
    $_SESSION['documento'] = $fila['documento'];

    header("Location: resumen.php");
    exit();

} else {
    echo "Usuario o contraseña incorrectos";
}
?>