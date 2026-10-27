# In-Depth Study Report

## Track A: Migrating an ASP.NET Core MVC Library Application to a React Single-Page Application

### A Design-Based Case Study of a University Library System

**Prepared by:** Sergio González and Arthur Chevreau  
**Course:** [Course name and code]  
**Institution:** [University name]  
**Instructor:** [Instructor name]  
**Date:** 25 October 2026

---

## Abstract

This study examines the migration of a university library management application from an ASP.NET Core MVC server-rendered architecture to a React single-page application (SPA), using a read-only book catalogue as a micro-artifact. The research addresses a practical question: how does this architectural shift affect state management, developer experience and routing, and under which conditions is it justified? A design-based case study was adopted. The existing Lab 2 application, built with C# .NET 10, Entity Framework Core, SQLite and ASP.NET Core Identity, was extended with a JSON API endpoint (`GET /api/books`), a CORS policy and a Vite/React client using Axios. The catalogue displays ten seeded books and provides client-side filtering by title and genre. Functional verification confirmed that the API returned all ten records, while the .NET build, Vite production build and linting completed without errors. The results show that MVC centralises state, routing and rendering on the server and offers a strongly integrated developer experience, whereas React moves state and navigation to the browser, improves component reuse and interaction speed, and requires an explicit API contract, CORS configuration and additional client tooling. Neither model is universally superior. For a small CRUD-oriented library application, MVC remains simpler and more secure by default; a React SPA becomes advantageous when rich interactivity, independent front-end delivery or multiple clients are required. A hybrid architecture is therefore recommended for future iterations.

**Keywords:** ASP.NET Core MVC; React; single-page application; state management; developer experience; routing; REST API

---

## 1. Introduction

### 1.1 Background

Web application architecture has shifted substantially over the past two decades. Traditional server-rendered applications generate HTML on the server and send a complete document to the browser for every interaction. In contrast, single-page applications (SPAs) load a JavaScript client once and update the user interface dynamically by exchanging data with one or more backend services (Mikowski and Powell, 2013). Both approaches are legitimate architectural choices, but they distribute responsibility for state, rendering, routing and security differently.

The starting point for this study is the Lab 2 library management application. The original system follows the ASP.NET Core MVC pattern and uses Razor views, Entity Framework Core (Microsoft, 2026b), SQLite and ASP.NET Core Identity. It supports a book catalogue, genre filtering, administrative CRUD operations, user authentication, book borrowing and loan returns. ASP.NET Core MVC applies the model-view-controller pattern to separate domain data, presentation and request handling (Microsoft, 2026a). The framework also provides integrated routing, model binding, validation, anti-forgery protection and authentication, which makes it well suited to a conventional CRUD application.

Track A of the in-depth study required a controlled migration of the catalogue experience to a React SPA. The objective was not to discard the existing backend, but to expose its data through a JSON API and build a separate client that consumes that API. This creates a useful comparative case because both implementations share the same domain model and database while differing in where state, rendering and navigation are handled.

The resulting micro-artifact consists of an ASP.NET Core API controller, a CORS policy that permits requests from the Vite development server, and a React client created with Vite. The client fetches the catalogue through Axios and provides a table view with filters for title and literary genre. Ten books across five genres are seeded automatically so that the application has a reproducible demonstration dataset.

### 1.2 Research Question

The study is guided by the following research question:

> **To what extent does migrating a server-rendered ASP.NET Core MVC library application to a React single-page application change state management, developer experience and routing, and what trade-offs does this architectural shift introduce for a small academic CRUD application?**

Three supporting questions operationalise the main question:

1. **State management:** How does responsibility for application state differ between server-rendered MVC views and a React client?
2. **Developer experience:** What changes in tooling, feedback loops, type safety and integration effort when the front end becomes a separate JavaScript application?
3. **Routing:** How does navigation move from server-side routing to client-side routing, and what additional configuration does this require?

The study does not attempt to prove that one architecture is universally better. Instead, it evaluates the conditions under which each approach is appropriate for a small library application.

### 1.3 Literature Review

The MVC pattern is one of the most established architectural patterns in interactive software. It separates an application into a model that represents domain state, a view that presents information and a controller that interprets user input and coordinates the other two (Gamma et al., 1994). In enterprise web systems, this separation is often combined with layering and a clear boundary between presentation logic and domain logic (Fowler, 2002). ASP.NET Core MVC implements this pattern through controllers, action methods, Razor views and a convention-based routing system. The framework integrates dependency injection, model validation, authentication and authorisation, reducing the amount of infrastructure code required for a CRUD application (Microsoft, 2026a).

