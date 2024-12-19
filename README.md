# Bank Account Event Sourcing Demo

## The Task and the Problem
I wanted to understand not just what the current balance of a bank account was, but **how it got there**. Traditional approaches only store the final state—if the account shows `$500`, I don’t know how many deposits, withdrawals, or limit changes happened along the way. Without that history:
- Auditing is tough,
- Debugging is harder,
- Managing concurrency (when two operations hit the same account simultaneously) can get messy.

My task was to design a system where every change—creating an account, depositing money, withdrawing funds, setting a withdrawal limit—is recorded as an **event**. By replaying these events, I can:
- Rebuild the account’s history at any time,
- See exactly what led to the current balance,
- Handle concurrent operations without overwriting changes.

---

## Approach
### Event Sourcing
Instead of just storing `Balance = $500`, I store **every event** that produced that balance. If the account ended up at `$500`, I know precisely which deposits, withdrawals, and limit settings occurred.

### Optimistic Concurrency Control (OCC)
When saving events, I check the account’s expected version. If it doesn’t match the current version, I throw a **concurrency exception** rather than silently overwriting someone else’s changes.

### Clean Architecture
- **Core (Domain):** Defines entities, events, and rules, no infrastructure details.
- **Application:** Commands and queries rely solely on domain interfaces.
- **Infrastructure:** Includes in-memory event store, a read model (projection), and other implementations hidden behind interfaces.

This layering keeps the system flexible. As I learn more or requirements evolve, I can swap out the storage layer or projection mechanism without touching the domain logic.

---

## Projections (Read Model)
Replaying all events to answer a simple query (like *“What’s the balance now?”*) can be slow. After saving events, I update a **read model**, making queries nearly instantaneous. 

- Here, I used an **in-memory model**, but in a real project, I could use a database or another technology.
- The point is to separate fast reads from event-based writes.

---

## Running the Project
1. Clone the repository.
3. Run the following commands:

   ```bash
   dotnet restore
   dotnet build
   run tests 

Unit tests
Directly calling command handlers and query handlers in code.
Unit Tests
I wrote unit tests to confirm that:

Domain rules hold up under various scenarios:
You can’t withdraw more than what’s available.
You can’t deposit after the account is closed.
Daily limits are respected.
The right events are emitted when I perform actions (e.g., depositing emits MoneyDeposited).
Concurrency conflicts raise clear exceptions instead of corrupting data.
The read model stays aligned with the events, ensuring queries return correct balances and states.
These tests give me confidence that the system works as intended and can be safely adapted or extended later.
