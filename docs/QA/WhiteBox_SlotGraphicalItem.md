# Informe de Análisis de Caja Blanca: `SlotGraphicalItem`

**Fecha:** 06/01/2026
**Objetivo:** Validar la robustez lógica, la cobertura de caminos y la complejidad del flujo de control en los métodos críticos de propagación de estado.
**SUT (System Under Test):** `src\Shared\Models\SlotGraphicalItem.cs`

---

## 1. Método: `SetInSlot`

Este es el punto de entrada principal. Orquesta la validación, la actualización del valor y la decisión de propagar cambios a los vecinos.

### 1.1 Análisis del Flujo de Control
El método presenta una estructura secuencial con "guard clauses" iniciales y bloques condicionales para la lógica de actualización.

*   **Nodo 1 (Inicio):** Asignación de `lastSelectionType`.
*   **Nodo 2 (Decisión):** Validación de límites de array (`SlotIndex >= 0 && SlotIndex < SlotsArray.Length`).
*   **Nodo 3 (Decisión):** `IsADoubleLinkedClickedSlot()`. Si es true, activa `forceUpdate`.
*   **Nodo 4 (Operación):** `GetMatchingSlot`.
*   **Nodo 5 (Decisión Compleja):** `MateriaSlotValue != newMateriaSlot` OR `forceUpdate`. Si no se cumple ninguna, sale retornando `false`.
*   **Nodo 6 (Actualización):** Asignación del nuevo valor.
*   **Nodo 7 (Decisión Compleja):** `updateDirection == Left` OR `updateDirection == Both`.
*   **Nodo 8 (Llamada):** `UpdateLeftSlot()`.
*   **Nodo 9 (Decisión Compleja):** `updateDirection == Right` OR `updateDirection == Both`.
*   **Nodo 10 (Llamada):** `UpdateRightSlot()`.
*   **Nodo 11 (Fin Éxito):** `return true`.
*   **Nodo 12 (Fin Fallo):** `return false` (alcanzado si falla validación de índice o no hay cambios).

### 1.2 Complejidad Ciclomática de McCabe ($V(G)$)
La fórmula utilizada es $V(G) = P + 1$, donde $P$ es el número de predicados (condiciones simples). Las condiciones compuestas (`&&`, `||`) se descomponen.

1.  `SlotIndex >= 0`
2.  `SlotIndex < SlotsArray.Length`
3.  `IsADoubleLinkedClickedSlot()`
4.  `MateriaSlotValue != newMateriaSlot`
5.  `forceUpdate` (en el OR)
6.  `updateDirection == Left`
7.  `updateDirection == Both`
8.  `updateDirection == Right`
9.  `updateDirection == Both`

**$V(G) = 9 + 1 = 10$**
*Interpretación:* El riesgo es **moderado**. Se recomienda refactorizar si la complejidad aumenta, pero para una máquina de estados es aceptable. Se requieren mínimo 10 casos de prueba para cubrir todas las ramas lógicas independientes.

### 1.3 Caminos Linealmente Independientes (Basis Path Testing)
1.  **Index Out of Bounds (Low):** `SlotIndex < 0` -> `return false`.
2.  **Index Out of Bounds (High):** `SlotIndex >= Length` -> `return false`.
3.  **No Change, No Force:** Index válido, no es DoubleLinkedClick, valor igual, force false -> `return false`.
4.  **Force Update via DoubleLinked:** Index válido, es DoubleLinkedClick (activa force), valor igual -> Entra en update -> Dir None (teórico) -> `return true`.
5.  **Change Value, Update Left:** Index válido, valor distinto, Dir = Left -> Ejecuta `UpdateLeftSlot` -> `return true`.
6.  **Change Value, Update Right:** Index válido, valor distinto, Dir = Right -> Ejecuta `UpdateRightSlot` -> `return true`.
7.  **Change Value, Update Both:** Index válido, valor distinto, Dir = Both -> Ejecuta ambos updates -> `return true`.

### 1.4 Tabla de Casos de Prueba (Cobertura 100%)

