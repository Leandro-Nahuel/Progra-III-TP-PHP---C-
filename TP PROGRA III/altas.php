<?php

$conexion = new mysqli("localhost", "root", "", "banco");

if ($conexion->connect_error) {
    die("Error de conexión");
}

$documento = $_POST['documento'];
$usuario = $_POST['usuario'];
$password1 = $_POST['passwordA'];
$password2 = $_POST['passwordB'];

// validar contraseñas
if ($password1 !== $password2) {
    die("Las contraseñas no coinciden");
}

// buscar usuario
$sql = "SELECT usuario FROM usuarios WHERE documento = ?";
$stmt = $conexion->prepare($sql);
$stmt->bind_param("s", $documento);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows == 0) {
    die("El DNI no existe");
}

$fila = $result->fetch_assoc();

// ya activado
if ($fila['usuario'] != NULL) {
    die("La cuenta ya fue activada");
}

// verificar tarjeta
$sql2 = "SELECT * FROM tarjetas WHERE dni_titular = ?";
$stmt2 = $conexion->prepare($sql2);
$stmt2->bind_param("s", $documento);
$stmt2->execute();
$result2 = $stmt2->get_result();

if ($result2->num_rows == 0) {
    die("No tiene tarjeta asociada");
}

// activar cuenta
$sql3 = "UPDATE usuarios SET usuario = ?, password = ? WHERE documento = ?";
$stmt3 = $conexion->prepare($sql3);
$stmt3->bind_param("sss", $usuario, $password1, $documento);

if ($stmt3->execute()) {
    echo "✔ Cuenta activada correctamente";
} else {
    echo "Error";
}

?>