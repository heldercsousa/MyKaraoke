# Future UX/UI Requirements & Navigation Proposal

## 1. Vision & Context
**Goal**: Evolve MyVocaList from a single-queue manager to a multi-queue "Social Network" for karaoke.
**Key Shift**: Moving from `StackPage` (current main) to `QueuePage` (list of all queues) as the entry point.
**Future**: Cloud API, user profiles, social interactions.

## 2. Navigation Architecture Analysis

### The "Hamburger" vs. "Bottom Nav" Dilemma
The user requested a "Hamburger Menu" for Settings/Exit. However, with the new requirement of a "Social Network" future, the navigation needs to be scalable.

#### Option A: Hybrid Navigation (Recommended)
- **Top Level (QueuePage)**:
    - **Header**: Hamburger Menu (Left) for App-wide actions (Settings, Profile, Exit).
    - **FAB**: Add New Queue.
    - **List Items**: Tap to view details.
- **Queue Details Level (QueueDetailsPage)**:
    - **Header**: Back Button (Left) to return to list.
    - **Bottom Nav**: Contextual actions for the *active* queue (e.g., "Current Singer", "History", "List").
    - **FAB**: Add Singer.

#### Option B: Full Drawer (Shell Flyout)
- Good for many top-level destinations (Queues, Musicians, Venues, Settings).
- Matches the "Hamburger" request natively.

#### Option C: Bottom Navigation (Main)
- Standard for Social Apps (Feed, Search, Create, Notifications, Profile).
- Might be too complex for the current MVP stage but aligns with the future vision.

**Proposal**: Start with **Option A (Hybrid)**. Use the Hamburger Menu on the main list for global app context, and keep the Bottom Nav within the specific Queue context if needed, or simplify it.

## 3. Detailed Page Requirements

### 3.1. QueuePage (New Main Page)
**Purpose**: List all registered queues from the database.
**Performance**: Must handle large datasets (virtualization/pagination).
**List Item Content**:
- Venue Name
- Date/Time (Start - End)
- Singer Count
- Status: Active (with duration HH:MM) vs. Inactive/Finished.
**Interactions**:
- **Tap**: Select item (exclusive selection).
- **Selection Mode Actions (Navbar)**:
    - `View`: Open QueueDetailsPage.
    - `Delete`: Only if inactive, not finished, and no participation data.
    - `Activate`: Only if no other queue is active AND it's the last registered queue.
- **FAB**: Add New Queue.

### 3.2. Add Queue Page (New)
**Purpose**: Form to create a new queue.
**Inputs**:
- Venue (Selection/Input)
- Type (Mechanical / Bandokê)
- "Set as Active" toggle.
**Flow**: Save -> Navigate to QueueDetailsPage (or QueuePage depending on "Active" state).

### 3.3. QueueDetailsPage (Evolution of StackPage)
**Purpose**: Manage a specific queue (Active or History).
**Features**:
- **List**: Ordered singers (First to Last).
- **Reorder**:
    - Long Press + Drag & Drop.
    - Tap to select + Up/Down buttons in Bottom Nav.
- **Swipe Actions**:
    - **Swipe Right (1st position)**: "Participate" -> Starts Timer -> Moves to bottom after round.
    - **Swipe Left**: "Absent" (skip round) or "Dequeue" (remove completely).
- **Admin Controls**:
    - Set "No longer available" (dequeue).
    - Register "Next Song" for any singer.
    - "Complete Queue": Stop accepting new rounds.
    - "Reactivate": Re-open a completed queue.

## 4. Technical Considerations
- **Database**: Ensure `Queue` entity supports new fields (Type, Status, Duration).
- **State Management**: `QueueService` needs to handle "Active Queue" logic robustly (only one active at a time).
- **MD3 Compliance**:
    - Use `CollectionView` with `SwipeView` (or custom behavior) for list actions.
    - Use `FloatingActionButton` (FAB) for primary creation actions.
    - Use `NavigationDrawer` (or simulated) for the Hamburger menu.

## 5. Next Steps
1.  **Approve/Refine this Document**: Confirm these are the correct requirements.
2.  **Database Updates**: Modify entities to support multiple queues and statuses.
3.  **UI Implementation**: Build `QueuePage` and `AddQueuePage`.

## 6. Technical Debt & Refactoring
- **[ ] Style Refactoring**: Replace all inline XAML styles with reusable StaticResources in `MaterialStyles.xaml`.
    - *Note*: Ensure safe extraction to avoid regressions (like the QueuePage card background issue).
- **[ ] Navigation Refactoring**: Move `StackPage` bottom navigation buttons (History, Settings, etc.) to the future Hamburger Menu (Shell Flyout).
