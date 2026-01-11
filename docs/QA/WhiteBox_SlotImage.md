# Análisis de Caja Blanca: `SlotImage`

**Objetivo:** Verificar la correcta selección de recursos gráficos basados en el estado del slot, crecimiento y materia equipada.
**SUT:** `SlotImage.cs` (Método `GetSlotResource`)

---

## 1. Análisis de Flujo de Control (`GetSlotResource` y `GetUnifiedSuffix`)

La lógica se ha unificado en un solo punto de entrada que construye dinámicamente el nombre del recurso.

*   **`GetSlotResource`**:
    1.  **Nodo 1 (Decisión):** Determina si hay materia (`hasMateria`).
    2.  **Nodo 2 (Operación):** Asigna el prefijo (`"materia_slot_"` o `"materia_slot_magic_"`).
    3.  **Nodo 3 (Llamada):** Invoca a `GetUnifiedSuffix` para obtener el sufijo.
    4.  **Nodo 4 (Operación):** Concatena prefijo y sufijo.
    5.  **Nodo 5 (Decisión):** `Enum.TryParse`. Si tiene éxito, retorna el recurso.
    6.  **Nodo 6 (Fin):** Si falla, retorna `defaultResource`.

*   **`GetUnifiedSuffix`**:
    1.  **Nodo 1 (Decisión):** Evalúa `hasMateria`.
    2.  **Rama A (Con Materia):** Lógica `if/else if` simple usando extensiones (`IsUnlinked`, `IsLeftLinked`, etc.).
    3.  **Rama B (Sin Materia):** Expresión `switch` sobre el enum `MateriaSlot` para obtener el sufijo numérico correcto (0-6, _dl1, _dl2).

## 2. Complejidad Ciclomática ($V(G)$)
El flujo unificado es más complejo de medir como un todo, pero sus partes son simples. La complejidad principal reside en la expresión `switch` de `GetUnifiedSuffix`.
*   **Predicados:** `hasMateria` (1) + `IsUnlinked` (1) + `IsLeftLinked` (1) + `IsRightLinked` (1) + 8 casos del `switch`.
*   **$V(G) \approx 12$**. La complejidad sigue siendo manejable y está bien estructurada.

## 3. Tabla de Casos de Prueba (Trazabilidad)
La tabla de casos de prueba valida los *resultados* del algoritmo unificado, cubriendo todas las ramas lógicas descritas.

| ID | Regla Validada | Entrada (State, DL, Growth, Materia) | Resultado Esperado |
|:---|:---|:---|:---|
| **TV-01** | [SIMAGE-EMPTY-UL-NORM] | UL (Normal), DL=No, Normal | `materia_slot1` |
| **TV-02** | [SIMAGE-EMPTY-UL-NO] | UL (Empty), DL=No, Empty | `materia_slot4` |
| **TV-03** | [SIMAGE-EMPTY-DL-NORM] | RL (Normal), DL=Yes, Normal | `materia_slot_dl1` |
| **TV-04** | [SIMAGE-EMPTY-DL-NO] | RL (Empty), DL=Yes, Empty | `materia_slot_dl2` |
| **TV-05** | [SIMAGE-MAT-UL] | UL (Normal), DL=No, Normal, Magic | `materia_slot_magic1` |
| **TV-06** | [SIMAGE-MAT-UL] | UL (Empty), DL=No, Empty, Magic | `materia_slot_magic1` (Growth ignorado) |
| **TV-07** | [SIMAGE-MAT-DL] | RL (Normal), DL=Yes, Normal, Command | `materia_slot_command_dl` |
| **TV-08** | [SIMAGE-MAT-DL] | RL (Empty), DL=Yes, Empty, Command | `materia_slot_command_dl` (Growth ignorado) |