| ID | Escenario | Estado Inicial Slots | Input `SetInSlot` (NewVal, UpdateDir, Force) | Resultado Esperado |
|:---|:---|:---|:---|:---|
| **SIS-01** | Índice inválido (negativo) | `[UL]` (Index -1) | `UL`, `Both`, `false` | Retorna `false` |
| **SIS-02** | Índice inválido (exceso) | `[UL]` (Index 8) | `UL`, `Both`, `false` | Retorna `false` |
| **SIS-03** | Sin cambios y sin forzar | `[UL]` (Index 0) | `UL`, `Both`, `false` | Retorna `false` |
| **SIS-04** | Forzado por lógica DoubleLinked | `[RL, RL, RL]` (Index 1) + Config DL | `RL`, `Both`, `false` (Click DL) | Retorna `true`, Propaga cambios |
| **SIS-05** | Cambio valor, Dirección Izquierda | `[LL, RL]` (Index 1) | `UL`, `Left`, `false` | Retorna `true`, Llama `UpdateLeftSlot` |
| **SIS-06** | Cambio valor, Dirección Derecha | `[LL, RL]` (Index 0) | `UL`, `Right`, `false` | Retorna `true`, Llama `UpdateRightSlot` |
| **SIS-07** | Cambio valor, Ambas direcciones | `[LL, RL, RL]` (Index 1) | `UL`, `Both`, `false` | Retorna `true`, Llama ambos updates |

---

## 2. Método: `UpdateLeftSlot`

Maneja la reacción del vecino izquierdo basándose en el nuevo estado del slot actual.

### 2.1 Análisis del Flujo de Control
Estructura `if-else if-else if` mutuamente excluyente basada en el estado del slot actual (`this`).

*   **Rama 1:** `IsLeftLinked()` -> Si `LeftSlot` es `LeftLinked` -> Romper enlace izquierdo (`SetInSlot` a `Unlinked`).
*   **Rama 2:** `IsRightLinked()` -> Si `LeftSlot` es `Unlinked` o `None` -> Unir (`SetInSlot` a `LeftLinked`).
*   **Rama 3:** `IsUnlinked()` o `IsNonSlot()` -> Si `LeftSlot` es `LeftLinked` -> Romper enlace izquierdo (`SetInSlot` a `Unlinked`).

### 2.2 Complejidad Ciclomática de McCabe ($V(G)$)
Predicados:
1.  `IsLeftLinked()`
2.  (Anidado) `LeftSlot.IsLeftLinked()`
3.  `IsRightLinked()`
4.  (Anidado) `LeftSlot.IsUnlinked()`
5.  (Anidado) `LeftSlot.IsNonSlot()` (en el OR)
6.  `IsUnlinked()`
7.  `IsNonSlot()` (en el OR principal)
8.  (Anidado) `LeftSlot.IsLeftLinked()`

**$V(G) = 8 + 1 = 9$**

### 2.3 Caminos Linealmente Independientes
1.  **Current=LL, Left=LL:** Detecta colisión LL-LL, corrige izquierda a UL.
2.  **Current=LL, Left!=LL:** No hace nada.
3.  **Current=RL, Left=UL:** Detecta hueco para unir, corrige izquierda a LL.
4.  **Current=RL, Left!=UL/None:** No hace nada (ya está enlazado correctamente o incompatible).
5.  **Current=UL, Left=LL:** Detecta enlace huérfano a la izquierda, corrige izquierda a UL.
6.  **Current=UL, Left!=LL:** No hace nada.

### 2.4 Tabla de Casos de Prueba (Cobertura 100%)

| ID | Escenario | Estado Inicial (Left, **Current**) | Resultado Esperado en `LeftSlot` |
|:---|:---|:---|:---|
| **ULS-01** | Current LL, conflicto con Left LL | `[LL, LL]` -> Update(1) | Left cambia a `Unlinked` |
| **ULS-02** | Current LL, Left compatible (UL) | `[UL, LL]` -> Update(1) | Left se mantiene `Unlinked` |
| **ULS-03** | Current RL, Left libre (UL) | `[UL, RL]` -> Update(1) | Left cambia a `LeftLinked` (Autolink) |
| **ULS-04** | Current RL, Left ocupado (LL) | `[LL, RL]` -> Update(1) | Left se mantiene `LeftLinked` |
| **ULS-05** | Current UL, Left colgando (LL) | `[LL, UL]` -> Update(1) | Left cambia a `Unlinked` |
| **ULS-06** | Current UL, Left bien (UL) | `[UL, UL]` -> Update(1) | Left se mantiene `Unlinked` |

