# Algoritmo de Lógica de Enlaces de Slots

Este documento define el comportamiento algorítmico esperado para la gestión de enlaces de materia, independiente de la implementación técnica.

## Leyenda
*   **NS**: NonSlot (None)
*   **UL**: Unlinked
*   **LL**: LeftLinked
*   **RL**: RightLinked
*   **DL**: DoubleLinked (Estado lógico, visualmente es RL)
*   **RS(x)**: RightSlot (Vecino derecho en posición x)
*   **LS(x)**: LeftSlot (Vecino izquierdo en posición x)

En la cadena de slots representaremos que hay enlace mediante "-" y que no hay enlace mediante "_".
Ejemplo: `LL-UL_LL-RL-RL` (Un enlace doble y uno triple).

---

## 1. Conceptos Fundamentales

### Estados de Slot
*   **None (NS)**: Espacio inexistente.
*   **Unlinked (UL)**: Espacio independiente.
*   **LeftLinked (LL)**: Inicio de una conexión. Visualmente se extiende hacia la derecha.
*   **RightLinked (RL)**: Visualmente recibe la conexión desde la izquierda. Si Multilink está activado, puede encadenarse (`LL-RL-RL...`).
*   **DoubleLinked (DL)**: Estado lógico que implica conexión a ambos lados. Depende de la opción Multilink.

---

## 2. Reglas de Asignación y Propagación

El comportamiento varía según si la opción **Multilink** está activada o no.

### 2.1 Multilink Desactivado (Nativo)
Solo existen NS, UL, LL y RL (como cierre de par simple).

#### Propagación Derecha (->)
*   **[SLOT-NAT-R-01]**: Asignar NS, UL, RL rompe el enlace derecho.
    *   Acción: Asignar **UL** a RS(1).
    *   Excepción: Si RS(1) ya es UL, NS o LL, no se propaga nada.
*   **[SLOT-NAT-R-02]**: Asignar LL crea un enlace derecho.
    *   Acción: Asignar **RL** a RS(1).

#### Propagación Izquierda (<-)
*   **[SLOT-NAT-L-01]**: Asignar NS, UL, LL rompe el enlace izquierdo.
    *   Acción: Asignar **UL** a LS(1).
    *   Excepción: Si LS(1) ya es UL, NS o RL, no se propaga nada.
*   **[SLOT-NAT-L-02]**: Asignar RL crea un enlace izquierdo.
    *   Acción: Asignar **LL** a LS(1).

### 2.2 Multilink Activado (Extendido)
Permite cadenas complejas y el estado DL.

#### Propagación Derecha (->)
*   **[SLOT-ML-R-01]**: Asignar LL crea enlace derecho.
    *   Acción: Asignar **RL** a RS(1).
*   **[SLOT-ML-R-02]**: Asignar UL, NS rompe enlace derecho.
    *   Condición 1: Si RS(1) es UL, NS o LL -> No propagar.
    *   Condición 2: Si RS(1) es RL -> Verificar RS(2).
        *   Si RS(2) es RL (era una cadena) -> Asignar **LL** a RS(1) (Nuevo inicio de cadena).
        *   Si no -> Asignar **UL** a RS(1).
*   **[SLOT-ML-R-03]**: Asignar RL (Caso A - Rotura): Si se selecciona RL normal (no DL) en el slot inicial, se aplica la lógica de rotura definida en **[SLOT-ML-R-02]**.
*   **[SLOT-ML-R-04]**: Asignar RL (Caso B - DoubleLinked): Si se selecciona explícitamente DL en el slot inicial, se aplica la lógica de creación definida en **[SLOT-ML-R-01]**.
*   **[SLOT-ML-R-05]**: Asignar RL (Caso C - Propagado): Si el slot actual recibió RL por propagación (no es el inicial), termina la propagación.

#### Propagación Izquierda (<-)
*   **[SLOT-ML-L-01]**: Asignar UL, NS, LL rompe enlace izquierdo.
    *   Acción: Si LS(1) es LL -> Asignar **UL** a LS(1).
*   **[SLOT-ML-L-02]**: Asignar RL crea enlace izquierdo (Auto-link).
    *   Acción: Si LS(1) es UL o NS -> Asignar **LL** a LS(1).

---

## 3. Reglas de Cambio de Crecimiento (Growth Rate)

El sistema debe reaccionar ante cambios globales en la tasa de crecimiento del equipo.

*   **[SLOT-GROWTH-01]**: Conversión de Variante.
    *   Si el nuevo crecimiento es **None** (Empty): Todos los slots `Normal*` deben convertirse a su equivalente `Empty*`.
    *   Si el nuevo crecimiento es **Normal/Double/Triple**: Todos los slots `Empty*` deben convertirse a su equivalente `Normal*`.
    *   Los slots `None` (NS) no se ven afectados.
    *   La lógica de enlace (UL, LL, RL) se mantiene intacta, solo cambia la variante de crecimiento.