Server-rendered MVC applications have several practical strengths. The server remains the authority for state, each request produces a complete and bookmarkable document, and sensitive logic stays on the server. Because HTML is generated before it reaches the browser, the approach is generally search-engine friendly and can offer a short time to first meaningful content. Security mechanisms such as anti-forgery tokens and same-origin form submissions are straightforward to apply. These advantages explain why MVC remains common in administrative systems, content-driven sites and applications that prioritise simplicity and server-side control.

The SPA model emerged as browsers became capable of running substantial JavaScript applications. In an SPA, the initial document acts as an application shell; subsequent navigation and rendering are handled by client-side code, while data is obtained from APIs (Mikowski and Powell, 2013). React supports this model through a component-based, declarative programming style in which the interface is expressed as a function of state (Meta, 2026a). Hooks such as `useState`, `useEffect` and `useMemo` allow stateful behaviour and derived values to be encapsulated within components, while the virtual DOM reduces the amount of direct browser manipulation required (Meta, 2026b).

The separation of client and server is frequently described through REST principles. Fielding (2000) defines representational state transfer as an architectural style based on resources, uniform interfaces and stateless interactions. A React client consuming `GET /api/books` follows this style at a basic level: the book collection is a resource, the server returns a representation in JSON, and each request carries the information needed to fulfil it. The Richardson Maturity Model offers a graded way to assess how fully an HTTP API adopts REST principles, from a single endpoint to hypermedia-driven interaction (Fowler, 2014). When client and API are served from different origins, the browser's same-origin policy requires explicit cross-origin resource sharing (CORS) configuration (Microsoft, 2026c).

The literature does not support a simple claim that SPAs supersede server-rendered applications. Mikowski and Powell (2013) present SPAs as a response to increasingly interactive web applications, but they also emphasise the importance of disciplined architecture, modularity and a clear data layer. Fowler (2002) cautions against distributing an application without considering the resulting complexity. React's own documentation stresses that state should be kept minimal and that data flow should remain explicit (Meta, 2026b). These positions suggest that the decision is contingent on the interaction requirements, team structure, security model and operational context rather than on fashion alone.

A gap exists in the form of detailed, small-scale comparisons grounded in a single academic CRUD system. Much of the available guidance is either framework documentation or broad architectural commentary. This study contributes a concrete migration case in which the same library domain is served first by Razor views and then by a React client, allowing state management, developer experience and routing to be compared within a bounded artifact.

---

## 2. Methods

### 2.1 Research Design

The study adopts a design-based case study approach. A micro-artifact was designed, implemented and evaluated against the functional requirements of Track A. The evaluation is qualitative and comparative rather than statistical: it examines how the artifact behaves and how the development process differs from the original MVC implementation. This design is appropriate because the research question concerns architectural consequences in a specific educational context, not the measurement of population-level effects.

The comparative framework uses three dimensions derived from the literature: state management, developer experience and routing. These dimensions were selected because they represent the main points at which server-rendered MVC and client-rendered SPA architectures diverge. Rendering, security and deployment are treated as cross-cutting considerations in the discussion.

### 2.2 Technology Stack

**Table 1. Technologies used in the micro-artifact**

| Layer | Technology | Version / role |
|---|---|---|
| Language and runtime | C# and .NET | .NET 10 (`net10.0`) |
| Backend framework | ASP.NET Core MVC / Web API | Controllers, JSON endpoint, CORS |
| Data access | Entity Framework Core | 10.0.13, `Include` and async queries |
| Database | SQLite | Local `app.db`, seeded on startup |
| Authentication | ASP.NET Core Identity | Retained from the Lab 2 backend |
| Front-end library | React | 19.3.1, component state and hooks |
| Build tool | Vite | 8.3.4, development server and bundling |
| HTTP client | Axios | 1.20.0, JSON requests and cancellation |
| Linting | oxlint | 1.88.0, static analysis for the React client |
| API origin | HTTPS | `https://localhost:7164` |
| Client origin | HTTP | `http://localhost:5173` |

