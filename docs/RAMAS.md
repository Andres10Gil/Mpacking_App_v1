# Estrategia de ramas — MPACKING

El repositorio usa un flujo de dos ramas permanentes más ramas temporales por tarea.

---

## Ramas permanentes

| Rama | Propósito | Quién escribe en ella |
|---|---|---|
| `main` | Código estable y entregables aprobados. Es lo que se muestra al presentar el proyecto. | Nadie directamente — solo se integra desde `develop` |
| `develop` | Rama de trabajo diario. Aquí se integra todo lo que el equipo va terminando. | El equipo, vía ramas de tarea |

---

## Ramas temporales

Se crean desde `develop` para cada tarea y se eliminan al integrarlas.

| Prefijo | Para qué | Ejemplo |
|---|---|---|
| `feature/` | Funcionalidad nueva | `feature/auth-jwt` |
| `fix/` | Corrección de un error | `fix/calculo-ecopesos` |
| `db/` | Cambios en base de datos | `db/tabla-notificaciones` |
| `docs/` | Documentación | `docs/manual-usuario` |

---

## Flujo de trabajo

### Empezar una tarea

```bash
git checkout develop
git pull
git checkout -b feature/auth-jwt
```

### Trabajar y guardar avances

```bash
git add .
git commit -m "feat: endpoint de login con JWT"
```

Hacer commits pequeños y frecuentes, no uno gigante al final.

### Subir la rama

```bash
git push -u origin feature/auth-jwt
```

### Integrar a develop

En GitHub aparece el botón **Compare & pull request**. Crear el pull request hacia `develop`, pedir revisión a un compañero y hacer merge.

Luego, en local:

```bash
git checkout develop
git pull
git branch -d feature/auth-jwt
```

---

## Cuándo pasar de develop a main

Solo al cerrar un hito del cronograma:

- Fin de una fase (backend completo, app móvil completa)
- El MVP de la semana 17
- La entrega final de la semana 22

```bash
git checkout main
git merge develop
git push
git tag -a v1.0-mvp -m "MVP funcional - semana 17"
git push --tags
```

Los tags dejan marcado el punto exacto de cada entrega, lo que sirve como evidencia de avance.

---

## Reglas del equipo

1. Nunca trabajar directamente sobre `main`.
2. Antes de crear una rama nueva, hacer `git pull` en `develop`.
3. Un pull request por tarea, no varios cambios mezclados.
4. No subir credenciales — `appsettings.Development.json` y `.env` están en `.gitignore`.
5. Mensajes de commit con prefijo (`feat:`, `fix:`, `docs:`, `db:`, `chore:`).

---

## Si dos personas tocan el mismo archivo

Git avisa con un conflicto. Para resolverlo:

```bash
git checkout develop
git pull
git checkout feature/mi-rama
git merge develop
```

Abrir los archivos marcados, decidir qué código queda, y luego:

```bash
git add .
git commit -m "chore: resolver conflicto con develop"
git push
```
