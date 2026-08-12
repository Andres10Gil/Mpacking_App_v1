# Cómo subir este repositorio a GitHub

El repositorio ya está inicializado con 5 commits y dos ramas (`main` y `develop`).
Solo falta conectarlo con GitHub y hacer push.

---

## 1. Crear el repositorio vacío en GitHub

1. Entrar a github.com → botón verde **New**
2. Nombre: `mpacking`
3. Visibilidad: **Private**
4. **NO** marcar "Add a README file" ni ninguna otra casilla — el repositorio ya trae todo
5. **Create repository**

---

## 2. Conectar y subir

Abrir la terminal dentro de esta carpeta y ejecutar (cambiando `TU-USUARIO`):

```bash
git remote add origin https://github.com/TU-USUARIO/mpacking.git
git push -u origin main
git push -u origin develop
```

Git pedirá autenticación: en Windows se abre el navegador para iniciar sesión.

---

## 3. Configurar develop como rama por defecto

En GitHub: **Settings** → **General** → sección *Default branch* → cambiar a `develop` → **Update**.

Así los pull requests apuntan a `develop` automáticamente y `main` queda protegida.

---

## 4. Verificar

```bash
git branch -a
git log --oneline
```

Deben aparecer las dos ramas y los 5 commits.

---

## Estado actual del repositorio

| Rama | Commits | Contenido |
|---|---|---|
| `main` | 4 | Entregables de las semanas 1 a 4 |
| `develop` | 5 | Lo de `main` más la estrategia de ramas |

Historial:

```
docs: estrategia de ramas del proyecto              (solo en develop)
db: esquema 12 tablas, 4 triggers, 10 indices y datos reales (S4)
docs: 19 mockups, plugin Figma, logo y mascota (S3)
docs: informe tecnico, avances, MER y presentaciones (S1-S2)
chore: estructura inicial del repositorio y guias del proyecto
```

---

## Empezar a trabajar

```bash
git checkout develop
git checkout -b feature/backend-inicial
```

Ver `docs/RAMAS.md` para el flujo completo de trabajo en equipo.