The backend and frontend run as separate processes. The API is exposed over HTTPS on port 7164, while the Vite development server runs on port 5173. The client reads the API base URL from the `VITE_API_URL` environment variable, with a fallback to `https://localhost:7164/api/books`.

### 2.3 Scope of the Micro-Artifact

The artifact is deliberately limited to a read-only catalogue. Its scope comprises:

- A JSON endpoint at `GET /api/books`.
- Loading books with their related genre through Entity Framework Core.
- A CORS policy allowing requests from `http://localhost:5173`.
- Automatic seeding of five genres and ten books when the catalogue is empty.
- A React table showing title, author and genre.
- Client-side filtering by partial title match and by genre.
- Loading, error and empty-result states.
- A count of filtered versus total books.

The following features of the original MVC application were intentionally excluded: administrative create, edit and delete operations; the borrowing and return workflow; Identity-based sign-in within the React client; server-side pagination; and multi-page client routing. These exclusions keep the artifact focused on the architectural comparison rather than turning it into a complete rewrite.

**Table 2. Seeded demonstration books**

| No. | Title | Author | Genre |
|---:|---|---|---|
| 1 | Dune | Frank Herbert | Science Fiction |
| 2 | Neuromancer | William Gibson | Science Fiction |
| 3 | The Hobbit | J.R.R. Tolkien | Fantasy |
| 4 | Harry Potter and the Philosopher's Stone | J.K. Rowling | Fantasy |
| 5 | The Name of the Wind | Patrick Rothfuss | Fantasy |
| 6 | The Hound of the Baskervilles | Arthur Conan Doyle | Mystery |
| 7 | Murder on the Orient Express | Agatha Christie | Mystery |
| 8 | The Name of the Rose | Umberto Eco | Historical Fiction |
| 9 | The Pillars of the Earth | Ken Follett | Historical Fiction |
| 10 | Pride and Prejudice | Jane Austen | Romance |

Seeding is idempotent at the catalogue level. Missing genres are inserted individually, but sample books are added only when the `Book` table is empty. This prevents duplicate demonstration records on subsequent application starts.

### 2.4 Implementation Procedure

The backend was extended with an API controller rather than modifying the existing Razor controller. The new controller injects the Entity Framework Core context, queries the `Book` set with its `Genre` navigation property, and projects the result into a small data-transfer object. Projection avoids returning the full object graph and therefore prevents cyclic JSON serialisation between books and genres. The query uses `AsNoTracking()` because the endpoint is read-only.

CORS was configured as a named policy in `Program.cs`. The policy allows any header and any HTTP method, but only from the Vite origin `http://localhost:5173`. The middleware is placed after routing and before authentication so that preflight requests can be handled correctly.

The React client was scaffolded with Vite (Vite, 2026) and uses Axios (Axios, 2026) for the HTTP request. `BookList.jsx` stores the book collection, title filter, genre filter, loading flag and error message in component state. The genre options are derived from the loaded books with `useMemo`, and the filtered list is also memoised. The filter is performed in the browser, which is appropriate for a ten-record dataset but would need to move to the server for a larger catalogue.

### 2.5 Verification and Analysis

Verification combined static and functional checks:

1. `dotnet build` was used to confirm that the backend compiles.
2. `npm run build` was used to confirm that the React application produces a production bundle.
3. `npm run lint` was used to check the client for common static-analysis issues.
4. The API was started and queried directly to confirm the HTTP status, JSON shape and record count.
5. The React view was inspected for the presence of the table, filters, loading state, error state and empty state.

The comparative analysis then mapped the observed implementation to the three evaluation dimensions. No human participants were involved, and no personal data were collected.

---

## 3. Results

### 3.1 Overview of the Implemented Micro-Artifact

The completed artifact follows a simple three-tier flow:

```text
React client (localhost:5173)
        |
        |  Axios GET /api/books
        v
ASP.NET Core API (localhost:7164)
        |
        |  EF Core query with Include(Genre)
        v
SQLite database (app.db)
```

The API exposes one endpoint, `GET /api/books`, and returns a JSON array. The controller is deliberately small and delegates persistence to Entity Framework Core. The following excerpt is taken from `Controllers/BooksApiController.cs`, lines 18–36:

```csharp
// GET: api/books
[HttpGet]
public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
{
    var books = await _context.Book
        .Include(b => b.Genre)
        .AsNoTracking()
        .Select(b => new BookDto(
            b.Id,
            b.Title,
            b.Author,
            b.GenreId,
            b.Genre != null ? b.Genre.Name : null))
        .ToListAsync();

    return Ok(books);
}

public record BookDto(int Id, string Title, string Author, int GenreId, string? GenreName);
```

The `BookDto` record defines the contract consumed by the client. It contains only the fields required by the table and the filters, which keeps the payload small and decouples the API from the persistence model. The `Include` call ensures that genre data is available, while the projection determines the exact JSON shape. A representative response record is:

```json
{
  "id": 1,
  "title": "Dune",
  "author": "Frank Herbert",
  "genreId": 1,
  "genreName": "Science Fiction"
}
```

The client side is implemented in `client/src/BookList.jsx`. The API location is configured with an environment variable, and the request is performed inside an effect with an `AbortController` so that an in-flight request is cancelled if the component unmounts (`BookList.jsx`, lines 5–40):

```jsx
const API_URL =
  import.meta.env.VITE_API_URL ?? 'https://localhost:7164/api/books'

const [books, setBooks] = useState([])
const [titleFilter, setTitleFilter] = useState('')
const [genreFilter, setGenreFilter] = useState('all')

useEffect(() => {
  const controller = new AbortController()

  const loadBooks = async () => {
    try {
      setLoading(true)
      setError('')
      const { data } = await axios.get(API_URL, {
        signal: controller.signal,
      })
      setBooks(data)
    } catch (err) {
      if (!axios.isCancel(err)) {
        setError(
          'No se pudieron cargar los libros. Verifica que la API esté en ejecución.',
        )
      }
    } finally {
      setLoading(false)
    }
  }

  loadBooks()
  return () => controller.abort()
}, [])
```

The filtering logic is also client-side and is expressed as derived state (`BookList.jsx`, lines 52–60):

```jsx
const filteredBooks = useMemo(() => {
  const term = titleFilter.trim().toLowerCase()
  return books.filter((book) => {
    const matchesTitle = !term || book.title.toLowerCase().includes(term)
    const matchesGenre =
      genreFilter === 'all' || String(book.genreId) === genreFilter
    return matchesTitle && matchesGenre
  })
}, [books, titleFilter, genreFilter])
```

This implementation satisfies the Track A requirements: the list is rendered in a table, the title search performs a case-insensitive partial match, and the genre selector is populated dynamically from the returned data. The interface also reports the number of matching books and provides a control for clearing both filters.

### 3.2 Verification Outcomes

The backend compiled successfully with `dotnet build`: zero errors were reported. Two warnings were present in pre-existing Razor and file-loading code and were unrelated to the new API controller. The Vite production build completed successfully, producing a bundled JavaScript and CSS asset set, and the lint command reported no findings for the client source.

A direct request to `GET /api/books` returned HTTP 200 with `application/json` content and ten records. The returned data included the seeded titles, authors and genre names, confirming that the `Include` query, projection, seeding logic and JSON serialisation worked together. The API was also verified after a restart to confirm that the idempotent seeding behaviour did not duplicate the catalogue.

The browser view displayed the ten records in a table. Entering a partial title reduced the visible rows to matching entries, selecting a genre restricted the table to that genre, and clearing the filters restored the full dataset. The loading, error and empty-result states were present in the component and were reachable through the corresponding state transitions.

These results establish functional correctness for the bounded artifact. They do not constitute a performance benchmark, a usability study or a security audit.

### 3.3 Comparative Analysis: MVC versus React SPA

**Table 3. Comparative evaluation of the original MVC implementation and the React SPA artifact**