---

## 3. Método: `UpdateRightSlot`

El método más complejo. Maneja la propagación hacia adelante, incluyendo la lógica de Double Linked.

### 3.1 Análisis del Flujo de Control
Estructura compleja con múltiples condiciones compuestas y verificaciones de "ClickedSlot" para determinar la intención del usuario.

*   **Rama 1 (LL & Clicked):** Si soy LL y fui clickado -> Forzar derecha a RL (si no lo es).
*   **Rama 2 (RL & DLClicked):** Si soy RL y fue un click Doble -> Forzar derecha a RL.
*   **Rama 3 (RL & Clicked):** Si soy RL y fui clickado (normal) -> Si hay cadena a la derecha (`Right` y `Right.Right` son RL) -> Convertir derecha inmediata a LL (romper cadena DL).
*   **Rama 4 (UL/None):** Si me desligo -> Verificar si rompo una cadena a la derecha. Si `Right` y `Right.Right` son RL -> `Right` se vuelve LL. Si no, `Right` se vuelve UL.

### 3.2 Complejidad Ciclomática de McCabe ($V(G)$)
Predicados:
1.  `IsLeftLinked()`
2.  `IsTheClickedSlot()`
3.  (Anidado) `!RightSlot.IsRightLinked()`
4.  `IsRightLinked()` (Rama 2)
5.  `IsADoubleLinkedClickedSlot()`
6.  (Anidado) `!RightSlot.IsRightLinked()`
7.  `IsRightLinked()` (Rama 3)
8.  `IsTheClickedSlot()`
9.  (Anidado) `RightSlot.IsRightLinked()`
10. (Anidado) `RightSlot.RightSlot.IsRightLinked()`
11. `IsUnlinked()` (Rama 4)
12. `IsNonSlot()`
13. (Anidado) `RightSlot.IsRightLinked()`
14. (Anidado) `RightSlot.RightSlot.IsRightLinked()`

**$V(G) = 14 + 1 = 15$**
*Interpretación:* **Alta complejidad**. Este método es propenso a errores. Contiene mucha lógica anidada sobre el estado de los vecinos lejanos (`Right.Right`). Es el candidato principal para refactorización futura (Strategy Pattern o State Pattern). Requiere testing exhaustivo.

### 3.3 Caminos Linealmente Independientes (Selección Crítica)
1.  **LL + Clicked -> Right!=RL:** Corrige Right a RL.
2.  **LL + !Clicked:** No hace nada.
3.  **RL + DLClicked -> Right!=RL:** Corrige Right a RL (crea cadena).
4.  **RL + Clicked + ChainRight:** Rompe cadena derecha (Right pasa de RL a LL).
5.  **UL + ChainRight:** Rompe cadena derecha, el siguiente pasa a ser inicio (LL).
6.  **UL + NoChain:** Desliga al vecino derecho.

### 3.4 Tabla de Casos de Prueba (Cobertura 100%)

| ID | Escenario | Estado Inicial (**Current**, Right, Right+1) | Input User | Resultado Esperado en `RightSlot` |
|:---|:---|:---|:---|:---|
| **URS-01** | Crear par LL (Click) | `[LL, UL, UL]` | Click en Current | Right cambia a `RightLinked` |
| **URS-02** | Propagación pasiva LL | `[LL, UL, UL]` | Click en otro lado | No cambia |
| **URS-03** | Crear DL (Click Menu) | `[LL, RL, UL]` | Click DL en Current (Middle) | Right cambia a `RightLinked` |
| **URS-04** | Romper DL desde izq | `[LL, RL, RL]` | Click RL en Current | Right cambia a `LeftLinked` (Nuevo inicio) |
| **URS-05** | Borrar slot en cadena | `[UL, RL, RL]` | Set a `Unlinked` | Right cambia a `LeftLinked` (Nuevo inicio) |
| **URS-06** | Borrar par simple | `[UL, RL, UL]` | Set a `Unlinked` | Right cambia a `Unlinked` |
