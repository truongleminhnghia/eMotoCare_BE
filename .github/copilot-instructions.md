# Copilot Instructions

## Project Guidelines
- User prefers only essential encapsulation (private setters where needed) and questions over-encapsulation; wants pragmatic Clean Architecture design.

## Code Generation Logic
- User wants reusable shared code-generation logic that accepts arbitrary text, generates numbered codes, and guarantees uniqueness per table via table-specific checks.
- For `InventoryLocation`, model `Code` as an Enum and derive `Name` automatically in the format 'Kệ + Code'.

## Entity Specifications
- In the `SlotTime` entity, derive `TimeSlot` from `StartTime` and `EndTime` in the format 'H:mm-H:mm' (e.g., '9:00-10:00') instead of being manually entered.