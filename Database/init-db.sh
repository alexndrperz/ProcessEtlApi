#!/bin/bash
set -e

SQLCMD=/opt/mssql-tools18/bin/sqlcmd
CONN=(-S "${SQL_HOST:-sqlserver}" -U sa -P "$MSSQL_SA_PASSWORD" -C -b -f 65001)

for script in 00_BaseDatos.sql 01_Origen.sql 02_Datos_Origen.sql 03_Staging.sql 04_DataWarehouse.sql 05_Procedimientos_Dimensiones.sql; do
    echo "Ejecutando $script..."
    $SQLCMD "${CONN[@]}" -i "/scripts/$script"
done

echo "Bases de datos AnalisisOpiniones y DataWarehouse listas."