| Dimension | ASP.NET Core MVC (server-side rendering) | React SPA (client-side rendering) | Interpretation for the library application |
|---|---|---|---|
| **State management** | The server owns the authoritative state for each request. Controllers query the database, populate models or `ViewBag`/`TempData`, and render a complete view. Client-side state is limited to what the browser stores in forms, cookies or session data. | React owns the presentation state in the browser. `useState` holds the loaded books and filters, while `useMemo` derives genres and filtered results. The server remains the source of truth, but its data is copied into a client-side cache that must be refreshed and kept consistent. | MVC has less state-synchronisation overhead for simple CRUD screens. React provides a more responsive experience for repeated filtering and can support complex interactions, but it introduces an explicit client cache and additional state transitions. |
| **Developer experience** | A single solution contains controllers, views, models and configuration. C# is strongly typed, Razor views are compiled, and debugging can follow a request from browser to database. Iteration requires a server rebuild or restart, and the UI is tightly coupled to the backend release cycle. | The client has its own toolchain and package ecosystem. Vite provides fast hot module replacement, and React components can be reused and tested independently. However, the project now has two runtimes, two dependency sets, a CORS configuration and a manually maintained API contract. | MVC offers lower integration ceremony and stronger end-to-end type safety. React offers faster visual iteration and clearer component boundaries, at the cost of a split toolchain and duplicated model knowledge. |
| **Routing** | Routing is configured on the server through conventional or attribute routes. A navigation action produces a new HTTP request and a full page render. Deep links work naturally, and search engines receive rendered HTML. | Navigation is handled in the browser. A multi-view SPA requires a client router, a fallback route on the server and careful handling of direct URL access. This artifact contains a single catalogue view, so no client router was needed; adding one would introduce additional configuration and bundle weight. | MVC is simpler for document-oriented navigation and public discoverability. A client router is preferable when the application behaves like a desktop-style tool with many independent views, but it is not justified for a single read-only catalogue screen. |
| **Rendering, initial load and security** | The server returns rendered HTML, which typically produces a meaningful first paint and supports search engine indexing. Same-origin forms, anti-forgery tokens and server-side validation are integrated into the framework. | The browser must download the application bundle before rendering. SEO requires additional measures such as server-side rendering or prerendering. The API becomes a separate attack surface and requires an explicit authentication and authorisation strategy once write operations are introduced. | MVC has a lower baseline for a small, secure CRUD application. The SPA model becomes more attractive when the client is highly interactive or when the API must serve several clients, but it shifts security work to token handling and API policy. |

The comparison shows that the migration changes the location of complexity rather than eliminating it. In the MVC version, state, routing and rendering are coordinated by the framework within a single request lifecycle. In the React version, those concerns are distributed between the API and the client. The client becomes more interactive and independently deployable, but the team must now maintain a contract, configure cross-origin access and reason about client-side state synchronisation.

---

## 4. Discussion and Conclusion

### 4.1 Interpretation of Findings

The most significant change produced by the migration is the relocation of state ownership. In the MVC application, a controller action reads the database, prepares a model and returns a view; once the response is sent, the request-scoped state disappears. In the React artifact, the book collection and filter values persist in the browser while the component is mounted. This makes repeated filtering instantaneous because no round trip is required, but it also means that the displayed data can become stale. The client would need refetching, cache invalidation or optimistic updates before it could safely support editing and borrowing workflows.

Developer experience changed in a similarly ambivalent way. The React client benefits from Vite's fast feedback loop and from the component model, which makes the table and filter controls independently comprehensible. The C# backend remains strongly typed and benefits from Entity Framework Core's query composition and compile-time checking. However, the boundary between them is not type-checked: the `BookDto` shape is duplicated conceptually in JavaScript, and a change to the API contract would only be discovered at runtime unless additional tooling such as TypeScript or generated clients were introduced. The split also adds CORS, environment configuration and two development servers to the setup.

Routing produced the clearest difference. The MVC application relies on server routes, and every navigation is a full request. The React artifact has only one view, so it does not yet demonstrate client-side routing; adding React Router (Remix Software, 2026) would allow app-like navigation without full reloads, but it would also require a server fallback and careful handling of deep links. For the current catalogue, server routing remains the simpler and more robust option.

The results therefore support a contingent conclusion. React is not inherently a better presentation layer for a library system. It is a better fit when the user interface needs frequent partial updates, when the front end is developed by a separate team, or when the same API must serve web, mobile and other clients. MVC is a better fit when the application is predominantly form-based, when server-side rendering and SEO matter, and when the team benefits from a single integrated toolchain.

### 4.2 Limitations

Several limitations constrain the interpretation of this study. First, the artifact contains only ten seeded books and a single read-only view. The filtering behaviour is therefore evaluated at a very small scale; a catalogue with thousands of records would require server-side paging, indexing and a different data-loading strategy. Second, the React client does not implement authentication, borrowing, returns or administrative CRUD. The security and state-management implications of those workflows are discussed conceptually but were not empirically tested.

