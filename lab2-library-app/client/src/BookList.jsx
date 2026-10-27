import { useEffect, useMemo, useState } from 'react'
import axios from 'axios'
import './BookList.css'

const API_URL =
  import.meta.env.VITE_API_URL ?? 'https://localhost:7164/api/books'

function BookList() {
  const [books, setBooks] = useState([])
  const [titleFilter, setTitleFilter] = useState('')
  const [genreFilter, setGenreFilter] = useState('all')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

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
          console.error(err)
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

  const genres = useMemo(() => {
    const genreMap = new Map()
    books.forEach((book) => {
      if (book.genreId != null && book.genreName) {
        genreMap.set(book.genreId, book.genreName)
      }
    })
    return [...genreMap.entries()].sort((a, b) => a[1].localeCompare(b[1]))
  }, [books])

  const filteredBooks = useMemo(() => {
    const term = titleFilter.trim().toLowerCase()
    return books.filter((book) => {
      const matchesTitle = !term || book.title.toLowerCase().includes(term)
      const matchesGenre =
        genreFilter === 'all' || String(book.genreId) === genreFilter
      return matchesTitle && matchesGenre
    })
  }, [books, titleFilter, genreFilter])

  const hasActiveFilters = titleFilter !== '' || genreFilter !== 'all'

  const clearFilters = () => {
    setTitleFilter('')
    setGenreFilter('all')
  }

  return (
    <section className="book-list">
      <header className="book-list__header">
        <div>
          <p className="book-list__eyebrow">Track A · React SPA</p>
          <h1>Book Catalog</h1>
          <p className="book-list__subtitle">
            Explore the library collection and filter by title or genre.
          </p>
        </div>
        <span className="book-list__badge">
          {filteredBooks.length} of {books.length} books
        </span>
      </header>

      <div className="book-list__filters">
        <div className="field">
          <label htmlFor="title-filter">Search by title</label>
          <input
            id="title-filter"
            type="search"
            placeholder="e.g., The Lord of the Rings"
            value={titleFilter}
            onChange={(event) => setTitleFilter(event.target.value)}
          />
        </div>

        <div className="field">
          <label htmlFor="genre-filter">Literary genre</label>
          <select
            id="genre-filter"
            value={genreFilter}
            onChange={(event) => setGenreFilter(event.target.value)}
          >
            <option value="all">All genres</option>
            {genres.map(([id, name]) => (
              <option key={id} value={id}>
                {name}
              </option>
            ))}
          </select>
        </div>

        <button
          type="button"
          className="book-list__clear"
          onClick={clearFilters}
          disabled={!hasActiveFilters}
        >
          Clear filters
        </button>
      </div>

      {loading && <p className="book-list__message">Loading books...</p>}

      {!loading && error && (
        <p className="book-list__message book-list__message--error" role="alert">
          {error}
        </p>
      )}

      {!loading && !error && filteredBooks.length === 0 && (
        <p className="book-list__message">
          No books found matching the selected filters.
        </p>
      )}

      {!loading && !error && filteredBooks.length > 0 && (
        <div className="book-list__table-wrapper">
          <table className="book-list__table">
            <thead>
              <tr>
                <th scope="col">Title</th>
                <th scope="col">Author</th>
                <th scope="col">Genre</th>
              </tr>
            </thead>
            <tbody>
              {filteredBooks.map((book) => (
                <tr key={book.id}>
                  <td>{book.title}</td>
                  <td>{book.author}</td>
                  <td>
                    <span className="book-list__genre">
                      {book.genreName ?? 'Without genre'}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default BookList
