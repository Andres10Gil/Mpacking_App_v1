# Guía de Git — MPACKING

Paso a paso para subir el proyecto a GitHub y mantenerlo actualizado.

---

## 1. Instalar Git (una sola vez)

1. Descargar desde [git-scm.com](https://git-scm.com)
2. Ejecutar el instalador dejando las opciones por defecto
3. Cerrar y volver a abrir la terminal

Verificar que quedó instalado:

```bash
git --version
```

---

## 2. Configurar la identidad (una sola vez)

```bash
git config --global user.name "Tu Nombre"
git config --global user.email "tucorreo@gmail.com"
```

Este nombre aparecerá en cada commit del historial.

---

## 3. Crear el repositorio en GitHub

1. Entrar a [github.com](https://github.com) e iniciar sesión
2. Botón verde **New** (arriba a la derecha)
3. Nombre del repositorio: `mpacking`
4. Visibilidad: **Private** (recomendado durante el desarrollo)
5. **No** marcar "Add a README file" — ya lo tenemos
6. Botón **Create repository**

GitHub mostrará la URL del repositorio. Copiarla, se usa en el paso 7.

---

## 4. Preparar la carpeta local

Descomprimir `MPACKING_Proyecto_Completo.zip` en una carpeta de trabajo, por ejemplo:

```
C:\Users\TuUsuario\Documents\mpacking
```

Abrir la terminal en esa carpeta y ejecutar:

```bash
cd C:\Users\TuUsuario\Documents\mpacking
git init
git branch -M main
```

---

## 5. Verificar qué se va a subir

```bash
git status
```

Deben aparecer las carpetas `docs/`, `database/` y los archivos `README.md`, `CRONOGRAMA.md`.

El archivo `.gitignore` ya está configurado para excluir contraseñas, binarios compilados y archivos temporales.

---

## 6. Primer commit

```bash
git add .
git commit -m "docs: requisitos, MER, mockups y base de datos (S1-S4)"
```

---

## 7. Conectar con GitHub y subir

Reemplazar `TU-USUARIO` por el nombre de usuario real:

```bash
git remote add origin https://github.com/TU-USUARIO/mpacking.git
git push -u origin main
```

Git pedirá autenticación. En Windows se abre una ventana del navegador para iniciar sesión en GitHub.

---

## 8. Flujo para cada avance posterior

Cada vez que se termine una actividad del cronograma:

```bash
git add .
git commit -m "feat: modulo de autenticacion JWT"
git push
```

---

## Convención de mensajes de commit

Usar un prefijo hace que el historial sirva como evidencia de avance:

| Prefijo | Cuándo usarlo | Ejemplo |
|---|---|---|
| `docs:` | Documentación | `docs: informe tecnico v3.0` |
| `feat:` | Funcionalidad nueva | `feat: endpoint de escaneo QR` |
| `fix:` | Corrección de error | `fix: calculo de ecopesos en decimales` |
| `db:` | Cambios en base de datos | `db: trigger T4 valida expiracion` |
| `chore:` | Configuración | `chore: gitignore para .NET` |

---

## Comandos útiles

| Comando | Qué hace |
|---|---|
| `git status` | Muestra qué archivos cambiaron |
| `git log --oneline` | Historial resumido de commits |
| `git diff` | Muestra los cambios exactos |
| `git pull` | Baja los cambios de otros integrantes |
| `git branch` | Lista las ramas |
| `git checkout -b nombre` | Crea y cambia a una rama nueva |

---

## Trabajar en equipo con ramas

Para evitar conflictos cuando varias personas trabajan al mismo tiempo:

```bash
# Crear una rama para tu tarea
git checkout -b feature/auth-jwt

# Trabajar normalmente y hacer commits
git add .
git commit -m "feat: login con JWT"

# Subir la rama
git push -u origin feature/auth-jwt
```

Luego en GitHub aparece el botón **Compare & pull request** para integrar los cambios a `main` tras revisión.

---

## Errores frecuentes

**`remote origin already exists`**
El remoto ya estaba configurado. Corregir con:
```bash
git remote set-url origin https://github.com/TU-USUARIO/mpacking.git
```

**`failed to push some refs`**
Hay cambios en GitHub que no están en local. Ejecutar primero:
```bash
git pull --rebase origin main
git push
```

**Subiste una contraseña por error**
Eliminar el archivo del seguimiento y agregarlo al `.gitignore`:
```bash
git rm --cached appsettings.json
git commit -m "chore: excluir credenciales"
git push
```
Después cambiar esa contraseña, porque queda en el historial.

---

## Estructura recomendada de Google Drive

Para compartir con quienes no usan Git:

```
MPACKING/
├── 01 Documentacion/
│   ├── Informe tecnico completo.docx
│   ├── Documento de avances.docx
│   └── Presentaciones/
├── 02 Diseno/
│   ├── MER y diagramas
│   └── Arquitectura
├── 03 Mockups/
│   ├── Plugin Figma
│   ├── SVG por rol
│   └── Logo y mascota
├── 04 Base de datos/
│   ├── Scripts SQL
│   └── Evidencias de pruebas
└── 05 Cronograma y actas/
```

Regla práctica: en Drive van los entregables para revisión (documentos, presentaciones, capturas). En Git va el código fuente y los scripts.
