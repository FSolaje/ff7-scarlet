# Flujo de Trabajo y Sistema de Producción

Este documento define el protocolo estándar de desarrollo para el proyecto FF7Scarlet, asegurando la calidad, la consistencia y la trazabilidad entre requisitos, pruebas y código.

---

## 1. El Ciclo de Vida del Desarrollo

Para cualquier nueva funcionalidad, refactorización o corrección de bugs, se debe seguir estrictamente este orden:

1.  **Definición (Algoritmo):**
    *   Consultar `docs/Architecture/Algorithm_*.md`.
    *   Si la lógica cambia, actualizar primero este documento.
    *   **Clave:** Asignar o verificar el ID de la regla (ej: `[RULE-01]`).

2.  **Validación (QA):**
    *   Crear o actualizar tests unitarios que fallen (*Red*) si la nueva lógica no está implementada.
    *   Los tests deben referenciar explícitamente el ID de la regla que validan.

3.  **Implementación (Código):**
    *   Escribir el código en C# para satisfacer los tests (*Green*).
    *   Incluir comentarios en el código referenciando el ID de la regla implementada (`// Implements [RULE-01]`).

4.  **Documentación Técnica (Diseño):**
    *   Actualizar `docs/Architecture/Implementation_*.md` explicando cómo se resolvió técnicamente.

5.  **Verificación Final:**
    *   Ejecutar todos los tests.
    *   Ejecutar escaneo de seguridad (`scripts/security_check.ps1`).

---

## 2. Estrategia de Trazabilidad

Para evitar la desincronización entre documentos y código, utilizamos un sistema de **Identificadores de Reglas**.

### A. En el Algoritmo (`.md`)
Cada regla lógica debe tener un ID único y estable.
> **[SLOT-RL-01]**: La aplicación de un valor RL crea un enlace hacia la izquierda.

### B. En el QA (`.cs`)
Los tests deben indicar qué regla están verificando en su descripción o nombre.
```csharp
[Test]
[Description("[SLOT-RL-01] Verify RL creates left link")]
public void UpdateLeftSlot_RL_CreatesLink() { ... }
```

### C. En el Código Fuente (`.cs`)
Los bloques lógicos complejos deben citar la regla que implementan.
```csharp
// [SLOT-RL-01]: Check if we need to auto-link left
else if (IsRightLinked()) { ... }
```

### D. En la Documentación de Implementación (`.md`)
Mapear la solución técnica a la regla.
> La regla **[SLOT-RL-01]** se implementa en el método `UpdateLeftSlot` mediante la condición `else if (IsRightLinked())`.

---

## 3. Seguridad y Commits

*   **Seguridad:** Tolerancia cero al *Secret Sprawl*. Ejecutar siempre el script de seguridad antes de un push.
*   **Commits:** Mensajes claros, descriptivos y siempre confirmados por el usuario antes de ejecutarse. Solo comitear lo que está `staged`.
