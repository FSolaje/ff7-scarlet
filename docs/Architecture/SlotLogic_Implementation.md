# Implementación de Lógica de Slots (`SlotGraphicalItem.cs`)

Este documento detalla la traducción técnica del [Algoritmo de Lógica de Enlaces](SlotLogic_Algorithm.md) al código C# en la clase `SlotGraphicalItem`.

## 1. Mapeo de Conceptos

| Algoritmo | Implementación C# | Notas |
|:---|:---|:---|
| **NS** | `MateriaSlot.None` | - |
| **UL** | `MateriaSlot.NormalUnlinkedSlot` | O `EmptyUnlinkedSlot` según `GrowthRate`. |
| **LL** | `MateriaSlot.NormalLeftLinkedSlot` | O `EmptyLeftLinkedSlot`. |
| **RL** | `MateriaSlot.NormalRightLinkedSlot` | O `EmptyRightLinkedSlot`. |
| **DL** | `IsDoubleLinked()` (bool) | No es un valor del enum, es un estado calculado. |
| **RS(1)** | `this.RightSlot` | Propiedad que devuelve la instancia del vecino derecho. |
| **LS(1)** | `this.LeftSlot` | Propiedad que devuelve la instancia del vecino izquierdo. |

---

## 2. Gestión de Estado Global

El sistema utiliza propiedades estáticas para mantener la coherencia durante la operación de propagación recursiva:

*   `isMultilinkedEnabled`: Bandera maestra que habilita/deshabilita la lógica extendida (2.2 del algoritmo).
*   `lastSelectionType`: Estructura (`TypeSelectedForSlot`) que almacena la intención original del usuario. Es crítica para diferenciar entre "Asignar RL (Rotura)" y "Asignar RL (DoubleLinked)".

---

## 3. Implementación de Propagación

La propagación se divide en dos métodos direccionales invocados por `SetInSlot`.

### 3.1 Hacia la Izquierda (`UpdateLeftSlot`)

Implementa la sección "Propagación Izquierda (<-)" del algoritmo.

#### Lógica General
Se evalúa el estado del slot actual (`this`) para decidir qué imponerle al vecino izquierdo (`LeftSlot`).

1.  **Caso: Asignar LL (`IsLeftLinked()`)**
    *   *Algoritmo:* Si LS(1) es LL, romper enlace (colisión).
    *   *Código:* `if (LeftSlot.IsLeftLinked()) LeftSlot.SetInSlot(..., Unlinked, ...)`
    *   *Justificación:* Evita `LL-LL`, forzando `UL-LL`.

2.  **Caso: Asignar RL (`IsRightLinked()`)**
    *   *Algoritmo:* Crea enlace izquierdo (Auto-link). Si LS(1) es UL/NS -> Asignar LL.
    *   *Código:* `else if (IsRightLinked()) { if (LeftSlot.IsUnlinked() || LeftSlot.IsNonSlot()) LeftSlot.SetInSlot(..., LeftLinked, ...); }`
    *   *Nota:* Si LS(1) ya es LL (enlazado), no hace nada (respeta el enlace existente).

3.  **Caso: Asignar UL/NS (`IsUnlinked()` / `IsNonSlot()`)**
    *   *Algoritmo:* Rompe enlace izquierdo. Si LS(1) es LL -> Asignar UL.
    *   *Código:* `else if (IsUnlinked() || IsNonSlot()) { if (LeftSlot.IsLeftLinked()) LeftSlot.SetInSlot(..., Unlinked, ...); }`
    *   *Justificación:* Un slot desliado no puede recibir un enlace, por lo que el emisor (LL) debe replegarse.

### 3.2 Hacia la Derecha (`UpdateRightSlot`)

Implementa la sección "Propagación Derecha (->)" del algoritmo, manejando la complejidad de Multilink.

#### Lógica de Decisión
Utiliza `lastSelectionType` y `IsTheClickedSlot()` para distinguir entre acciones directas del usuario y propagaciones automáticas.

1.  **Caso: Asignar LL**
    *   *Algoritmo:* Crea enlace derecho. RS(1) -> RL.
    *   *Código:* `if (IsLeftLinked() && IsTheClickedSlot()) { ... RightSlot.SetInSlot(..., RightLinked, ...); }`
    *   *Condición:* Solo se propaga si RS(1) no es ya RL.

2.  **Caso: Asignar DL (Multilink Activado)**
    *   *Algoritmo:* Asignar RL -> Caso B (DoubleLinked).
    *   *Código:* `else if (IsRightLinked() && IsADoubleLinkedClickedSlot()) { ... RightSlot.SetInSlot(..., RightLinked, ...); }`
    *   *Mecanismo:* `IsADoubleLinkedClickedSlot` verifica si el menú seleccionado fue específicamente "Double Linked". Esto fuerza la creación de cadena.

3.  **Caso: Asignar RL (Rotura / Propagación)**
    *   *Algoritmo:* Asignar RL -> Caso A (Rotura) y Caso C (Propagado).
    *   *Código:* `else if (IsRightLinked() && IsTheClickedSlot())`
    *   *Lógica:* Si soy el slot clickado y NO es DL (es RL normal), actúo como "rompe-enlaces" hacia la derecha.
    *   *Gestión de Cadenas:* Verifica `RightSlot.RightSlot` (RS2).
        *   `if (RightSlot.IsRightLinked() && RightSlot.RightSlot.IsRightLinked())`: Detecta cadena `RL-RL`.
        *   Acción: `RightSlot.SetInSlot(..., LeftLinked, ...)` (Convierte RS1 en nuevo inicio).

4.  **Caso: Asignar UL/NS**
    *   *Algoritmo:* Rompe enlace derecho.
    *   *Código:* `else if (IsUnlinked() || IsNonSlot())`
    *   *Gestión de Cadenas:* Igual que en el caso RL (Rotura).
        *   Si RS(1) y RS(2) son RL -> RS(1) se vuelve `LeftLinked` (salva la cadena restante).
        *   Si no -> RS(1) se vuelve `Unlinked` (desliga simple).

---

## 4. Diferencias con Lógica Nativa

La implementación no separa explícitamente el código en bloques "Nativo" vs "Multilink" mediante `if (multilink)`. En su lugar:
*   La lógica **Nativa** es el comportamiento base.
*   La lógica **Multilink** se habilita mediante las condiciones adicionales:
    *   La existencia de la opción `SlotMenuValue.DoubleLinked`.
    *   La capacidad de `IsRightLinked()` de encadenar verificaciones hacia `RightSlot.RightSlot`.

Si Multilink está desactivado (`isMultilinkedEnabled = false`), `IsDoubleLinked()` siempre retorna `false`, y las opciones de menú DL no aparecen, forzando al sistema a comportarse como Nativo (pares simples LL-RL).