Third, the evaluation relies on build results and manual smoke tests rather than automated integration tests, end-to-end tests or performance measurements. No load testing, bundle-size comparison, accessibility audit or search-engine evaluation was performed. Fourth, the comparison is based on one codebase and one development context, so the findings are not statistically generalisable. Finally, the project uses very recent framework versions (.NET 10, React 19 and Vite 8); some tooling behaviour may differ in older or long-term-support environments.

These limitations do not invalidate the architectural comparison, but they mean that the conclusions should be read as design guidance for a small academic CRUD application rather than as universal claims about MVC and SPAs.

### 4.3 Trade-Offs and Recommendations

The migration demonstrates a clear trade-off between integration and flexibility. MVC integrates state, rendering, routing and security into a single server-side lifecycle. This reduces the number of moving parts and is well suited to a library system whose main operations are forms, lists and role-based administration. React introduces a network boundary between the UI and the data, which creates additional work but also enables independent evolution of the client, richer interactions and reuse of the API by other consumers.

For future iterations, a hybrid architecture is recommended. The existing MVC application could retain Identity, administrative CRUD, borrowing and returns, where server-side validation and anti-forgery protection are valuable. The React SPA could be extended for the public catalogue, where interactive filtering, component reuse and a more app-like experience provide clear benefits. If the SPA is extended to write operations, the next steps should include token-based authentication, server-side authorisation on every endpoint, centralised API error handling, automated integration tests and a shared contract strategy such as TypeScript types or an OpenAPI-generated client.

### 4.4 Conclusion

This study set out to determine how migrating a server-rendered ASP.NET Core MVC library application to a React SPA changes state management, developer experience and routing, and what trade-offs follow for a small academic CRUD application. The micro-artifact shows that the migration is technically feasible without discarding the existing domain model: Entity Framework Core continues to provide data access, a small API controller exposes the catalogue, and React provides a responsive, component-based view with client-side filtering.

The answer to the research question is that the shift is real but conditional. State management moves from request-scoped server control to persistent client state; developer experience moves from an integrated, strongly typed stack to a faster but more fragmented two-runtime workflow; and routing moves from server-owned URLs to a client-side concern that requires additional infrastructure once multiple views are introduced. These changes are advantageous when interactivity, front-end autonomy or multiple clients are priorities. For a small library CRUD system, however, ASP.NET Core MVC remains the simpler and safer default. The most defensible conclusion is therefore not that React replaces MVC, but that React is a valuable complement when the application's interaction profile justifies the additional architectural boundary.

---

## References

Axios (2026) *Getting Started*. Available at: https://axios-http.com/docs/intro (Accessed: 25 October 2026).

Fielding, R.T. (2000) *Architectural Styles and the Design of Network-based Software Architectures*. Doctoral dissertation. University of California, Irvine. Available at: https://www.ics.uci.edu/~fielding/pubs/dissertation/top.htm (Accessed: 25 October 2026).

Fowler, M. (2002) *Patterns of Enterprise Application Architecture*. Boston, MA: Addison-Wesley.

Fowler, M. (2014) *Richardson Maturity Model: steps toward the glory of REST*. Available at: https://martinfowler.com/articles/richardsonMaturityModel.html (Accessed: 25 October 2026).

Gamma, E., Helm, R., Johnson, R. and Vlissides, J. (1994) *Design Patterns: Elements of Reusable Object-Oriented Software*. Reading, MA: Addison-Wesley.

Meta (2026a) *React documentation*. Available at: https://react.dev/ (Accessed: 25 October 2026).

Meta (2026b) *Managing State*. Available at: https://react.dev/learn/managing-state (Accessed: 25 October 2026).

Microsoft (2026a) *ASP.NET Core MVC overview*. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview (Accessed: 25 October 2026).

Microsoft (2026b) *Entity Framework Core documentation*. Available at: https://learn.microsoft.com/en-us/ef/core/ (Accessed: 25 October 2026).

Microsoft (2026c) *Enable Cross-Origin Requests (CORS) in ASP.NET Core*. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/cors (Accessed: 25 October 2026).

Mikowski, M.S. and Powell, J.C. (2013) *Single Page Web Applications: JavaScript End-to-End*. Shelter Island, NY: Manning Publications.

Remix Software (2026) *React Router documentation*. Available at: https://reactrouter.com/ (Accessed: 25 October 2026).

Vite (2026) *Getting Started*. Available at: https://vite.dev/guide/ (Accessed: 25 October 2026).
