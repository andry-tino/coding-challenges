------------------------------- MODULE queue -------------------------------
    (******************************************************************)
    (* Specification of a queueing system consisting of a queue       *)
    (* with fixed length allowing items to be enqueued and dequeued.  *)
    (******************************************************************)
    
    (******************************************************************)
    (* Important: This is a failing spec, unbound variables, TLC runs *)
    (* indefinitely. *)
    (******************************************************************)

EXTENDS Naturals

CONSTANT L (* The fixed max length of the queue *)
VARIABLE q (* Represents the queue as the number of items in it *)
----------------------------------------------------------------------------
TypeInvariant == q \in (0 .. L)
----------------------------------------------------------------------------
Init == q = 0

NoOp == q' = q (* Queue unchanged *)

Enqueue == q' = q + 1 (* Element added *)

Dequeue == q' = q - 1 (* Element removed *)

\********************************************************
\* The next state is either unchanged queue, enqueued
\* or dequeued.
\********************************************************
Next ==
  \/ NoOp
  \/ Enqueue
  \/ Dequeue
----------------------------------------------------------------------------
Spec == Init /\ [][Next]_q
----------------------------------------------------------------------------
THEOREM Spec => TypeInvariant
============================================================================
\* Modification History
\* Last modified Sun Nov 22 08:45:40 CET 2020 by antino
\* Created Sat Nov 21 21:09:43 CET 2020 by antino
